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

using System;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Utils
{
    [TestFixture]
    internal class PairTest
    {
        [Test]
        public void ReturnsOtherValue()
        {
            var uut = new Pair<int>(0, 1);

            Assert.AreEqual(1, uut.ReturnOther(0));
            Assert.AreEqual(0, uut.ReturnOther(1));
        }

        [Test]
        public void ReturnsTrueIfValueIsContained()
        {
            var uut = new Pair<int>(0, 1);

            Assert.IsTrue(uut.Contains(0));
            Assert.IsTrue(uut.Contains(1));
        }

        [Test]
        public void ReturnsFalseIfValueIsNotContained()
        {
            var uut = new Pair<int>(0, 1);

            Assert.IsFalse(uut.Contains(2));
        }

        [Test]
        public void EqualsTest()
        {
            const string first = "whatever";
            const string second = "pair";
            var firstPair = new Pair<string>(first, second);
            var secondPair = new Pair<string>(first, second);

            Assert.AreEqual(firstPair, secondPair);
        }

        [Test]
        public void ShouldThrowIfInputValueIsNotInPair()
        {
            Assert.Throws<ArgumentException>(() => new Pair<int>(0, 0).ReturnOther(1));
        }
    }
}