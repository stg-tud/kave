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
    internal class SwitchBlockTest : SSTBaseTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new SwitchBlock();
            Assert.AreEqual(new VariableReference(), sut.Reference);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.Sections);
            Assert.AreEqual(Lists.NewList<IStatement>(), sut.DefaultSection);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new SwitchBlock
            {
                Reference = SomeVarRef("a"),
                Sections = {new CaseBlock()},
                DefaultSection =
                {
                    new ReturnStatement()
                }
            };
            Assert.AreEqual(SomeVarRef("a"), sut.Reference);
            Assert.AreEqual(Lists.NewList(new CaseBlock()), sut.Sections);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.DefaultSection);
        }

        [Test]
        public void ChildrenIdentity()
        {
            var sut = new SwitchBlock
            {
                Reference = SomeVarRef("a"),
                Sections = {new CaseBlock()},
                DefaultSection =
                {
                    new ReturnStatement()
                }
            };
            AssertChildren(sut, sut.Reference, sut.DefaultSection.First());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new SwitchBlock();
            var b = new SwitchBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new SwitchBlock
            {
                Reference = SomeVarRef("a"),
                Sections = {new CaseBlock()},
                DefaultSection =
                {
                    new ReturnStatement()
                }
            };
            var b = new SwitchBlock
            {
                Reference = SomeVarRef("a"),
                Sections = {new CaseBlock()},
                DefaultSection =
                {
                    new ReturnStatement()
                }
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentReference()
        {
            var a = new SwitchBlock {Reference = SomeVarRef("a")};
            var b = new SwitchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSections()
        {
            var a = new SwitchBlock();
            a.Sections.Add(new CaseBlock());
            var b = new SwitchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentDefaultSection()
        {
            var a = new SwitchBlock();
            a.DefaultSection.Add(new ReturnStatement());
            var b = new SwitchBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void VisitorIsImplemented()
        {
            var sut = new SwitchBlock();
            sut.Accept(23).Verify(v => v.Visit(sut, 23));
        }

        [Test]
        public void VisitorWithReturnIsImplemented()
        {
            var sut = new SwitchBlock();
            sut.Accept(23).VerifyWithReturn(v => v.Visit(sut, 23));
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new SwitchBlock());
        }
    }
}