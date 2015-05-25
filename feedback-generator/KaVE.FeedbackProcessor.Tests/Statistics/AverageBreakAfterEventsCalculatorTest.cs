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
 *    - Mattis Manfred Kämmerer
 */

using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class AverageBreakAfterEventsCalculatorTest
    {
        private AverageBreakAfterEventsCalculator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new AverageBreakAfterEventsCalculator();
            AverageBreakAfterEventsCalculator.MinBreakTime = TimeSpan.FromSeconds(1);
            AverageBreakAfterEventsCalculator.MaxEventsBeforeBreak = 2;
            AverageBreakAfterEventsCalculator.AddEventAfterBreakToStatistic = false;
        }

        [Test]
        public void ShouldNotFillStatisticBeforeStreamEnds()
        {
            _uut.OnEvent(IDEEventTestFactory.SomeEvent());

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldFillStatisticAtStreamEnds()
        {
            AverageBreakAfterEventsCalculator.MaxEventsBeforeBreak = 1;
            AverageBreakAfterEventsCalculator.MinBreakTime = TimeSpan.Zero;

            var now = DateTime.Now;

            var listOfEvents = new List<IDEEvent>
            {
                new TestIDEEvent
                {
                    TriggeredAt = now
                },
                new CommandEvent
                {
                    TriggeredAt = now + TimeSpan.FromSeconds(2)
                },
                new TestIDEEvent
                {
                    TriggeredAt = now + TimeSpan.FromSeconds(3)
                },
                new TestIDEEvent
                {
                    TriggeredAt = now + TimeSpan.FromSeconds(4)
                },
                new CommandEvent
                {
                    TriggeredAt = now + TimeSpan.FromSeconds(10)
                },
                new TestIDEEvent
                {
                    TriggeredAt = now + TimeSpan.FromSeconds(15)
                }
            };

            var expectedStatistic = new Dictionary<string, Tuple<TimeSpan, int>>
            {
                {
                    EventMappingUtils.GetAbstractStringOf(
                        new TestIDEEvent()),
                    new Tuple<TimeSpan, int>(TimeSpan.FromSeconds(3), 3)
                },
                {
                    EventMappingUtils.GetAbstractStringOf(
                        new CommandEvent()),
                    new Tuple<TimeSpan, int>(TimeSpan.FromSeconds(3), 2)
                }
            };

            listOfEvents.ForEach(@event => _uut.OnEvent(@event));
            _uut.OnStreamEnds();

            CollectionAssert.AreEquivalent(expectedStatistic, _uut.Statistic);
        }

        [Test]
        public void ShouldUseMultipleEventsWhenMaxEventsBeforeBreakIsGreaterThanOne()
        {
            AverageBreakAfterEventsCalculator.MaxEventsBeforeBreak = 2;

            var triggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var firstEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt
            };
            var secondEvent = new CommandEvent
            {
                TriggeredAt = triggeredAt
            };
            var lateEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt + AverageBreakAfterEventsCalculator.MinBreakTime
            };


            var listOfEvents = new List<IDEEvent>
            {
                firstEvent,
                secondEvent,
                lateEvent
            };

            _uut.OnStreamStarts(new Developer());
            listOfEvents.ForEach(@event => _uut.OnEvent(@event));
            _uut.OnStreamEnds();


            var expectedStatistic = new Dictionary<string, Tuple<TimeSpan, int>>
            {
                {
                    String.Format(
                        "{0}{1}{2}",
                        EventMappingUtils.GetAbstractStringOf(new TestIDEEvent()),
                        AverageBreakAfterEventsCalculator.StatisticStringSeparator,
                        EventMappingUtils.GetAbstractStringOf(new CommandEvent())),
                    new Tuple<TimeSpan, int>(AverageBreakAfterEventsCalculator.MinBreakTime, 1)
                }
            };
            CollectionAssert.AreEquivalent(expectedStatistic, _uut.Statistic);
        }

        [Test]
        public void ShouldNotUseMoreThanMaximumEventsBeforeBreakForStatistic()
        {
            AverageBreakAfterEventsCalculator.MaxEventsBeforeBreak = 1;

            var triggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var firstEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt
            };
            var secondEvent = new CommandEvent
            {
                TriggeredAt = triggeredAt
            };
            var lateEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt + AverageBreakAfterEventsCalculator.MinBreakTime
            };

            var listOfEvents = new List<IDEEvent>
            {
                firstEvent,
                secondEvent,
                lateEvent
            };

            _uut.OnStreamStarts(new Developer());
            listOfEvents.ForEach(@event => _uut.OnEvent(@event));
            _uut.OnStreamEnds();

            var expectedStatistic = new Dictionary<string, Tuple<TimeSpan, int>>
            {
                {
                    EventMappingUtils.GetAbstractStringOf(new CommandEvent()),
                    new Tuple<TimeSpan, int>(AverageBreakAfterEventsCalculator.MinBreakTime, 1)
                }
            };
            CollectionAssert.AreEquivalent(expectedStatistic, _uut.Statistic);
        }

        [Test]
        public void ShouldCombineResultsForMultipleDeveloperStreams()
        {
            AverageBreakAfterEventsCalculator.MaxEventsBeforeBreak = 2;

            var triggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var firstEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt
            };
            var secondEvent = new CommandEvent
            {
                TriggeredAt = triggeredAt
            };
            var lateEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt + AverageBreakAfterEventsCalculator.MinBreakTime
            };
            
            var listOfEvents = new List<IDEEvent>
            {
                firstEvent,
                secondEvent,
                lateEvent
            };

            _uut.OnStreamStarts(new Developer());
            listOfEvents.ForEach(@event => _uut.OnEvent(@event));
            _uut.OnStreamEnds();

            _uut.OnStreamStarts(new Developer());
            listOfEvents.ForEach(@event => _uut.OnEvent(@event));
            _uut.OnStreamEnds();


            var expectedStatistic = new Dictionary<string, Tuple<TimeSpan, int>>
            {
                {
                    String.Format(
                        "{0}{1}{2}",
                        EventMappingUtils.GetAbstractStringOf(new TestIDEEvent()),
                        AverageBreakAfterEventsCalculator.StatisticStringSeparator,
                        EventMappingUtils.GetAbstractStringOf(new CommandEvent())),
                    new Tuple<TimeSpan, int>(AverageBreakAfterEventsCalculator.MinBreakTime, 2)
                }
            };
            CollectionAssert.AreEquivalent(expectedStatistic, _uut.Statistic);
        }

        [Test]
        public void ShouldAddNextEventWhenEnabled()
        {
            AverageBreakAfterEventsCalculator.AddEventAfterBreakToStatistic = true;
            
            var triggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var someEvent = new TestIDEEvent
            {
                TriggeredAt = triggeredAt
            };
            var eventAfterBreak = new CommandEvent
            {
                TriggeredAt = triggeredAt + AverageBreakAfterEventsCalculator.MinBreakTime
            };

            _uut.OnStreamStarts(new Developer());
            _uut.OnEvent(someEvent);
            _uut.OnEvent(eventAfterBreak);
            _uut.OnStreamEnds();

            var expectedStatistic = new Dictionary<string, Tuple<TimeSpan, int>>
            {
                {
                    String.Format(
                        "{0}{1}{2}",
                        EventMappingUtils.GetAbstractStringOf(new TestIDEEvent()),
                        AverageBreakAfterEventsCalculator.EventAfterBreakSeparator,
                        EventMappingUtils.GetAbstractStringOf(new CommandEvent())),
                    new Tuple<TimeSpan, int>(AverageBreakAfterEventsCalculator.MinBreakTime, 1)
                }
            };
            CollectionAssert.AreEquivalent(expectedStatistic, _uut.Statistic);
        }
    }
}