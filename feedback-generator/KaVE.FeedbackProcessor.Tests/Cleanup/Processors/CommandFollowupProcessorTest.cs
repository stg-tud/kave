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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    class CommandFollowupProcessorTest
    {
        private CommandFollowupProcessor _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new CommandFollowupProcessor();
        }

        [Test]
        public void ShouldDropAllEventsWithoutPrecedingCommandEvent()
        {
            var eventList = TestEventFactory.SomeEvents(5);
            eventList.ForEach(ideEvent => CollectionAssert.AreEquivalent(new KaVEHashSet<IDEEvent>(), _uut.Map(ideEvent)));
        }

        [Test]
        public void GeneratesNewConcurrentEventForCommandAndFollowup()
        {
            var expectedCommandEvent = new CommandEvent();
            var expectedFollowupEvent = TestEventFactory.SomeEvent();
            var eventList = new KaVEList<IDEEvent>
            {
                TestEventFactory.SomeEvent(),
                expectedCommandEvent,
                expectedFollowupEvent
            };

            ISet<IDEEvent> resultingSet = new HashSet<IDEEvent>();
            eventList.ForEach(ideEvent => resultingSet = _uut.Map(ideEvent));

            Assert.AreEqual(1, resultingSet.Count);

            var resultingConcurrentEvent = (ConcurrentEvent) resultingSet.First();
            Assert.AreEqual(2, resultingConcurrentEvent.ConcurrentEventList.Count);
            CollectionAssert.Contains(resultingConcurrentEvent.ConcurrentEventList, expectedCommandEvent);
            CollectionAssert.Contains(resultingConcurrentEvent.ConcurrentEventList, expectedFollowupEvent);
        }

        [Test]
        public void OnlyGeneratesOneConcurrentEventForCommandAndFollowup()
        {
            var expectedCommandEvent = new CommandEvent();
            var firstFollowupEvent = TestEventFactory.SomeEvent();
            var secondFollowupEvent = TestEventFactory.SomeEvent();

            var expectedConcurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = new List<IDEEvent>
                {
                    expectedCommandEvent,
                    firstFollowupEvent
                }
            };

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(),_uut.Map(expectedCommandEvent));
            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(expectedConcurrentEvent),_uut.Map(firstFollowupEvent));
            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(),_uut.Map(secondFollowupEvent));
        }
    }
}
