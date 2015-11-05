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

using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Model.Names;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.References
{
    public class TypeReferenceTest
    {
        private static readonly ITypeName SomeType = TestNameFactory.GetAnonymousTypeName();

        [Test]
        public void DefaultValues()
        {
            var sut = new TypeReference();
            Assert.AreEqual(TypeName.UnknownName,sut.TypeName);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TypeReference { TypeName = SomeType };
            Assert.AreEqual(SomeType, sut.TypeName);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TypeReference();
            var b = new TypeReference();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TypeReference { TypeName = SomeType };
            var b = new TypeReference { TypeName = SomeType };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new TypeReference { TypeName = SomeType };
            var b = new TypeReference();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new TypeReference();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new TypeReference();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TypeReference());
        }

    }
}