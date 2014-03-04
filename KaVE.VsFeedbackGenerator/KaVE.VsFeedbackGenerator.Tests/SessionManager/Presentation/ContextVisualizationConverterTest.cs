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
        private const string CompletionMarker = "<Italic Foreground=\"Blue\">@Completion</Italic>";

        [Test]
        public void ShouldHandleNoContext()
        {
            var xaml = ((Context) null).ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        // TODO review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleEmptyContextLikeNoContext()
        {
            var context = new Context();

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        // TODO review: is it better to return a message that refers to an incomplete context?
        public void ShouldHandleContextWithoutHierarchyLikeNoContext()
        {
            var context = new Context
            {
                EnclosingMethodDeclaration = Create("N.Return", "N.Class", "Method", "N.Argument")
            };

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleMinimalContext()
        {
            var context = new Context {EnclosingClassHierarchy = CreateTypeHierarchy("N.Class")};

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
                EnclosingClassHierarchy = new TypeHierarchy(CreateType("N.Class"))
                {
                    Extends = CreateTypeHierarchy("N.Super")
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
                EnclosingClassHierarchy = new TypeHierarchy(CreateType("N.Class"))
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        CreateTypeHierarchy("N.I")
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
                EnclosingClassHierarchy = new TypeHierarchy(CreateType("N.Class"))
                {
                    Extends = CreateTypeHierarchy("N.Super"),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        CreateTypeHierarchy("N.I")
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
                EnclosingClassHierarchy = new TypeHierarchy(CreateType("N.Class"))
                {
                    Extends = CreateTypeHierarchy("N.Super"),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        CreateTypeHierarchy("N.I1"),
                        CreateTypeHierarchy("N.I2")
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
                EnclosingClassHierarchy = CreateTypeHierarchy("N.Class"),
                EnclosingMethodDeclaration = Create("N.Return", "N.Class", "Method")
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
                EnclosingClassHierarchy = CreateTypeHierarchy("N.Class"),
                EnclosingMethodDeclaration = Create("N.Return", "N.Class", "Method", "N.Argument")
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
                EnclosingClassHierarchy = CreateTypeHierarchy("N.Class"),
                EnclosingMethodDeclaration = Create("N.Return", "N.Class", "Method", "N.Arg0", "N.Arg1", "N.Arg2")
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

        private static MethodDeclaration Create(string returnTypeName,
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
            return new MethodDeclaration(MethodName.Get(methodSignature));
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