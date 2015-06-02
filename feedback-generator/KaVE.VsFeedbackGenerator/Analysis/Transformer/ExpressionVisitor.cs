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
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Collections;
using KaVE.VsFeedbackGenerator.Analysis.CompletionTarget;
using KaVE.VsFeedbackGenerator.Analysis.Util;
using KaVE.VsFeedbackGenerator.Utils.Names;

namespace KaVE.VsFeedbackGenerator.Analysis.Transformer
{
    public class ExpressionVisitor : TreeNodeVisitor<IList<IStatement>, IAssignableExpression>
    {
        private readonly UniqueVariableNameGenerator _nameGen;
        private readonly CompletionTargetMarker _marker;
        private readonly ToAssignableExpression _toAssignableExpr;
        private readonly ToAssignableReference _toAssignableRef;

        public ExpressionVisitor(UniqueVariableNameGenerator nameGen, CompletionTargetMarker marker)
        {
            _nameGen = nameGen;
            _marker = marker;
            _toAssignableExpr = new ToAssignableExpression(nameGen, marker);
            _toAssignableRef = new ToAssignableReference(nameGen);
        }

        public IAssignableExpression ToAssignableExpr(IVariableInitializer csExpr, IList<IStatement> body)
        {
            var exprInit = csExpr as IExpressionInitializer;
            if (exprInit == null || exprInit.Value == null)
            {
                return new UnknownExpression();
            }
            return exprInit.Value.Accept(this, body) ?? new UnknownExpression();
        }

        public IAssignableExpression ToAssignableExpr(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null ? new UnknownExpression() : csExpr.Accept(_toAssignableExpr, body);
        }

        public IAssignableReference ToAssignableRef(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null ? new UnknownReference() : csExpr.Accept(_toAssignableRef, body);
        }

        public ISimpleExpression ToSimpleExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null ? new UnknownExpression() : (ISimpleExpression) csExpr.Accept(_toAssignableExpr, body);
        }

        public ILoopHeaderExpression ToLoopHeaderExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null
                ? new UnknownExpression()
                : (ILoopHeaderExpression) csExpr.Accept(_toAssignableExpr, body) ?? new UnknownExpression();
        }

        public IVariableReference ToVariableRef(IUnaryExpression csExpr, IList<IStatement> body)
        {
            var refExpr = csExpr as IReferenceExpression;
            if (refExpr != null && refExpr.NameIdentifier != null)
            {
                return new VariableReference {Identifier = refExpr.NameIdentifier.Name};
            }

            return new VariableReference();
        }

        public override IAssignableExpression VisitDefaultExpression(IDefaultExpression expr, IList<IStatement> body)
        {
            return new ConstantValueExpression();
        }

        public override IAssignableExpression VisitCSharpLiteralExpression(ICSharpLiteralExpression litExpr,
            IList<IStatement> context)
        {
            var isNull = litExpr.ConstantValue.IsPureNull(CSharpLanguage.Instance);
            if (isNull)
            {
                return new NullExpression();
            }
            return new ConstantValueExpression();
        }

        public override IAssignableExpression VisitInvocationExpression(IInvocationExpression inv,
            IList<IStatement> body)
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
                        return new UnknownExpression();
                    }
                    var args = GetArgumentList(inv.ArgumentList, body);
                    return SSTUtil.InvocationExpression(callee, methodName, args);
                }
            }

            return new UnknownExpression();
        }

        public IList<ISimpleExpression> GetArgumentList(IArgumentList argumentListParam, IList<IStatement> body)
        {
            var args = Lists.NewList<ISimpleExpression>();
            foreach (var arg in argumentListParam.Arguments)
            {
                var toArgumentRef = new ToArgumentRef();
                var id = arg.Value.Accept(toArgumentRef, body);
                args.Add(id);
            }
            return args;
        }

        public override IAssignableExpression VisitReferenceExpression(IReferenceExpression expr,
            IList<IStatement> context)
        {
            var hasName = expr.NameIdentifier != null;
            var hasQualifier = expr.QualifierExpression != null;

            if (hasName)
            {
                var refName = expr.NameIdentifier.Name;
                if (expr == _marker.AffectedNode)
                {
                    return new CompletionExpression
                    {
                        Token = refName
                    };
                }
                return new ReferenceExpression {Reference = new VariableReference {Identifier = refName}};
            }
            return new UnknownExpression();
        }
    }
}