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
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    internal class ToConcurrentEventMapperTest
    {
        private static readonly Random Random = new Random();

        private ToConcurrentEventMapper _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new ToConcurrentEventMapper();
        }

        [Test]
        public void GeneratesConcurrentEventForConcurrentEvents()
        {
            var concurrentEventList =
                GenerateEvents(ConcurrentEventHeuristic.EventTimeDifference.Ticks);

            var expectedEventSet = Sets.NewHashSet(
                new ConcurrentEvent
                {
                    ConcurrentEventList = concurrentEventList.GetRange(0, concurrentEventList.Count - 1),
                    Duration = concurrentEventList.Last().TerminatedAt - concurrentEventList.First().TriggeredAt,
                    TriggeredAt = concurrentEventList.First().TriggeredAt
                }
                );

            IKaVESet<IDEEvent> resultEventSet = new KaVEHashSet<IDEEvent>();
            concurrentEventList.ForEach(ideEvent => resultEventSet = _uut.Map(ideEvent));

            CollectionAssert.AreEquivalent(expectedEventSet, resultEventSet);
        }

        public static List<IDEEvent> GenerateEvents(long eventTimeDifferenceInTicks)
        {
            var eventList = TestEventFactory.SomeEvents(Random.Next(2, 10));
            var lastEventTime = DateTimeFactory.SomeWorkingHoursDateTime();
            eventList.ForEach(
                ideEvent =>
                {
                    lastEventTime = new DateTime(lastEventTime.Ticks + eventTimeDifferenceInTicks);
                    ideEvent.TriggeredAt = lastEventTime;
                });

            var lateEvent = TestEventFactory.SomeEvent();
            lateEvent.TriggeredAt = new DateTime(lastEventTime.Ticks + eventTimeDifferenceInTicks + 1);
            eventList.Add(lateEvent);

            return eventList;
        }
    }
}