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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class DuplicateCommandFilterProcessorTest
    {
        private DuplicateCommandFilterProcessor _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new DuplicateCommandFilterProcessor();
        }

        [Test]
        public void FiltersDuplicatedCommandEvents()
        {
            var firstEventTime = DateTimeFactory.SomeDateTime();
            var commandEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt = firstEventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt = firstEventTime + ConcurrentEventHeuristic.EventTimeDifference
            };
            var commandEvent3 = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt =
                    firstEventTime + ConcurrentEventHeuristic.EventTimeDifference +
                    ConcurrentEventHeuristic.EventTimeDifference
            };
            var commandEvent4 = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt = firstEventTime + ConcurrentEventHeuristic.EventTimeDifference +
                              ConcurrentEventHeuristic.EventTimeDifference.Add(
                                  new TimeSpan(TimeSpan.TicksPerSecond))
            };

            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(commandEvent), _uut.Map(commandEvent));
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(), _uut.Map(commandEvent2));
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(), _uut.Map(commandEvent3));
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(commandEvent4), _uut.Map(commandEvent4));
        }

        [Test]
        public void ShouldNotFilterAnyOtherCommandEvents()
        {
            var commandEvent = new CommandEvent {CommandId = "Test"};
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(commandEvent), _uut.Map(commandEvent));
        }
    }
}