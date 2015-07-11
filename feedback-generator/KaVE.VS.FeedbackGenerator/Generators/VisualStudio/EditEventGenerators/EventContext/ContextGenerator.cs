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
using EnvDTE;
using JetBrains.Application.Components;
using JetBrains.DocumentManagers;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Exceptions;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Analysis;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext
{
    [SolutionComponent(ProgramConfigurations.VS_ADDIN)]
    internal class ContextGenerator
    {
        public const int LimitInMs = 1000;

        private Context CurrentContext { get; set; }

        private readonly TextControlManager _textControlManager;
        private readonly DocumentManager _documentManager;
        private readonly ISolution _solution;
        private readonly ILogger _logger;
        private readonly KaVECancellationTokenSource _tokenSource;

        public ContextGenerator(TextControlManager textControlManager,
            IntellisenseManager intellisenseManager,
            ILogger logger)
        {
            CurrentContext = new Context();

            _textControlManager = textControlManager;
            _documentManager = intellisenseManager.DocumentManager;
            _logger = logger;
            _solution = intellisenseManager.Solution;

            _tokenSource = new KaVECancellationTokenSource();

            NewContextProvider(this);
        }

        public delegate void ContextProviderChangedHandler(object sender);

        public static event ContextProviderChangedHandler NewContextProvider;

        public Context GetCurrentContext([NotNull] Document vsDocument)
        {
            ComputeNewContextByFilePath(vsDocument.FullName);
            return CurrentContext;
        }

        public Context GetCurrentContext([NotNull] TextPoint startPoint)
        {
            ComputeNewContextByFilePath(startPoint.DTE.ActiveDocument.FullName);
            return CurrentContext;
        }

        private void ComputeNewContextByFilePath([NotNull] string filePath)
        {
            try
            {
                ComputeNewContext(_documentManager.GetOrCreateDocument(FileSystemPath.Parse(filePath)));
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "parsing file path for generating new context failed");
            }
        }

        public void ComputeNewContext([NotNull] IDocument document)
        {
            var textControl =
                _textControlManager.TextControls.FirstOrDefault(tc => tc.Document.Moniker.Equals(document.Moniker));
            if (textControl != null)
            {
                var psiFile = TextControlToPsi.GetElement<ITreeNode>(_solution, textControl);
                if (psiFile != null)
                {
                    RunAnalysis(psiFile);

                    if (CurrentContext.Equals(new Context()))
                    {
                        FallbackAnalysisOnClassDeclaration(psiFile);
                    }

                    return;
                }
            }

            CurrentContext = new Context();
        }

        private void FallbackAnalysisOnClassDeclaration(ITreeNode psiFile)
        {
            var classDeclaration = psiFile.GetContainingNode<IClassDeclaration>(true);
            if (classDeclaration != null)
            {
                RunAnalysis(classDeclaration);
            }
        }

        private void RunAnalysis(ITreeNode psiFile)
        {
            var token = _tokenSource.CancelAndCreate();

            Func<Context> analysis = () =>
            {
                Context result = null;
                ReadLockCookie.Execute(
                    () =>
                    {
                        result =
                            ContextAnalysis.Analyze(
                                psiFile,
                                _logger).Context;
                    });
                return result;
            };

            TimeLimitRunner.Run(
                analysis,
                LimitInMs,
                token,
                OnSuccess,
                OnTimeout,
                OnError);
        }

        public void OnSuccess(Context newContext)
        {
            CurrentContext = newContext;
        }

        public void OnTimeout()
        {
            CurrentContext = new Context();
            _logger.Error("timeout! analysis did not finish within {0}ms", LimitInMs);
        }

        public void OnError(Exception exception)
        {
            CurrentContext = new Context();
            _logger.Error(exception);
        }
    }
}