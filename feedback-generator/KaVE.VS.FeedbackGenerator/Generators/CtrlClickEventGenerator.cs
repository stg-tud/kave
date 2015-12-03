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

using System.Collections.Specialized;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.Interop.WinApi;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Utils.Names;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    [SolutionComponent]
    internal class CtrlClickEventGenerator : EventGeneratorBase
    {
        private readonly Lifetime _myLifetime;
        private readonly ITreeNodeProvider _treeNodeProvider;

        public CtrlClickEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ITextControlManager textControlManager,
            [NotNull] ITreeNodeProvider treeNodeProvider,
            [NotNull] Lifetime lifetime) : base(env, messageBus, dateUtils)
        {
            _myLifetime = lifetime;
            _treeNodeProvider = treeNodeProvider;

            foreach (var textControl in textControlManager.TextControls)
            {
                AdviceOnClick(textControl);
            }

            textControlManager.TextControls.CollectionChanged += AdviceOnNewControls;
        }

        public void OnClick(TextControlMouseEventArgs args)
        {
            if (args.KeysAndButtons == KeyStateMasks.MK_CONTROL)
            {
                var clickedNode = _treeNodeProvider.GetTreeNode(args.TextControl);

                var ctrlClickEvent = Create<NavigationEvent>();
                ctrlClickEvent.Target = GetTarget(clickedNode);
                ctrlClickEvent.Location = GetLocation(clickedNode);
                ctrlClickEvent.TriggeredBy = IDEEvent.Trigger.Click;

                Fire(ctrlClickEvent);
            }
        }

        private void AdviceOnNewControls(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (var newTextControl in args.NewItems.Cast<ITextControl>())
                {
                    AdviceOnClick(newTextControl);
                }
            }
        }

        private void AdviceOnClick(ITextControl textControl)
        {
            textControl.Window.MouseUp.Advise(_myLifetime, OnClick);
        }

        [Pure, NotNull]
        private static IName GetTarget(ITreeNode clickedNode)
        {
            var targetName = Name.UnknownName;

            var reference = clickedNode.GetContainingNode<IReferenceExpression>(true);
            if (reference != null)
            {
                var resolvedReference = reference.Reference.Resolve();
                var declaredElement = resolvedReference.DeclaredElement;
                if (declaredElement != null)
                {
                    targetName = declaredElement.GetName(declaredElement.GetIdSubstitutionSafe());
                }
            }
            else
            {
                var declaration = clickedNode.GetContainingNode<IDeclaration>();
                if (declaration != null && declaration.DeclaredElement != null)
                {
                    var declaredElement = declaration.DeclaredElement;
                    targetName = declaredElement.GetName(declaredElement.GetIdSubstitutionSafe());
                }
            }

            return targetName;
        }

        [Pure, NotNull]
        private static IName GetLocation(ITreeNode clickedNode)
        {
            var locationName = Name.UnknownName;

            var surroundingMethodDeclaration = clickedNode.GetContainingNode<IMethodDeclaration>();
            if (surroundingMethodDeclaration != null)
            {
                locationName = surroundingMethodDeclaration.GetName();
            }
            else
            {
                var surroundingTypeDeclaration = clickedNode.GetContainingNode<ITypeDeclaration>();
                if (surroundingTypeDeclaration != null && surroundingTypeDeclaration.DeclaredElement != null)
                {
                    locationName = surroundingTypeDeclaration.DeclaredElement.GetName();
                }
            }

            return locationName;
        }
    }

    public interface ITreeNodeProvider
    {
        ITreeNode GetTreeNode(ITextControl textControl);
    }

    [SolutionComponent]
    internal class TreeNodeProvider : ITreeNodeProvider
    {
        [NotNull]
        private readonly ISolution _solution;

        public TreeNodeProvider([NotNull] ISolution solution)
        {
            _solution = solution;
        }

        [Pure]
        public ITreeNode GetTreeNode(ITextControl textControl)
        {
            return TextControlToPsi.GetElement<ITreeNode>(_solution, textControl);
        }
    }
}