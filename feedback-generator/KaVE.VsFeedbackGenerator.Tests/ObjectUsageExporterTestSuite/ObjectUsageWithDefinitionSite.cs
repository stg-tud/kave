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
 *    - Roman Fojtik
 */

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.Tests.ObjectUsageExporterTestSuite.ObjectUsageExporterTestFixture;
using ICV = KaVE.VsFeedbackGenerator.ObjectUsageExport.InvocationCollectorVisitor;

// TODO implement property right

namespace KaVE.VsFeedbackGenerator.Tests.ObjectUsageExporterTestSuite
{
    internal class ObjectUsageWithDefinitionSite : BaseObjectUsageExporterTest
    {
        [Test]
        public void VarDefinitionAsConstant()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new ConstantValueExpression {Value = "value"}
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("R"), Type("A"), "methodA"),
                        Reference = VarRef("a")
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    definition = new DefinitionSite
                    {
                        kind = DefinitionSiteKind.CONSTANT
                    },
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("R"), Type("A"), "methodA"))
                    }
                });
        }

        [Test]
        public void VarDefinitionByConstructor()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Fix.Void, Type("B"), ".ctor"),
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    definition = DefinitionSites.CreateDefinitionByConstructor(Method(Fix.Void, Type("B"), ".ctor")),
                    type = Type("A").ToCoReName()
                },
                new Query
                {
                    type = Type("B").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstructor(Method(Fix.Void, Type("B"), ".ctor"))
                });
        }

        [Test]
        public void DefinitionAsFieldWithCallsite()
        {
            var calledMethodName = Method(Fix.Void, Type("A"), "Method");

            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("A"), Type("TDecl"), "_fieldA")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("_fieldA")
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(Field(Type("A"), Type("TDecl"), "_fieldA")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName.ToCoReName().Name)
                    }
                });
        }

        [Test]
        public void DefinitionAsParam()
        {
            var enclosingMethod = Method(Fix.Void, Type("C"), "Method", Fix.IntParam);

            SetupEnclosingMethod(enclosingMethod);

            AssertQueries(
                enclosingMethod,
                false,
                new Query
                {
                    type = Fix.IntType.ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByParam(Method(Fix.Void, Type("C"), "Method", Fix.IntParam), 0)
                });
        }

        [Test]
        public void DefinitionAsPropertyWithCallsite()
        {
            var enclosingMethod = Method(Type("A"), Type("TDecl"), "M");
            var calledMethodName = Method(Fix.Void, Type("A"), "Method");

            SetupDefaultEnclosingMethodWithProperties(
                new IPropertyDeclaration[]
                {
                    new PropertyDeclaration
                    {
                        Name = Property(Type("A"), Type("TDecl"), "PropertyA")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("PropertyA")
                    }
                });

            AssertQueries(
                enclosingMethod,
                false,
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(Field(Type("A"), Type("TDecl"), "_PropertyA")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName.ToCoReName().Name)
                    }
                });
        }

        [Test]
        public void VarDefinitionByReturnFromMethodCallOnVariable()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("TRet"), Type("TDecl"), "method"),
                        Reference = VarRef("b")
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByReturn(Method(Type("TRet"), Type("TDecl"), "method")),
                },
                new Query
                {
                    type = Type("B").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("TRet"), Type("TDecl"), "method"))
                    }
                });
        }

        [Test]
        public void DefinitionKindThisByMethodCall()
        {
            SetupEnclosingMethod(
                Method(Type("A"), Type("TDecl"), "M"),
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("TRet"), Type("TDecl"), "method"),
                        Reference = VarRef("this")
                    }
                });

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("TDecl").ToCoReName(),
                    definition = new DefinitionSite
                    {
                        kind = DefinitionSiteKind.THIS
                    },
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("TRet"), Type("TDecl"), "method"))
                    }
                });
        }

        [Test]
        public void VarDefinitionByReturnFromMethodCallOnThis()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Type = Type("A"),
                    Reference = VarRef("a")
                },
                new Assignment
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(Type("TRet"), Type("Decl"), "method"),
                        Reference = VarRef("this")
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("TDecl").ToCoReName(),
                    definition = new DefinitionSite
                    {
                        kind = DefinitionSiteKind.THIS
                    },
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("TRet"), Type("Decl"), "method"))
                    }
                },
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByReturn(Method(Type("TRet"), Type("Decl"), "method"))
                });
        }

        [Test]
        public void DefinitionAsProperty()
        {
            // treating property as field
            SetupDefaultEnclosingMethodWithProperties(
                new IPropertyDeclaration[]
                {
                    new PropertyDeclaration
                    {
                        Name = Property(Type("TValue"), Type("TDecl"), "PropName")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("PropName"),
                        MethodName = Method(Fix.Void, Type("B"), "calledMethod")
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("TValue").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(Field(Type("TValue"), Type("TDecl"), "_PropName")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("B"), "calledMethod"))
                    }
                });
        }

        [Test]
        public void DefinitionAsField()
        {
            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("TValue"), Type("TDecl"), "_fieldA")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("_fieldA"),
                        MethodName = Method(Fix.Void, Type("B"), "calledMethod")
                    }
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("TValue").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(Field(Type("TValue"), Type("TDecl"), "_fieldA")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("B"), "calledMethod"))
                    }
                });
        }

        [Test]
        public void VarDefinitionUnknownByNotInitialising()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                });

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite()
                });
        }

        [Test]
        public void VarDefinitionByVariableAssignment()
        {
            // A a; B b; a=b -> defsite a = unknown
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Expression = new ReferenceExpression
                    {
                        Reference = VarRef("b")
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite()
                },
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite()
                });
        }

        [Test]
        public void VarDefinitionByOutterClassFieldAssignment()
        {
            var fieldA = Field(Type("TValue"), Type("FDecl"), "_fieldA");
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Expression = new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = fieldA,
                            Reference = VarRef("b")
                        }
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(fieldA)
                },
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void VarDefinitionByThisClassFieldAssignment()
        {
            var fieldA = Field(Type("TValue"), Type("FDecl"), "_fieldA");
            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = fieldA
                    }
                },
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new Assignment
                {
                    Expression = new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = fieldA,
                            Reference = VarRef("this")
                        }
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(fieldA)
                },
                new Query
                {
                    type = fieldA.ValueType.ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(fieldA)
                });
        }

        [Test]
        public void VarDefinitionByOutterClassPropertyAssignment()
        {
            // treating property as field
            var propertyA = Property(Type("TValue"), Type("PDecl"), "PropertyA");
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Expression = new ReferenceExpression
                    {
                        Reference = new PropertyReference
                        {
                            PropertyName = propertyA,
                            Reference = VarRef("b")
                        }
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(ICV.PropertyToFieldName(propertyA))
                },
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void VarDefinitionByThisClassPropertyAssignment()
        {
            // treating property as field
            var propertyA = Property(Type("TValue"), Type("PDecl"), "PropertyA");
            SetupDefaultEnclosingMethodWithProperties(
                new IPropertyDeclaration[]
                {
                    new PropertyDeclaration
                    {
                        Name = propertyA
                    }
                },
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new Assignment
                {
                    Expression = new ReferenceExpression
                    {
                        Reference = new PropertyReference
                        {
                            PropertyName = propertyA,
                            Reference = VarRef("this")
                        }
                    },
                    Reference = VarRef("a")
                }
                );

            AssertQueriesInDefaultWithDefSite(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByField(ICV.PropertyToFieldName(propertyA))
                },
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(ICV.PropertyToFieldName(propertyA))
                });
        }
    }
}