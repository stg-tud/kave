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

using KaVE.Model.SSTs.Blocks;
using KaVE.Model.SSTs.Statements;
using NUnit.Framework;

namespace KaVE.Model.Tests.SSTs.Blocks
{
    public class UsingBlockTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UsingBlock();
            Assert.IsNull(sut.Identifier);
        }

        [Test]
        public void SettingValues()
        {
            var sut = new UsingBlock {Identifier = "a"};
            Assert.AreEqual("a", sut.Identifier);
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
            var a = new UsingBlock {Identifier = "b"};
            var b = new UsingBlock {Identifier = "b"};
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentIdentifier()
        {
            var a = new UsingBlock {Identifier = "a"};
            var b = new UsingBlock {Identifier = "b"};
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
    }
}