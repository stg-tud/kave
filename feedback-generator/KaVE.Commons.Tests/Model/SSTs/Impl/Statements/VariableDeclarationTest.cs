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
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Statements
{
    internal class VariableDeclarationTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new VariableDeclaration();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Names.UnknownType, sut.Type);
            Assert.True(sut.IsMissing);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new VariableDeclaration
            {
                Reference = SSTUtil.VariableReference("a"),
                Type = Names.Type("T,P")
            };

            Assert.False(sut.IsMissing);
            Assert.AreEqual(SSTUtil.VariableReference("a"), sut.Reference);
            Assert.AreEqual(Names.Type("T,P"), sut.Type);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new VariableDeclaration();
            var b = new VariableDeclaration();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyEquals()
        {
            var a = new VariableDeclaration {Reference = SSTUtil.VariableReference("a"), Type = Names.Type("T1,P1")};
            var b = new VariableDeclaration {Reference = SSTUtil.VariableReference("a"), Type = Names.Type("T1,P1")};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new VariableDeclaration {Reference = SSTUtil.VariableReference("a")};
            var b = new VariableDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new VariableDeclaration {Type = Names.Type("T1,P1")};
            var b = new VariableDeclaration();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new VariableDeclaration();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new VariableDeclaration();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new VariableDeclaration());
        }
    }
}