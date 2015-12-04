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

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.VS.FeedbackGenerator.Generators.Navigation
{
    public interface INavigationUtils
    {
        [NotNull]
        IName GetTarget(ITextControl textControl);

        [NotNull]
        IName GetLocation(ITextControl textControl);

        [NotNull]
        IName GetTarget(ITreeNode psiNode);

        [NotNull]
        IName GetLocation(ITreeNode psiNode);
    }

    [SolutionComponent]
    internal class NavigationUtils : INavigationUtils
    {
        [NotNull]
        private readonly ISolution _solution;

        public NavigationUtils([NotNull] ISolution solution)
        {
            _solution = solution;
        }

        [Pure]
        public IName GetTarget(ITextControl textControl)
        {
            return GetTarget(GetTreeNode(textControl));
        }

        [Pure]
        public IName GetLocation(ITextControl textControl)
        {
            return GetLocation(GetTreeNode(textControl));
        }

        [Pure]
        public IName GetTarget(ITreeNode psiNode)
        {
            var targetName = Name.UnknownName;

            var reference = psiNode.GetContainingNode<IReferenceExpression>(true);
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
                var declaration = psiNode.GetContainingNode<IDeclaration>();
                if (declaration != null && declaration.DeclaredElement != null)
                {
                    var declaredElement = declaration.DeclaredElement;
                    targetName = declaredElement.GetName(declaredElement.GetIdSubstitutionSafe());
                }
            }

            return targetName;
        }

        [Pure]
        public IName GetLocation(ITreeNode psiNode)
        {
            var locationName = Name.UnknownName;

            var surroundingMethodDeclaration = psiNode.GetContainingNode<IMethodDeclaration>();
            if (surroundingMethodDeclaration != null)
            {
                locationName = surroundingMethodDeclaration.GetName();
            }
            else
            {
                var surroundingTypeDeclaration = psiNode.GetContainingNode<ITypeDeclaration>();
                if (surroundingTypeDeclaration != null && surroundingTypeDeclaration.DeclaredElement != null)
                {
                    locationName = surroundingTypeDeclaration.DeclaredElement.GetName();
                }
            }

            return locationName;
        }

        [Pure]
        private ITreeNode GetTreeNode(ITextControl textControl)
        {
            return TextControlToPsi.GetElement<ITreeNode>(_solution, textControl);
        }
    }
}