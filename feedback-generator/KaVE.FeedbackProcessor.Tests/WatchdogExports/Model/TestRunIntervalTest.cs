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
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Model
{
    internal class TestRunIntervalTest
    {
        [Test]
        public void Equality_Default()
        {
            var a = new TestRunInterval();
            var b = new TestRunInterval();

            Assert.AreEqual(a, b);
        }

        [Test]
        public void Equality_ProjectName()
        {
            var a = new TestRunInterval {ProjectName = "a"};
            var b = new TestRunInterval {ProjectName = "a"};
            var c = new TestRunInterval {ProjectName = "b"};

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(b, c);
        }

        [Test]
        public void Equality_Result()
        {
            var a = new TestRunInterval {Result = TestResult.Success};
            var b = new TestRunInterval {Result = TestResult.Success};
            var c = new TestRunInterval {Result = TestResult.Failed};

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(b, c);
        }

        [Test]
        public void Equality_TestClasses()
        {
            var a = new TestRunInterval {TestClasses = {new TestRunInterval.TestClassResult {TestClassName = "a"}}};
            var b = new TestRunInterval {TestClasses = {new TestRunInterval.TestClassResult {TestClassName = "a"}}};
            var c = new TestRunInterval {TestClasses = {new TestRunInterval.TestClassResult {TestClassName = "b"}}};

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(b, c);
        }

        [Test]
        public void TestClass_Equality_Default()
        {
            var a = new TestRunInterval.TestClassResult();
            var b = new TestRunInterval.TestClassResult();

            Assert.AreEqual(a, b);
        }

        [Test]
        public void TestClass_Equality_TestMethods()
        {
            var a = new TestRunInterval.TestClassResult
            {
                TestMethods = {new TestRunInterval.TestMethodResult {TestMethodName = "a"}}
            };
            var b = new TestRunInterval.TestClassResult
            {
                TestMethods = {new TestRunInterval.TestMethodResult {TestMethodName = "a"}}
            };
            var c = new TestRunInterval.TestClassResult
            {
                TestMethods = {new TestRunInterval.TestMethodResult {TestMethodName = "c"}}
            };

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(b, c);
        }

        [Test]
        public void TestMethod_Equality_Default()
        {
            var a = new TestRunInterval.TestMethodResult();
            var b = new TestRunInterval.TestMethodResult();

            Assert.AreEqual(a, b);
        }
    }
}