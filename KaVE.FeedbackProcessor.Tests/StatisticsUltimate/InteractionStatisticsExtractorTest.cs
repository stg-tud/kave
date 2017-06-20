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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.StatisticsUltimate;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.StatisticsUltimate
{
    internal class InteractionStatisticsExtractorTest
    {
        private Random _rng;

        private InteractionStatisticsExtractor _sut;

        [SetUp]
        public void Setup()
        {
            _rng = new Random();
            _sut = new InteractionStatisticsExtractor();
        }

        [Test]
        public void SomeNumbers()
        {
            var es = new List<IDEEvent>
            {
                EventAt(1234, 5, 6),
                EventAt(1234, 5, 6),
                EventAt(1234, 5, 7),
                EventAt(1234, 8, 9)
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(4, actual.NumEvents);
            Assert.AreEqual(3, actual.NumDays);
            Assert.AreEqual(2, actual.NumMonth);
            Assert.AreEqual(GetTime(1234, 5, 6).Date, actual.DayFirst);
            Assert.AreEqual(GetTime(1234, 8, 9).Date, actual.DayLast);
        }

        [Test]
        public void SomeNumbers_Reverse()
        {
            var es = new List<IDEEvent>
            {
                EventAt(1234, 8, 9),
                EventAt(1234, 5, 7),
                EventAt(1234, 5, 6),
                EventAt(1234, 5, 6)
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(4, actual.NumEvents);
            Assert.AreEqual(3, actual.NumDays);
            Assert.AreEqual(2, actual.NumMonth);
            Assert.AreEqual(GetTime(1234, 5, 6).Date, actual.DayFirst);
            Assert.AreEqual(GetTime(1234, 8, 9).Date, actual.DayLast);
        }

        [Test]
        public void DoesNotCrashForAbsentTriggerDates()
        {
            var e = EventAt(1234, 8, 9);
            e.TriggeredAt = null;

            var actual = _sut.CreateStatistics(
                new List<IDEEvent>
                {
                    e
                });
            Assert.AreEqual(1, actual.NumEvents);
            Assert.AreEqual(1, actual.NumDays);
            Assert.AreEqual(1, actual.NumMonth);
            Assert.AreEqual(DateTime.MinValue.Date, actual.DayFirst);
            Assert.AreEqual(DateTime.MinValue.Date, actual.DayLast);
        }

        [Test]
        public void StoresLastEducationAndPosition()
        {
            var es = new List<IDEEvent>
            {
                new UserProfileEvent
                {
                    Education = Educations.Bachelor,
                    Position = Positions.Student
                },
                new UserProfileEvent
                {
                    Education = Educations.Master,
                    Position = Positions.SoftwareEngineer
                }
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(Educations.Master, actual.Education);
            Assert.AreEqual(Positions.SoftwareEngineer, actual.Position);
        }

        [Test]
        public void UnknownEducationAndPositionDoNotOverrideFormerValues()
        {
            var es = new List<IDEEvent>
            {
                new UserProfileEvent
                {
                    Education = Educations.Bachelor,
                    Position = Positions.Student
                },
                new UserProfileEvent
                {
                    Education = Educations.Unknown,
                    Position = Positions.Unknown
                }
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(Educations.Bachelor, actual.Education);
            Assert.AreEqual(Positions.Student, actual.Position);
        }

        [Test]
        public void CountsCompletionEvents()
        {
            var es = new List<IDEEvent>
            {
                new CompletionEvent()
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(1, actual.NumCodeCompletion);
        }

        [Test]
        public void CountsTestRunEvents()
        {
            var es = new List<IDEEvent>
            {
                new TestRunEvent()
            };
            var actual = _sut.CreateStatistics(es);
            Assert.AreEqual(1, actual.NumTestRuns);
        }

        [Test]
        public void ActiveTimeDefault()
        {
            var actual = Analyze();
            Assert.AreEqual(TimeSpan.Zero, actual.ActiveTime);
        }

        [Test]
        public void ActiveTime_IsAdded()
        {
            var actual = Analyze(E(1, 1), E(2, 2));
            Assert.AreEqual(TimeSpan.FromSeconds(3), actual.ActiveTime);
        }

        [Test]
        public void ActiveTime_ShortBreaksAreMerged()
        {
            var actual = Analyze(E(1, 1), E(3, 2));
            Assert.AreEqual(TimeSpan.FromSeconds(4), actual.ActiveTime);
        }

        [Test]
        public void ActiveTime_LargeBreaksAreSeparated()
        {
            var actual = Analyze(E(1, 1), E(31, 2));
            Assert.AreEqual(TimeSpan.FromSeconds(3), actual.ActiveTime);
        }

        [Test]
        public void ActiveTime_Complex()
        {
            // 1234567890123456789012345678901234567890123456789012345678901234567890
            // xxxxxxxxxx
            //  xxx
            //          xx                |
            //                              x                |
            //                                                  xxx
            //                                                      xxxxx
            var actual = Analyze(E(1, 10), E(2, 3), E(10, 2), E(30, 1), E(50, 3), E(54, 5));
            // test is flaky without this implicit rounding
            var diff = Math.Round((actual.ActiveTime - TimeSpan.FromSeconds(21)).TotalSeconds);
            Assert.AreEqual(0, diff);
        }

        private static IIDEEvent E(int startTimeInS, int duration)
        {
            var startTime = DateTime.Now.AddSeconds(startTimeInS);
            return new WindowEvent
            {
                TriggeredAt = startTime,
                Duration = startTime.AddSeconds(duration) - startTime
            };
        }

        private IInteractionStatistics Analyze(params IIDEEvent[] es)
        {
            return _sut.CreateStatistics(es);
        }

        private ActivityEvent EventAt(int year, int month, int day)
        {
            return new ActivityEvent {TriggeredAt = GetTime(year, month, day)};
        }

        private DateTime GetTime(int year, int month, int day)
        {
            var date =
                DateTime.MinValue.AddYears(year - 1)
                        .AddMonths(month - 1)
                        .AddDays(day - 1)
                        .AddSeconds(_rng.Next() % 60);
            return date;
        }
    }
}