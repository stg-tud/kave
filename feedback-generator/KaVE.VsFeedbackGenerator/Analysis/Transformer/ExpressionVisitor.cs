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
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Assertion;
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
        private readonly ToAssignableReference _toAssignableRef;

        public ExpressionVisitor(UniqueVariableNameGenerator nameGen, CompletionTargetMarker marker)
        {
            _nameGen = nameGen;
            _marker = marker;
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
            return csExpr == null
                ? new UnknownExpression()
                : csExpr.Accept(this, body) ?? new UnknownExpression();
        }

        public IAssignableReference ToAssignableRef(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null
                ? new UnknownReference()
                : csExpr.Accept(_toAssignableRef, body) ?? new UnknownReference();
        }

        public ISimpleExpression ToSimpleExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null
                ? new UnknownExpression()
                : csExpr.Accept(this, body) as ISimpleExpression ?? new UnknownExpression();
        }

        public ILoopHeaderExpression ToLoopHeaderExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            return csExpr == null
                ? new UnknownExpression()
                : csExpr.Accept(this, body) as ILoopHeaderExpression ?? new UnknownExpression();
        }

        public IVariableReference ToVariableRef(ICSharpExpression csExpr, IList<IStatement> body)
        {
            if (csExpr == null)
            {
                return new VariableReference();
            }

            var refExpr = csExpr as IReferenceExpression;
            if (refExpr != null)
            {
                var hasName = refExpr.NameIdentifier != null;
                var isSimpleReference = refExpr.QualifierExpression == null;
                if (hasName && isSimpleReference)
                {
                    return VarRef(refExpr.NameIdentifier.Name);
                }
            }

            var expr = ToAssignableExpr(csExpr, body);
            var assigneeId = _nameGen.GetNextVariableName();
            var assigneeRef = VarRef(assigneeId);
            body.Add(new VariableDeclaration {Reference = assigneeRef, Type = TypeName.UnknownName});
            body.Add(
                new Assignment
                {
                    Reference = assigneeRef,
                    Expression = expr
                });

            return assigneeRef;
        }

        private static IVariableReference VarRef(string id)
        {
            return new VariableReference {Identifier = id};
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

        public override IAssignableExpression VisitExpressionInitializer(IExpressionInitializer exprInit,
            IList<IStatement> context)
        {
            return exprInit.Value.Accept(this, context);
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
                    var varRef = methodName.IsStatic
                        ? new VariableReference()
                        : FindVariableReference(invokedExpression);
                    var args = GetArgumentList(inv.ArgumentList, body);
                    return new InvocationExpression
                    {
                        Reference = varRef,
                        MethodName = methodName,
                        Parameters = args
                    };
                }
            }

            return new UnknownExpression();
        }

        private static VariableReference FindVariableReference(IReferenceExpression invokedExpression)
        {
            var varRef = new VariableReference();
            if (invokedExpression.QualifierExpression == null ||
                invokedExpression.QualifierExpression is IThisExpression)
            {
                varRef.Identifier = "this";
            }
            else if (invokedExpression.QualifierExpression is IBaseExpression)
            {
                varRef.Identifier = "base";
            }
            else if (invokedExpression.QualifierExpression is IReferenceExpression)
            {
                var referenceExpression = invokedExpression.QualifierExpression as IReferenceExpression;
                if (referenceExpression.IsClassifiedAsVariable)
                {
                    varRef.Identifier = referenceExpression.NameIdentifier.Name;
                }
            }
            else if (invokedExpression.QualifierExpression is IInvocationExpression)
            {
                varRef.Identifier = invokedExpression.QualifierExpression.GetReference(null);
            }
            return varRef;
        }

        public override IAssignableExpression VisitObjectCreationExpression(IObjectCreationExpression expr,
            IList<IStatement> body)
        {
            var r = expr.ConstructorReference.Resolve();
            if (r.IsValid() && r.DeclaredElement != null)
            {
                var methodName = r.DeclaredElement.GetName<IMethodName>(r.Result.Substitution);
                Asserts.That(methodName.IsConstructor);

                var parameters = Lists.NewList<ISimpleExpression>();
                foreach (var argument in expr.Arguments)
                {
                    var parameter = ToSimpleExpression(argument.Value, body);
                    parameters.Add(parameter);
                }

                return new InvocationExpression
                {
                    MethodName = methodName,
                    Parameters = parameters
                };
            }
            return new UnknownExpression();
        }

        public IKaVEList<ISimpleExpression> GetArgumentList(IArgumentList argumentListParam, IList<IStatement> body)
        {
            var args = Lists.NewList<ISimpleExpression>();
            foreach (var arg in argumentListParam.Arguments)
            {
                var argExpr = ToSimpleExpression(arg.Value, body) ?? new UnknownExpression();
                args.Add(argExpr);
            }
            return args;
        }

        public override IAssignableExpression VisitReferenceExpression(IReferenceExpression expr,
            IList<IStatement> context)
        {
            var name = expr.NameIdentifier != null ? expr.NameIdentifier.Name : "";
            var hasQualifier = expr.QualifierExpression != null;

            IVariableReference varRef = null;
            if (hasQualifier)
            {
                varRef = ToVariableRef(expr.QualifierExpression, context);
            }

            if (expr == _marker.AffectedNode)
            {
                return new CompletionExpression
                {
                    Token = name,
                    VariableReference = varRef
                };
            }

            if (varRef == null)
            {
                return new ReferenceExpression
                {
                    Reference = new VariableReference
                    {
                        Identifier = name
                    },
                };
            }
            return new ReferenceExpression
            {
                Reference = new FieldReference
                {
                    Reference = varRef,
                    FieldName = FieldName.Get(string.Format("[{0}] [{0}].{1}", TypeName.UnknownName, name))
                }
            };
        }
    }
}