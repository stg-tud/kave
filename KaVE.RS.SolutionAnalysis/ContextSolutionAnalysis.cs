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
 */

using System;
using System.Linq;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.RS.Commons.Analysis;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.RS.SolutionAnalysis
{
    public class ContextSolutionAnalysis : BaseSolutionAnalysis
    {
        private readonly ILogger _logger;
        private readonly Action<Context> _cbContext;

        public ContextSolutionAnalysis(ISolution solution, ILogger logger, Action<Context> cbContext)
            : base(solution, logger)
        {
            _logger = logger;
            _cbContext = cbContext;
        }

        protected override void AnalyzePrimaryPsiModule(IPsiModule primaryPsiModule)
        {
            foreach (var file in primaryPsiModule.SourceFiles)
            {
                var isCSharpFile = file.LanguageType.Is<CSharpProjectFileType>();
                if (isCSharpFile)
                {
                    AnalyzeFile(file, primaryPsiModule);
                }
            }
        }

        private void AnalyzeFile(IPsiSourceFile psiSourceFile, IPsiModule primaryPsiModule)
        {
            _logger.Info("");
            _logger.Info("--- Analyzing file '{0}'... -------------", psiSourceFile.DisplayName);
            var psiFile = ParseFile(psiSourceFile, primaryPsiModule);
            AnalyzeTypeAndNamespaceHolder(psiFile, psiSourceFile);
        }

        private static ICSharpFile ParseFile(IPsiSourceFile psiSourceFile, IPsiModule primaryPsiModule)
        {
            var languageService =
                CSharpLanguage.Instance.LanguageService().NotNull("CSharp language service not available");
            ILexer lexer = languageService.CreateCachingLexer(psiSourceFile.Document.Buffer);
            var csharpParser = (ICSharpParser) languageService.CreateParser(lexer, primaryPsiModule, psiSourceFile);
            csharpParser.SetChameleonExpansionMode(true);
            var psiFile = (ICSharpFile) csharpParser.ParseFile();
            SandBox.CreateSandBoxFor(psiFile, primaryPsiModule);
            return psiFile;
        }

        private void AnalyzeTypeAndNamespaceHolder(ICSharpTypeAndNamespaceHolderDeclaration psiFile,
            IPsiSourceFile psiSourceFile)
        {
            foreach (var typeDecl in psiFile.TypeDeclarations)
            {
                AnalyzeType(typeDecl, psiSourceFile);
            }
            foreach (var nsDecl in psiFile.NamespaceDeclarations)
            {
                AnalyzeTypeAndNamespaceHolder(nsDecl, psiSourceFile);
            }
        }

        private void AnalyzeType(ICSharpTypeDeclaration aType, IPsiSourceFile psiSourceFile)
        {
            _logger.Info("Analyzing type '{0}'...", aType.CLRName);

            try
            {
                var ctx = ContextAnalysis.Analyze(aType, psiSourceFile, _logger).Context;
                _cbContext(ctx);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            foreach (var innerType in aType.TypeDeclarations.OfType<ICSharpTypeDeclaration>())
            {
                AnalyzeType(innerType, psiSourceFile);
            }
        }
    }
}