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
 *    - Sebastian Proksch
 */

using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace KaVE.VsFeedbackGenerator.Analysis.CompletionTarget
{
    public class CompletionTargetAnalysis
    {
        public CompletionTargetMarker Analyze(ITreeNode targetNode)
        {
            var finder = new TargetFinder();
            ((ICSharpTreeNode) targetNode).Accept(finder);
            return finder.Result;
        }

        //private static IName GetName(IReference reference)
        //{
        //    var resolvedReference = reference.Resolve();
        //    var result = resolvedReference.Result;
        //    var declaredElement = result.DeclaredElement;
        //    return declaredElement != null ? declaredElement.GetName(result.Substitution) : null;
        //}

        private class TargetFinder : TreeNodeVisitor
        {
            public CompletionTargetMarker Result { get; private set; }

            public TargetFinder()
            {
                Result = new CompletionTargetMarker();
            }

            public override void VisitNode(ITreeNode tNode)
            {
                var target = tNode as ICSharpTreeNode;
                if (target == null)
                {
                    // TODO handle?!
                    return;
                }

                var isAssign = CSharpTokenType.EQ == tNode.GetTokenType();

                if (target.IsWhitespaceToken() || isAssign)
                {
                    FindAvailableTarget(target);
                }

                if (isAssign)
                {
                    Result.Case = CompletionCase.Undefined;
                }

                if (target is IIdentifier)
                {
                    Result.AffectedNode = target.Parent as ICSharpTreeNode;
                    Result.Case = CompletionCase.Undefined;
                }
            }

            private void FindAvailableTarget(ICSharpTreeNode target)
            {
                var isOutsideMethodBody = target.Parent is IClassBody;
                if (isOutsideMethodBody)
                {
                    // TODO think about this simplification...
                    Result.AffectedNode = null;
                    Result.Case = CompletionCase.Undefined;
                    return;
                }

                var prev = FindPrevNonWhitespaceNode(target.PrevSibling);
                var next = FindNextNonWhitespaceNode(target.NextSibling);

                if (prev != null)
                {
                    var expr = prev as IExpressionStatement;
                    var decl = prev as IDeclarationStatement;

                    if (expr != null)
                    {
                        Result.AffectedNode = prev.FirstChild as ICSharpTreeNode;
                        Result.Case = CompletionCase.EmptyCompletionAfter;
                    }
                    else if (decl != null && HasError(prev))
                    {
                        Result.Case = CompletionCase.Undefined;
                        var multi = decl.Declaration as IMultipleLocalVariableDeclaration;
                        Result.AffectedNode = multi != null ? multi.DeclaratorsEnumerable.Last() : prev;
                    }

                    else if (decl != null)
                    {
                        Result.Case = CompletionCase.EmptyCompletionAfter;
                        var multi = decl.Declaration as IMultipleLocalVariableDeclaration;
                        Result.AffectedNode = multi != null ? multi.DeclaratorsEnumerable.Last() : prev;
                    }
                    else
                    {
                        Result.AffectedNode = prev;
                        Result.Case = CompletionCase.EmptyCompletionAfter;
                    }
                }
                else if (next != null)
                {
                    var decl = next as IDeclarationStatement;

                    if (decl != null)
                    {
                        Result.Case = CompletionCase.EmptyCompletionBefore;
                        var multi = decl.Declaration as IMultipleLocalVariableDeclaration;
                        Result.AffectedNode = multi != null ? multi.DeclaratorsEnumerable.Last() : next;
                    }
                    else
                    {
                        Result.AffectedNode = next;
                        Result.Case = CompletionCase.EmptyCompletionBefore;
                    }
                }
                else
                {
                    Result.AffectedNode = FindNonBlockParent(target);
                    Result.Case = CompletionCase.InBody;
                }
            }

            private static bool HasError(ICSharpTreeNode prev)
            {
                return prev.LastChild is IErrorElement;
            }

            private ICSharpTreeNode FindNonBlockParent(ICSharpTreeNode target)
            {
                var parent = target.Parent as ICSharpTreeNode;

                if (parent is IChameleonNode)
                {
                    var methDecl = parent.Parent as IMethodDeclaration;
                    if (methDecl != null)
                    {
                        return methDecl;
                    }
                }

                var block = parent as IBlock;
                if (block != null)
                {
                    var parentStatement = block.Parent as ICSharpStatement;
                    if (parentStatement != null)
                    {
                        return parentStatement;
                    }

                    // TODO: why is the following needed?
                    FindAvailableTarget(block);
                    return Result.AffectedNode;
                }
                return parent;
            }

            private static ICSharpTreeNode FindNextNonWhitespaceNode(ITreeNode node)
            {
                if (node == null || node.NextSibling == null)
                {
                    return null;
                }
                if (node.IsWhitespaceToken())
                {
                    node = FindNextNonWhitespaceNode(node.NextSibling);
                }
                return node as ICSharpTreeNode;
            }

            private ICSharpTreeNode FindPrevNonWhitespaceNode(ITreeNode node)
            {
                if (node == null || node.PrevSibling == null)
                {
                    return null;
                }
                if (node.IsWhitespaceToken())
                {
                    node = FindPrevNonWhitespaceNode(node.PrevSibling);
                }
                return node as ICSharpTreeNode;
            }

            public override void VisitClassDeclaration(IClassDeclaration classDecl)
            {
                //Result.Parent = classDecl;
                // TODO add type for type completion
            }

            public override void VisitMethodDeclaration(IMethodDeclaration methodDeclarationParam)
            {
                //Result.Parent = methodDeclarationParam;
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
                        //Result.Completion = new CompletionExpression
                        //{
                        //    VariableReference = SSTUtil.VariableReference(refName),
                        //    Token = token
                        //};
                    }
                    else
                    {
                        var token = refExpr.Reference.GetName();
                        //Result.Completion = new CompletionExpression {Token = token};
                    }
                }
            }
        }
    }
}