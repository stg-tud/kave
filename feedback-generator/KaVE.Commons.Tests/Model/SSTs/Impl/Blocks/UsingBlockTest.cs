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

using System.Linq;
using KaVE.Commons.Model.SSTs;
using KaVE.Commons.Model.SSTs.Impl.Blocks;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.Impl.Statements;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.SSTs.Impl.Blocks
{
    internal class UsingBlockTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UsingBlock();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Body);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new UsingBlock
            {
                Reference = SomeVarRef("a"),
                Body =
                {
                    new ReturnStatement()
                }
            };
            Assert.AreEqual(SomeVarRef("a"), sut.Reference);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Body);
        }

        [Test]
        public void ChildrenIdentity()
        {
            var sut = new UsingBlock
            {
                Reference = SomeVarRef("a"),
                Body =
                {
                    new ReturnStatement()
                }
            };
            AssertChildren(sut, sut.Reference, sut.Body.First());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new UsingBlock();
            var b = new UsingBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new UsingBlock {Reference = SomeVarRef("b")};
            a.Body.Add(new ReturnStatement());
            var b = new UsingBlock {Reference = SomeVarRef("b")};
            b.Body.Add(new ReturnStatement());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new UsingBlock {Reference = SomeVarRef("a")};
            var b = new UsingBlock {Reference = SomeVarRef("b")};
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new UsingBlock();
            a.Body.Add(new ContinueStatement());
            var b = new UsingBlock();
            b.Body.Add(new GotoStatement());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new UsingBlock();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new UsingBlock();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new UsingBlock());
        }
    }
}