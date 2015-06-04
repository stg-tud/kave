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
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;

//TODO property is treated as field, implement property correctly

namespace KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite
{
    public class ObjectUsageCreationTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void CreatesQueryForVariableDeclarationWithConstructor()
        {
            var objectUsageType = Type("A");
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = objectUsageType
                },
                new Assignment
                {
                    Reference = VarRef("a"),
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), ".ctor"),
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = objectUsageType.ToCoReName(),
                });
        }

        [Test]
        public void CreatesQueryForVariableDeclarationNotUsingRightHandTypeByConstructor()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");

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
                        MethodName = Method(ObjectUsageExporterTestFixture.Void, Type("B"), ".ctor")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("a"),
                        MethodName = calledMethodName
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                },
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void CollectsReceiverCallsiteForDeclaredVariableType()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("a")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                });
        }

        [Test]
        public void CreatesQueryForParameterDeclaration()
        {
            var enclosingMethod = Method(
                ObjectUsageExporterTestFixture.Void,
                Type("C"),
                "Method",
                ObjectUsageExporterTestFixture.IntParam,
                Parameter(Type("PType"), "param"));

            SetupEnclosingMethod(enclosingMethod);

            AssertQueries(
                enclosingMethod,
                true,
                new Query
                {
                    type = ObjectUsageExporterTestFixture.IntType.ToCoReName(),
                },
                new Query
                {
                    type = Type("PType").ToCoReName()
                });
        }

        [Test]
        public void CollectsReceiverCallsiteForVariableInitializedBySubtype()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");

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
                        MethodName = Method(ObjectUsageExporterTestFixture.Void, Type("B"), ".ctor")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("a")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                },
                // Object usage is created at constructor call
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void CreatesQueryForVariableDeclarationNotUsingRightHandTypeByVariableReferenceWithCallsite()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "m");

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
                    Expression = new ReferenceExpression
                    {
                        Reference = VarRef("b")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("a")
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                },
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void CreatesQueryForVariableDeclarationNotUsingRightHandTypeByFieldReferenceWithCallsite()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");

            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("A"), Type("TDecl"), "_fieldA")
                    }
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Reference = VarRef("b"),
                    Expression = new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = Field(Type("A"), Type("TDecl"), "_fieldA"),
                            Reference = VarRef("_fieldA")
                        }
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("b")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                },
                new Query
                {
                    type = Type("A").ToCoReName()
                });
        }

        [Test]
        public void CreatesQueryForVariableAByDeclarationNotUsingRightHandTypeByInvocationExpression()
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
                        MethodName = Method(Type("C"), Type("B"), "M1"),
                        Reference = VarRef("b")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite()
                },
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("C"), Type("B"), "M1"))
                    }
                });
        }

        [Test]
        public void CreatesNoQueryForFieldDeclarationWithNoMethod()
        {
            Ctx.SST = new SST
            {
                EnclosingType = Type("TDecl"),
                Fields =
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("T3"), Type("A"), "_fieldA")
                    }
                }
            };

            var actuals = Sut.Export(Ctx);

            Assert.AreEqual(0, actuals.Count);
        }

        [Test]
        public void CreatesNoQueryForPropertyDeclarationWithNoMethod()
        {
            // Treating property as field
            Ctx.SST = new SST
            {
                EnclosingType = Type("TDecl"),
                Properties =
                {
                    new PropertyDeclaration
                    {
                        Name = Property(Type("TPvalue"), Type("TPdecl"), "PropertyName")
                    }
                }
            };

            var actuals = Sut.Export(Ctx);

            Assert.AreEqual(0, actuals.Count);
        }

        [Test]
        public void CreatesQueryForPropertyDeclarationWithEmptyMethod()
        {
            // Treating property as field
            SetupDefaultEnclosingMethodWithProperties(
                new IPropertyDeclaration[]
                {
                    new PropertyDeclaration
                    {
                        Name = Property(Type("T3"), Type("A"), "PropertyA")
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("T3").ToCoReName()
                });
        }

        [Test]
        public void CreatesQueryForPropertyDeclarationWithCallsite()
        {
            // Treating property as field
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");

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
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                });
        }

        [Test]
        public void CreatesQueryForFieldDeclarationWithEmptyMethod()
        {
            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("T3"), Type("A"), "_fieldA")
                    }
                });

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("T3").ToCoReName()
                });
        }

        [Test]
        public void CreatesQueryForFieldDeclarationWithCallsite()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "Method");

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

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                });
        }

        [Test]
        public void CreatesQueryForFieldDeclarationByAssigningToVariableWithoutCallsite()
        {
            var calledMethodName = Method(ObjectUsageExporterTestFixture.Void, Type("B"), "Method");

            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("A"), Type("TDecl"), "_fieldA")
                    }
                },
                new VariableDeclaration
                {
                    Reference = VarRef("b"),
                    Type = Type("B")
                },
                new Assignment
                {
                    Reference = VarRef("b"),
                    Expression = new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = Field(Type("A"), Type("TDecl"), "_fieldA"),
                            Reference = VarRef("this")
                        }
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = calledMethodName,
                        Reference = VarRef("b")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                },
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(calledMethodName)
                    }
                });
        }

        [Test]
        public void CreatesQueryForReferenceTypeInInvocationExpression()
        {
            SetupDefaultEnclosingMethod(
                new VariableDeclaration
                {
                    Reference = VarRef("a"),
                    Type = Type("A")
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(ObjectUsageExporterTestFixture.Void, Type("B"), "MethodTB"),
                        Reference = VarRef("a")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        MethodName = Method(ObjectUsageExporterTestFixture.Void, Type("A"), "MethodTA"),
                        Reference = VarRef("a")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(ObjectUsageExporterTestFixture.Void, Type("B"), "MethodTB")),
                        CallSites.CreateReceiverCallSite(
                            Method(ObjectUsageExporterTestFixture.Void, Type("A"), "MethodTA"))
                    }
                });
        }

        [Test]
        public void CreatesQueryWithMethodContextFromHierarchyFirst()
        {
            var methodCtx = Method(Type("R"), Type("SomeI"), "m"); // FIRST
            Ctx.TypeShape.MethodHierarchies.Add(
                new MethodHierarchy
                {
                    Element = Method(Type("R"), Type("Dec"), "m"),
                    First = methodCtx,
                    Super = Method(Type("SuperR"), Type("SuperDec"), "m"),
                });

            var enclosingMethod = Method(Type("R"), Type("Dec"), "m");
            SetupEnclosingMethod(
                enclosingMethod,
                new IStatement[]
                {
                    new VariableDeclaration
                    {
                        Reference = VarRef("a"),
                        Type = Type("A")
                    }
                });

            AssertQueriesWithMethodCtx(
                enclosingMethod,
                methodCtx,
                new[]
                {
                    new Query
                    {
                        type = Type("A").ToCoReName()
                    }
                });
        }

        [Test]
        public void CreatesQueryWithMethodContextFromHierarchySuper()
        {
            var methodCtx = Method(Type("SuperR"), Type("SuperDec"), "m"); // Super
            Ctx.TypeShape.MethodHierarchies.Add(
                new MethodHierarchy
                {
                    Element = Method(Type("R"), Type("Dec"), "m"),
                    First = null,
                    Super = methodCtx,
                });

            var enclosingMethod = Method(Type("R"), Type("Dec"), "m");
            SetupEnclosingMethod(
                enclosingMethod,
                new IStatement[]
                {
                    new VariableDeclaration
                    {
                        Reference = VarRef("a"),
                        Type = Type("A")
                    }
                });

            AssertQueriesWithMethodCtx(
                enclosingMethod,
                methodCtx,
                new[]
                {
                    new Query
                    {
                        type = Type("A").ToCoReName()
                    }
                });
        }

        [Test]
        public void CreatesQueryWithMethodContextFromHierarchyElement()
        {
            // Choose element when first and super are null
            var methodCtx = Method(Type("R"), Type("Dec"), "m");
            Ctx.TypeShape.MethodHierarchies.Add(
                new MethodHierarchy
                {
                    Element = Method(Type("R"), Type("Dec"), "m"),
                    First = null,
                    Super = null,
                });

            var enclosingMethod = Method(Type("R"), Type("Dec"), "m");
            SetupEnclosingMethod(
                enclosingMethod,
                new IStatement[]
                {
                    new VariableDeclaration
                    {
                        Reference = VarRef("a"),
                        Type = Type("A")
                    }
                });

            AssertQueriesWithMethodCtx(
                enclosingMethod,
                methodCtx,
                new[]
                {
                    new Query
                    {
                        type = Type("A").ToCoReName()
                    }
                });
        }

        [Test]
        public void CreatesQueryForThis()
        {
            SetupDefaultEnclosingMethod(
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("this"),
                        MethodName = Method(Type("Ret"), Type("Decl"), "method")
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
                        CallSites.CreateReceiverCallSite(Method(Type("Ret"), Type("Decl"), "method"))
                    }
                });
        }

        [Test]
        public void CreatesQueryForBase()
        {
            SetupDefaultEnclosingMethod(
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("base"),
                        MethodName = Method(Type("Ret"), Type("Decl"), "method")
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
                        CallSites.CreateReceiverCallSite(Method(Type("Ret"), Type("Decl"), "method"))
                    }
                });
        }

        [Test]
        public void CreatesNoQueriesWithNoCallSides()
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
                });

            var actuals = Sut.CleanExport(Ctx);

            Assert.AreEqual(0, actuals.Count);
        }

        [Test]
        public void CreatesNoFieldQueriesWithNoCallSides()
        {
            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("TValue"), Type("FDecl"), "_fieldA")
                    }
                });

            var actuals = Sut.CleanExport(Ctx);

            Assert.AreEqual(0, actuals.Count);
        }

        [Test]
        public void CreatesNoPropertyQueriesWithNoCallSides()
        {
            SetupDefaultEnclosingMethodWithProperties(
                new IPropertyDeclaration[]
                {
                    new PropertyDeclaration
                    {
                        Name = Property(Type("TValue"), Type("PDecl"), "PropertyA")
                    }
                });

            var actuals = Sut.CleanExport(Ctx);

            Assert.AreEqual(0, actuals.Count);
        }

        [Test]
        public void CreatesFieldQueryOnlyWithNoCallSides()
        {
            SetupDefaultEnclosingMethodWithFields(
                new IFieldDeclaration[]
                {
                    new FieldDeclaration
                    {
                        Name = Field(Type("TValue"), Type("FDecl"), "_fieldA")
                    },
                    new FieldDeclaration
                    {
                        Name = Field(Type("TValueB"), Type("FDeclB"), "_fieldB")
                    }
                },
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("_fieldA"),
                        MethodName = Method(Type("TRet"), Type("MDecl"), "method")
                    }
                });

            AssertQueriesCleanInDefault(
                new Query
                {
                    type = Type("TValue").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("TRet"), Type("MDecl"), "method"))
                    }
                });
        }

        [Test]
        public void CreatesQueriesOnlyWithNoCallSides()
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
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("b"),
                        MethodName = Method(Type("TRet"), Type("MDecl"), "method")
                    }
                });

            AssertQueriesCleanInDefault(
                new Query
                {
                    type = Type("B").ToCoReName(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Type("TRet"), Type("MDecl"), "method"))
                    }
                });
        }
    }
}