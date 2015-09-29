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
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Analysis;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VS.FeedbackGenerator.Generators.VisualStudio.EditEventGenerators.EventContext
{
    [SolutionComponent]
    internal class ContextGenerator
    {
        private Context CurrentContext { get; set; }

        private readonly TextControlManager _textControlManager;
        private readonly DocumentManager _documentManager;
        private readonly ISolution _solution;
        private readonly ILogger _logger;

        public ContextGenerator(TextControlManager textControlManager,
            IntellisenseManager intellisenseManager,
            ILogger logger)
        {
            _textControlManager = textControlManager;
            _documentManager = intellisenseManager.DocumentManager;
            _logger = logger;
            _solution = intellisenseManager.Solution;

            if (NewContextProvider != null)
            {
                NewContextProvider(this);
            }
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
            CurrentContext = Context.Default;

            var document = GetDocument(filePath);
            if (document != null)
            {
                var node = FindEntryNode(document);
                if (node != null)
                {
                    RunAnalysis(node);
                }
            }
        }

        private IDocument GetDocument(string filePath)
        {
            try
            {
                var path = FileSystemPath.Parse(filePath);
                return _documentManager.GetOrCreateDocument(path);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "failed to access the current document");
            }
            return null;
        }

        private ITreeNode FindEntryNode(IDocument document)
        {
            ITreeNode node = null;
            ReadLockCookie.Execute(
                () =>
                {
                    node = FindCurrentTreeNode(document);

                    if (node == null)
                    {
                        return;
                    }

                    if (!HasSourroundingMethod(node))
                    {
                        node = FindSourroundingClassDeclaration(node);
                    }
                });
            return node;
        }

        private ITreeNode FindCurrentTreeNode(IDocument document)
        {
            var textControl =
                _textControlManager.TextControls.FirstOrDefault(
                    tc => tc.Document.Moniker.Equals(document.Moniker));
            return textControl == null ? null : TextControlToPsi.GetElement<ITreeNode>(_solution, textControl);
        }

        private static bool HasSourroundingMethod(ITreeNode node)
        {
            var method = node.GetContainingNode<IMethodDeclaration>(true);
            return method != null;
        }

        private static IClassDeclaration FindSourroundingClassDeclaration(ITreeNode psiFile)
        {
            return psiFile.GetContainingNode<IClassDeclaration>(true);
        }

        private void RunAnalysis(ITreeNode node)
        {
            ContextAnalysis.Analyse(
                node,
                _logger,
                context => { CurrentContext = context; },
                delegate { },
                delegate { });
        }
    }
}