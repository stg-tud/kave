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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    class MergeEquivalentCommandsTest
    {
        public MergeEquivalentCommands _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new MergeEquivalentCommands();
        }

        [Test]
        public void ShouldAddMappingForTwoConcurrentCommandEvents()
        {
            var eventTime = new DateTime(1984, 1, 1, 1, 1, 1, 10);
            const string expectedString1 = "Edit.Copy";
            const string expectedString2 = "Copy";

            var commandEvent1 = new CommandEvent
            {
                CommandId = expectedString1,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = expectedString2,
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference
            };

            _uut.Process(commandEvent1);
            _uut.Process(commandEvent2);

            CollectionAssert.Contains(
                _uut.Statistic,
                new SortedSet<string>{
                
                    expectedString1, 
                    expectedString2
                 
                });
        }

        [Test]
        public void ShouldNotAddMappingForNonConcurrentCommandEvents()
        {
            var eventTime = new DateTime(1984, 1, 1, 1, 1, 1, 10);

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference + TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };

            _uut.Process(commandEvent1);
            _uut.Process(commandEvent2);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldDropEventsUntilNonConcurrentEvent()
        {
            var eventTime = new DateTime(1984, 1, 1, 1, 1, 1, 10);

            var commandEvent1 = new CommandEvent
            {
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference
            };
            var commandEvent3 = new CommandEvent
            {
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference + TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };
            
            CollectionAssert.AreEqual(Sets.NewHashSet<IDEEvent>(),_uut.Process(commandEvent1));
            CollectionAssert.AreEqual(Sets.NewHashSet<IDEEvent>(),_uut.Process(commandEvent2));

        }

        [Test]
        public void ShouldReplaceNonConcurrentEventWithLastCommandEvent()
        {
            var eventTime = new DateTime(1984, 1, 1, 1, 1, 1, 10);

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference + TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };

            _uut.Process(commandEvent1);
            var actualSet = _uut.Process(commandEvent2);
            
            Assert.AreEqual(Sets.NewHashSet<IDEEvent>(commandEvent1),actualSet);
        }

        [Test]
        public void ShouldMergeTwoConcurrentCommandEvents()
        {
            var eventTime = new DateTime(1984, 1, 1, 1, 1, 1, 10);

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:15:Edit.Copy",
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference
            };
            var commandEvent3 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TriggeredAt = eventTime + MergeEquivalentCommands.EventTimeDifference + TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };

            var mergedEvent = MergeCommandHeuristic.MergeCommandEvents(commandEvent1, commandEvent2);

            _uut.Process(commandEvent1);
            _uut.Process(commandEvent2);

            var resultSet = _uut.Process(commandEvent3);

            CollectionAssert.AreEqual(Sets.NewHashSet<IDEEvent>(mergedEvent),resultSet);
        }
    }
}
