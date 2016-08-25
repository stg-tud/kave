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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    internal class TestRunIntervalTransformer : IEventToIntervalTransformer<TestRunInterval>
    {
        private readonly TransformerContext _context;
        private readonly IList<TestRunInterval> _currentIntervals;

        public TestRunIntervalTransformer(TransformerContext context)
        {
            _context = context;
            _currentIntervals = new List<TestRunInterval>();
        }

        public void ProcessEvent(IDEEvent @event)
        {
            var testRunEvent = @event as TestRunEvent;
            if (testRunEvent != null)
            {
                var testsByProject = testRunEvent.Tests.GroupBy(t => t.TestMethod.DeclaringType.Assembly.Name);
                foreach (var testProject in testsByProject)
                {
                    var interval = _context.CreateIntervalFromEvent<TestRunInterval>(@event);
                    interval.ProjectName = testProject.Key;

                    foreach (var testClass in testProject.GroupBy(p => p.TestMethod.DeclaringType.FullName))
                    {
                        var testClassResult = new TestRunInterval.TestClassResult {TestClassName = testClass.Key};

                        foreach (var testMethod in testClass)
                        {
                            // TODO untested
                            if (testMethod.StartTime.HasValue && @event.TriggeredAt.HasValue)
                            {
                                var delta =
                                    Math.Round((@event.TriggeredAt.Value - testMethod.StartTime.Value).TotalHours);

                                var now = DateTime.Now;
                                var detltaTs = now.AddHours(delta) - now;
                                testMethod.StartTime = testMethod.StartTime.Value + detltaTs;
                            }
                            var testMethodResult = new TestRunInterval.TestMethodResult
                            {
                                TestMethodName = testMethod.TestMethod.Name + testMethod.Parameters,
                                // TODO untested
                                StartedAt = testMethod.StartTime,
                                Duration = testMethod.Duration,
                                Result = testMethod.Result
                            };

                            testClassResult.TestMethods.Add(testMethodResult);

                            testClassResult.Duration += testMethod.Duration;

                            testClassResult.Result = UpdateCumulativeTestResult(
                                testClassResult.Result,
                                testMethod.Result);

                            interval.Result = UpdateCumulativeTestResult(
                                testClassResult.Result,
                                testMethod.Result);
                        }

                        interval.TestClasses.Add(testClassResult);

                        interval.Duration += testClassResult.Duration;
                    }

                    _currentIntervals.Add(interval);
                }
            }
        }

        private static TestResult UpdateCumulativeTestResult(TestResult current, TestResult newResult)
        {
            if (newResult == TestResult.Ignored)
            {
                return current;
            }

            return (TestResult) Math.Max((int) current, (int) newResult);
        }

        public IEnumerable<TestRunInterval> SignalEndOfEventStream()
        {
            return _currentIntervals;
        }
    }
}