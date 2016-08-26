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
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.TestRunEvents
{
    internal class TestCaseResultTest
    {
        private readonly DateTime _someDateTime = DateTime.MinValue.AddSeconds(1);

        [Test]
        public void DefaultValues()
        {
            var sut = new TestCaseResult();
            Assert.AreEqual(Names.UnknownMethod, sut.TestMethod);
            Assert.AreEqual("", sut.Parameters);
            Assert.False(sut.StartTime.HasValue);
            Assert.AreEqual(TimeSpan.Zero, sut.Duration);
            Assert.AreEqual(TestResult.Unknown, sut.Result);

            Assert.AreNotEqual(0, sut.GetHashCode());
            Assert.AreNotEqual(1, sut.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var sut = new TestCaseResult
            {
                TestMethod = Names.Method("[T,P] [T,P].M()"),
                Parameters = "p",
                StartTime = _someDateTime,
                Duration = TimeSpan.FromSeconds(3),
                Result = TestResult.Success
            };
            Assert.AreEqual(Names.Method("[T,P] [T,P].M()"), sut.TestMethod);
            Assert.AreEqual("p", sut.Parameters);
            Assert.AreEqual(_someDateTime, sut.StartTime);
            Assert.AreEqual(TimeSpan.FromSeconds(3), sut.Duration);
            Assert.AreEqual(TestResult.Success, sut.Result);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new TestCaseResult();
            var b = new TestCaseResult();

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new TestCaseResult
            {
                TestMethod = Names.Method("[T,P] [T,P].M()"),
                Parameters = "p",
                StartTime = _someDateTime,
                Duration = TimeSpan.FromSeconds(3),
                Result = TestResult.Success
            };
            var b = new TestCaseResult
            {
                TestMethod = Names.Method("[T,P] [T,P].M()"),
                Parameters = "p",
                StartTime = _someDateTime,
                Duration = TimeSpan.FromSeconds(3),
                Result = TestResult.Success
            };

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentMethod()
        {
            var a = new TestCaseResult
            {
                TestMethod = Names.Method("[T,P] [T,P].M()")
            };
            var b = new TestCaseResult();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentParameters()
        {
            var a = new TestCaseResult
            {
                Parameters = "p"
            };
            var b = new TestCaseResult();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentStartTime()
        {
            var a = new TestCaseResult
            {
                StartTime = _someDateTime
            };
            var b = new TestCaseResult();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentDuration()
        {
            var a = new TestCaseResult
            {
                Duration = TimeSpan.FromSeconds(3)
            };
            var b = new TestCaseResult();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentResult()
        {
            var a = new TestCaseResult
            {
                Result = TestResult.Success
            };
            var b = new TestCaseResult();

            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new TestCaseResult());
        }

        [Test]
        public void StableEnum()
        {
            // do not change, as this will affect serialization
            Assert.AreEqual(0, (int) TestResult.Unknown);
            Assert.AreEqual(1, (int) TestResult.Success);
            Assert.AreEqual(2, (int) TestResult.Failed);
            Assert.AreEqual(3, (int) TestResult.Error);
            Assert.AreEqual(4, (int) TestResult.Ignored);
        }
    }
}