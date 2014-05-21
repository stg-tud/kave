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

        [Test]
        public void ShouldHandleNoContext()
        {
            var xaml = ((Context) null).ToXaml();
            Assert.IsNull(xaml);
        }

        [Test, Ignore]
        // TODO review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleEmptyContextLikeNoContext()
        {
            var context = new Context();

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test, Ignore]
        // TODO review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleContextWithoutHierarchyLikeNoContext()
        {
            var context = new Context
            {
                EnclosingMethod = Method("N.Return", "N.Class", "Method", new[] {"N.Argument"})
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
        public void ShouldHandleContextWithExtends()
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

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class " + Bold(":") + @" Super
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithImplements()
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

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class " + Bold(":") + @" I
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndImplements()
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

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class " + Bold(":") + @" Super, I
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndMultipleImplements()
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

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class " + Bold(":") + @" Super, I1, I2
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithMethodAndNoParameters()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method")
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method()
    {
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithMethodAndSingleParameter()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method", new[] {"N.Argument"})
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method(Argument arg0)
    {
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithMethodAndMultipleMethodParameters()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method", new[] {"N.Arg0", "N.Arg1", "N.Arg2"})
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method(Arg0 arg0, Arg1 arg1, Arg2 arg2)
    {
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleCompletionInsideOfConstructor()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Class", "N.Class", ".ctor")
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Class()
    {
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIncludeAllCalledMethodsInArbitraryMethod()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = CreateTypeHierarchy("N.Class")
                },
                EnclosingMethod = Method("N.Return", "N.Class", "Method1"),
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method2"), new HashSet<IMethodName>
                        {
                            Method("N.R1", "N.T", "M1"),
                            Method("N.R2", "N.T", "M2", "N.Arg0"),
                            Method("N.R3", "N.T", "M3", "N.Arg0", "N.Arg1")
                        }
                    }
                }
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method1()
    {
      " + CompletionMarker + @"
    }
    Return Method2()
    {
      T.M1();
      T.M2(Arg0);
      T.M3(Arg0, Arg1);
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
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
                EnclosingMethod = Method("N.Return", "N.Class", "Method"),
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("N.Return", "N.Class", "Method"), new HashSet<IMethodName>
                        {
                            Method("N.T", "N.T", ".ctor", "N.Arg0")
                        }
                    }
                }
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method()
    {
      new T(Arg0);
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
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

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Return Method()
    {
      T.M1();
      T.M2(Arg0);
      T.M3(Arg0, Arg1);
      " + CompletionMarker + @"
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleTriggerTarget()
        {
            var context = new Context
            {
                TypeShape = new TypeShape {TypeHierarchy = CreateTypeHierarchy("N.Class")},
                TriggerTarget = Name.Get("Target")
            };

            var expected = Bold("namespace") + @" N
{
  " + Bold("class") + @" Class
  {
    Target." + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIncludeAllUsings()
        {
            var context = new Context
            {
                TypeShape = new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy(CreateType("N.Class"))
                    {
                        Extends = CreateTypeHierarchy("NsS.Super"),
                        Implements = new HashSet<ITypeHierarchy>
                        {
                            CreateTypeHierarchy("NsI1.I1"),
                            CreateTypeHierarchy("NsI2.I2")
                        }
                    }
                },
                // TODO can DeclaringType differ?
                EnclosingMethod =
                    Method("NsR1.Return1", "N.Class", "Method1", new[] {"NsA0.Arg0", "NsA1.Arg1", "NsA2.Arg2"}),
                EntryPointToCalledMethods = new Dictionary<IMethodName, ISet<IMethodName>>
                {
                    {
                        Method("NsR1.Return1", "N.Class", "Method1", new[] {"NsA0.Arg0", "NsA1.Arg1", "NsA2.Arg2"}),
                        new HashSet<IMethodName>
                        {
                            Method("NsR2.R2", "NsT1.T1", "M1"),
                            Method("NsR3.R3", "NsT2.T2", "M2", "NsA3.Arg3"),
                            Method("NsR4.R4", "NsT3.T3", "M3", "NsA4.Arg4", "NsA5.Arg5")
                        }
                    },
                    {
                        Method("NsR6.Return2", "N.Class", "Method2", new[] {"NsA6.Arg6"}), new HashSet<IMethodName>
                        {
                            Method("NsR5.R5", "NsT4.T4", "M", "NsA7.Arg7"),
                        }
                    }
                }
            };

            var expected = Bold("using") + @" NsA0
" + Bold("using") + @" NsA1
" + Bold("using") + @" NsA2
" + Bold("using") + @" NsA3
" + Bold("using") + @" NsA4
" + Bold("using") + @" NsA5
" + Bold("using") + @" NsA6
" + Bold("using") + @" NsA7
" + Bold("using") + @" NsI1
" + Bold("using") + @" NsI2
" + Bold("using") + @" NsR1
" + Bold("using") + @" NsR2
" + Bold("using") + @" NsR3
" + Bold("using") + @" NsR4
" + Bold("using") + @" NsR5
" + Bold("using") + @" NsR6
" + Bold("using") + @" NsS
" + Bold("using") + @" NsT1
" + Bold("using") + @" NsT2
" + Bold("using") + @" NsT3
" + Bold("using") + @" NsT4
" + Bold("namespace") + @" N
{
  " + Bold("class") + @" Class " + Bold(":") + @" Super, I1, I2
  {
    Return1 Method1(Arg0 arg0, Arg1 arg1, Arg2 arg2)
    {
      T1.M1();
      T2.M2(Arg3);
      T3.M3(Arg4, Arg5);
      " + CompletionMarker + @"
    }
    Return2 Method2(Arg6 arg0)
    {
      T4.M(Arg7);
    }
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
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

        private static TypeHierarchy CreateTypeHierarchy(string typename)
        {
            return new TypeHierarchy(CreateType(typename));
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