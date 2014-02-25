using System;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    // TODO make classname match tested class's name
    [TestFixture]
    internal class ContextVisualizationTest
    {
        #region AssertionHelper

        // TODO move this region to a fixture class

        private const string LineBreak = ContextVisualizationConverter.LineBreak;
        private const string Indent = ContextVisualizationConverter.Indent;
        private const string CompletionMarker = ContextVisualizationConverter.Completion;
        private const string Class = "<Bold>class</Bold>";
        private const string Inherits = "<Bold> : </Bold>";

        private const string ValidMethod =
            "[Namespace.Return, Assembly, Version=1.0.0.0] [Namespace.Class, Assembly, Version=1.0.0.0].Method([Namespace.Argument, Assembly, Version=1.0.0.0] argument)";

        private static string ValidType(string type = "Class")
        {
            return string.Format("Namespace.{0}, Assembly, Version=1.0.0.0", type);
        }

        #endregion

        // TODO move each region into a separate test class
        // TODO kommentare in methodennamen ueberfuehren
        // TODO alles was mehr als eine Konstante ist in actual/expected variablen schreiben vor dem assert

        #region NoContext

        [Test]
        public void ShouldHandleNullAsContext()
        {
            var actual = ContextVisualizationConverter.ToXaml((Context) null);
            Assert.IsNull(actual);
        }

        [Test]
        public void ShouldHandleEmptyContext()
        {
            var context = new Context();

            // empty context does not offer any information and is therefore equivalent to no context
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithEnclosingMethod()
        {
            var context = new Context
            {
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            // context with only a method is invalid because method has to be defined inside some class
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithEmptyHierarchy()
        {
            // TODO neuen konstruktor fuer type hierarchy benutzen
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy()
            };

            // empty hierarchy does not offer any information and is therefore equivalent to no context
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithExtends()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Extends = new TypeHierarchy()
                }
            };

            // hierarchy without Element is invalid
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithNamedExtends()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType())
                    }
                }
            };

            // hierarchy without Element is invalid
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithEmptyImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy()
            };

            // hierarchy without Element is invalid
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy()
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy());

            // hierarchy without Element is invalid
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithNamedImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy()
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy {Element = TypeName.Get(ValidType())});

            // hierarchy without Element is invalid
            Assert.IsNull(ContextVisualizationConverter.ToXaml(context));
        }

        #endregion

        #region ValidHierarchyWithoutMethod

        [Test]
        public void ShouldHandleContextWithHierarchyWithElement()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType())
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            // TODO expectations umformulieren
            /*expected = Bold("class") + " Namespace.Class\n\r" +
                       "{\n\r" +
                       Indent + CompletionMarker + "\n\r" +
                       "}";

            expected = @"class Namespace.Class
{
  CompletionMarker
}";*/

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        private static string Bold(string text)
        {
            return "<Bold>" + text + "</Bold>";
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndExtends()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy()
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtends()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    }
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndEmptyImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy());

            var expected = String.Format(
                "{2} Namespace.Class{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy {Element = TypeName.Get(ValidType("Interface"))});

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Interface{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtendsAndImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    },
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy {Element = TypeName.Get(ValidType("Interface"))});

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtendsAndMultipleImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    },
                }
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy {Element = TypeName.Get(ValidType("Interface1"))});
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy {Element = TypeName.Get(ValidType("Interface2"))});

            var expected =
                String.Format(
                    "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface1, Namespace.Interface2{0}{{{0}{3}{1}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        #endregion

        #region ValidHierarchyWithMethod

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType())
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndExtendsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy()
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtendsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    }
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndEmptyImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(new TypeHierarchy());

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType("Interface"))
                });

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Interface{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtendsAndImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    },
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType("Interface"))
                });

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                LineBreak,
                CompletionMarker,
                Class,
                Indent,
                Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedExtendsAndMultipleImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Extends = new TypeHierarchy
                    {
                        Element = TypeName.Get(ValidType("Super"))
                    },
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType("Interface1"))
                });
            context.EnclosingClassHierarchy.Implements.Add(
                new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType("Interface2"))
                });

            var expected =
                String.Format(
                    "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface1, Namespace.Interface2{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    CompletionMarker,
                    Class,
                    Indent,
                    Inherits);

            Assert.AreEqual(expected, ContextVisualizationConverter.ToXaml(context));
        }

        #endregion
    }
}