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

using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.TestUtils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.TestRunEvents
{
    internal class TestRunEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new TestRunEvent();
            Assert.AreEqual(false, sut.WasAborted);
            Assert.AreEqual(Sets.NewHashSet<TestCaseResult>(), sut.Tests);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TestRunEvent
            {
                WasAborted = true,
                Tests =
                {
                    new TestCaseResult()
                }
            };
            Assert.AreEqual(true, sut.WasAborted);
            Assert.AreEqual(Sets.NewHashSet(new TestCaseResult()), sut.Tests);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TestRunEvent();
            var b = new TestRunEvent();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TestRunEvent
            {
                WasAborted = true,
                Tests =
                {
                    new TestCaseResult()
                }
            };
            var b = new TestRunEvent
            {
                WasAborted = true,
                Tests =
                {
                    new TestCaseResult()
                }
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentWasAborted()
        {
            var a = new TestRunEvent
            {
                WasAborted = true
            };
            var b = new TestRunEvent();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTests()
        {
            var a = new TestRunEvent
            {
                Tests =
                {
                    new TestCaseResult()
                }
            };
            var b = new TestRunEvent();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TestRunEvent());
        }
    }
}