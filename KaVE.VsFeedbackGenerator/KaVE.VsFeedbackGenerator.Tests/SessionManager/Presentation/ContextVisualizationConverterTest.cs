using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;
using Fix = KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation.ContextVisualizationConverterFixtures;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class ContextVisualizationConverterTest
    {
        [Test]
        public void ShouldHandleNoContext()
        {
            var xaml = ((Context) null).ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleEmptyContextLikeNoContext()
        {
            var context = new Context();

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleContextWithoutHierarchyLikeNoContext()
        {
            var context = new Context
            {
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleMinimalContext()
        {
            var context = new Context {EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())};

            var expected = Fix.Bold("class") + @" N.Class
{
" + Fix.Indent + Fix.CompletionMarker + @"
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtends()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                }
            };

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super
{
" + Fix.Indent + Fix.CompletionMarker + @"
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithImplements()
        {
            var context = new Context {EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())};
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.I
{
" + Fix.Indent + Fix.CompletionMarker + @"
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super, N.I
{
" + Fix.Indent + Fix.CompletionMarker + @"
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndMultipleImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I1")));
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I2")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super, N.I1, N.I2
{
" + Fix.Indent + Fix.CompletionMarker + @"
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType()),
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };

            var expected = Fix.Bold("class") + @" N.Class
{
" + Fix.Indent + @"N.Return Method(N.Argument argument)
" + Fix.Indent + @"{
" + Fix.Indent + Fix.Indent + Fix.CompletionMarker + @"
" + Fix.Indent + @"}
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                },
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super
{
" + Fix.Indent + @"N.Return Method(N.Argument argument)
" + Fix.Indent + @"{
" + Fix.Indent + Fix.Indent + Fix.CompletionMarker + @"
" + Fix.Indent + @"}
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType()),
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.I
{
" + Fix.Indent + @"N.Return Method(N.Argument argument)
" + Fix.Indent + @"{
" + Fix.Indent + Fix.Indent + Fix.CompletionMarker + @"
" + Fix.Indent + @"}
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                },
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super, N.I
{
" + Fix.Indent + @"N.Return Method(N.Argument argument)
" + Fix.Indent + @"{
" + Fix.Indent + Fix.Indent + Fix.CompletionMarker + @"
" + Fix.Indent + @"}
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithExtendsAndMultipleImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())
                {
                    Extends = new TypeHierarchy(Fix.CreateType("Super"))
                },
                EnclosingMethodDeclaration = new MethodDeclaration(MethodName.Get(Fix.GetMethod))
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I1")));
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I2")));

            var expected = Fix.Bold("class") + @" N.Class" + Fix.Bold(" : ") + @"N.Super, N.I1, N.I2
{
" + Fix.Indent + @"N.Return Method(N.Argument argument)
" + Fix.Indent + @"{
" + Fix.Indent + Fix.Indent + Fix.CompletionMarker + @"
" + Fix.Indent + @"}
}";

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }
    }
}