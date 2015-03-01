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

using KaVE.Model.Collections;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Impl.Blocks;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Blocks
{
    internal class SwitchBlockTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new SwitchBlock();
            Assert.Null(sut.Identifier);
            Assert.NotNull(sut.Sections);
            Assert.NotNull(sut.DefaultSection);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new SwitchBlock {Identifier = "a"};
            sut.Sections.Add(new CaseBlock());
            sut.DefaultSection.Add(new ReturnStatement());
            Assert.AreEqual("a", sut.Identifier);
            Assert.AreEqual(Lists.NewList(new CaseBlock()), sut.Sections);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.DefaultSection);
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
            var a = new SwitchBlock {Identifier = "a"};
            a.Sections.Add(new CaseBlock());
            a.DefaultSection.Add(new ReturnStatement());
            var b = new SwitchBlock {Identifier = "a"};
            b.Sections.Add(new CaseBlock());
            b.DefaultSection.Add(new ReturnStatement());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new SwitchBlock {Identifier = "a"};
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
    }
}