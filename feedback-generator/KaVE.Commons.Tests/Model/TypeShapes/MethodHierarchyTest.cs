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
 *    - Sven Amann
 */

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils.Model.Names;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class MethodHierarchyTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new MethodHierarchy();
            Assert.AreEqual(MethodName.UnknownName, sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void DefaultValues_CustomConstructor()
        {
            var sut = new MethodHierarchy(M("x"));
            Assert.AreEqual(M("x"), sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new MethodHierarchy
            {
                Element = M("a"),
                Super = M("b"),
                First = M("c")
            };
            Assert.AreEqual(M("a"), sut.Element);
            Assert.AreEqual(M("b"), sut.Super);
            Assert.AreEqual(M("c"), sut.First);
        }

        [Test]
        public void ShouldBeOverrideOrImplementationWhenFirstIsSet()
        {
            var uut = new MethodHierarchy
            {
                First = TestNameFactory.GetAnonymousMethodName()
            };
            Assert.IsTrue(uut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new MethodHierarchy();
            var b = new MethodHierarchy();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new MethodHierarchy
            {
                Element = M("a"),
                Super = M("b"),
                First = M("c")
            };
            var b = new MethodHierarchy
            {
                Element = M("a"),
                Super = M("b"),
                First = M("c")
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElement()
        {
            var a = new MethodHierarchy {Element = M("a")};
            var b = new MethodHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSuper()
        {
            var a = new MethodHierarchy {Super = M("b")};
            var b = new MethodHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFirst()
        {
            var a = new MethodHierarchy {First = M("c")};
            var b = new MethodHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        private static IMethodName M(string s)
        {
            return MethodName.Get(string.Format("[T1,P1] [T2,P2].{0}()", s));
        }
    }
}