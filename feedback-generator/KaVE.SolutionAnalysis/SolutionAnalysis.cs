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

using System.Collections.Generic;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvents;
using KaVE.VsFeedbackGenerator.Analysis;
using ILogger = KaVE.Utils.Exceptions.ILogger;

namespace KaVE.SolutionAnalysis
{
    public class SolutionAnalysis
    {
        public class AnalysesResults
        {
            public readonly IList<Context> AnalyzedContexts = new List<Context>();
        }

        private readonly ISolution _solution;
        private readonly ILogger _logger;

        public SolutionAnalysis(ISolution solution, ILogger logger)
        {
            _solution = solution;
            _logger = logger;
        }

        /// <summary>
        /// Requires re-entrency guard (ReentrancyGuard.Current.Execute) and read lock (ReadLockCookie.Execute).
        /// </summary>
        public IList<Context> AnalyzeAllProjects()
        {
            var projects = _solution.GetAllProjects();
            projects.Remove(_solution.MiscFilesProject);
            projects.Remove(_solution.SolutionProject);
            // eager evaluation required, because caller context needs to asure guard/lock
            return projects.SelectMany(AnalyzeProject).ToList();
        }

        private IEnumerable<Context> AnalyzeProject(IProject project)
        {
            _logger.Info("Analyzing project '" + project.Name + "'...");

            var psiModules = _solution.PsiModules();
            var primaryPsiModule = psiModules.GetPrimaryPsiModule(project).NotNull("no psi module");
            var csharpSourceFiles = primaryPsiModule.SourceFiles.Where(IsCSharpFile);
            return csharpSourceFiles.SelectMany(file => AnalyzeFile(file, primaryPsiModule));
        }

        private static bool IsCSharpFile(IPsiSourceFile file)
        {
            return file.LanguageType.Is<CSharpProjectFileType>();
        }

        private IEnumerable<Context> AnalyzeFile(IPsiSourceFile psiSourceFile, IPsiModule primaryPsiModule)
        {
            _logger.Info(" - Analyzing file '" + psiSourceFile.DisplayName + "'...");

            var psiFile = ParseFile(psiSourceFile, primaryPsiModule);
            return AnalyzeTypeAndNamespaceHolder(psiFile);
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

        private IEnumerable<Context> AnalyzeTypeAndNamespaceHolder(ICSharpTypeAndNamespaceHolderDeclaration psiFile)
        {
            var contexts = new List<Context>();
            contexts.AddRange(psiFile.TypeDeclarations.SelectMany(AnalyzeType));
            contexts.AddRange(psiFile.NamespaceDeclarations.SelectMany(AnalyzeTypeAndNamespaceHolder));
            return contexts;
        }

        private IEnumerable<Context> AnalyzeType(ICSharpTypeDeclaration aType)
        {
            var contexts = new List<Context>();
            if (aType is IClassDeclaration || aType is IStructDeclaration)
            {
                _logger.Info("   - Analyzing type '" + aType.CLRName + "'...");

                contexts.Add(ContextAnalysis.Analyze(aType, _logger));
            }
            contexts.AddRange(AnalyzeInnerTypes(aType));
            return contexts;
        }

        private IEnumerable<Context> AnalyzeInnerTypes(ITypeDeclarationHolder aType)
        {
            return aType.TypeDeclarations.OfType<ICSharpTypeDeclaration>().SelectMany(AnalyzeType);
        }
    }
}