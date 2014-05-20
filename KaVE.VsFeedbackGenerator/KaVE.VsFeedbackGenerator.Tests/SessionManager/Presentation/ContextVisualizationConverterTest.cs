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
        private const string CompletionMarker = ""; //"<Italic Foreground=\"Blue\">@Completion</Italic>";

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

            var expected = Bold("class") + @" N.Class
{
  " + CompletionMarker + @"
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

            var expected = Bold("class") + " N.Class" + Bold(" : ") + @"N.Super
{
  " + CompletionMarker + @"
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

            var expected = Bold("class") + " N.Class" + Bold(" : ") + @"N.I
{
  " + CompletionMarker + @"
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

            var expected = Bold("class") + " N.Class" + Bold(" : ") + @"N.Super, N.I
{
  " + CompletionMarker + @"
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

            var expected = Bold("class") + " N.Class" + Bold(" : ") + @"N.Super, N.I1, N.I2
{
  " + CompletionMarker + @"
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

            var expected = Bold("class") + @" N.Class
{
  N.Return Method()
  {
    " + CompletionMarker + @"
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

            var expected = Bold("class") + @" N.Class
{
  N.Return Method(N.Argument arg0)
  {
    " + CompletionMarker + @"
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

            var expected = Bold("class") + @" N.Class
{
  N.Return Method(N.Arg0 arg0, N.Arg1 arg1, N.Arg2 arg2)
  {
    " + CompletionMarker + @"
  }
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldIncludeAllCalledMethods()
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
                        Method("M.Return", "N.Class", "Method2"), new HashSet<IMethodName>
                        {
                            Method("N.R1", "N.T", "M1"),
                            Method("N.R2", "N.T", "M2", "N.Arg0"),
                            Method("N.R3", "N.T", "M3", "N.Arg0", "N.Arg1"),
                            Method("N.R1", "X.X", "Y")
                        }
                    }
                }
            };

            var expected = Bold("class") + @" N.Class
{
  N.Return Method1()
  {
    
  }
  M.Return Method2()
  {
    T.M1();
    T.M2(Arg0);
    T.M3(Arg0, Arg1);
    X.Y();
    " + CompletionMarker + @"
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