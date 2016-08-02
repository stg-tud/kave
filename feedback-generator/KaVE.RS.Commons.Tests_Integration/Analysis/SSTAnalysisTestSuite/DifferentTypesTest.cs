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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.TypeShapes;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Integration.Analysis.SSTAnalysisTestSuite
{
    internal class DifferentTypesTest : BaseSSTAnalysisTest
    {
        [Test]
        public void InClass()
        {
            CompleteInNamespace(@"
                class C {
                    $
                }
            ");

            AssertTriggerInEmptyType("N.C, TestProject");
        }

        [Test]
        public void InClassWithDeclarations()
        {
            CompleteInNamespace(@"
                class C {
                    public delegate int D();
                    public event Action E;
                    public int F;
                    public int P { get; set; }
                    public void M() {}
                    $
                }
            ");

            var type = Names.Type("N.C, TestProject");
            var method = Names.Method("[p:void] [N.C, TestProject].M()");
            Assert.AreEqual(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = type
                        },
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = method
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = type,
                        Delegates =
                        {
                            new DelegateDeclaration
                            {
                                Name = Names.Type("d:[p:int] [N.C+D, TestProject].()").AsDelegateTypeName
                            }
                        },
                        Events =
                        {
                            new EventDeclaration
                            {
                                Name =
                                    Names.Event(
                                        "[d:[p:void] [System.Action, mscorlib, 4.0.0.0].()] [N.C, TestProject].E")
                            }
                        },
                        Fields = {new FieldDeclaration {Name = Names.Field("[p:int] [N.C, TestProject].F")}},
                        Methods =
                        {
                            new MethodDeclaration {Name = method, IsEntryPoint = true}
                        },
                        Properties =
                        {
                            new PropertyDeclaration {Name = Names.Property("set get [p:int] [N.C, TestProject].P()")}
                        }
                    }
                },
                ResultContext);
        }

        [Test]
        public void InEnum()
        {
            CompleteInNamespace(@"
                enum E {
                    $
                }
            ");

            AssertTriggerInEmptyType("e:N.E, TestProject");
        }

        [Test]
        public void InEnumWithDeclarations()
        {
            CompleteInNamespace(@"
                enum E {
                    X,Y,$
                }
            ");

            var type = Names.Type("e:N.E, TestProject");
            Assert.AreEqual(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = type
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = type,
                        Fields =
                        {
                            new FieldDeclaration {Name = Names.Field("[{0}] [{0}].X", type)},
                            new FieldDeclaration {Name = Names.Field("[{0}] [{0}].Y", type)}
                        }
                    }
                },
                ResultContext);
        }

        [Test]
        public void InInterface()
        {
            CompleteInNamespace(@"
                interface I {
                    $
                }
            ");

            AssertTriggerInEmptyType("i:N.I, TestProject");
        }

        [Test]
        public void InInterfaceWithDeclarations()
        {
            CompleteInNamespace(@"
                interface I {
                    event Action E;
                    int P { get; set; }
                    void M();
                    $
                }
            ");

            var type = Names.Type("i:N.I, TestProject");
            var method = Names.Method("[p:void] [i:N.I, TestProject].M()");
            Assert.AreEqual(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = type
                        },
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = method
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = type,
                        Events =
                        {
                            new EventDeclaration
                            {
                                Name =
                                    Names.Event(
                                        "[d:[p:void] [System.Action, mscorlib, 4.0.0.0].()] [i:N.I, TestProject].E")
                            }
                        },
                        Methods =
                        {
                            new MethodDeclaration {Name = method}
                        },
                        Properties =
                        {
                            new PropertyDeclaration {Name = Names.Property("set get [p:int] [i:N.I, TestProject].P()")}
                        }
                    }
                },
                ResultContext);
        }

        [Test]
        public void InStruct()
        {
            CompleteInNamespace(@"
                struct S {
                    $
                }
            ");

            AssertTriggerInEmptyType("s:N.S, TestProject");
        }

        [Test]
        public void InStructWithDeclarations()
        {
            CompleteInNamespace(@"
                struct S {
                    public delegate int D();
                    public event Action E;
                    public int F;
                    public int P { get; set; }
                    public void M() {}
                  $
                }
            ");

            var type = Names.Type("s:N.S, TestProject");
            var method = Names.Method("[p:void] [s:N.S, TestProject].M()");
            Assert.AreEqual(
                new Context
                {
                    TypeShape = new TypeShape
                    {
                        TypeHierarchy = new TypeHierarchy
                        {
                            Element = type
                        },
                        MethodHierarchies =
                        {
                            new MethodHierarchy
                            {
                                Element = method
                            }
                        }
                    },
                    SST = new SST
                    {
                        EnclosingType = type,
                        Delegates =
                        {
                            new DelegateDeclaration
                            {
                                Name = Names.Type("d:[p:int] [N.S+D, TestProject].()").AsDelegateTypeName
                            }
                        },
                        Events =
                        {
                            new EventDeclaration
                            {
                                Name =
                                    Names.Event(
                                        "[d:[p:void] [System.Action, mscorlib, 4.0.0.0].()] [s:N.S, TestProject].E")
                            }
                        },
                        Fields = {new FieldDeclaration {Name = Names.Field("[p:int] [s:N.S, TestProject].F")}},
                        Methods =
                        {
                            new MethodDeclaration {Name = method, IsEntryPoint = true}
                        },
                        Properties =
                        {
                            new PropertyDeclaration {Name = Names.Property("set get [p:int] [s:N.S, TestProject].P()")}
                        }
                    }
                },
                ResultContext);
        }

        [Test]
        public void InNestedClass()
        {
            CompleteInNested(@"
                class C {
                    $
                }
            ");

            AssertTriggerInEmptyType("N.O+C, TestProject");
        }

        [Test]
        public void InNestedInterface()
        {
            CompleteInNested(@"
                interface I {
                    $
                }
            ");

            AssertTriggerInEmptyType("i:N.O+I, TestProject");
        }

        [Test]
        public void InNestedStruct()
        {
            CompleteInNested(@"
                struct S {
                    $
                }
            ");

            AssertTriggerInEmptyType("s:N.O+S, TestProject");
        }

        private void CompleteInNested(string declaration)
        {
            CompleteInNamespace(@"
                class O {
                    " + declaration + @"
                }
            ");
        }

        private void AssertTriggerInEmptyType(string typeId)
        {
            var actual = ResultContext;
            var expected = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy
                    {
                        Element = Names.Type(typeId)
                    }
                },
                SST = new SST {EnclosingType = Names.Type(typeId)}
            };
            Assert.AreEqual(expected, actual);
        }
    }
}