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

using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.Model.SSTs;
using KaVE.Model.SSTs.Impl;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events.CompletionEvent
{
    internal class ContextTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new Context();
            Assert.AreEqual(new TypeShape(), sut.TypeShape);
            Assert.AreEqual(new SST(), sut.SST);
        }

        [Test]
        public void ValuesCanBeSet()
        {
            var sut = new Context
            {
                TypeShape = TS("A"),
                SST = new SST {EnclosingType = T("B.C")}
            };
            Assert.AreEqual(TS("A"), sut.TypeShape);
            Assert.AreEqual(new SST {EnclosingType = T("B.C")}, sut.SST);
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
                TypeShape = TS("A"),
                SST = new SST()
            };
            var b = new Context
            {
                TypeShape = TS("A"),
                SST = new SST()
            };
            Assert.AreEqual(a, b);
            Assert.IsTrue(a.GetHashCode() == b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTypeShape()
        {
            var a = new Context {TypeShape = TS("A")};
            var b = new Context();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSST()
        {
            var a = new Context {SST = new SST {EnclosingType = T("C.D")}};
            var b = new Context();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        public static IMethodName M(string methodName)
        {
            return
                MethodName.Get(
                    "[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0]." + methodName +
                    "(opt [System.Int32, mscore, 4.0.0.0] length)");
        }

        private static ITypeName T(string typeName)
        {
            return TypeName.Get(typeName + ", mscore, 4.0.0.0");
        }

        // ReSharper disable once InconsistentNaming
        private static TypeShape TS(string type)
        {
            return new TypeShape {TypeHierarchy = new TypeHierarchy(type)};
        }
    }
}