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
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class EventsPerDeveloperDayStatisticCalculatorTest
    {
        private static readonly TimeSpan MinBreakSpan = TimeSpan.FromMinutes(5);

        private EventsPerDeveloperDayStatisticCalculator _uut;

        [SetUp]
        public void CreateCalculator()
        {
            _uut = new EventsPerDeveloperDayStatisticCalculator(MinBreakSpan);
        }

        [Test]
        public void CountsEventsOnSameDay()
        {
            var someDeveloper = TestFactory.SomeDeveloper();
            _uut.OnStreamStarts(someDeveloper);
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 10, 52, 13)});
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 16, 06, 00)});
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 23, 59, 59)});
            _uut.OnStreamEnds();

            var statistic = _uut.Statistic[someDeveloper];
            var developerDay = statistic.First();
            Assert.AreEqual(3, developerDay.NumberOfEvents);
        }

        [Test]
        public void CountsEventsPerDay()
        {
            var someDeveloper = TestFactory.SomeDeveloper();
            _uut.OnStreamStarts(someDeveloper);
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 29, 16, 06, 00)});
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 10, 52, 13)});
            _uut.OnEvent(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 30, 23, 59, 59)});
            _uut.OnStreamEnds();

            var actuals = _uut.Statistic[someDeveloper];
            Assert.AreEqual(new DateTime(2015, 4, 28), actuals[0].Day);
            Assert.AreEqual(new DateTime(2015, 4, 29), actuals[1].Day);
            Assert.AreEqual(new DateTime(2015, 4, 30), actuals[2].Day);
        }

        [Test]
        public void CountsEventsPerDeveloper()
        {
            var developer1 = TestFactory.SomeDeveloper();
            var event1 = TestFactory.SomeEventFor(developer1);
            var developer2 = TestFactory.SomeDeveloper();
            var event2 = TestFactory.SomeEventFor(developer2);

            _uut.OnStreamStarts(developer1);
            _uut.OnEvent(event1);
            _uut.OnStreamEnds();
            _uut.OnStreamStarts(developer2);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            var actual = _uut.Statistic;
            CollectionAssert.IsNotEmpty(actual[developer1]);
            CollectionAssert.IsNotEmpty(actual[developer2]);
        }

        [Test]
        public void ComputesSpreeLength()
        {
            var someDeveloper = TestFactory.SomeDeveloper();

            _uut.OnStreamStarts(someDeveloper);
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 06, 00) });
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 07, 00) });
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 08, 00) });
            _uut.OnStreamEnds();

            Assert.AreEqual(TimeSpan.FromMinutes(2), _uut.Statistic[someDeveloper][0].AverageSpreeTime);
        }

        [Test]
        public void ComputesAverageSpreeLength()
        {
            var someDeveloper = TestFactory.SomeDeveloper();

            _uut.OnStreamStarts(someDeveloper);
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 06, 00) });
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 07, 00) });
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 12, 00) });
            _uut.OnEvent(new TestIDEEvent { TriggeredAt = new DateTime(2015, 4, 1, 16, 15, 00) });
            _uut.OnStreamEnds();

            Assert.AreEqual(TimeSpan.FromMinutes(2), _uut.Statistic[someDeveloper][0].AverageSpreeTime);
        }
    }
}