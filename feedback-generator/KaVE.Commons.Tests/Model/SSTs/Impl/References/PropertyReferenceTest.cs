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
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.References
{
    public class PropertyReferenceTest
    {
        private static IVariableReference SomeRef
        {
            get { return new VariableReference {Identifier = "i"}; }
        }

        private static IPropertyName SomeProperty
        {
            get { return Names.Property("get [T1,P1] [T2,P2].P()"); }
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new PropertyReference();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Names.UnknownProperty, sut.PropertyName);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new PropertyReference
            {
                Reference = SomeRef,
                PropertyName = SomeProperty
            };
            Assert.AreEqual(SomeRef, sut.Reference);
            Assert.AreEqual(SomeProperty, sut.PropertyName);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new PropertyReference();
            var b = new PropertyReference();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new PropertyReference
            {
                Reference = SomeRef,
                PropertyName = SomeProperty
            };
            var b = new PropertyReference
            {
                Reference = SomeRef,
                PropertyName = SomeProperty
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new PropertyReference {Reference = SomeRef};
            var b = new PropertyReference();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new PropertyReference {PropertyName = SomeProperty};
            var b = new PropertyReference();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new PropertyReference();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new PropertyReference();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new PropertyReference());
        }
    }
}