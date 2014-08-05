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
using KaVE.CompletionTraceGenerator.Model;
using NUnit.Framework;

namespace KaVE.CompletionTraceGenerator.Tests
{
    [TestFixture]
    class CompletionTraceTest
    {
        private CompletionTrace _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new CompletionTrace();
        }

        [Test]
        public void ShouldInitializeActions()
        {
            Assert.IsNotNull(_sut.Actions);
        }

        [Test]
        public void ShouldInitiallyContainNoActions()
        {
            Assert.AreEqual(0, _sut.Actions.Count);
        }

        [Test]
        public void ShouldAppendNewActions()
        {
            var expected = CreateAnonymousCompletionAction();
            _sut.AppendAction(expected);
            var actual = _sut.Actions[0];
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHaveDefaultDuration()
        {
            Assert.AreEqual(0, _sut.DurationInMillis);
        }

        [Test]
        public void ShouldStoreDuration()
        {
            _sut.DurationInMillis = 132;
            Assert.AreEqual(132, _sut.DurationInMillis);
        }

        [Test]
        public void ShouldEqualEmptyTraceIfEmpty()
        {
            var other = new CompletionTrace();
            Assert.IsTrue(_sut.Equals(other));
        }

        [Test]
        public void ShouldEqualTraceWithSameActions()
        {
            var action = CreateAnonymousCompletionAction();
            _sut.AppendAction(action);
            var other = new CompletionTrace();
            other.AppendAction(action);
            Assert.IsTrue(_sut.Equals(other));
        }

        [Test]
        public void ShouldNotEqualTraceWithDifferentActions()
        {
            var action = CreateAnonymousCompletionAction();
            _sut.AppendAction(action);
            var other = new CompletionTrace();
            Assert.IsFalse(_sut.Equals(other));
        }

        private static CompletionAction CreateAnonymousCompletionAction()
        {
            return CompletionAction.NewApply();
        }
    }
}
