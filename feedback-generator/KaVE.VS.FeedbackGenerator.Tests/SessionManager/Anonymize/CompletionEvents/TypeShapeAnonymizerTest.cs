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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Utils.Naming;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
{
    internal class TypeShapeAnonymizerTest
    {
        private TypeShapeAnonymizer _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new TypeShapeAnonymizer();
        }

        [Test]
        public void TypeHierarchy()
        {
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy
                    {
                        Element = T("elem"),
                        Extends = H("ext"),
                        Implements = {H("impl")}
                    }
                });
            var expected =
                new TypeShape
                {
                    TypeHierarchy = new TypeHierarchy
                    {
                        Element = T("elem").ToAnonymousName(),
                        Extends = AnonH("ext"),
                        Implements = {AnonH("impl")}
                    }
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void NestedTypes()
        {
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    NestedTypes =
                    {
                        T("TN")
                    }
                });
            var expected = new TypeShape
            {
                NestedTypes =
                {
                    T("TN").ToAnonymousName()
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Delegates()
        {
            var delType = Names.Type("d:[T,P] [T,P].D()").AsDelegateTypeName;
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    Delegates =
                    {
                        delType
                    }
                });
            var expected = new TypeShape
            {
                Delegates =
                {
                    delType.ToAnonymousName()
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void EventHierarchies()
        {
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    EventHierarchies =
                    {
                        new EventHierarchy
                        {
                            Element = E("A"),
                            Super = E("B"),
                            First = E("C")
                        }
                    }
                });
            var expected =
                new TypeShape
                {
                    EventHierarchies =
                    {
                        new EventHierarchy
                        {
                            Element = E("A").ToAnonymousName(),
                            Super = E("B").ToAnonymousName(),
                            First = E("C").ToAnonymousName()
                        }
                    }
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Fields()
        {
            var field = Names.Field("[T,P] [T,P]._f");
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    Fields =
                    {
                        field
                    }
                });
            var expected = new TypeShape
            {
                Fields =
                {
                    field.ToAnonymousName()
                }
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MethodHierarchies()
        {
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    MethodHierarchies =
                    {
                        new MethodHierarchy
                        {
                            Element = M("A"),
                            Super = M("B"),
                            First = M("C")
                        }
                    }
                });
            var expected =
                new TypeShape
                {
                    MethodHierarchies =
                    {
                        new MethodHierarchy
                        {
                            Element = M("A").ToAnonymousName(),
                            Super = M("B").ToAnonymousName(),
                            First = M("C").ToAnonymousName()
                        }
                    }
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PropertyHierarchies()
        {
            var actual = _sut.Anonymize(
                new TypeShape
                {
                    PropertyHierarchies =
                    {
                        new PropertyHierarchy
                        {
                            Element = P("A"),
                            Super = P("B"),
                            First = P("C")
                        }
                    }
                });
            var expected =
                new TypeShape
                {
                    PropertyHierarchies =
                    {
                        new PropertyHierarchy
                        {
                            Element = P("A").ToAnonymousName(),
                            Super = P("B").ToAnonymousName(),
                            First = P("C").ToAnonymousName()
                        }
                    }
                };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DefaultSafe()
        {
            var actual = _sut.Anonymize(new TypeShape());
            var expected = new TypeShape();
            Assert.AreEqual(expected, actual);
        }

        private static ITypeName T(string typeName)
        {
            return Names.Type(typeName + ", P");
        }

        private static ITypeHierarchy H(string name)
        {
            return new TypeHierarchy
            {
                Element = T(name),
                Extends = new TypeHierarchy
                {
                    Element = T(name + "_ext")
                },
                Implements =
                {
                    new TypeHierarchy
                    {
                        Element = T(name + "_impl")
                    }
                }
            };
        }

        private static ITypeHierarchy AnonH(string name)
        {
            return new TypeHierarchy
            {
                Element = T(name).ToAnonymousName(),
                Extends = new TypeHierarchy
                {
                    Element = T(name + "_ext").ToAnonymousName()
                },
                Implements =
                {
                    new TypeHierarchy
                    {
                        Element = T(name + "_impl").ToAnonymousName()
                    }
                }
            };
        }

        private static IEventName E(string name)
        {
            return Names.Event(string.Format("[T1,P1] [T2,P2].{0}", name));
        }

        private static IMethodName M(string name)
        {
            return Names.Method(string.Format("[T1,P1] [T2,P2].{0}()", name));
        }

        private static IPropertyName P(string name)
        {
            return Names.Property(string.Format("get set [T1,P1] [T2,P2].{0}()", name));
        }
    }
}