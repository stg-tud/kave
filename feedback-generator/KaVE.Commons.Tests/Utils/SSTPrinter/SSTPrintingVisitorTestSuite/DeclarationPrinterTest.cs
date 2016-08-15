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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.SSTPrinter;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.SSTPrinter.SSTPrintingVisitorTestSuite
{
    internal class DeclarationPrinterTest : SSTPrintingVisitorTestBase
    {
        [Test]
        public void SSTDeclaration_EmptyClass()
        {
            var sst = new SST {EnclosingType = Names.Type("TestClass,TestProject")};

            AssertPrint(
                sst,
                "class TestClass",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_PartialClass()
        {
            var sst = new SST
            {
                EnclosingType = Names.Type("TestClass,TestProject"),
                PartialClassIdentifier = "TestClass_1.cs"
            };

            AssertPrint(
                sst,
                "partial class TestClass",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_WithSupertypes()
        {
            var thisType = Names.Type("TestClass,P");
            var superType = Names.Type("SuperClass,P");
            var interface1 = Names.Type("i:IDoesSomething,P");
            var interface2 = Names.Type("i:IDoesSomethingElse,P");

            var sst = new SST {EnclosingType = thisType};
            var typeShape = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = thisType,
                    Extends = new TypeHierarchy {Element = superType},
                    Implements =
                    {
                        new TypeHierarchy {Element = interface1},
                        new TypeHierarchy {Element = interface2}
                    }
                }
            };

            AssertPrintWithCustomContext(
                sst,
                new SSTPrintingContext {TypeShape = typeShape},
                "class TestClass : SuperClass, IDoesSomething, IDoesSomethingElse",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_WithSupertypes_OnlyInterface()
        {
            var thisType = Names.Type("TestClass,P");
            var interface1 = Names.Type("i:IDoesSomething,P");

            var sst = new SST {EnclosingType = thisType};
            var typeShape = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = thisType,
                    Implements =
                    {
                        new TypeHierarchy {Element = interface1}
                    }
                }
            };

            AssertPrintWithCustomContext(
                sst,
                new SSTPrintingContext {TypeShape = typeShape},
                "class TestClass : IDoesSomething",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_WithSupertypes_OnlySuperclass()
        {
            var thisType = Names.Type("TestClass,P");
            var superType = Names.Type("SuperClass,P");

            var sst = new SST {EnclosingType = thisType};
            var typeShape = new TypeShape
            {
                TypeHierarchy = new TypeHierarchy
                {
                    Element = thisType,
                    Extends = new TypeHierarchy {Element = superType}
                }
            };

            AssertPrintWithCustomContext(
                sst,
                new SSTPrintingContext {TypeShape = typeShape},
                "class TestClass : SuperClass",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_FullClass()
        {
            var sst = new SST
            {
                EnclosingType = Names.Type("TestClass,P"),
                Delegates =
                {
                    new DelegateDeclaration {Name = Names.Type("d:[T,P][TestDelegate,P].()").AsDelegateTypeName}
                },
                Events =
                {
                    new EventDeclaration
                    {
                        Name = Names.Event("[EventType,P] [TestClass,P].SomethingHappened")
                    }
                },
                Fields =
                {
                    new FieldDeclaration {Name = Names.Field("[FieldType,P] [TestClass,P].SomeField")},
                    new FieldDeclaration {Name = Names.Field("[FieldType,P] [TestClass,P].AnotherField")}
                },
                Properties =
                {
                    new PropertyDeclaration
                    {
                        Name = Names.Property("get set [PropertyType,P] [TestClass,P].SomeProperty()")
                    }
                },
                Methods =
                {
                    new MethodDeclaration {Name = Names.Method("[ReturnType,P] [TestClass,P].M([ParameterType,P] p)")},
                    new MethodDeclaration
                    {
                        Name = Names.Method("[ReturnType,P] [TestClass,P].M2()"),
                        Body = {new BreakStatement()}
                    }
                }
            };

            AssertPrint(
                sst,
                "class TestClass",
                "{",
                "    delegate TestDelegate();",
                "",
                "    event EventType SomethingHappened;",
                "",
                "    FieldType SomeField;",
                "    FieldType AnotherField;",
                "",
                "    PropertyType SomeProperty { get; set; }",
                "",
                "    ReturnType M(ParameterType p) { }",
                "",
                "    ReturnType M2()",
                "    {",
                "        break;",
                "    }",
                "}");
        }

        [Test]
        public void SSTDeclaration_Interface()
        {
            var sst = new SST {EnclosingType = Names.Type("i:SomeInterface,P")};

            AssertPrint(
                sst,
                "interface SomeInterface",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_Struct()
        {
            var sst = new SST {EnclosingType = Names.Type("s:SomeStruct,P")};

            AssertPrint(
                sst,
                "struct SomeStruct",
                "{",
                "}");
        }

        [Test]
        public void SSTDeclaration_Enum()
        {
            var sst = new SST {EnclosingType = Names.Type("e:SomeEnum,P")};

            AssertPrint(
                sst,
                "enum SomeEnum",
                "{",
                "}");
        }

        [Test]
        public void DelegateDeclaration_Parameterless()
        {
            var sst = new DelegateDeclaration
            {
                Name = Names.Type("d:[R, P] [Some.DelegateType, P].()").AsDelegateTypeName
            };
            AssertPrint(sst, "delegate DelegateType();");
        }

        [Test]
        public void DelegateDeclaration_WithParameters()
        {
            var sst = new DelegateDeclaration
            {
                Name = Names.Type("d:[R, P] [Some.DelegateType, P].([C, P] p1, [D, P] p2)").AsDelegateTypeName
            };

            AssertPrint(sst, "delegate DelegateType(C p1, D p2);");
        }

        [Test]
        public void DelegateDeclaration_Generic()
        {
            var sst = new DelegateDeclaration
            {
                Name = Names.Type("d:[R, P] [Some.DelegateType`1[[T]], P].([T] p1)").AsDelegateTypeName
            };

            AssertPrint(sst, "delegate DelegateType<T>(T p1);");
        }

        [Test]
        public void DelegateDeclaration_Generic2()
        {
            var sst = new DelegateDeclaration
            {
                Name = Names.Type("d:[R, P] [Some.DelegateType`1[[T -> ?]], P].([T -> ?] p1)").AsDelegateTypeName
            };

            AssertPrint(sst, "delegate DelegateType<?>(? p1);");
        }

        [Test]
        public void DelegateDeclaration_Generic3()
        {
            var sst = new DelegateDeclaration
            {
                Name =
                    Names.Type("d:[R, P] [Some.DelegateType`1[[T -> n.G1,P]], P].([T -> n.G1,P] p1)").AsDelegateTypeName
            };

            AssertPrint(sst, "delegate DelegateType<G1>(G1 p1);");
        }

        [Test]
        public void EventDeclaration()
        {
            var sst = new EventDeclaration
            {
                Name = Names.Event("[EventType,P] [DeclaringType,P].E")
            };

            AssertPrint(sst, "event EventType E;");
        }

        [Test]
        public void EventDeclaration_GenericEventArgsType()
        {
            var sst = new EventDeclaration
            {
                Name = Names.Event("[EventType`1[[T -> EventArgsType,P]],P] [DeclaringType,P].E")
            };

            AssertPrint(sst, "event EventType<EventArgsType> E;");
        }

        [Test]
        public void FieldDeclaration()
        {
            var sst = new FieldDeclaration
            {
                Name = Names.Field("[FieldType,P] [DeclaringType,P].F")
            };

            AssertPrint(sst, "FieldType F;");
        }

        [Test]
        public void FieldDeclaration_Static()
        {
            var sst = new FieldDeclaration
            {
                Name = Names.Field("static [FieldType,P] [DeclaringType,P].F")
            };

            AssertPrint(sst, "static FieldType F;");
        }

        [Test]
        public void FieldDeclaration_Array()
        {
            var sst = new FieldDeclaration {Name = Names.Field("[d:[V, A] [N.TD, A].()[]] [DT, A]._delegatesField")};

            AssertPrint(sst, "TD[] _delegatesField;");
        }

        [Test]
        public void PropertyDeclaration_GetterOnly()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("get [PropertyType,P] [DeclaringType,P].P()")
            };

            AssertPrint(sst, "PropertyType P { get; }");
        }

        [Test]
        public void PropertyDeclaration_SetterOnly()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("set [PropertyType,P] [DeclaringType,P].P()")
            };

            AssertPrint(sst, "PropertyType P { set; }");
        }

        [Test]
        public void PropertyDeclaration()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("get set [PropertyType,P] [DeclaringType,P].P()")
            };

            AssertPrint(sst, "PropertyType P { get; set; }");
        }

        [Test]
        public void PropertyDeclaration_WithBodies()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("get set [PropertyType,P] [DeclaringType,P].P()"),
                Get =
                {
                    new ContinueStatement(),
                    new BreakStatement()
                },
                Set =
                {
                    new BreakStatement(),
                    new ContinueStatement()
                }
            };

            AssertPrint(
                sst,
                "PropertyType P",
                "{",
                "    get",
                "    {",
                "        continue;",
                "        break;",
                "    }",
                "    set",
                "    {",
                "        break;",
                "        continue;",
                "    }",
                "}");
        }

        [Test]
        public void PropertyDeclaration_WithOnlyGetterBody()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("get set [PropertyType,P] [DeclaringType,P].P()"),
                Get =
                {
                    new BreakStatement()
                }
            };

            AssertPrint(
                sst,
                "PropertyType P",
                "{",
                "    get",
                "    {",
                "        break;",
                "    }",
                "    set;",
                "}");
        }

        [Test]
        public void PropertyDeclaration_WithOnlySetterBody()
        {
            var sst = new PropertyDeclaration
            {
                Name = Names.Property("get set [PropertyType,P] [DeclaringType,P].P()"),
                Set =
                {
                    new BreakStatement()
                }
            };

            AssertPrint(
                sst,
                "PropertyType P",
                "{",
                "    get;",
                "    set",
                "    {",
                "        break;",
                "    }",
                "}");
        }

        [Test]
        public void MethodDeclaration_EmptyMethod()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M(ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_Static()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("static [ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)")
            };

            AssertPrint(
                sst,
                "static ReturnType M(ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_ExtensionMethod()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("static [ReturnType,P] [DeclaringType,P].M(this [ParameterType,P] p)")
            };

            AssertPrint(
                sst,
                "static ReturnType M(this ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_ParameterModifiers_PassedByReference()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M(ref [p:int] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M(ref int p) { }");
        }

        [Test]
        public void MethodDeclaration_ParameterModifiers_Output()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M(out [ParameterType,P] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M(out ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_ParameterModifiers_Params()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M(params [ParameterType[],P] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M(params ParameterType[] p) { }");
        }

        [Test]
        public void MethodDeclaration_ParameterModifiers_Optional()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M(opt [ParameterType,P] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M(opt ParameterType p) { }");
        }

        [Test]
        public void MethodDeclaration_WithBody()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType,P] [DeclaringType,P].M([ParameterType,P] p)"),
                Body =
                {
                    new ContinueStatement(),
                    new BreakStatement()
                }
            };

            AssertPrint(
                sst,
                "ReturnType M(ParameterType p)",
                "{",
                "    continue;",
                "    break;",
                "}");
        }

        [Test]
        public void MethodDeclaration_Generic()
        {
            var sst = new MethodDeclaration
            {
                Name = Names.Method("[ReturnType, P] [DeclaringType, P].M`1[[T -> T]]([T] p)")
            };

            AssertPrint(
                sst,
                "ReturnType M<T>(T p) { }");
        }

        [Test]
        public void VariableDeclaration()
        {
            var sst = SSTUtil.Declare("var", Names.Type("T,P"));

            AssertPrint(sst, "T var;");
        }

        [Test]
        public void VariableDeclaration_TranslatesSimpleTypeBackToAlias()
        {
            var sst = SSTUtil.Declare("var", Names.Type("System.Int32, mscore, 4.0.0.0"));

            AssertPrint(sst, "int var;");
        }
    }
}