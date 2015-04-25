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
 *    - Markus Zimmermann
 */

using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    class EquivalentCommandProcessorTest
    {
        private EquivalentCommandProcessor _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new EquivalentCommandProcessor();
        }

        [Test]
        public void DropConcurrentEventsWithoutDuplicateCommandEvents()
        {
            ConcurrentEvent concurrentEvent1 = GenerateConcurrentEventWithTestIDEEvents();
            ConcurrentEvent concurrentEvent2 = GenerateConcurrentEventWithTestIDEEvents();

            CollectionAssert.AreEquivalent(new KaVEHashSet<IDEEvent>(),_uut.Process(concurrentEvent1));
            CollectionAssert.AreEquivalent(new KaVEHashSet<IDEEvent>(),_uut.Process(concurrentEvent2));
        }

        [Test]
        public void PassConcurrentEventsWithEquivalentCommands()
        {
            var eventList = new List<IDEEvent>
            {
                new CommandEvent
                {
                    TriggeredBy = IDEEvent.Trigger.Click,
                    CommandId = "ClickCommand"
                },
                new CommandEvent
                {
                    TriggeredBy = IDEEvent.Trigger.Unknown,
                    CommandId = "CommandBehindClick"
                }
            };
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = eventList
            };

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(concurrentEvent),_uut.Process(concurrentEvent));
        }

        [Test]
        public void DropConcurrentEventWithMoreThanTwoCommandEvents()
        {
            var eventList = new List<IDEEvent>
            {
                new CommandEvent
                {
                    TriggeredBy = IDEEvent.Trigger.Click,
                    CommandId = "ClickCommand"
                },
                new CommandEvent
                {
                    TriggeredBy = IDEEvent.Trigger.Unknown,
                    CommandId = "CommandBehindClick"
                },
                new CommandEvent
                {
                    TriggeredBy = IDEEvent.Trigger.Unknown,
                    CommandId = "AnotherCommand"
                },
            };
            var concurrentEvent = new ConcurrentEvent
            {
                ConcurrentEventList = eventList
            };

            CollectionAssert.AreEquivalent(new KaVEHashSet<IDEEvent>(),_uut.Process(concurrentEvent));
        }

        private ConcurrentEvent GenerateConcurrentEventWithTestIDEEvents()
        {
            var eventList = IDEEventTestFactory.SomeEvents(3);
            return new ConcurrentEvent
            {
                ConcurrentEventList = eventList
            };
        }
    }
}
