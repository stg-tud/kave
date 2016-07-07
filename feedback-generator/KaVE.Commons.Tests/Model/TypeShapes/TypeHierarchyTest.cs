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
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class TypeHierarchyTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new TypeHierarchy();
            Assert.AreEqual(Names.UnknownType, sut.Element);
            Assert.Null(sut.Extends);
            Assert.AreEqual(Sets.NewHashSet<ITypeHierarchy>(), sut.Implements);
            Assert.False(sut.HasSuperclass);
            Assert.False(sut.HasSupertypes);
            Assert.False(sut.IsImplementingInterfaces);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void DefaultValues_CustomConstructor()
        {
            var sut = new TypeHierarchy("T,P");
            Assert.AreEqual(Names.Type("T,P"), sut.Element);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TypeHierarchy
            {
                Element = Names.Type("T1,P1"),
                Extends = SomeHierarchy("x"),
                Implements = {SomeHierarchy("y")}
            };
            Assert.AreEqual(Names.Type("T1,P1"), sut.Element);
            Assert.AreEqual(SomeHierarchy("x"), sut.Extends);
            Assert.AreEqual(Sets.NewHashSet<ITypeHierarchy>(SomeHierarchy("y")), sut.Implements);
            Assert.True(sut.HasSuperclass);
            Assert.True(sut.HasSupertypes);
            Assert.True(sut.IsImplementingInterfaces);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TypeHierarchy();
            var b = new TypeHierarchy();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TypeHierarchy
            {
                Element = Names.Type("T1,P1"),
                Extends = SomeHierarchy("x"),
                Implements = {SomeHierarchy("y")}
            };
            var b = new TypeHierarchy
            {
                Element = Names.Type("T1,P1"),
                Extends = SomeHierarchy("x"),
                Implements = {SomeHierarchy("y")}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentElement()
        {
            var a = new TypeHierarchy {Element = Names.Type("T1,P1")};
            var b = new TypeHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExtends()
        {
            var a = new TypeHierarchy {Extends = SomeHierarchy("a")};
            var b = new TypeHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentImplements()
        {
            var a = new TypeHierarchy {Implements = {SomeHierarchy("i")}};
            var b = new TypeHierarchy();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TypeHierarchy());
        }

        private static TypeHierarchy SomeHierarchy(string simpleType)
        {
            return new TypeHierarchy {Element = Names.Type(simpleType + ",P")};
        }
    }
}