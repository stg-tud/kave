using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Events.CompletionEvent;
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
                EnclosingMethodHierarchy = Create("N.Return", "N.Class", "Method", "N.Argument")
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
                EnclosingMethodHierarchy = Create("N.Return", "N.Class", "Method")
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
                EnclosingMethodHierarchy = Create("N.Return", "N.Class", "Method", "N.Argument")
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
                EnclosingMethodHierarchy = Create("N.Return", "N.Class", "Method", "N.Arg0", "N.Arg1", "N.Arg2")
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
                EnclosingMethodHierarchy = Create("N.Return", "N.Class", "Method"),
            };
            context.CalledMethods.Add(Call("N.R1", "N.T", "M1"));
            context.CalledMethods.Add(Call("N.R2", "N.T", "M2", "N.Arg0"));
            context.CalledMethods.Add(Call("N.R3", "N.T", "M3", "N.Arg0", "N.Arg1"));
            context.CalledMethods.Add(Call("N.R1", "X.X", "Y"));

            var expected = Bold("class") + @" N.Class
{
  N.Return Method()
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

        private static MethodHierarchy Create(string returnTypeName,
            string className,
            string methodName,
            params string[] argTypes)
        {
            return new MethodHierarchy(Call(returnTypeName, className, methodName, argTypes));
        }

        private static MethodName Call(string returnTypeName,
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
            return string.Format("{0}, Assembly, Version=1.0.0.0", type);
        }

        private static string Bold(string el)
        {
            return "<Bold>" + el + "</Bold>";
        }
    }
}