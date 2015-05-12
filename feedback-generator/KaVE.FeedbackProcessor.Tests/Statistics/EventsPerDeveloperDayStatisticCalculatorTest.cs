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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class EventsPerDeveloperDayStatisticCalculatorTest
    {
        private EventsPerDeveloperDayStatisticCalculator _uut;

        [SetUp]
        public void CreateCalculator()
        {
            _uut = new EventsPerDeveloperDayStatisticCalculator();
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

            var expected = new[]
            {
                new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(new DateTime(2015, 4, 28)),
                new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(new DateTime(2015, 4, 29)),
                new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(new DateTime(2015, 4, 30))
            };
            var actual = _uut.Statistic[someDeveloper];
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void CountsEventsPerDeveloper()
        {
            var developer1 = TestFactory.SomeDeveloper();
            _uut.OnStreamStarts(developer1);
            var event1 = TestFactory.SomeEventFor(developer1);
            _uut.OnEvent(event1);
            _uut.OnStreamEnds();
            var developer2 = TestFactory.SomeDeveloper();
            _uut.OnStreamStarts(developer2);
            var event2 = TestFactory.SomeEventFor(developer2);
            _uut.OnEvent(event2);
            _uut.OnStreamEnds();

            // ReSharper disable PossibleInvalidOperationException
            var expected = new Dictionary<Developer, IList<EventsPerDeveloperDayStatisticCalculator.DeveloperDay>>
            {
                {
                    developer1,
                    new[]
                    {
                        new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(event1.TriggeredAt.Value.Date)
                    }
                },
                {
                    developer2,
                    new[]
                    {
                        new EventsPerDeveloperDayStatisticCalculator.DeveloperDay(event2.TriggeredAt.Value.Date)
                    }
                }
            };
            // ReSharper restore PossibleInvalidOperationException
            var actual = _uut.Statistic;
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}