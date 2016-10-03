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
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class PropertyHierarchyTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new PropertyHierarchy();
            Assert.AreEqual(Names.UnknownProperty, sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void DefaultValues_CustomConstructor()
        {
            var sut = new PropertyHierarchy(P("x"));
            Assert.AreEqual(P("x"), sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new PropertyHierarchy
            {
                Element = P("a"),
                Super = P("b"),
                First = P("c")
            };
            Assert.AreEqual(P("a"), sut.Element);
            Assert.AreEqual(P("b"), sut.Super);
            Assert.AreEqual(P("c"), sut.First);
        }

        [Test]
        public void ShouldBeOverrideOrImplementationWhenFirstIsSet()
        {
            var uut = new PropertyHierarchy
            {
                First = P("first")
            };
            Assert.IsTrue(uut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new PropertyHierarchy();
            var b = new PropertyHierarchy();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new PropertyHierarchy
            {
                Element = P("a"),
                Super = P("b"),
                First = P("c")
            };
            var b = new PropertyHierarchy
            {
                Element = P("a"),
                Super = P("b"),
                First = P("c")
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElement()
        {
            var a = new PropertyHierarchy {Element = P("a")};
            var b = new PropertyHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSuper()
        {
            var a = new PropertyHierarchy {Super = P("b")};
            var b = new PropertyHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFirst()
        {
            var a = new PropertyHierarchy {First = P("c")};
            var b = new PropertyHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new PropertyHierarchy());
        }

        private static IPropertyName P(string s)
        {
            return Names.Property(string.Format("get set [T1,P1] [T2,P2].{0}()", s));
        }
    }
}