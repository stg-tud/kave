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
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.DateTimes;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    internal class DeveloperDayTest
    {
        private EventsPerDeveloperDayStatisticCalculator.DeveloperDay _uut;
        private static readonly TimeSpan BreakThreshold = TimeSpan.FromMinutes(5);

        [SetUp]
        public void SetUp()
        {
            _uut = new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(new DateTime(2015, 05, 12), BreakThreshold);
        }

        [Test]
        public void CountsEvents()
        {
            _uut.AddEvent(TestEventFactory.SomeEvent());
            _uut.AddEvent(TestEventFactory.SomeEvent());
            _uut.AddEvent(TestEventFactory.SomeEvent());

            Assert.AreEqual(3, _uut.NumberOfEvents);
        }

        [Test]
        public void TracksFirstActivityOnDay()
        {
            var firstActionTriggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            _uut.AddEvent(TestEventFactory.SomeEvent(firstActionTriggeredAt));
            _uut.AddEvent(TestEventFactory.SomeEvent(firstActionTriggeredAt.AddMinutes(1)));

            Assert.AreEqual(firstActionTriggeredAt, _uut.FirstActivityAt);
        }

        [Test]
        public void TracksLastActivityOnDay()
        {
            var lastActionTriggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            _uut.AddEvent(TestEventFactory.SomeEvent(lastActionTriggeredAt.AddMinutes(-10)));
            _uut.AddEvent(TestEventFactory.SomeEvent(lastActionTriggeredAt));

            Assert.AreEqual(lastActionTriggeredAt, _uut.LastActivityAt);
        }

        [Test]
        public void CountsNoBreaksIfOnlySmallGapsBetweenEvents()
        {
            var someTriggeredAt = new DateTime(2015, 05, 12, 13, 14, 15);

            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt.AddMinutes(1)));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt.AddMinutes(5)));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt.AddMinutes(7)));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt.AddMinutes(10)));

            Assert.AreEqual(0, _uut.NumberOfBreaks);
        }

        [Test]
        public void CountsBreakIfTwoSubsequentEventsAreFarApart()
        {
            var someTriggeredAt = new DateTime(2015, 05, 12, 13, 14, 15);

            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(2)));

            Assert.AreEqual(1, _uut.NumberOfBreaks);
        }

        [Test]
        public void CountsMultipleBreaks()
        {
            var someTriggeredAt = new DateTime(2015, 05, 12, 13, 14, 15);

            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(2)));
            _uut.AddEvent(
                TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(2) + TimeSpan.FromMinutes(1)));
            _uut.AddEvent(
                TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(4) + TimeSpan.FromMinutes(5)));

            Assert.AreEqual(2, _uut.NumberOfBreaks);
        }

        [Test]
        public void TracksBreakDurations()
        {
            var someTriggeredAt = new DateTime(2015, 05, 12, 13, 14, 15);

            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt));
            _uut.AddEvent(TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(2)));
            _uut.AddEvent(
                TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(2) + TimeSpan.FromMinutes(1)));
            _uut.AddEvent(
                TestEventFactory.SomeEvent(someTriggeredAt + BreakThreshold.Times(4) + TimeSpan.FromMinutes(5)));

            var breaks = new[] {TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(14)};

            CollectionAssert.AreEqual(breaks, _uut.Breaks);
        }

        [Test]
        public void ComputesTotalBreakTime_NoBreaks()
        {
            Assert.AreEqual(TimeSpan.FromMinutes(0), _uut.TotalBreakTime);
        }

        [Test]
        public void ComputesTotalBreakTime_OneBreak()
        {
            _uut.Breaks.Add(TimeSpan.FromMinutes(5));

            Assert.AreEqual(TimeSpan.FromMinutes(5), _uut.TotalBreakTime);
        }

        [Test]
        public void ComputesTotalBreakTime_MultipleBreaks()
        {
            _uut.Breaks.Add(TimeSpan.FromMinutes(5));
            _uut.Breaks.Add(TimeSpan.FromMinutes(20));
            _uut.Breaks.Add(TimeSpan.FromMinutes(17));

            Assert.AreEqual(TimeSpan.FromMinutes(42), _uut.TotalBreakTime);
        }
    }
}