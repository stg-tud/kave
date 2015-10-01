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
    internal class LockBlockTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new LockBlock();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Body);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new LockBlock
            {
                Reference = new VariableReference {Identifier = "x"},
                Body = {new BreakStatement()}
            };
            Assert.AreEqual(new VariableReference {Identifier = "x"}, sut.Reference);
            Assert.AreEqual(Lists.NewList(new BreakStatement()), sut.Body);
        }

        [Test]
        public void ChildrenIdentity()
        {
            var sut = new LockBlock
            {
                Reference = new VariableReference {Identifier = "x"},
                Body = {new BreakStatement()}
            };
            AssertChildren(sut, sut.Reference, sut.Body.First());
        }

        [Test]
        public void Equality_default()
        {
            var a = new LockBlock();
            var b = new LockBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_reallyTheSame()
        {
            var a = new LockBlock
            {
                Reference = new VariableReference {Identifier = "x"},
                Body = {new BreakStatement()}
            };
            var b = new LockBlock
            {
                Reference = new VariableReference {Identifier = "x"},
                Body = {new BreakStatement()}
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentReference()
        {
            var a = new LockBlock
            {
                Reference = new VariableReference {Identifier = "a"}
            };
            var b = new LockBlock
            {
                Reference = new VariableReference {Identifier = "b"}
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_differentBody()
        {
            var a = new LockBlock
            {
                Body = {new BreakStatement()}
            };
            var b = new LockBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new LockBlock();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new LockBlock();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new LockBlock());
        }
    }
}