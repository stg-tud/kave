using System;
using System.Collections.Generic;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class ContextVisualizationTest
    {
        #region AssertionHelper

        private const string NoContext = ContextVisualizationTextBlock.NoContext;
        private const string LineBreak = ContextVisualizationTextBlock.LineBreak;
        private const string Indent = ContextVisualizationTextBlock.Indent;
        private const string Completion = ContextVisualizationTextBlock.Completion;
        private const string Class = "<Bold>class</Bold>";
        private const string Inherits = "<Bold> : </Bold>";

        private const string ValidMethod =
            "[Namespace.Return, Assembly, Version=1.0.0.0] [Namespace.Class, Assembly, Version=1.0.0.0].Method([Namespace.Argument, Assembly, Version=1.0.0.0] argument)";

        private static string ValidType(string type = "Class")
        {
            return string.Format("Namespace.{0}, Assembly, Version=1.0.0.0", type);
        }

        private static ContextVisualizationTextBlock TextBlock
        {
            get { return new ContextVisualizationTextBlock(); }
        }

        private static void AssertConvertable(Context context, string expectedConvertion)
        {
            Assert.AreEqual(expectedConvertion, ContextVisualizationTextBlock.ToXamlRepresentation(context));
            try
            {
                //TextBlock.Context = context;
            }
            catch (Exception e)
            {
                Assert.Fail(
                    "Context with expected representation \"{0}\" wasn't accepted by ContextVisualizationTextBlock: {1}",
                    expectedConvertion,
                    e.Message);
            }
        }

        #endregion

        #region NoContext

        [Test]
        public void ShouldHandleNullAsContext()
        {
            AssertConvertable(null, NoContext);
        }

        [Test]
        public void ShouldHandleEmptyContext()
        {
            var context = new Context();

            // empty context does not offer any information and is therefore equivalent to no context
            AssertConvertable(context, NoContext);
        }

        [Test]
        public void ShouldHandleContextWithEnclosingMethod()
        {
            var context = new Context
            {
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            // context with only a method is invalid because method has to be defined inside some class
            AssertConvertable(context, NoContext);
        }

        [Test]
        public void ShouldHandleContextWithEmptyHierarchy()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy()
            };

            // empty hierarchy does not offer any information and is therefore equivalent to no context
            AssertConvertable(context, NoContext);
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
            AssertConvertable(context, NoContext);
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
            AssertConvertable(context, NoContext);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithEmptyImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Implements = new HashSet<ITypeHierarchy>()
                }
            };

            // hierarchy without Element is invalid
            AssertConvertable(context, NoContext);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy()
                    }
                }
            };

            // hierarchy without Element is invalid
            AssertConvertable(context, NoContext);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithNamedImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType())
                        }
                    }
                }
            };

            // hierarchy without Element is invalid
            AssertConvertable(context, NoContext);
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

            var expected = String.Format("{2} Namespace.Class{0}{{{0}{3}{1}{0}}}", LineBreak, Completion, Class, Indent, Inherits);

            AssertConvertable(context, expected);
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

            var expected = String.Format("{2} Namespace.Class{0}{{{0}{3}{1}{0}}}", LineBreak, Completion, Class, Indent, Inherits);

            AssertConvertable(context, expected);
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
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndEmptyImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>()
                }
            };

            var expected = String.Format("{2} Namespace.Class{0}{{{0}{3}{1}{0}}}", LineBreak, Completion, Class, Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy()
                    }
                }
            };

            var expected = String.Format("{2} Namespace.Class{0}{{{0}{3}{1}{0}}}", LineBreak, Completion, Class, Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedImplements()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface"))
                        }
                    }
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Interface{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface"))
                        }
                    }
                }
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface{0}{{{0}{3}{1}{0}}}",
                LineBreak,
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface1"))
                        },
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface2"))
                        }
                    }
                }
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface1, Namespace.Interface2{0}{{{0}{3}{1}{0}}}",
                    LineBreak,
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
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
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndEmptyImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>()
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy()
                    }
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
        }

        [Test]
        public void ShouldHandleContextWithHierarchyWithElementAndNamedImplementsAndMethod()
        {
            var context = new Context
            {
                EnclosingClassHierarchy = new TypeHierarchy
                {
                    Element = TypeName.Get(ValidType()),
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface"))
                        }
                    }
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Interface{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                LineBreak,
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface"))
                        }
                    }
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected = String.Format(
                "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                LineBreak,
                Completion,
                Class,
                Indent, Inherits);

            AssertConvertable(context, expected);
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
                    Implements = new HashSet<ITypeHierarchy>
                    {
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface1"))
                        },
                        new TypeHierarchy
                        {
                            Element = TypeName.Get(ValidType("Interface2"))
                        }
                    }
                },
                EnclosingMethod = MethodName.Get(ValidMethod)
            };

            var expected =
                String.Format(
                    "{2} Namespace.Class{4}Namespace.Super, Namespace.Interface1, Namespace.Interface2{0}{{{0}{3}Namespace.Return Method(Namespace.Argument argument){0}{3}{{{0}{3}{3}{1}{0}{3}}}{0}}}",
                    LineBreak,
                    Completion,
                    Class,
                    Indent, Inherits);

            AssertConvertable(context, expected);
        }

        #endregion
    }
}