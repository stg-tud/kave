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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VS.Statistics.Calculators;
using KaVE.VS.Statistics.Statistics;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Calculators
{
    [TestFixture]
    internal class GlobalCalculatorTest : StatisticCalculatorTestBase<GlobalCalculator>
    {
        public GlobalCalculatorTest() : base(new GlobalEvent()) {}

        protected override bool IsNewStatistic(IStatistic statistic)
        {
            var globalStatistic = statistic as GlobalStatistic;
            if (globalStatistic == null)
            {
                return false;
            }
            return globalStatistic.TotalEvents == 0 &&
                   globalStatistic.TotalNumberOfEdits == 0 &&
                   globalStatistic.CurrentNumberOfEditsBetweenCommits == 0 &&
                   globalStatistic.MaxNumberOfEditsBetweenCommits == 0 &&
                   globalStatistic.TimeInDebugSession == new TimeSpan() &&
                   globalStatistic.TotalWorkTime == new TimeSpan();
        }

        private static readonly IDEEvent[][] TotalEventsTestSources =
        {
            new IDEEvent[] {new GlobalEvent()},
            new IDEEvent[] {new FindEvent(), new IDEStateEvent()},
            new IDEEvent[] {new InfoEvent(), new SolutionEvent(), new ErrorEvent()},
            new IDEEvent[] {new DocumentEvent(), new WindowEvent(), new BuildEvent()},
            new IDEEvent[] {new CommandEvent(), new CompletionEvent(), new DebuggerEvent(), new EditEvent()}
        };

        private static readonly EditEvent[][] EditEvents =
        {
            new[]
            {
                new EditEvent {NumberOfChanges = 0}
            },
            new[]
            {
                new EditEvent {NumberOfChanges = 3},
                new EditEvent {NumberOfChanges = 5}
            },
            new[]
            {
                new EditEvent {NumberOfChanges = 40},
                new EditEvent {NumberOfChanges = 0},
                new EditEvent {NumberOfChanges = 2}
            },
            new[]
            {
                new EditEvent {NumberOfChanges = 1000},
                new EditEvent {NumberOfChanges = 300},
                new EditEvent {NumberOfChanges = 30},
                new EditEvent {NumberOfChanges = 7}
            }
        };

        private static readonly object[] EditsBetweenCommitsTestSources =
        {
            new object[]
            {
                new IDEEvent[]
                {
                    new ErrorEvent(),
                    new FindEvent(),
                    new IDEStateEvent(),
                    new InfoEvent(),
                    new SolutionEvent(),
                    new DocumentEvent(),
                    new WindowEvent(),
                    new BuildEvent(),
                    new CommandEvent(),
                    new CompletionEvent(),
                    new DebuggerEvent(),
                    new EditEvent()
                },
                0
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new EditEvent {NumberOfChanges = 10}
                },
                20
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent()
                },
                10
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Commit"},
                    new EditEvent {NumberOfChanges = 5}
                },
                5
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Commit and Push"},
                    new EditEvent {NumberOfChanges = 5}
                },
                5
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Commit and Sync"},
                    new EditEvent {NumberOfChanges = 5}
                },
                5
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Comm_it"},
                    new EditEvent {NumberOfChanges = 5}
                },
                5
            }
        };

        private static readonly object[] MaxEditsBetweenCommitsTestSources =
        {
            new object[]
            {
                new IDEEvent[]
                {
                    new ErrorEvent(),
                    new FindEvent(),
                    new IDEStateEvent(),
                    new InfoEvent(),
                    new SolutionEvent(),
                    new DocumentEvent(),
                    new WindowEvent(),
                    new BuildEvent(),
                    new CommandEvent(),
                    new CompletionEvent(),
                    new DebuggerEvent(),
                    new EditEvent()
                },
                0
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10}
                },
                0
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 15},
                    new CommandEvent {CommandId = "Commit"}
                },
                15
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Commit"},
                    new EditEvent {NumberOfChanges = 5},
                    new CommandEvent {CommandId = "Commit"}
                },
                10
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 5},
                    new CommandEvent {CommandId = "Commit"},
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Commit"}
                },
                10
            },
            new object[]
            {
                new IDEEvent[]
                {
                    new EditEvent {NumberOfChanges = 5},
                    new CommandEvent {CommandId = "Comm_it"},
                    new EditEvent {NumberOfChanges = 10},
                    new CommandEvent {CommandId = "Comm_it"}
                },
                10
            }
        };

        private static readonly object[] TimeInDebugSessionsTestSources =
        {
            new object[]
            {
                new[]
                {
                    new DebuggerEvent {Reason = "dbgEventReasonLaunchProgram"},
                    new DebuggerEvent {Reason = "dbgEventReasonStopDebugging"}
                },
                new TimeSpan()
            },
            new object[]
            {
                new[]
                {
                    new DebuggerEvent {Reason = "dbgEventReasonGo", TriggeredAt = new DateTime(0)},
                    new DebuggerEvent {Reason = "dbgEventReasonStopDebugging", TriggeredAt = new DateTime(10)}
                },
                new TimeSpan(10)
            },
            new object[]
            {
                new[]
                {
                    new DebuggerEvent {Reason = "dbgEventReasonLaunchProgram", TriggeredAt = new DateTime(0)},
                    new DebuggerEvent {Reason = "dbgEventReasonEndProgram", TriggeredAt = new DateTime(10)}
                },
                new TimeSpan(10)
            },
            new object[]
            {
                new[]
                {
                    new DebuggerEvent {Reason = "dbgEventReasonAttachProgram", TriggeredAt = new DateTime(0)},
                    new DebuggerEvent {Reason = "dbgEventReasonDetachProgram", TriggeredAt = new DateTime(10)}
                },
                new TimeSpan(10)
            },
            new object[]
            {
                new[]
                {
                    new DebuggerEvent {Reason = "dbgEventReasonLaunchProgram", TriggeredAt = new DateTime(0)},
                    new DebuggerEvent {Reason = "dbgEventReasonGo", TriggeredAt = new DateTime(5)},
                    new DebuggerEvent {Reason = "dbgEventReasonEndProgram", TriggeredAt = new DateTime(10)}
                },
                new TimeSpan(10)
            }
        };

        private static readonly object[] TotalWorkTimeTestSources =
        {
            new object[]
            {
                new IDEEvent[]
                {
                    new CommandEvent {TriggeredAt = new DateTime(1, 1, 1, 0, 0, 0)},
                    new EditEvent {TriggeredAt = new DateTime(1, 1, 1, 0, 1, 0)},
                    new DebuggerEvent {TriggeredAt = new DateTime(1, 1, 1, 0, 3, 0)}
                },
                new TimeSpan(0, 1, 0)
            }
        };

        [Test, TestCaseSource("MaxEditsBetweenCommitsTestSources")]
        public void MaxNumberOfEditsBetweenCommitsTest(IDEEvent[] events, int expectedValue)
        {
            Publish(events);

            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            var actualValue = actualStatistic.MaxNumberOfEditsBetweenCommits;
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test, TestCaseSource("EditsBetweenCommitsTestSources")]
        public void NumberOfEditsBetweenCommitsTest(IDEEvent[] events, int expectedValue)
        {
            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);

            Publish(events);

            var actualValue = actualStatistic.CurrentNumberOfEditsBetweenCommits;
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test, TestCaseSource("TimeInDebugSessionsTestSources")]
        public void TimeInDebugSessionTest(IDEEvent[] events, TimeSpan expectedValue)
        {
            Publish(events);

            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            var actualValue = actualStatistic.TimeInDebugSession;
            Assert.AreEqual(actualValue, expectedValue);
        }

        [Test, TestCaseSource("TotalEventsTestSources")]
        public void TotalEventsTest(IDEEvent[] events)
        {
            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            var previousValue = actualStatistic.TotalEvents;
            var expectedValue = previousValue + events.Length;

            Publish(events);

            var actualValue = actualStatistic.TotalEvents;
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test, TestCaseSource("EditEvents")]
        public void TotalNumberOfEditsTest(EditEvent[] editEvents)
        {
            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            var previousValue = actualStatistic.TotalNumberOfEdits;

            var sumOfEdits = editEvents.Sum(ideEvent => ideEvent.NumberOfChanges);
            var expectedValue = previousValue + sumOfEdits;

            Publish(editEvents);

            var actualValue = actualStatistic.TotalNumberOfEdits;
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test, TestCaseSource("TotalWorkTimeTestSources")]
        public void TotalWorkTimeTest(IDEEvent[] events, TimeSpan expectedValue)
        {
            Publish(events);

            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            var actualValue = actualStatistic.TotalWorkTime;
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void EventTimeTest()
        {
            var actualStatistic = (GlobalStatistic) ListingMock.Object.GetStatistic(Sut.StatisticType);
            actualStatistic.EarliestEventTime = new DateTime(2015, 1, 1, 15, 0, 0);
            actualStatistic.LatestEventTime = new DateTime(2015, 1, 1, 9, 0, 0);
            var expectedValue = new DateTime(2015, 1, 1, 12, 0, 0);
            var commandEvent = new CommandEvent
            {
                TriggeredAt = expectedValue
            };
            Publish(commandEvent);

            Assert.AreEqual(expectedValue, actualStatistic.EarliestEventTime);
            Assert.AreEqual(expectedValue, actualStatistic.LatestEventTime);
        }
    }

    internal class GlobalEvent : IDEEvent {}
}