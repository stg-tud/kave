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
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Util;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Expressions;
using KaVE.Commons.Model.SSTs.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
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
using ICastExpression = JetBrains.ReSharper.Psi.CSharp.Tree.ICastExpression;
using IInvocationExpression = JetBrains.ReSharper.Psi.CSharp.Tree.IInvocationExpression;
using ILambdaExpression = JetBrains.ReSharper.Psi.CSharp.Tree.ILambdaExpression;
using IReference = KaVE.Commons.Model.SSTs.IReference;
using IReferenceExpression = JetBrains.ReSharper.Psi.CSharp.Tree.IReferenceExpression;

namespace KaVE.RS.Commons.Analysis.Transformer
{
    public partial class ExpressionVisitor : TreeNodeVisitor<IList<IStatement>, IAssignableExpression>
    {
        public readonly ITypeName Bool = TypeName.Get("System.Boolean, mscorlib, 4.0.0.0");

        private readonly UniqueVariableNameGenerator _nameGen;
        private readonly CompletionTargetMarker _marker;

        public ExpressionVisitor(UniqueVariableNameGenerator nameGen, CompletionTargetMarker marker)
        {
            _nameGen = nameGen;
            _marker = marker;
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
            var exprIType = csExpr.GetExpressionType().ToIType();
            // TODO write test for this null check
            var exprType = exprIType == null ? TypeName.UnknownName : exprIType.GetName();
            body.Add(
                new VariableDeclaration
                {
                    Reference = newRef,
                    Type = exprType
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
            // keep this for cases where the AST is missing value while typing!
            if (csExpr == null)
            {
                return new VariableReference();
            }

            var thisExpr = csExpr as IThisExpression;
            if (thisExpr != null)
            {
                return new VariableReference {Identifier = "this"};
            }

            var baseExpr = csExpr as IBaseExpression;
            if (baseExpr != null)
            {
                return new VariableReference {Identifier = "base"};
            }

            var refExpr = csExpr as IReferenceExpression;
            if (refExpr != null)
            {
                if (!IsResolved(refExpr))
                {
                    return new VariableReference();
                }

                var isMember = IsMember(refExpr);

                var hasName = refExpr.NameIdentifier != null;
                var isSimpleReference = !isMember && refExpr.QualifierExpression == null;
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

        private static bool IsResolved(IReferenceExpression refExpr)
        {
            var elem = refExpr.Reference.Resolve().DeclaredElement;
            return elem != null;
        }

        private static bool IsMember(IReferenceExpression refExpr)
        {
            if (refExpr == null)
            {
                return false;
            }
            var elem = refExpr.Reference.Resolve().DeclaredElement;
            var isMember = elem is IEvent || elem is IField || elem is IMethod || elem is IProperty;
            return isMember;
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

                var qExpr = invokedExpression.QualifierExpression;

                var parameters = Lists.NewList<ISimpleExpression>();
                IVariableReference varRef = new VariableReference();
                if (resolvedMethod != null)
                {
                    if (resolvedMethod.Element.IsStatic && !resolvedMethod.Element.IsExtensionMethod)
                    {
                        varRef = new VariableReference();
                    }
                    else if (invokedExpression.IsClassifiedAsVariable)
                    {
                        varRef = ToVariableRef(invokedExpression, body);
                    }
                    else if (qExpr != null &&
                             (qExpr.IsClassifiedAsVariable || qExpr is IThisExpression || qExpr is IInvocationExpression))
                    {
                        varRef = ToVariableRef(qExpr, body);
                    }
                    else if (qExpr is ICSharpLiteralExpression)
                    {
                        varRef = ToVariableRef(qExpr, body);
                    }
                    else if (!resolvedMethod.Element.IsStatic && HasImpliciteThis(invokedExpression))
                    {
                        varRef = new VariableReference {Identifier = "this"};
                    }
                    else if (!resolvedMethod.Element.IsStatic && IsMember(invokedExpression))
                    {
                        varRef = ToVariableRef(invokedExpression.QualifierExpression, body);
                    }
                    else if (IsMember(invokedExpression.QualifierExpression as IReferenceExpression))
                    {
                        varRef = ToVariableRef(invokedExpression.QualifierExpression, body);
                    }
                    else
                    {
                        varRef = new VariableReference();
                    }

                    if (resolvedMethod.Element.IsExtensionMethod)
                    {
                        var defaultVarRef = new VariableReference();
                        if (!varRef.Equals(defaultVarRef))
                        {
                            parameters.Add(
                                new ReferenceExpression
                                {
                                    Reference = varRef
                                });
                            varRef = defaultVarRef;
                        }
                    }
                }

                foreach (var arg in GetArgumentList(inv.ArgumentList, body))
                {
                    parameters.Add(arg);
                }

                return new InvocationExpression
                {
                    Reference = varRef,
                    MethodName = methodName,
                    Parameters = parameters
                };
            }

            return new UnknownExpression();
        }

        private static bool HasImpliciteThis(IReferenceExpression refExpr)
        {
            return IsMember(refExpr) && refExpr.QualifierExpression == null;
        }

        [NotNull]
        private IReference GetReference(IReferenceExpression refExpr, IVariableReference baseRef)
        {
            if (refExpr != null)
            {
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

        [NotNull]
        private IReference ToReference(ICSharpExpression csExpr,
            IList<IStatement> body)
        {
            if (csExpr == null)
            {
                return new UnknownReference();
            }

            if (csExpr is IThisExpression)
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
                var elem = refExpr.Reference.Resolve().DeclaredElement;
                if (elem == null)
                {
                    return new UnknownReference();
                }

                var typeMember = elem as ITypeMember;
                if (typeMember != null)
                {
                    return ToReference(refExpr, typeMember, body);
                }

                var localVar = elem as ILocalVariable;
                var parameter = elem as IParameter;
                if (localVar != null || parameter != null)
                {
                    return new VariableReference {Identifier = elem.ShortName};
                }
            }

            var elementAccessExpr = csExpr as IElementAccessExpression;
            if (elementAccessExpr != null)
            {
                return new IndexAccessReference
                {
                    Expression = (IIndexAccessExpression) VisitElementAccessExpression(elementAccessExpr, body)
                };
            }

            return new UnknownReference();
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private IReference ToReference(IReferenceExpression refExpr, ITypeMember elem, IList<IStatement> body)
        {
            IVariableReference baseRef = new VariableReference();
            if (!elem.IsStatic)
            {
                baseRef = HasImpliciteThis(refExpr)
                    ? new VariableReference {Identifier = "this"}
                    : ToVariableRef(refExpr.QualifierExpression, body);
            }

            return ToReference(elem, baseRef);
        }

        private static IReference ToReference(ITypeMember elem, IVariableReference baseRef)
        {
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

            return new UnknownReference();
        }

        public override IAssignableExpression VisitObjectCreationExpression(IObjectCreationExpression expr,
            IList<IStatement> body)
        {
            var r = expr.ConstructorReference.Resolve();
            if (r.IsValid() && r.DeclaredElement != null)
            {
                var varDeclName = GetNameFromDeclaration(expr);

                var methodName = r.DeclaredElement.GetName<IMethodName>(r.Result.Substitution);
                Asserts.That(methodName.IsConstructor);

                var parameters = Lists.NewList<ISimpleExpression>();
                foreach (var argument in expr.Arguments)
                {
                    var parameter = ToSimpleExpression(argument.Value, body);
                    parameters.Add(parameter);
                }

                var sstInv = new InvocationExpression
                {
                    MethodName = methodName,
                    Parameters = parameters
                };

                var oInit = expr.Initializer as IObjectInitializer;
                var cInit = expr.Initializer as ICollectionInitializer;
                if (oInit != null || cInit != null)
                {
                    IVariableReference newVar;
                    if (varDeclName != null)
                    {
                        newVar = new VariableReference {Identifier = varDeclName};
                    }
                    else
                    {
                        newVar = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
                        body.Add(
                            new VariableDeclaration
                            {
                                Reference = newVar,
                                Type = sstInv.MethodName.DeclaringType
                            });
                    }
                    body.Add(
                        new Assignment
                        {
                            Reference = newVar,
                            Expression = sstInv
                        });

                    if (oInit != null)
                    {
                        foreach (var mInit in oInit.MemberInitializersEnumerable)
                        {
                            IAssignableReference reference = null;
                            var rr = mInit.Reference.Resolve();
                            var elem = rr.DeclaredElement;
                            var typeMember = elem as ITypeMember;
                            if (typeMember != null)
                            {
                                reference = ToReference(typeMember, newVar) as IAssignableReference;
                            }
                            if (reference == null)
                            {
                                reference = new UnknownReference();
                            }

                            // check that operator is "="
                            if (mInit.Expression != null)
                            {
                                body.Add(
                                    new Assignment
                                    {
                                        Reference = reference,
                                        Expression = ToAssignableExpr(mInit.Expression, body)
                                    });
                            }
                            else
                            {
                                var prop = elem as IProperty;
                                var pInit = mInit as IPropertyInitializer;
                                if (prop != null && pInit != null && pInit.Initializer != null)
                                {
                                    var nextVar = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
                                    body.Add(
                                        new VariableDeclaration
                                        {
                                            Reference = nextVar,
                                            Type = prop.ReturnType.GetName()
                                        });
                                    if (varDeclName == null)
                                    {
                                        body.Add(
                                            new Assignment
                                            {
                                                Reference = nextVar,
                                                Expression = new ReferenceExpression
                                                {
                                                    Reference = reference
                                                }
                                            });
                                    }

                                    ISubstitution substitution = EmptySubstitution.INSTANCE;
                                    var ctype = pInit.Initializer.ConstructedType as IDeclaredType;
                                    if (ctype != null)
                                    {
                                        var rctype = ctype.Resolve();
                                        substitution = rctype.Substitution;
                                    }

                                    var addName = FindAdd(prop, substitution);

                                    foreach (var eInit in pInit.Initializer.InitializerElements)
                                    {
                                        var ceInit = eInit as ICollectionElementInitializer;
                                        if (ceInit != null)
                                        {
                                            foreach (var arg in ceInit.Arguments)
                                            {
                                                body.Add(
                                                    new ExpressionStatement
                                                    {
                                                        Expression = new InvocationExpression
                                                        {
                                                            Reference = nextVar,
                                                            MethodName = addName,
                                                            Parameters = {ToSimpleExpression(arg.Value, body)}
                                                        }
                                                    });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (cInit != null)
                    {
                        var m = r.DeclaredElement as IConstructor;
                        var addName = FindAdd(m, r.Result.Substitution) ?? MethodName.UnknownName;

                        foreach (var eInit in cInit.ElementInitializersEnumerable)
                        {
                            foreach (var arg in eInit.Arguments)
                            {
                                body.Add(
                                    new ExpressionStatement
                                    {
                                        Expression = new InvocationExpression
                                        {
                                            Reference = newVar,
                                            MethodName = addName,
                                            Parameters = {ToSimpleExpression(arg.Value, body)}
                                        }
                                    });
                            }
                        }
                    }

                    return new ReferenceExpression
                    {
                        Reference = newVar
                    };
                }
                return sstInv;
            }
            return new InvocationExpression
            {
                MethodName = MethodName.UnknownName
            };
        }

        private static string GetNameFromDeclaration(IObjectCreationExpression expr)
        {
            if (expr.Parent != null)
            {
                var varDecl = expr.Parent.Parent as ILocalVariableDeclaration;
                if (varDecl != null)
                {
                    var nameFromAssign = varDecl.NameIdentifier;

                    return nameFromAssign.Name;
                }
            }
            return null;
        }

        private IMethodName FindAdd(IProperty c, [NotNull] ISubstitution substitution)
        {
            if (c != null)
            {
                var declType = c.ReturnType.GetTypeElement();
                if (declType != null)
                {
                    foreach (var m in declType.Methods)
                    {
                        if ("Add".Equals(m.ShortName) && m.Parameters.Count == 1)
                        {
                            return m.GetName<IMethodName>(substitution);
                        }
                    }
                }
            }

            return MethodName.UnknownName;
        }

        private static IMethodName FindAdd(IConstructor c, [NotNull] ISubstitution substitution)
        {
            if (c != null)
            {
                var declType = c.GetContainingType();
                if (declType != null)
                {
                    foreach (var m in declType.Methods)
                    {
                        if ("Add".Equals(m.ShortName) && m.Parameters.Count == 1)
                        {
                            return m.GetName<IMethodName>(substitution);
                        }
                    }
                }
            }

            return MethodName.UnknownName;
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
            var qualifierExpr = expr.QualifierExpression;
            var name = expr.NameIdentifier != null ? expr.NameIdentifier.Name : "";
            if (expr == _marker.AffectedNode && _marker.AffectedNode != null)
            {
                var ce = new CompletionExpression
                {
                    Token = name
                };
                if (qualifierExpr != null)
                {
                    if (qualifierExpr.IsClassifiedAsVariable ||
                        qualifierExpr is IInvocationExpression ||
                        qualifierExpr is IThisExpression ||
                        qualifierExpr is IBaseExpression)
                    {
                        ce.VariableReference = ToVariableRef(qualifierExpr, context);
                    }
                    else
                    {
                        ce.TypeReference = ToTypeRef(qualifierExpr);
                    }
                }
                return ce;
            }

            if (IsMember(expr))
            {
                IVariableReference varRef;
                if (IsStatic(expr))
                {
                    varRef = new VariableReference();
                }
                else
                {
                    varRef = HasImpliciteThis(expr)
                        ? new VariableReference {Identifier = "this"}
                        : ToVariableRef(qualifierExpr, context);
                }

                return new ReferenceExpression
                {
                    Reference = GetReference(expr, varRef)
                };
            }

            return new ReferenceExpression
            {
                Reference = ToReference(expr, context)
            };
        }

        private static ITypeName ToTypeRef(ICSharpExpression expr)
        {
            var typeExpr = expr as IPredefinedTypeExpression;
            if (typeExpr != null)
            {
                var elem = typeExpr.PredefinedTypeName.Reference.Resolve().DeclaredElement;
                if (elem != null)
                {
                    return elem.GetName<ITypeName>(elem.GetIdSubstitutionSafe());
                }
            }

            var refExpr = expr as IReferenceExpression;
            if (refExpr != null)
            {
                var elem = refExpr.Reference.Resolve().DeclaredElement;
                if (elem != null)
                {
                    return elem.GetName<ITypeName>(elem.GetIdSubstitutionSafe());
                }
            }
            return TypeName.UnknownName;
        }

        private static bool IsStatic(IReferenceExpression expr)
        {
            var elem = expr.Reference.Resolve().DeclaredElement;
            var typeMember = elem as ITypeMember;
            return typeMember != null && typeMember.IsStatic;
        }

        public override IAssignableExpression VisitLambdaExpression(ILambdaExpression expr, IList<IStatement> body)
        {
            if (expr.DeclaredElement == null)
            {
                return new UnknownExpression();
            }

            var lambdaName = expr.GetName();
            var lambdaBody = new KaVEList<IStatement>();
            var bodyVisitor = new BodyVisitor(_nameGen, _marker);

            if (expr.BodyBlock != null)
            {
                if (expr.BodyBlock == _marker.AffectedNode && _marker.Case == CompletionCase.EmptyCompletionAfter)
                {
                    lambdaBody.Add(new ExpressionStatement {Expression = new CompletionExpression()});
                }

                expr.BodyBlock.Accept(bodyVisitor, lambdaBody);
            }
            else if (expr.BodyExpression != null)
            {
                var varRef = ToVariableRef(expr.BodyExpression, lambdaBody);
                lambdaBody.Add(
                    new ReturnStatement {IsVoid = false, Expression = new ReferenceExpression {Reference = varRef}});
            }

            return new LambdaExpression
            {
                Name = lambdaName,
                Body = lambdaBody
            };
        }

        public override IAssignableExpression VisitAnonymousMethodExpression(IAnonymousMethodExpression expr,
            IList<IStatement> body)
        {
            var lambdaName = expr.GetName();
            var lambdaBody = new KaVEList<IStatement>();
            var bodyVisitor = new BodyVisitor(_nameGen, _marker);

            if (expr.Body == _marker.AffectedNode && _marker.Case == CompletionCase.EmptyCompletionAfter)
            {
                lambdaBody.Add(new ExpressionStatement {Expression = new CompletionExpression()});
            }

            expr.Body.Accept(bodyVisitor, lambdaBody);

            return new LambdaExpression
            {
                Name = lambdaName,
                Body = lambdaBody
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
                Reference = ToVariableRef(expr.Op, body),
                TargetType = expr.Type().GetName()
            };
        }

        public override IAssignableExpression VisitAsExpression(IAsExpression expr, IList<IStatement> body)
        {
            return new CastExpression
            {
                Reference = ToVariableRef(expr.Operand, body),
                TargetType = expr.Type().GetName()
            };
        }

        public override IAssignableExpression VisitIsExpression(IIsExpression expr, IList<IStatement> body)
        {
            return new TypeCheckExpression
            {
                Reference = ToVariableRef(expr.Operand, body),
                Type = expr.IsType.GetName()
            };
        }

        public override IAssignableExpression VisitUncheckedExpression(IUncheckedExpression expr, IList<IStatement> body)
        {
            var uncheckedBlock = new UncheckedBlock();
            var assignable = ToAssignableExpr(expr.Operand, uncheckedBlock.Body);
            if (assignable is IConstantValueExpression)
            {
                return assignable;
            }

            var newRef = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
            body.Add(
                new VariableDeclaration
                {
                    Reference = newRef,
                    Type = expr.Type().GetName()
                });

            uncheckedBlock.Body.Add(
                new Assignment
                {
                    Reference = newRef,
                    Expression = assignable
                });

            body.Add(uncheckedBlock);

            return new ReferenceExpression {Reference = newRef};
        }

        public override IAssignableExpression VisitElementAccessExpression(IElementAccessExpression expr,
            IList<IStatement> body)
        {
            return new IndexAccessExpression
            {
                Reference = ToVariableRef(expr.Operand, body),
                Indices = GetArgumentList(expr.ArgumentList, body)
            };
        }

        public override IAssignableExpression VisitNullCoalescingExpression(INullCoalescingExpression expr,
            IList<IStatement> context)
        {
            var lref = ToVariableRef(expr.LeftOperand, context);

            var v0 = new VariableReference {Identifier = _nameGen.GetNextVariableName()};
            context.Add(
                new VariableDeclaration
                {
                    Reference = v0,
                    Type = Bool
                });
            context.Add(
                new Assignment
                {
                    Reference = v0,
                    Expression = new ComposedExpression {References = {lref}}
                });
            return new IfElseExpression
            {
                Condition = new ReferenceExpression {Reference = v0},
                ThenExpression = new ReferenceExpression {Reference = lref},
                ElseExpression = ToSimpleExpression(expr.RightOperand, context)
            };
        }

        #region ComposedExpressionCreator entry points

        #endregion
    }
}