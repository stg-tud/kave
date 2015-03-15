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
 *    - Sebastian Proksch
 */

using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.CSharp.MemberNames;
using KaVE.Model.TypeShapes;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize;
using KaVE.VsFeedbackGenerator.SessionManager.Anonymize.CompletionEvents;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Anonymize.CompletionEvents
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
        public void DefaultSafe()
        {
            var actual = _sut.Anonymize(new TypeShape());
            var expected = new TypeShape();
            Assert.AreEqual(expected, actual);
        }

        private static ITypeName T(string typeName)
        {
            return TypeName.Get(typeName + ", P");
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

        private static IMethodName M(string name)
        {
            return MethodName.Get(string.Format("[T1,P1] [T2,P2].{0}()", name));
        }
    }
}