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
using KaVE.Commons.Utils.Collections;
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
                var assignment = stmt.Expression as IAssignmentExpression;
                var prefix = stmt.Expression as IPrefixOperatorExpression;
                var postfix = stmt.Expression as IPostfixOperatorExpression;
                if (assignment != null)
                {
                    assignment.Accept(this, body);
                    // TODO repeat the same trick for invocation expressions
                }
                else if (prefix != null)
                {
                    prefix.Accept(this, body);
                }
                else if (postfix != null)
                {
                    postfix.Accept(this, body);
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

        public override void VisitPrefixOperatorExpression(IPrefixOperatorExpression expr, IList<IStatement> body)
        {
            if (IsTargetMatch(expr, CompletionCase.EmptyCompletionBefore))
            {
                body.Add(EmptyCompletionExpression);
            }
            var varRef = _exprVisitor.ToVariableRef(expr.Operand, body);
            body.Add(
                new Assignment
                {
                    Reference = varRef,
                    Expression = new ComposedExpression
                    {
                        References = {varRef}
                    }
                });
            if (IsTargetMatch(expr, CompletionCase.EmptyCompletionAfter))
            {
                body.Add(EmptyCompletionExpression);
            }
        }

        public override void VisitPostfixOperatorExpression(IPostfixOperatorExpression expr, IList<IStatement> body)
        {
            if (IsTargetMatch(expr, CompletionCase.EmptyCompletionBefore))
            {
                body.Add(EmptyCompletionExpression);
            }
            var varRef = _exprVisitor.ToVariableRef(expr.Operand, body);
            body.Add(
                new Assignment
                {
                    Reference = varRef,
                    Expression = new ComposedExpression
                    {
                        References = {varRef}
                    }
                });
            if (IsTargetMatch(expr, CompletionCase.EmptyCompletionAfter))
            {
                body.Add(EmptyCompletionExpression);
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

        public override void VisitForStatement(IForStatement stmt, IList<IStatement> body)
        {
            if (IsTargetMatch(stmt, CompletionCase.EmptyCompletionBefore))
            {
                body.Add(EmptyCompletionExpression);
            }

            var forLoop = new ForLoop();
            body.Add(forLoop);

            if (IsTargetMatch(stmt, CompletionCase.InBody))
            {
                forLoop.Body.Add(EmptyCompletionExpression);
            }

            VisitForStatement_Init(stmt.Initializer, forLoop.Init, body);
            forLoop.Condition = _exprVisitor.ToLoopHeaderExpression(stmt.Condition, body);
            foreach (var expr in stmt.IteratorExpressionsEnumerable)
            {
                expr.Accept(this, forLoop.Step);
            }

            if (stmt.Body != null)
            {
                stmt.Body.Accept(this, forLoop.Body);
            }

            if (IsTargetMatch(stmt, CompletionCase.EmptyCompletionAfter))
            {
                body.Add(EmptyCompletionExpression);
            }
        }

        private void VisitForStatement_Init(IForInitializer init, IKaVEList<IStatement> forInit, IList<IStatement> body)
        {
            if (init == null)
            {
                return;
            }

            // case 1: single declaration
            var isDeclaration = init.Declaration != null;
            if (isDeclaration)
            {
                var decl = init.Declaration.Declarators[0];
                decl.Accept(this, forInit);
            }

            // case 2: multiple statements
            var hasStatements = init.Expressions.Count > 0;
            if (hasStatements)
            {
                foreach (var expr in init.ExpressionsEnumerable)
                {
                    expr.Accept(this, forInit);
                }
            }
        }

        #endregion
    }
}