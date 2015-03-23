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
using System.Linq;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
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
        private readonly ToAssignableExpression _toAssignableExpr;
        private readonly ToAssignableReference _toAssignableRef;

        private readonly CompletionTargetMarker _marker;

        public BodyVisitor(CompletionTargetMarker marker)
        {
            _marker = marker;
            _toAssignableExpr = new ToAssignableExpression(new UniqueVariableNameGenerator(), marker);
            _toAssignableRef = new ToAssignableReference(new UniqueVariableNameGenerator());
        }

        public override void VisitNode(ITreeNode node, IList<IStatement> context)
        {
            node.Children<ICSharpTreeNode>().ForEach(child => child.Accept(this, context));
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration decl, IList<IStatement> body)
        {
            if (decl == _marker.AffectedNode && _marker.Case == CompletionCase.EmptyCompletionBefore)
            {
                body.Add(new ExpressionStatement {Expression = new CompletionExpression()});
            }

            var id = decl.DeclaredName;
            var type = decl.Type.GetName();
            body.Add(SSTUtil.Declare(id, type));

            IAssignableExpression initializer = null;
            if (decl.Initial != null)
            {
                initializer = decl.Initial.Accept(_toAssignableExpr, body);
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
                body.Add(new ExpressionStatement {Expression = new CompletionExpression()});
            }
        }

        public override void VisitAssignmentExpression(IAssignmentExpression expr, IList<IStatement> body)
        {
            var isTarget = _marker.AffectedNode == expr;

            body.Add(
                new Assignment
                {
                    Reference = expr.Dest != null ? expr.Dest.Accept(_toAssignableRef, body) : new UnknownReference(),
                    Expression =
                        isTarget
                            ? new CompletionExpression()
                            : expr.Source != null
                                ? expr.Source.Accept(_toAssignableExpr, body)
                                : new UnknownExpression()
                });
        }

        public override void VisitIfStatement(IIfStatement stmt, IList<IStatement> body)
        {
            var condition = stmt.Condition == null
                ? new UnknownExpression()
                : (ISimpleExpression) stmt.Condition.Accept(_toAssignableExpr, body);
            var ifElseBlock = new IfElseBlock
            {
                // TODO introduce new visitor for ISimpleExpressions
                Condition = condition
            };
            if (_marker.AffectedNode == stmt && _marker.Case == CompletionCase.InBody)
            {
                ifElseBlock.Then.Add(new ExpressionStatement {Expression = new CompletionExpression()});
            }
            //if (_marker.AffectedNode == stmt && _marker.Case == CompletionCase.InElse)
            //{
            //    ifElseBlock.Else.Add(new ExpressionStatement { Expression = new CompletionExpression() });
            //}
            if (stmt.Then != null)
            {
                stmt.Then.Accept(this, ifElseBlock.Then);
            }
            if (stmt.Else != null)
            {
                stmt.Else.Accept(this, ifElseBlock.Else);
            }

            body.Add(ifElseBlock);
        }

        public override void VisitInvocationExpression(IInvocationExpression inv, IList<IStatement> body)
        {
            var invokedExpression = inv.InvokedExpression as IReferenceExpression;
            if (inv.Reference != null && invokedExpression != null)
            {
                var resolvedMethod = inv.Reference.ResolveMethod();
                if (resolvedMethod != null)
                {
                    var methodName = resolvedMethod.GetName<IMethodName>();
                    string callee = null;
                    if (invokedExpression.QualifierExpression == null ||
                        invokedExpression.QualifierExpression is IThisExpression)
                    {
                        callee = "this";
                    }
                    else if (invokedExpression.QualifierExpression is IBaseExpression)
                    {
                        callee = "base";
                    }
                    else if (invokedExpression.QualifierExpression is IReferenceExpression)
                    {
                        var referenceExpression = invokedExpression.QualifierExpression as IReferenceExpression;
                        if (referenceExpression.IsClassifiedAsVariable)
                        {
                            callee = referenceExpression.NameIdentifier.Name;
                        }
                    }
                    else if (invokedExpression.QualifierExpression is IInvocationExpression)
                    {
                        callee = invokedExpression.QualifierExpression.GetReference(null);
                    }
                    else
                    {
                        return;
                    }
                    var args =
                        GetArgumentList(inv.ArgumentList, body)
                            .Select<string, ISimpleExpression>(
                                id => new ReferenceExpression {Reference = new VariableReference {Identifier = id}})
                            .AsArray();
                    body.Add(
                        new ExpressionStatement {Expression = SSTUtil.InvocationExpression(callee, methodName, args)});
                }
            }
        }

        public IList<string> GetArgumentList(IArgumentList argumentListParam, IList<IStatement> body)
        {
            var args = Lists.NewList<string>();
            foreach (var arg in argumentListParam.Arguments)
            {
                var toArgumentRef = new ToArgumentRef();
                var id = arg.Value.Accept(toArgumentRef, body);
                // TODO: fix this hacky solution!
                args.Add(id ?? "%UNRESOLVED%");
            }
            return args;
        }
    }

    public class ToArgumentRef : TreeNodeVisitor<IList<IStatement>, string>
    {
        public override string VisitInvocationExpression(IInvocationExpression expr, IList<IStatement> body)
        {
            var invoked = expr.InvokedExpression as IReferenceExpression;

            if (invoked != null && expr.Reference != null)
            {
                var returnType = TypeName.UnknownName;
                var resolvedMethod = expr.Reference.ResolveMethod();
                if (resolvedMethod != null)
                {
                    var methodName = resolvedMethod.GetName<IMethodName>();
                    returnType = methodName.ReturnType;
                }

                var varName = "%UNKNOWN_VAR_NAME%";
                var qualifier = invoked.QualifierExpression as IReferenceExpression;
                if (qualifier != null)
                {
                    varName = qualifier.NameIdentifier.Name;
                }

                body.Add(
                    new VariableDeclaration
                    {
                        Type = returnType,
                        Reference = new VariableReference {Identifier = varName}
                    });
                body.Add(
                    new Assignment
                    {
                        Reference = new VariableReference {Identifier = varName},
                        Expression = new InvocationExpression()
                    });
                return varName;
            }
            return "%ERROR%";
        }

        public override string VisitReferenceExpression(IReferenceExpression expr, IList<IStatement> body)
        {
            return expr.NameIdentifier.Name;
        }

        public override string VisitThisExpression(IThisExpression thisExpressionParam, IList<IStatement> context)
        {
            return "this";
        }
    }
}