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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.CompletionEvent
{
    internal class ContextTest
    {
        private static ISST SomeSST
        {
            get { return new SST {EnclosingType = Names.Type("T, P")}; }
        }

        private static ITypeShape SomeTypeShape
        {
            get { return new TypeShape {TypeHierarchy = new TypeHierarchy {Element = Names.Type("T2, P2")}}; }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new Context();
            Assert.AreEqual(new TypeShape(), sut.TypeShape);
            Assert.AreEqual(new SST(), sut.SST);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new Context
            {
                TypeShape = SomeTypeShape,
                SST = SomeSST
            };
            Assert.AreEqual(SomeTypeShape, sut.TypeShape);
            Assert.AreEqual(SomeSST, sut.SST);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Context();
            var b = new Context();
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }


        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new Context
            {
                TypeShape = SomeTypeShape,
                SST = SomeSST
            };
            var b = new Context
            {
                TypeShape = SomeTypeShape,
                SST = SomeSST
            };
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeShape()
        {
            var a = new Context {TypeShape = SomeTypeShape};
            var b = new Context();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSST()
        {
            var a = new Context {SST = SomeSST};
            var b = new Context();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ImplementsCustomToString()
        {
            ToStringAssert.Reflection(new Context());
        }
    }
}