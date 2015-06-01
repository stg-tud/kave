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

using System.Collections.Generic;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;
using IStatement = KaVE.Commons.Model.SSTs.IStatement;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class BodyVisitor : TreeNodeVisitor<IList<IStatement>>
    {
        private readonly CompletionTargetMarker _marker;
        private readonly ExpressionVisitor _exprVisitor;

        private static ExpressionStatement EmptyCompletionExpression
        {
            get { return new ExpressionStatement {Expression = new CompletionExpression()}; }
        }

        public BodyVisitor(CompletionTargetMarker marker)
        {
            _marker = marker;
            _exprVisitor = new ExpressionVisitor(new UniqueVariableNameGenerator(), marker);
        }

        public override void VisitNode(ITreeNode node, IList<IStatement> context)
        {
            node.Children<ICSharpTreeNode>().ForEach(child => child.Accept(this, context));
        }

        #region statements

        public override void VisitBreakStatement(IBreakStatement stmt, IList<IStatement> body)
        {
            body.Add(new BreakStatement());
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration decl, IList<IStatement> body)
        {
            if (IsTargetMatch(decl, CompletionCase.EmptyCompletionBefore))
            {
                body.Add(EmptyCompletionExpression);
            }

            var id = decl.DeclaredName;
            var type = decl.Type.GetName();
            body.Add(SSTUtil.Declare(id, type));

            IAssignableExpression initializer = null;
            if (decl.Initial != null)
            {
                initializer = _exprVisitor.ToAssignableExpr(decl.Initial, body);
            }
            else if (_marker.AffectedNode == decl && _marker.Case == CompletionCase.Undefined)
            {
                initializer = new CompletionExpression();
            }

            if (initializer != null)
            {
                body.Add(SSTUtil.AssignmentToLocal(id, initializer));
            }

            if (decl == _marker.AffectedNode && _marker.Case == CompletionCase.EmptyCompletionAfter)
            {
                body.Add(EmptyCompletionExpression);
            }
        }

        private bool IsTargetMatch(ICSharpTreeNode o, CompletionCase completionCase)
        {
            return o == _marker.AffectedNode && _marker.Case == completionCase;
        }

        public override void VisitAssignmentExpression(IAssignmentExpression expr, IList<IStatement> body)
        {
            var isTarget = _marker.AffectedNode == expr;

            body.Add(
                new Assignment
                {
                    Reference =
                        expr.Dest != null ? _exprVisitor.ToAssignableRef(expr.Dest, body) : new UnknownReference(),
                    Expression =
                        isTarget
                            ? new CompletionExpression()
                            : _exprVisitor.ToAssignableExpr(expr.Source, body)
                });
        }

        public override void VisitExpressionStatement(IExpressionStatement stmt, IList<IStatement> body)
        {
            if (stmt.Expression != null)
            {
                var isAssignment = stmt.Expression is IAssignmentExpression;
                if (isAssignment)
                {
                    stmt.Expression.Accept(this, body);
                    // TODO repeat the same trick for invocation expressions
                }
                else
                {
                    body.Add(
                        new ExpressionStatement
                        {
                            Expression = stmt.Expression.Accept(_exprVisitor, body) ?? new UnknownExpression()
                        });
                }
            }
        }

        #endregion

        #region blocks

        public override void VisitIfStatement(IIfStatement stmt, IList<IStatement> body)
        {
            if (IsTargetMatch(stmt, CompletionCase.EmptyCompletionBefore))
            {
                body.Add(EmptyCompletionExpression);
            }
            var ifElseBlock = new IfElseBlock
            {
                Condition = _exprVisitor.ToSimpleExpression(stmt.Condition, body) ?? new UnknownExpression()
            };
            if (IsTargetMatch(stmt, CompletionCase.InBody))
            {
                ifElseBlock.Then.Add(EmptyCompletionExpression);
            }
            if (IsTargetMatch(stmt, CompletionCase.InElse))
            {
                ifElseBlock.Else.Add(EmptyCompletionExpression);
            }
            if (stmt.Then != null)
            {
                stmt.Then.Accept(this, ifElseBlock.Then);
            }
            if (stmt.Else != null)
            {
                stmt.Else.Accept(this, ifElseBlock.Else);
            }

            body.Add(ifElseBlock);

            if (IsTargetMatch(stmt, CompletionCase.EmptyCompletionAfter))
            {
                body.Add(EmptyCompletionExpression);
            }
        }

        public override void VisitWhileStatement(IWhileStatement rsLoop, IList<IStatement> body)
        {
            if (_marker.AffectedNode == rsLoop && _marker.Case == CompletionCase.EmptyCompletionBefore)
            {
                body.Add(EmptyCompletionExpression);
            }

            var loop = new WhileLoop
            {
                Condition = _exprVisitor.ToLoopHeaderExpression(rsLoop.Condition, body)
            };

            body.Add(loop);

            rsLoop.Body.Accept(this, loop.Body);

            if (_marker.AffectedNode == rsLoop && _marker.Case == CompletionCase.InBody)
            {
                loop.Body.Add(EmptyCompletionExpression);
            }

            if (_marker.AffectedNode == rsLoop && _marker.Case == CompletionCase.EmptyCompletionAfter)
            {
                body.Add(EmptyCompletionExpression);
            }
        }

        #endregion
    }
}