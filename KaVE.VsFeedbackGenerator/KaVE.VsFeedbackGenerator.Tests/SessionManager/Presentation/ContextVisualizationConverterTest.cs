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
 *    - Dennis Albrecht
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class ContextVisualizationConverterTest
    {
        private const string CompletionMarker = "<Italic Foreground=\"Blue\">$</Italic>";
        private const string SomeOtherNamespace = "ecapseman";
        private const string SomeGenericType = "NGeneric.Generic`1[[T -> TT]]";
        private const string GenericTypeRepresentation = "Generic`1&lt;T&gt;";

        [Test]
        public void ShouldHandleNoContext()
        {
            var xaml = ((Context) null).ToXaml();
            Assert.IsNull(xaml);
        }

        [Test, Ignore]
        // TODO @Seb review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleEmptyContextLikeNoContext()
        {
            var context = Context.Empty;

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test, Ignore]
        // TODO @Seb review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleContextWithoutHierarchyLikeNoContext()
        {
            var context = new Context
            {
                EnclosingMethod = Method("N.Return", "N.Class", "Method", "N.Argument")
            };

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleMinimalContext()
        {
            var context = new Context {TypeShape = new TypeShape {TypeHierarchy = CreateTypeHierarchy("N.Class")}};

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleTypeWithExtends()
        {
            var context = new Context
            {
                TypeShape =
                    new TypeShape
                    {
                        TypeHierarchy =
                            new TypeHierarchy(CreateType("N.Class")) {Extends = CreateTypeHierarchy("N.Super")}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, Bold("class") + " Class " + Bold(":") + " Super");
        }

        [Test]
        public void ShouldHandleTypeWithImplements()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Implements = new HashSet<ITypeHierarchy>
                        {
                            CreateTypeHierarchy("N.I")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, Bold("class") + " Class " + Bold(":") + " I");
        }

        [Test]
        public void ShouldHandleTypetWithExtendsAndImplements()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Extends = CreateTypeHierarchy("N.Super"),
                        Implements = new HashSet<ITypeHierarchy>
                        {
                            CreateTypeHierarchy("N.I")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, Bold("class") + " Class " + Bold(":") + " Super, I");
        }

        [Test]
        public void ShouldHandleTypeWithExtendsAndMultipleImplements()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Extends = CreateTypeHierarchy("N.Super"),
                        Implements = new HashSet<ITypeHierarchy>
                        {
                            CreateTypeHierarchy("N.I1"),
                            CreateTypeHierarchy("N.I2")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, Bold("class") + " Class " + Bold(":") + " Super, I1, I2");
        }

        [Test]
        public void ShouldHandleContextWithEnclosingMethod()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method")
            };

            var expected = Bold("class") + @" Class
  {
    Return Method()
    {
      " + CompletionMarker + @"
    }
  }";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleEnclosingMethodWithSingleParameters()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method", "N.Argument")
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, "Return Method(Argument arg0)");
        }

        [Test]
        public void ShouldHandleEnclosingMethodWithMultipleParameters()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method", "N.Arg0", "N.Arg1", "N.Arg2")
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, "Return Method(Arg0 arg0, Arg1 arg1, Arg2 arg2)");
        }

        [Test]
        public void ShouldHandleTriggerTarget()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = CreateTypeHierarchy("N.Class")},
                TriggerTarget = Name.Get("Target")
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, "[Target]." + CompletionMarker);
        }

        [Test]
        public void ShouldIncludeAllEntryPoints()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method1"), new HashSet<IMethodName>()
                    },
                    {
                        Method("N.Return", "N.Class", "Method2"), new HashSet<IMethodName>()
                    }
                }
            };

            const string expected = @"Return Method1()
    {
    }
    Return Method2()
    {
    }";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleConstructorAsEntryPoint()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.T", "N.T", ".ctor", "N.Arg"), new HashSet<IMethodName>()
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, "T(Arg arg0)");
            StringAssert.DoesNotContain("T .ctor(Arg arg0)", actual);
        }

        [Test]
        public void ShouldIncludeAllCalledMethodsInEntryPoint()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.R1", "N.T", "M1"),
                            Method("N.R2", "N.T", "M2", "N.Arg0"),
                            Method("N.R3", "N.T", "M3", "N.Arg0", "N.Arg1")
                        }
                    }
                }
            };

            const string expected = @"Return Method()
    {
      T.M1();
      T.M2(Arg0);
      T.M3(Arg0, Arg1);
    }";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldIncludeAllCalledMethodsInEnclosingMethod()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method"),
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.R1", "N.T", "M1"),
                            Method("N.R2", "N.T", "M2", "N.Arg0"),
                            Method("N.R3", "N.T", "M3", "N.Arg0", "N.Arg1")
                        }
                    }
                }
            };

            const string expected = @"Return Method()
    {
      T.M1();
      T.M2(Arg0);
      T.M3(Arg0, Arg1);

      " + CompletionMarker + @"
    }";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleCalledConstructor()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.T", "N.T", ".ctor", "N.Arg")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, "new T(Arg)");
            StringAssert.DoesNotContain("T..ctor(Arg)", actual);
        }

        [Test]
        public void ShouldHandleGenericDeclaringType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType(SomeGenericType))}
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericSuperType()
        {
            var context = new Context
            {
                TypeShape =
                    new TypeShape
                    {
                        TypeHierarchy =
                            new TypeHierarchy(CreateType("N.Class")) {Extends = CreateTypeHierarchy(SomeGenericType)}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericInterfaceType()
        {
            var context = new Context
            {
                TypeShape =
                    new TypeShape
                    {
                        TypeHierarchy =
                            new TypeHierarchy(CreateType("N.Class"))
                            {
                                Implements = new HashSet<ITypeHierarchy>
                                {
                                    CreateTypeHierarchy(SomeGenericType)
                                }
                            }
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericEnclosingMethodsReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EnclosingMethod = Method(SomeGenericType, "N.Ty", "Method", "N.Arg")
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericEnclosingMethodsArgumentType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EnclosingMethod = Method("N.Ret", "N.Ty", "Method", SomeGenericType)
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericEntryPointsReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {Method(SomeGenericType, "N.Ty", "Method", "N.Arg"), new HashSet<IMethodName>()}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericEntryPointsArgumentType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {Method("N.Ret", "N.Ty", "Method", SomeGenericType), new HashSet<IMethodName>()}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericConstructorEntryPointsDeclaringType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {Method(SomeGenericType, SomeGenericType, ".ctor", "N.Arg"), new HashSet<IMethodName>()}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericConstructorEntryPointsArgumentType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {Method("N.Ret", "N.Ty", ".ctor", SomeGenericType), new HashSet<IMethodName>()}
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericCalledMethodsDeclaringType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Ty", "Method", "N.Arg"),
                            new HashSet<IMethodName> {Method("N.Ret1", SomeGenericType, "Method1", "N.Arg1")}
                        }
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericCalledMethodsArgumentType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Ty", "Method", "N.Arg"),
                            new HashSet<IMethodName> {Method("N.Ret1", "N.Ty1", "Method1", SomeGenericType)}
                        }
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericCalledConstructorsDeclaringType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Ty", "Method", "N.Arg"),
                            new HashSet<IMethodName> {Method(SomeGenericType, SomeGenericType, ".ctor", "N.Arg1")}
                        }
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericCalledConstructorsArgumentType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Ty", "Method", "N.Arg"),
                            new HashSet<IMethodName> {Method("N.Ret1", "N.Ty1", ".ctor", SomeGenericType)}
                        }
                    }
            };

            var actual = context.ToXaml();
            AssertContainsOnce(actual, GenericTypeRepresentation);
        }

        [Test]
        public void ShouldHandleGenericEnclosingMethod()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EnclosingMethod = Method("N.Ret", "N.Class", "Method[[T -> TT]]", "N.Arg")
            };

            const string expected = "Method&lt;T&gt;";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleGenericEntryPoint()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Class", "Method[[T -> TT]]", "N.Arg"),
                            new HashSet<IMethodName> {Method("N.Ret1", "N.Ty1", ".ctor", "N.Param")}
                        }
                    }
            };

            const string expected = "Method&lt;T&gt;";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleGenericCalledMethod()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            Method("N.Ret", "N.Class", "Method", "N.Arg"),
                            new HashSet<IMethodName> {Method("N.Ret1", "N.Ty1", "Method[[T -> TT]]", "N.Param")}
                        }
                    }
            };

            const string expected = ".Method&lt;T&gt;";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleGenericEntryPointWithUnboundParameter()
        {
            var context = new Context
            {
                TypeShape = new TypeShape { TypeHierarchy = new TypeHierarchy(CreateType("N.Class")) },
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            MethodName.Get("[N.Ret, A, 1.0.0.0] [N.Class, A, 1.0.0.0].Method[[T -> T]]([T] arg0)"),
                            new HashSet<IMethodName>()
                        }
                    }
            };

            const string expected = "Ret Method&lt;T&gt;(T arg0)";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldHandleGenericEntryPointWithUnboundReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))},
                EntryPointToCalledMethods =
                    new Dictionary<IMethodName, ISet<IMethodName>>
                    {
                        {
                            MethodName.Get("[T] [N.Class, A, 1.0.0.0].Method[[T -> T]]([N.Arg, A, 1.0.0.0] arg0)"),
                            new HashSet<IMethodName>()
                        }
                    }
            };

            const string expected = "T Method&lt;T&gt;(Arg arg0)";

            var actual = context.ToXaml();
            AssertContainsOnce(actual, expected);
        }

        [Test]
        public void ShouldIncludeUsingForSuperclass()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Extends = CreateTypeHierarchy(SomeOtherNamespace, "Super")
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, context.TypeShape.TypeHierarchy.Extends.Element);
        }

        [Test]
        public void ShouldIncludeUsingForInterface()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Implements = new HashSet<ITypeHierarchy> {CreateTypeHierarchy(SomeOtherNamespace, "Inter")}
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, context.TypeShape.TypeHierarchy.Implements.First().Element);
        }

        [Test]
        public void ShouldIncludeUsingForEnclosingMethodParameter()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EnclosingMethod = Method("N.Ret", "N.Ty", "Method", SomeOtherNamespace + ".Arg")
            };

            var actual = context.ToXaml();
            var enclosingMethod = context.EnclosingMethod;
            Assert.NotNull(enclosingMethod);
            AssertContainsUsing(actual, enclosingMethod.Parameters[0].ValueType);
        }

        [Test]
        public void ShouldIncludeUsingForEnclosingMethodReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EnclosingMethod = Method(SomeOtherNamespace + ".Ret", "N.Ty", "Method")
            };

            var actual = context.ToXaml();
            var enclosingMethod = context.EnclosingMethod;
            Assert.NotNull(enclosingMethod);
            AssertContainsUsing(actual, enclosingMethod.ReturnType);
        }

        [Test]
        public void ShouldIncludeUsingForEntryPointParameter()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {Method("N.Ret", "N.Ty", "Method", SomeOtherNamespace + ".Arg"), new HashSet<IMethodName>()}
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, context.EntryPoints.First().Parameters[0].ValueType);
        }

        [Test]
        public void ShouldIncludeUsingForEntryPointReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {Method(SomeOtherNamespace + ".Ret", "N.Ty", "Method"), new HashSet<IMethodName>()}
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, context.EntryPoints.First().ReturnType);
        }

        [Test]
        public void ShouldIncludeUsingForCalledMethodParameter()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Ret", "N.Ty", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.Ret", "N.Ty", "Called", SomeOtherNamespace + ".Arg")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, GetFirstCalledMethodFromFirstEntryPoint(context).Parameters[0].ValueType);
        }

        [Test]
        public void ShouldIncludeUsingForCalledMethodDeclaringType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Ret", "N.Ty", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.Ret", SomeOtherNamespace + ".Type", "Called")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            AssertContainsUsing(actual, GetFirstCalledMethodFromFirstEntryPoint(context).DeclaringType);
        }

        [Test]
        public void ShouldNotIncludeUsingForCalledMethodReturnType()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                },
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Ret", "N.Ty", "Method"), new HashSet<IMethodName>
                        {
                            Method(SomeOtherNamespace + ".Ret", "N.Type", "Called")
                        }
                    }
                }
            };

            var actual = context.ToXaml();
            StringAssert.DoesNotContain("using", actual);
            var typeName = GetFirstCalledMethodFromFirstEntryPoint(context).ReturnType;
            StringAssert.DoesNotContain(typeName.Namespace + "." + typeName.Name, actual);
        }

        private static void AssertContainsUsing(string actual, ITypeName typeName)
        {
            var usingDeclaration = Bold("using") + " " + typeName.Namespace + ";";
            StringAssert.StartsWith(usingDeclaration + "\r\n\r\n" + Bold("namespace"), actual);
            AssertContainsOnce(actual, usingDeclaration);
            StringAssert.DoesNotContain(typeName.Namespace + "." + typeName.Name, actual);
        }

        private static void AssertContainsOnce(string actual, string pattern)
        {
            StringAssert.Contains(pattern, actual);
            var index = actual.IndexOf(pattern, System.StringComparison.Ordinal);
            Assert.AreEqual(
                -1,
                actual.IndexOf(pattern, index + 1, System.StringComparison.Ordinal),
                "Found second occurrence");
        }

        private static IMethodName GetFirstCalledMethodFromFirstEntryPoint(Context context)
        {
            return context.EntryPointToCalledMethods.First().Value.First();
        }

        private static IMethodName Method(string returnTypeName,
            string className,
            string methodName,
            params string[] argTypes)
        {
            var argNo = 0;
            var args = string.Join(", ", argTypes.Select(t => "[" + CreateType(t) + "] arg" + argNo++));
            var methodSignature = string.Format(
                "[{0}] [{1}].{2}({3})",
                CreateType(returnTypeName),
                CreateType(className),
                methodName,
                args);
            return MethodName.Get(methodSignature);
        }

        private static TypeHierarchy CreateTypeHierarchy(string namespaceName, string typeName)
        {
            return CreateTypeHierarchy(namespaceName + "." + typeName);
        }

        private static TypeHierarchy CreateTypeHierarchy(string typeName)
        {
            return new TypeHierarchy(CreateType(typeName));
        }

        private static string CreateType(string type)
        {
            return string.Format("{0}, Assembly, 1.0.0.0", type);
        }

        private static string Bold(string el)
        {
            return "<Bold>" + el + "</Bold>";
        }
    }
}
