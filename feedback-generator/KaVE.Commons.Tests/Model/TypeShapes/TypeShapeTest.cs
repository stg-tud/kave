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

using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.TypeShapes
{
    internal class TypeShapeTest
    {
        private static TypeHierarchy SomeTypeHierarchy
        {
            get { return new TypeHierarchy {Element = TypeName.Get("T,P")}; }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new TypeShape();
            Assert.AreEqual(new TypeHierarchy(), sut.TypeHierarchy);
            Assert.AreEqual(Sets.NewHashSet<IMethodHierarchy>(), sut.MethodHierarchies);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                MethodHierarchies = {new MethodHierarchy()}
            };
            Assert.AreEqual(SomeTypeHierarchy, sut.TypeHierarchy);
            Assert.AreEqual(Sets.NewHashSet(new MethodHierarchy()), sut.MethodHierarchies);
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
            var a = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                MethodHierarchies = {new MethodHierarchy()}
            };
            var b = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy,
                MethodHierarchies = {new MethodHierarchy()}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new TypeShape
            {
                TypeHierarchy = SomeTypeHierarchy
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethods()
        {
            var a = new TypeShape
            {
                MethodHierarchies = { new MethodHierarchy() }
            };
            var b = new TypeShape();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
           ToStringAssert.Reflection(new TypeShape());
        }
    }
}