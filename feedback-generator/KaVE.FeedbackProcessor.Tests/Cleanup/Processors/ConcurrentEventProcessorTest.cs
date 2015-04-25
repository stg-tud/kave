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
 *    - Markus Zimmermann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class ConcurrentEventProcessorTest
    {
        private ConcurrentEventProcessor _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new ConcurrentEventProcessor();
        }

        [Test]
        public void GeneratesConcurrentEventForConcurrentEvents()
        {
            var concurrentEventList = GenerateEvents(ConcurrentEventProcessor.EventTimeDifference.Ticks);

            var expectedEventSet = Sets.NewHashSet(
                new ConcurrentEvent
                {
                    ConcurrentEventList = concurrentEventList.GetRange(0,concurrentEventList.Count-1),
                    Duration = concurrentEventList.Last().TerminatedAt - concurrentEventList.First().TriggeredAt,
                    TriggeredAt = concurrentEventList.First().TriggeredAt
                }
            );

            IKaVESet<IDEEvent> resultEventSet = new KaVEHashSet<IDEEvent>();
            concurrentEventList.ForEach(ideEvent => resultEventSet = _uut.Process(ideEvent));

            CollectionAssert.AreEquivalent(expectedEventSet,resultEventSet);
        }

        [Test]
        public void DropsAllEventsExceptConcurrentEvents()
        {
            var eventList = GenerateEvents(ConcurrentEventProcessor.EventTimeDifference.Ticks + 1);

            eventList.ForEach(ideEvent => CollectionAssert.AreEquivalent(new KaVEHashSet<IDEEvent>(),_uut.Process(ideEvent)));
        }

        [Test, ExpectedException(typeof(AssertException), ExpectedMessage = "Events should have a DateTime value in TriggeredAt")]
        public void ThrowExceptionWhenEventHasInvalidDateTime()
        {
            var eventList = GenerateEvents(0);
            eventList.First().TriggeredAt = null;

            eventList.ForEach(ideEvent => _uut.Process(ideEvent));
        }

        private static List<IDEEvent> GenerateEvents(long eventTimeDifferenceInTicks)
        {
            var eventList = IDEEventTestFactory.SomeEvents(2);
            var lastEventTime = new DateTime(1984, 10, 15, 13, 0, 2);
            eventList.ForEach(
                ideEvent =>
                {
                    lastEventTime = new DateTime(lastEventTime.Ticks + eventTimeDifferenceInTicks);
                    ideEvent.TriggeredAt = lastEventTime;
                });

            var lateEvent = IDEEventTestFactory.SomeEvent();
            lateEvent.TriggeredAt = new DateTime(lastEventTime.Ticks + eventTimeDifferenceInTicks + 1);
            eventList.Add(lateEvent);

            return eventList;
        }
    }
}