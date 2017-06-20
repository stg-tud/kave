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
using System.Linq;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    [Ignore]
    internal class TestRunTransformerTest : TransformerTestBase<TestRunInterval>
    {
        private static readonly IMethodName TestMethod1 =
            Names.Method("[System.Void, mscore, 4.0.0.0] [Test.TestClass, TestProj1, 1.0.0.0].Test()");

        private static readonly IMethodName TestMethod2 =
            Names.Method("[System.Void, mscore, 4.0.0.0] [Test.TestClass, TestProj1, 1.0.0.0].Test2()");

        private static readonly IMethodName TestMethodInOtherProject =
            Names.Method("[System.Void, mscore, 4.0.0.0] [Test.TestClass, TestProj2, 1.0.0.0].Test()");

        [Test]
        public void TransformsTestResultCorrectly()
        {
            var testEvent = new TestRunEvent
            {
                Tests = {new TestCaseResult {TestMethod = TestMethod1}}
            };

            var sut = new TestRunIntervalTransformer(_context);
            sut.ProcessEvent(testEvent);

            var actual = sut.SignalEndOfEventStream().First();
            Assert.AreEqual("TestProj1", actual.ProjectName);
            Assert.AreEqual(1, actual.TestClasses.Count);
            Assert.AreEqual("Test.TestClass", actual.TestClasses[0].TestClassName);
            Assert.AreEqual(1, actual.TestClasses[0].TestMethods.Count);
            Assert.AreEqual("Test", actual.TestClasses[0].TestMethods[0].TestMethodName);
        }

        [Test]
        public void SplitsUpEventsWithMultipleProjects()
        {
            var testEvent = new TestRunEvent
            {
                Tests =
                {
                    new TestCaseResult {TestMethod = TestMethod1},
                    new TestCaseResult {TestMethod = TestMethodInOtherProject}
                }
            };

            var sut = new TestRunIntervalTransformer(_context);
            sut.ProcessEvent(testEvent);

            var actual = sut.SignalEndOfEventStream();
            Assert.AreEqual(2, actual.Count());
        }

        [Test]
        public void ResultsAndDurationsAreSetAndPropagatedCorrectly()
        {
            var testEvent = new TestRunEvent
            {
                Tests =
                {
                    new TestCaseResult
                    {
                        TestMethod = TestMethod1,
                        Result = TestResult.Success,
                        Duration = TimeSpan.FromSeconds(0.1)
                    },
                    new TestCaseResult
                    {
                        TestMethod = TestMethod2,
                        Result = TestResult.Failed,
                        Duration = TimeSpan.FromSeconds(0.2)
                    }
                }
            };

            var sut = new TestRunIntervalTransformer(_context);
            sut.ProcessEvent(testEvent);

            var actual = sut.SignalEndOfEventStream().First();

            Assert.AreEqual("TestProj1", actual.ProjectName);
            Assert.AreEqual(1, actual.TestClasses.Count);
            Assert.AreEqual(TimeSpan.FromSeconds(0.3), actual.Duration);
            Assert.AreEqual(TestResult.Failed, actual.Result);

            Assert.AreEqual("Test.TestClass", actual.TestClasses[0].TestClassName);
            Assert.AreEqual(TimeSpan.FromSeconds(0.3), actual.TestClasses[0].Duration);
            Assert.AreEqual(2, actual.TestClasses[0].TestMethods.Count);
            Assert.AreEqual(TestResult.Failed, actual.TestClasses[0].Result);

            foreach (var method in actual.TestClasses[0].TestMethods)
            {
                if (method.TestMethodName == "Test")
                {
                    Assert.AreEqual(TestResult.Success, method.Result);
                    Assert.AreEqual(TimeSpan.FromSeconds(0.1), method.Duration);
                }
                else if (method.TestMethodName == "Test2")
                {
                    Assert.AreEqual(TestResult.Failed, method.Result);
                    Assert.AreEqual(TimeSpan.FromSeconds(0.2), method.Duration);
                }
                else
                {
                    Assert.Fail();
                }
            }
        }
    }
}