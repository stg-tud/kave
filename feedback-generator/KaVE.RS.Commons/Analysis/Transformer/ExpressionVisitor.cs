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

using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.LoopHeader;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.Commons.Analysis.CompletionTarget;
using KaVE.RS.Commons.Analysis.Util;
using KaVE.RS.Commons.Utils.Names;

namespace KaVE.RS.Commons.Analysis.Transformer
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
            if (csExpr == null)
            {
                return new UnknownReference();
            }

            var reference = ToReference(csExpr, body);
            var assignableRef = reference as IAssignableReference ?? new UnknownReference();
            return assignableRef;
        }

        public ISimpleExpression ToSimpleExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            if (csExpr == null)
            {
                return new UnknownExpression();
            }

            var expr = csExpr.Accept(this, body);
            if (expr == null)
            {
                return new UnknownExpression();
            }

            if (expr is ISimpleExpression)
            {
                return expr as ISimpleExpression;
            }

            var newRef = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
            body.Add(
                new VariableDeclaration
                {
                    Reference = newRef,
                    Type = csExpr.GetExpressionType().ToIType().GetName()
                });
            body.Add(
                new Assignment
                {
                    Reference = newRef,
                    Expression = expr
                });
            return new ReferenceExpression {Reference = newRef};
        }

        public ILoopHeaderExpression ToLoopHeaderExpression(ICSharpExpression csExpr, IList<IStatement> body)
        {
            if (csExpr == null)
            {
                return new UnknownExpression();
            }

            var nestedBody = Lists.NewList<IStatement>();
            var expr = ToSimpleExpression(csExpr, nestedBody);

            if (nestedBody.Count == 0)
            {
                return expr;
            }

            nestedBody.Add(new ReturnStatement {Expression = expr});

            return new LoopHeaderBlockExpression {Body = nestedBody};
        }

        public IVariableReference ToVariableRef(ICSharpExpression csExpr, IList<IStatement> body)
        {
            if (csExpr == null)
            {
                return new VariableReference();
            }

            var baseExpr = csExpr as IBaseExpression;
            if (baseExpr != null)
            {
                return new VariableReference {Identifier = "base"};
            }

            var thisExpr = csExpr as IThisExpression;
            if (thisExpr != null)
            {
                return new VariableReference {Identifier = "this"};
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

            var tmpVar = VarRef(_nameGen.GetNextVariableName());
            body.Add(
                new VariableDeclaration
                {
                    Reference = tmpVar,
                    Type = GetTypeName(csExpr)
                });
            body.Add(
                new Assignment
                {
                    Reference = tmpVar,
                    Expression = expr
                });

            return tmpVar;
        }

        private static ITypeName GetTypeName([NotNull] ICSharpExpression csExpr)
        {
            var type = csExpr.GetExpressionType().ToIType();
            return type != null ? type.GetName() : TypeName.UnknownName;
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

        public override IAssignableExpression VisitThisExpression(IThisExpression expr, IList<IStatement> body)
        {
            return new ReferenceExpression {Reference = new VariableReference {Identifier = "this"}};
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
                var methodName = resolvedMethod != null
                    ? resolvedMethod.GetName<IMethodName>()
                    : MethodName.UnknownName;

                var varRef = methodName.IsStatic
                    ? new VariableReference()
                    : CreateVariableReference(invokedExpression.QualifierExpression, body);

                var parameters = GetArgumentList(inv.ArgumentList, body);

                return new InvocationExpression
                {
                    Reference = varRef,
                    MethodName = methodName,
                    Parameters = parameters
                };
            }

            return new UnknownExpression();
        }

        private VariableReference CreateVariableReference(ICSharpExpression refExpr,
            IList<IStatement> body)
        {
            var reference = ToReference(refExpr, body);

            if (reference is UnknownReference)
            {
                return new VariableReference();
            }

            var varRef = reference as VariableReference;
            if (varRef != null)
            {
                return varRef;
            }

            var type = refExpr == null ? TypeName.UnknownName : refExpr.GetExpressionType().ToIType().GetName();

            var newVarRef = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
            body.Add(
                new VariableDeclaration
                {
                    Reference = newVarRef,
                    Type = type
                });
            body.Add(
                new Assignment
                {
                    Reference = newVarRef,
                    Expression = new ReferenceExpression {Reference = reference}
                });
            return newVarRef;
        }

        [NotNull]
        private IReference ToReference(ICSharpExpression csExpr,
            IList<IStatement> body)
        {
            if (csExpr == null || csExpr is IThisExpression)
            {
                return new VariableReference {Identifier = "this"};
            }

            if (csExpr is IBaseExpression)
            {
                return new VariableReference {Identifier = "base"};
            }

            if (csExpr is IPredefinedTypeExpression)
            {
                // (= qualifier is static type)
                return new VariableReference();
            }

            var invExpr = csExpr as IInvocationExpression;
            if (invExpr != null)
            {
                var assInv = VisitInvocationExpression(invExpr, body);

                var tmpVar = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
                var type = invExpr.GetExpressionType().ToIType().GetName();
                body.Add(new VariableDeclaration {Reference = tmpVar, Type = type});

                body.Add(
                    new Assignment
                    {
                        Reference = tmpVar,
                        Expression = assInv
                    });

                return tmpVar;
            }

            var refExpr = csExpr as IReferenceExpression;
            if (refExpr != null)
            {
                var baseRef = CreateVariableReference(refExpr.QualifierExpression, body);

                var resolveResult = refExpr.Reference.Resolve();
                var elem = resolveResult.DeclaredElement;
                if (elem == null)
                {
                    return new UnknownReference();
                }

                var field = elem as IField;
                if (field != null)
                {
                    return new FieldReference
                    {
                        FieldName = field.GetName<IFieldName>(),
                        Reference = baseRef
                    };
                }

                var property = elem as IProperty;
                if (property != null)
                {
                    return new PropertyReference
                    {
                        PropertyName = property.GetName<IPropertyName>(),
                        Reference = baseRef
                    };
                }

                var @event = elem as IEvent;
                if (@event != null)
                {
                    return new EventReference
                    {
                        EventName = @event.GetName<IEventName>(),
                        Reference = baseRef
                    };
                }

                var method = elem as IMethod;
                if (method != null)
                {
                    return new MethodReference
                    {
                        MethodName = method.GetName<IMethodName>(),
                        Reference = baseRef
                    };
                }

                var localVar = elem as ILocalVariable;
                var parameter = elem as IParameter;
                if (localVar != null || parameter != null)
                {
                    return new VariableReference {Identifier = elem.ShortName};
                }
            }

            return new UnknownReference();
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
                    Reference = ToReference(expr, context)
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

        public override IAssignableExpression VisitConditionalTernaryExpression(IConditionalTernaryExpression expr,
            IList<IStatement> body)
        {
            var condition = ToSimpleExpression(expr.ConditionOperand, body);
            var thenExpression = ToSimpleExpression(expr.ThenResult, body);
            var elseExpression = ToSimpleExpression(expr.ElseResult, body);

            return new IfElseExpression
            {
                Condition = condition,
                ThenExpression = thenExpression,
                ElseExpression = elseExpression
            };
        }

        public override IAssignableExpression VisitCastExpression(ICastExpression expr, IList<IStatement> body)
        {
            return new CastExpression
            {
                VariableReference = ToVariableRef(expr.Op, body),
                TargetType = expr.Type().GetName()
            };
        }

        public override IAssignableExpression VisitAsExpression(IAsExpression expr, IList<IStatement> body)
        {
            return new CastExpression
            {
                VariableReference = ToVariableRef(expr.Operand, body),
                TargetType = expr.Type().GetName()
            };
        }

        #region ComposedExpressionCreator entry points
        public override IAssignableExpression VisitAdditiveExpression(IAdditiveExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitMultiplicativeExpression(IMultiplicativeExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitParenthesizedExpression(IParenthesizedExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitConditionalAndExpression(IConditionalAndExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitConditionalOrExpression(IConditionalOrExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitEqualityExpression(IEqualityExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitBitwiseAndExpression(IBitwiseAndExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitBitwiseExclusiveOrExpression(IBitwiseExclusiveOrExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitBitwiseInclusiveOrExpression(IBitwiseInclusiveOrExpression expr,
            IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitUnaryOperatorExpression(IUnaryOperatorExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitShiftExpression(IShiftExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }

        public override IAssignableExpression VisitRelationalExpression(IRelationalExpression expr, IList<IStatement> context)
        {
            return ComposedExpressionCreator.Create(this, expr, context);
        }
        #endregion
    }
}