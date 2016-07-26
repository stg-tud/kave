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
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Declarations
{
    internal class DelegateDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new DelegateDeclaration();
            Assert.AreEqual(Names.UnknownDelegateType, sut.Name);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new DelegateDeclaration {Name = SomeDelegateType()};
            Assert.AreEqual(SomeDelegateType(), sut.Name);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new DelegateDeclaration();
            var b = new DelegateDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new DelegateDeclaration {Name = SomeDelegateType()};
            var b = new DelegateDeclaration {Name = SomeDelegateType()};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new DelegateDeclaration {Name = SomeDelegateType()};
            var b = new DelegateDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new DelegateDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new DelegateDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new DelegateDeclaration());
        }

        private static IDelegateTypeName SomeDelegateType()
        {
            return Names.Type("d:[R,P] [SomeDelegateType,P].()").AsDelegateTypeName;
        }
    }
}