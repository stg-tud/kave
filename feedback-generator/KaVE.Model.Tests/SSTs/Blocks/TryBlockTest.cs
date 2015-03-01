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
 *    - 
 */

using KaVE.Model.Collections;
using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Impl.Statements;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Blocks
{
    internal class TryBlockTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new TryBlock();
            Assert.NotNull(sut.Body);
            Assert.NotNull(sut.CatchBlocks);
            Assert.NotNull(sut.Finally);
            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TryBlock();
            sut.Body.Add(new ContinueStatement());
            sut.CatchBlocks.Add(new CatchBlock());
            sut.Finally.Add(new ReturnStatement());

            Assert.AreEqual(Lists.NewList(new ContinueStatement()), sut.Body);
            Assert.AreEqual(Lists.NewList(new CatchBlock()), sut.CatchBlocks);
            Assert.AreEqual(Lists.NewList(new ReturnStatement()), sut.Finally);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TryBlock();
            var b = new TryBlock();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TryBlock();
            a.Body.Add(new ContinueStatement());
            a.CatchBlocks.Add(new CatchBlock());
            a.Finally.Add(new ReturnStatement());

            var b = new TryBlock();
            b.Body.Add(new ContinueStatement());
            b.CatchBlocks.Add(new CatchBlock());
            b.Finally.Add(new ReturnStatement());

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentBody()
        {
            var a = new TryBlock();
            a.Body.Add(new ContinueStatement());
            var b = new TryBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentCatchs()
        {
            var a = new TryBlock();
            a.CatchBlocks.Add(new CatchBlock());
            var b = new TryBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentFinally()
        {
            var a = new TryBlock();
            a.Finally.Add(new ReturnStatement());
            var b = new TryBlock();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}