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
    internal abstract class MemberHierarchyTestBase<TMember> where TMember : IMemberName
    {
        protected abstract IMemberHierarchy<TMember> CreateSut();
        protected abstract IMemberHierarchy<TMember> CreateSut(TMember name);

        protected abstract TMember Get(int num = 0);

        [Test]
        public void DefaultValues()
        {
            var sut = CreateSut();
            Assert.AreEqual(Get(), sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void DefaultValues_CustomConstructor()
        {
            var sut = CreateSut(Get(1));
            Assert.AreEqual(Get(1), sut.Element);
            Assert.Null(sut.Super);
            Assert.Null(sut.First);
            Assert.IsFalse(sut.IsDeclaredInParentHierarchy);
        }

        [Test]
        public void SettingValues()
        {
            var sut = CreateSut();
            sut.Element = Get(1);
            sut.Super = Get(2);
            sut.First = Get(3);

            Assert.AreEqual(Get(1), sut.Element);
            Assert.AreEqual(Get(2), sut.Super);
            Assert.AreEqual(Get(3), sut.First);
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
            var a = CreateSut();
            var b = CreateSut();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = CreateSut();
            a.Element = Get(1);
            a.Super = Get(2);
            a.First = Get(3);

            var b = CreateSut();
            b.Element = Get(1);
            b.Super = Get(2);
            b.First = Get(3);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElement()
        {
            var a = CreateSut();
            a.Element = Get(1);

            var b = CreateSut();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSuper()
        {
            var a = CreateSut();
            a.Super = Get(2);

            var b = CreateSut();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFirst()
        {
            var a = CreateSut();
            a.First = Get(3);

            var b = CreateSut();

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