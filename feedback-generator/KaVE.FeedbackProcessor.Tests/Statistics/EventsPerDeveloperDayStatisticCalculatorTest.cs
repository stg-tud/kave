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
using System.Linq;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using MongoDB.Bson;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class EventsPerDeveloperDayStatisticCalculatorTest
    {
        [Test]
        public void CountsEventsOnSameDay()
        {
            var uut = new EventsPerDeveloperDayStatisticCalculator();

            var someDeveloper = SomeDeveloper();
            uut.Developer = someDeveloper;
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 10, 52, 13)});
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 16, 06, 00)});
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 23, 59, 59)});

            var statistic = uut.GetStatistic();
            var developerDays = statistic[someDeveloper];
            var developerDay = developerDays.First();
            Assert.AreEqual(3, developerDay.NumberOfEvents);
        }

        [Test]
        public void CountsEventsPerDay()
        {
            var uut = new EventsPerDeveloperDayStatisticCalculator();

            var someDeveloper = SomeDeveloper();
            uut.Developer = someDeveloper;
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 10, 52, 13)});
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 29, 16, 06, 00)});
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 30, 23, 59, 59)});

            var statistic = uut.GetStatistic();
            var developerDays = statistic[someDeveloper];
            Assert.AreEqual(3, developerDays.Count);
        }

        [Test]
        public void CountsEventsPerDeveloper()
        {
            var uut = new EventsPerDeveloperDayStatisticCalculator();

            var developer1 = SomeDeveloper();
            uut.Developer = developer1;
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 28, 10, 52, 13)});
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 29, 16, 06, 00)});

            var developer2 = SomeDeveloper();
            uut.Developer = developer2;
            uut.Process(new TestIDEEvent {TriggeredAt = new DateTime(2015, 4, 30, 23, 59, 59)});

            var statistic = uut.GetStatistic();
            Assert.AreEqual(2, statistic.Count);
            var developer1Days = statistic[developer1];
            Assert.AreEqual(2, developer1Days.Count);
            var developer2Days = statistic[developer2];
            Assert.AreEqual(1, developer2Days.Count);
        }

        private static int _developerId = 1;
        public static Developer SomeDeveloper()
        {
            var someDeveloper = new Developer{Id = new ObjectId(string.Format("{0:D24}", _developerId))};
            _developerId++;
            return someDeveloper;
        }
    }
}