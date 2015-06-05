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
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Model.SSTs.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Assignable;
using KaVE.Commons.Model.SSTs.Impl.Expressions.Simple;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Utils.ObjectUsageExport;
using NUnit.Framework;
using Fix = KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite.ObjectUsageExporterTestFixture;

namespace KaVE.Commons.Tests.Utils.ObjectUsageExporterTestSuite
{
    /// <summary>
    ///     tests if all definition sites are identified correctly
    /// </summary>
    internal class DefinitionSiteDetectionTest : BaseObjectUsageExporterTest
    {
        [Test]
        public void VarDefinitionByConstant()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", new ConstantValueExpression()),
                InvokeStmt("a", Method(Fix.Void, Type("A"), "M")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstant(),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(Method(Fix.Void, Type("A"), "M"))
                    }
                });
        }

        [Test]
        public void VarDefinitionByConstructor_SameType()
        {
            var ctor = Constructor(Type("A"));

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", ctor),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstructor(ctor.MethodName),
                    sites =
                    {
                        SomeCallSiteOnType("A")
                    }
                });
        }

        [Test]
        public void VarDefinitionByConstructor_Subtype()
        {
            var ctor = Constructor(Type("B"));

            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign("a", ctor),
                InvokeStmt("a", SomeMethodOnType("A")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByConstructor(ctor.MethodName),
                    sites =
                    {
                        SomeCallSiteOnType("A")
                    }
                });
        }

        [Test]
        public void DefinitionAsParam_Method_Default()
        {
            SetupDefaultEnclosingMethod();

            AssertQueriesInDefault(
                new Query
                {
                    type = Fix.Int.ToCoReName()
                });
        }

        [Test]
        public void DefinitionAsParam_Method_NonDefault()
        {
            var enclosingMethod = Method(
                Fix.Void,
                Type("C"),
                "M",
                Parameter(Fix.Int, "p1"),
                Parameter(Type("PType"), "p2"));

            SetupEnclosingMethod(enclosingMethod);

            AssertQueries(
                enclosingMethod,
                new Query
                {
                    type = Fix.Int.ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByParam(enclosingMethod, 0)
                });
        }

        [Test]
        public void DefinitionAsParam_For() {}

        [Test]
        public void DefinitionAsParam_Foreach() {}

        [Test]
        public void DefinitionAsParam_CatchBlock() {}

        [Test]
        public void VarDefinitionByReturn_SameType()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign(
                    "a",
                    new InvocationExpression
                    {
                        Reference = VarRef("this"),
                        MethodName = Method(Type("A"), DefaultClassContext, "M")
                    }));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByReturn(Method(Type("TRet"), Type("TDecl"), "method")),
                });
        }

        [Test]
        public void VarDefinitionByReturn_Subtype()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign(
                    "a",
                    new InvocationExpression
                    {
                        Reference = VarRef("this"),
                        MethodName = Method(Type("B"), DefaultClassContext, "M")
                    }));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateDefinitionByReturn(Method(Type("TRet"), Type("TDecl"), "method")),
                });
        }

        [Test]
        public void DefinitionByThis()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("this", Method(Fix.Void, DefaultClassContext, "M")));

            AssertQueriesInDefault(
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
        public void DefinitionByThis_Base()
        {
            SetupDefaultEnclosingMethod(
                InvokeStmt("base", Method(Fix.Void, DefaultClassContext, "M")));

            AssertQueriesInDefault(
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
        public void DefinitionAsProperty()
        {
            // treating property as field
            var p = new IPropertyDeclaration[]
            {
                new PropertyDeclaration
                {
                    Name = Property(Type("TValue"), Type("TDecl"), "PropName")
                }
            };
            SetupDefaultEnclosingMethod(
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("PropName"),
                        MethodName = Method(Fix.Void, Type("B"), "calledMethod")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("TValue").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(Field(Type("TValue"), Type("TDecl"), "_PropName")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "calledMethod"))
                    }
                });
        }

        [Test]
        public void DefinitionAsField()
        {
            var fs = new IFieldDeclaration[]
            {
                new FieldDeclaration
                {
                    Name = Field(Type("TValue"), Type("TDecl"), "_fieldA")
                }
            };
            SetupDefaultEnclosingMethod(
                new ExpressionStatement
                {
                    Expression = new InvocationExpression
                    {
                        Reference = VarRef("_fieldA"),
                        MethodName = Method(Fix.Void, Type("B"), "calledMethod")
                    }
                }
                );

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("TValue").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(Field(Type("TValue"), Type("TDecl"), "_fieldA")),
                    sites =
                    {
                        CallSites.CreateReceiverCallSite(
                            Method(Fix.Void, Type("B"), "calledMethod"))
                    }
                });
        }

        [Test]
        public void VarDefinitionByUnknown_NotInitialized()
        {
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")));

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition = DefinitionSites.CreateUnknownDefinitionSite()
                });
        }

        [Test]
        public void VarDefinitionByUnknown_InvocationOnNotDeclared() {}


        [Test]
        public void VarDefinitionByUnknown_AssignmentToNotDeclared() {}

        [Test]
        public void VarDefinitionByAssignment_Variable()
        {
            // A a; B b; a=b -> defsite a = unknown
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                VarDecl("b", Type("B")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = VarRef("b")
                    }));

            AssertQueriesInDefault(
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
        public void VarDefinitionByAssignment_Field_OtherClass()
        {
            var fieldA = Field(Type("TValue"), Type("FDecl"), "_fieldA");
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                VarDecl("b", Type("B")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = fieldA,
                            Reference = VarRef("b")
                        }
                    }));

            AssertQueriesInDefault(
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
        public void VarDefinitionByAssignment_Field_This()
        {
            var fieldA = Field(Type("TValue"), Type("FDecl"), "_fieldA");
            var fs = new IFieldDeclaration[]
            {
                new FieldDeclaration
                {
                    Name = fieldA
                }
            };
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                Assign(
                    "a",
                    new ReferenceExpression
                    {
                        Reference = new FieldReference
                        {
                            FieldName = fieldA,
                            Reference = VarRef("this")
                        }
                    }));

            AssertQueriesInDefault(
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
        public void VarDefinitionByAssignment_Property_OtherClass()
        {
            // treating property as field
            var propertyA = Property(Type("TValue"), Type("PDecl"), "PropertyA");
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
                VarDecl("b", Type("B")),
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

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(
                            InvocationCollectorVisitor.PropertyToFieldName(propertyA))
                },
                new Query
                {
                    type = Type("B").ToCoReName()
                });
        }

        [Test]
        public void VarDefinitionByAssignment_Property_This()
        {
            // treating property as field
            var propertyA = Property(Type("TValue"), Type("PDecl"), "PropertyA");
            var ps = new IPropertyDeclaration[]
            {
                new PropertyDeclaration
                {
                    Name = propertyA
                }
            };
            SetupDefaultEnclosingMethod(
                VarDecl("a", Type("A")),
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

            AssertQueriesInDefault(
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(
                            InvocationCollectorVisitor.PropertyToFieldName(propertyA))
                },
                new Query
                {
                    type = Type("A").ToCoReName(),
                    definition =
                        DefinitionSites.CreateDefinitionByField(
                            InvocationCollectorVisitor.PropertyToFieldName(propertyA))
                });
        }
    }
}