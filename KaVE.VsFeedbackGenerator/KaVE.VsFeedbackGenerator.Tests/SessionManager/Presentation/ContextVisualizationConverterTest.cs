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
            var context = new Context {EnclosingMethod = MethodName.Get(Fix.GetMethod)};

            var xaml = context.ToXaml();
            Assert.IsNull(xaml);
        }

        [Test]
        public void ShouldHandleMinimalContext()
        {
            var context = new Context {EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())};

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, Fix.CompletionMarker),
                "}");

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

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, Fix.CompletionMarker),
                "}");

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithImplements()
        {
            var context = new Context {EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType())};
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.I"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, Fix.CompletionMarker),
                "}");

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

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super, Namespace.I"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, Fix.CompletionMarker),
                "}");

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

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super, Namespace.I1, Namespace.I2"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, Fix.CompletionMarker),
                "}");

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType()),
                EnclosingMethod = MethodName.Get(Fix.GetMethod)
            };

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, "Namespace.Return Method(Namespace.Argument argument)"),
                Fix.Line(Fix.Indent, "{"),
                Fix.Line(Fix.Indent, Fix.Indent, Fix.CompletionMarker),
                Fix.Line(Fix.Indent, "}"),
                "}");

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
                EnclosingMethod = MethodName.Get(Fix.GetMethod)
            };

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, "Namespace.Return Method(Namespace.Argument argument)"),
                Fix.Line(Fix.Indent, "{"),
                Fix.Line(Fix.Indent, Fix.Indent, Fix.CompletionMarker),
                Fix.Line(Fix.Indent, "}"),
                "}");

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHandleContextWithImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy(Fix.CreateType()),
                EnclosingMethod = MethodName.Get(Fix.GetMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.I"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, "Namespace.Return Method(Namespace.Argument argument)"),
                Fix.Line(Fix.Indent, "{"),
                Fix.Line(Fix.Indent, Fix.Indent, Fix.CompletionMarker),
                Fix.Line(Fix.Indent, "}"),
                "}");

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
                EnclosingMethod = MethodName.Get(Fix.GetMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I")));

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super, Namespace.I"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, "Namespace.Return Method(Namespace.Argument argument)"),
                Fix.Line(Fix.Indent, "{"),
                Fix.Line(Fix.Indent, Fix.Indent, Fix.CompletionMarker),
                Fix.Line(Fix.Indent, "}"),
                "}");

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
                EnclosingMethod = MethodName.Get(Fix.GetMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I1")));
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy(Fix.CreateType("I2")));

            var expected = string.Concat(
                Fix.Line(Fix.Class, " Namespace.Class", Fix.Inherits, "Namespace.Super, Namespace.I1, Namespace.I2"),
                Fix.Line("{"),
                Fix.Line(Fix.Indent, "Namespace.Return Method(Namespace.Argument argument)"),
                Fix.Line(Fix.Indent, "{"),
                Fix.Line(Fix.Indent, Fix.Indent, Fix.CompletionMarker),
                Fix.Line(Fix.Indent, "}"),
                "}");

            var actual = context.ToXaml();
            Assert.AreEqual(expected, actual);
        }
    }
}