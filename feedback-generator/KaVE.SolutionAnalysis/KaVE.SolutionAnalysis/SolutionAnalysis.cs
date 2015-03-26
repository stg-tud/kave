/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CompleteStatement;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvents;
using KaVE.VsFeedbackGenerator.Analysis;
using KaVE.VsFeedbackGenerator.VsIntegration;
using ILogger = KaVE.Utils.Exceptions.ILogger;

namespace KaVE.SolutionAnalysis
{
    [ShellComponent]
    public class AnalysisSession : IIDESession
    {
        public string UUID
        {
            get { return "SolutionAnalysisUUID"; }
        }

        public DTE DTE
        {
            get { throw new NotSupportedException("no IDE in analysis session"); }
        }
    }

    //[SolutionComponent]
    public class SolutionAnalysis
    {
        public class AnalysesResults
        {
            public readonly IList<string> AnalyzedProjects = new List<string>();
            public readonly IList<string> AnalyzedFiles = new List<string>();
            public readonly IList<string> AnalyzedTypeNames = new List<string>();
            public readonly IList<Context> AnalyzedContexts = new List<Context>();
        }

        private readonly ISolution _solution;
        private readonly ILogger _logger;

        public SolutionAnalysis(ISolution solution, ILogger logger)
        {
            _solution = solution;
            _logger = logger;
        }

        public AnalysesResults AnalyzeAllProjects()
        {
            var results = new AnalysesResults();
            var projects = _solution.GetAllProjects();
            projects.Remove(_solution.MiscFilesProject);
            projects.Remove(_solution.SolutionProject);
            projects.ForEach(project => AnalyzeProject(project, results));
            return results;
        }

        private void AnalyzeProject(IProject project, AnalysesResults results)
        {
            results.AnalyzedProjects.Add(project.Name);

            var psiModules = _solution.PsiModules();
            var primaryPsiModule = psiModules.GetPrimaryPsiModule(project).NotNull("no psi module");
            var csharpSourceFiles = primaryPsiModule.SourceFiles.Where(file => file.Name.EndsWith(".cs"));
            csharpSourceFiles.ForEach(file => AnalyzeFile(file, primaryPsiModule, results));
        }

        private void AnalyzeFile(IPsiSourceFile psiSourceFile, IPsiModule primaryPsiModule, AnalysesResults results)
        {
            results.AnalyzedFiles.Add(psiSourceFile.DisplayName);

            var psiFile = ParseFile(psiSourceFile, primaryPsiModule);
            AnalyzeTypeAndNamespaceHolder(psiFile, results);
        }

        private static ICSharpFile ParseFile(IPsiSourceFile psiSourceFile, IPsiModule primaryPsiModule)
        {
            var languageService = CSharpLanguage.Instance.LanguageService().NotNull("CSharp language service not available");
            ILexer lexer = languageService.CreateCachingLexer(psiSourceFile.Document.Buffer);
            var csharpParser = (ICSharpParser) languageService.CreateParser(lexer, primaryPsiModule, psiSourceFile);
            csharpParser.ExpandChameleons = csharpParser.OpenChameleonStrategy;
            var psiFile = (ICSharpFile) csharpParser.ParseFile();
            SandBox.CreateSandBoxFor(psiFile, primaryPsiModule);
            return psiFile;
        }

        private void AnalyzeTypeAndNamespaceHolder(ICSharpTypeAndNamespaceHolderDeclaration psiFile, AnalysesResults results)
        {
            psiFile.TypeDeclarations.ForEach(aType => AnalyzeType(aType, results));
            psiFile.NamespaceDeclarations.ForEach(psiFile1 => AnalyzeTypeAndNamespaceHolder(psiFile1, results));
        }

        private void AnalyzeType(ICSharpTypeDeclaration aType, AnalysesResults results)
        {
            aType.TypeDeclarations.OfType<ICSharpTypeDeclaration>().ForEach(innerType => AnalyzeType(innerType, results));
            if (aType is IInterfaceDeclaration)
                return;
            results.AnalyzedTypeNames.Add(aType.CLRName);
            results.AnalyzedContexts.Add(ContextAnalysis.Analyze(aType, _logger));
        }
    }
}