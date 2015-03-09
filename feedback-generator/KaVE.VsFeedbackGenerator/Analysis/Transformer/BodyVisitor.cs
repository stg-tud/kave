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
using KaVE.Model.Collections;
using KaVE.Model.Names;
using KaVE.Model.SSTs.Expressions;
using KaVE.Model.SSTs.Impl;
using KaVE.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Model.SSTs.Impl.References;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;
using IStatement = KaVE.Model.SSTs.IStatement;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class BodyVisitor : TreeNodeVisitor<IList<IStatement>>
    {
        private readonly CompletionTargetAnalysis.TriggerPointMarker _marker;
        private readonly ToBasicExpressionReducer _toBasicExprReducer;

        public BodyVisitor(CompletionTargetAnalysis.TriggerPointMarker marker)
        {
            _marker = marker;
            _toBasicExprReducer = new ToBasicExpressionReducer(new UniqueVariableNameGenerator());
        }

        public override void VisitNode(ITreeNode node, IList<IStatement> context)
        {
            node.Children<ICSharpTreeNode>().ForEach(child => child.Accept(this, context));
        }

        public override void VisitLocalVariableDeclaration(ILocalVariableDeclaration decl, IList<IStatement> context)
        {
            var id = decl.DeclaredName;
            var type = decl.Type.GetName();
            context.Add(SSTUtil.Declare(id, type));

            var isInitialized = decl.Initial != null;
            if (isInitialized)
            {
                var initialiser = decl.Initial.Accept(_toBasicExprReducer, context);
                // TODO
                context.Add(SSTUtil.AssignmentToLocal(id, null));
            }
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
                args.Add(id);
            }
            return args;
        }
    }

    public class ToArgumentRef : TreeNodeVisitor<IList<IStatement>, string>
    {
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