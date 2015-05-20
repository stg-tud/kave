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
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class MapEquivalentCommandsProcessorTest
    {
        private MapEquivalentCommandsProcessor _uut;
        private SortedCommandPair _saveAllPair;

        [SetUp]
        public void Setup()
        {
            var copyPair = SortedCommandPair.NewSortedPair("Copy", "Text.Copy");
            var leftPair = SortedCommandPair.NewSortedPair("Left", "TextControl.Left");
            _saveAllPair = SortedCommandPair.NewSortedPair(
                "Save All",
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:224:File.SaveAll");

            _uut = new MapEquivalentCommandsProcessor(new List<SortedCommandPair>{copyPair,leftPair,_saveAllPair});
        }

        [Test]
        public void ShouldAlwaysMapLeftSide()
        {
            var inputEvent = new CommandEvent {CommandId = _saveAllPair.Item1};

            var actualSet = _uut.Map(inputEvent);

            var expectedCommandEvent = new CommandEvent {CommandId = _saveAllPair.Item2};
            CollectionAssert.AreEquivalent(actualSet, Sets.NewHashSet<IDEEvent>(expectedCommandEvent));
        }

        [Test]
        public void ShouldKeepEventsWithoutMapping()
        {
            var inputEvent = new CommandEvent { CommandId = "Test" };

            var actualSet = _uut.Map(inputEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(inputEvent), actualSet);
        }

        [Test]
        public void ShouldDropRightSide()
        {
            var inputEvent = new CommandEvent { CommandId = _saveAllPair.Item2 };

            var actualSet = _uut.Map(inputEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(), actualSet);
        }

        [Test]
        public void ShouldInsertRightSideWhenNoLeftSideOccured()
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var rightSideEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredAt = triggeretAt
            };
            var someLateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference + TimeSpan.FromSeconds(1)
            };

            _uut.Map(rightSideEvent);
            var actualSet = _uut.Map(someLateEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(rightSideEvent, someLateEvent), actualSet);
        }

        [Test]
        public void ShouldNotInsertRightSideWhenLeftSideOccured()
        {
            var triggeredAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var rightSideEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredAt = triggeredAt
            };

            triggeredAt = triggeredAt + MapEquivalentCommandsProcessor.EventTimeDifference;
            var leftSideEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item1,
                TriggeredAt = triggeredAt
            };

            triggeredAt = triggeredAt + MapEquivalentCommandsProcessor.EventTimeDifference + TimeSpan.FromSeconds(1);
            var someLateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt = triggeredAt
            };

            _uut.Map(rightSideEvent);
            _uut.Map(leftSideEvent);
            var actualSet = _uut.Map(someLateEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(someLateEvent), actualSet);
        }
    }
}