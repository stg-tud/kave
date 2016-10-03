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
    internal class EventHierarchyTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new EventHierarchy();
            Assert.AreEqual(Names.UnknownEvent, sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void DefaultValues_CustomConstructor()
        {
            var sut = new EventHierarchy(E("x"));
            Assert.AreEqual(E("x"), sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new EventHierarchy
            {
                Element = E("a"),
                Super = E("b"),
                First = E("c")
            };
            Assert.AreEqual(E("a"), sut.Element);
            Assert.AreEqual(E("b"), sut.Super);
            Assert.AreEqual(E("c"), sut.First);
        }

        [Test]
        public void ShouldBeOverrideOrImplementationWhenFirstIsSet()
        {
            var uut = new EventHierarchy
            {
                First = E("first")
            };
            Assert.IsTrue(uut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new EventHierarchy();
            var b = new EventHierarchy();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new EventHierarchy
            {
                Element = E("a"),
                Super = E("b"),
                First = E("c")
            };
            var b = new EventHierarchy
            {
                Element = E("a"),
                Super = E("b"),
                First = E("c")
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElement()
        {
            var a = new EventHierarchy {Element = E("a")};
            var b = new EventHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSuper()
        {
            var a = new EventHierarchy {Super = E("b")};
            var b = new EventHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFirst()
        {
            var a = new EventHierarchy {First = E("c")};
            var b = new EventHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new EventHierarchy());
        }

        private static IEventName E(string s)
        {
            return Names.Event(string.Format("[T1,P1] [T2,P2].{0}", s));
        }
    }
}