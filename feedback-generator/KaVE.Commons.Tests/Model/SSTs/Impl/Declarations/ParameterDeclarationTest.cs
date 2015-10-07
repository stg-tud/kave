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

using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.Declarations;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.TestUtils;
using KaVE.Commons.TestUtils.Model.Names;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Declarations
{
    public class ParameterDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new ParameterDeclaration();
            Assert.AreEqual(new SimpleName(), sut.Name);
            Assert.AreEqual(new TypeReference(), sut.Type);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new ParameterDeclaration
            {
                Name = new SimpleName { Name = "x" },
                Type = new TypeReference()
            };
            Assert.AreEqual(new SimpleName { Name = "x" }, sut.Name);
            Assert.AreEqual(new TypeReference(), sut.Type);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new ParameterDeclaration();
            var b = new ParameterDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new ParameterDeclaration
            {
                Name = new SimpleName { Name = "x" },
                Type = new TypeReference()
            };
            var b = new ParameterDeclaration
            {
                Name = new SimpleName { Name = "x" },
                Type = new TypeReference()
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentName()
        {
            var a = new ParameterDeclaration
            {
                Name = new SimpleName { Name = "x" },
            };
            var b = new ParameterDeclaration();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new ParameterDeclaration
            {
                Type = new TypeReference { TypeName = TestNameFactory.GetAnonymousTypeName()}
            };
            var b = new ParameterDeclaration();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new ParameterDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new ParameterDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new ParameterDeclaration());
        }
    }
}