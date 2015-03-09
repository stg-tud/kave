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

using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions.Assignable;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis
{
    public class CompletionTargetAnalysis
    {
        public class TriggerPointMarker
        {
            public ICSharpTreeNode Parent { get; set; }
            public ICSharpTreeNode Predecessor { get; set; }
            public ICSharpTreeNode Expression { get; set; }
            public ICompletionExpression Completion { get; set; }
        }

        public TriggerPointMarker Analyze(ITreeNode targetNode)
        {
            var finder = new TargetFinder();
            ((ICSharpTreeNode) targetNode).Accept(finder);
            return finder.Result;
        }

        private static IName GetName(IReference reference)
        {
            var resolvedReference = reference.Resolve();
            var result = resolvedReference.Result;
            var declaredElement = result.DeclaredElement;
            return declaredElement != null ? declaredElement.GetName(result.Substitution) : null;
        }

        private class TargetFinder : TreeNodeVisitor
        {
            public TriggerPointMarker Result { get; private set; }

            public TargetFinder()
            {
                Result = new TriggerPointMarker();
            }

            public override void VisitNode(ITreeNode node)
            {
                Asserts.NotNull(node.Parent, "reached top of tree... missed something?");

                var parent = node.Parent as ICSharpTreeNode;
                if (parent != null)
                {
                    parent.Accept(this);
                }
            }

            public override void VisitClassDeclaration(IClassDeclaration classDecl)
            {
                Result.Parent = classDecl;
                // TODO add type for type completion
            }

            public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam)
            {
                Result.Parent = methodDeclarationParam;
                Result.Completion = new CompletionExpression();
            }

            public override void VisitReferenceExpression(IReferenceExpression refExpr)
            {
                var parent = refExpr.Parent as ICSharpTreeNode;
                if (parent != null)
                {
                    parent.Accept(this);

                    // in case of member access, refExpr.QualifierExpression and refExpr.Delimiter are set
                    var qRrefExpr = refExpr.QualifierExpression as IReferenceExpression;
                    if (qRrefExpr != null && refExpr.Delimiter != null)
                    {
                        var refName = qRrefExpr.Reference.GetName();
                        var token = refExpr.Reference.GetName();
                        Result.Completion = new CompletionExpression
                        {
                            ObjectReference = SSTUtil.VariableReference(refName),
                            Token = token
                        };
                    }
                    else
                    {
                        var token = refExpr.Reference.GetName();
                        Result.Completion = new CompletionExpression {Token = token};
                    }
                }
            }
        }
    }
}