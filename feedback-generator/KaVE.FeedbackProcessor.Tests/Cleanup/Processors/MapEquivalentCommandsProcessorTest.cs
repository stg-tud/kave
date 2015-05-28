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
        private SortedCommandPair _reSharperToVsPair;

        [SetUp]
        public void Setup()
        {
            var copyPair = SortedCommandPair.NewSortedPair(
                "Copy",
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:15:Edit.Copy");
            var cutPair = SortedCommandPair.NewSortedPair("Cut", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:16:Edit.Cut");
            var pastePair = SortedCommandPair.NewSortedPair(
                "Paste",
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste");
            var leftPair = SortedCommandPair.NewSortedPair("Left", "TextControl.Left");
            _saveAllPair = SortedCommandPair.NewSortedPair(
                "Save All",
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:224:File.SaveAll");
            _reSharperToVsPair = SortedCommandPair.NewSortedPair(
                "TextControl.Delete",
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:17:Edit.Delete");

            var testResourceProvider = new TestResourceProvider
            {
                Mappings =
                    new List<SortedCommandPair>
                    {
                        copyPair,
                        cutPair,
                        pastePair,
                        leftPair,
                        _saveAllPair,
                        _reSharperToVsPair
                    }
            };

            _uut = new MapEquivalentCommandsProcessor(testResourceProvider);
        }

        [Test]
        public void ShouldMapLeftSideIfItDoesNotHaveAnUnknownTrigger()
        {
            var inputEvent = new CommandEvent {CommandId = _saveAllPair.Item1, TriggeredBy = IDEEvent.Trigger.Click};

            var actualSet = _uut.Map(inputEvent);

            var expectedCommandEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredBy = IDEEvent.Trigger.Click
            };
            CollectionAssert.AreEquivalent(actualSet, Sets.NewHashSet<IDEEvent>(expectedCommandEvent));
        }

        [Test]
        public void ShouldKeepEventsWithoutMapping()
        {
            var inputEvent = new CommandEvent {CommandId = "Test"};

            var actualSet = _uut.Map(inputEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(inputEvent), actualSet);
        }

        [Test]
        public void ShouldDropRightSideIfNoLeftSideWithUnknownTriggerOccured()
        {
            var inputEvent = new CommandEvent {CommandId = _saveAllPair.Item2};

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
        public void ShouldInsertCachedEventOnStreamEnds()
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var rightSideEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredAt = triggeretAt
            };

            _uut.Map(rightSideEvent);
            var actualSet = _uut.OnStreamEnds();

            CollectionAssert.AreEquivalent(Sets.NewHashSet(rightSideEvent),actualSet);
        }

        [Test]
        public void ShouldDropLeftSideIfItHasAnUnknownTrigger()
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var leftSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item1,
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TriggeredAt = triggeretAt
            };

            var actualSet = _uut.Map(leftSideEvent);

            CollectionAssert.IsEmpty(actualSet);
        }

        [Test]
        public void ShouldKeepRightSideWhenLeftSideWithUnknownTriggerOccured()
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var leftSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item1,
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TriggeredAt = triggeretAt
            };
            var rightSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item2,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };

            _uut.Map(leftSideEvent);
            var actualSet = _uut.Map(rightSideEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(rightSideEvent), actualSet);
        }

        [Test]
        public void ShouldInsertRightSideWhenLeftSideWithUnknownTriggerOccurs()
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var leftSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item1,
                TriggeredBy = IDEEvent.Trigger.Unknown,
                TriggeredAt = triggeretAt
            };
            var rightSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item2,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };

            _uut.Map(rightSideEvent);
            var actualSet = _uut.Map(leftSideEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(rightSideEvent), actualSet);
        }

        [Test]
        public void ShouldNotInsertRightSideWhenLeftSideOccuredAndWasNotTriggeredByUnknown()
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
                TriggeredBy = IDEEvent.Trigger.Click,
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

        [TestCase("Start", "{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start", "Debug.Start"),
         TestCase("Continue", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start",
             "{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget", "Debug.Continue")]
        public void ShouldInsertNewDebugEventWhenTwoDebugEventsFollows(string debugClickId,
            string visualStudioDebugId,
            string secondVisualStudioDebugId,
            string expectedCommandId)
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var event1 = new CommandEvent
            {
                CommandId = debugClickId,
                TriggeredAt = triggeretAt
            };
            var event2 = new CommandEvent
            {
                CommandId = visualStudioDebugId,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var event3 = new CommandEvent
            {
                CommandId = secondVisualStudioDebugId,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var lateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt =
                    triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference.Add(TimeSpan.FromSeconds(1))
            };

            var expectedEvent = new CommandEvent
            {
                CommandId = expectedCommandId,
            };

            expectedEvent.CopyIDEEventPropertiesFrom(event1);

            CollectionAssert.AreEquivalent(Sets.NewHashSet(expectedEvent), _uut.Map(event1));
            CollectionAssert.IsEmpty(_uut.Map(event2));
            CollectionAssert.IsEmpty(_uut.Map(event3));
            CollectionAssert.AreEquivalent(Sets.NewHashSet(lateEvent), _uut.Map(lateEvent));
        }

        [TestCase("Start", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start", "Debug.Start"),
         TestCase("Continue", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start", "Debug.Continue"),
         TestCase("Start", "{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget", "Debug.Start"),
         TestCase("Continue", "{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget", "Debug.Continue")]
        public void ShouldAddNewDebugEventWhenOneDebugEventFollows(string debugClickId,
            string visualStudioDebugId,
            string expectedCommandId)
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var event1 = new CommandEvent
            {
                CommandId = debugClickId,
                TriggeredAt = triggeretAt
            };
            var event2 = new CommandEvent
            {
                CommandId = visualStudioDebugId,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var lateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt =
                    triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference.Add(TimeSpan.FromSeconds(1))
            };

            var expectedEvent = new CommandEvent
            {
                CommandId = expectedCommandId
            };

            expectedEvent.CopyIDEEventPropertiesFrom(event1);

            CollectionAssert.AreEquivalent(Sets.NewHashSet(expectedEvent), _uut.Map(event1));
            CollectionAssert.IsEmpty(_uut.Map(event2));

            CollectionAssert.AreEquivalent(Sets.NewHashSet(lateEvent), _uut.Map(lateEvent));
        }

        [TestCase("Copy", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:15:Edit.Copy", "TextControl.Copy"),
         TestCase("Cut", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:16:Edit.Cut", "TextControl.Cut"),
         TestCase("Paste", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste", "TextControl.Paste")]
        public void ShouldMergeTextControlCommands(string clickCommandId,
            string visualStudioCommandId,
            string resharperCommandId)
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var event1 = new CommandEvent
            {
                CommandId = clickCommandId,
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = triggeretAt
            };
            var event2 = new CommandEvent
            {
                CommandId = visualStudioCommandId,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var event3 = new CommandEvent
            {
                CommandId = resharperCommandId,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var lateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt =
                    triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference.Add(TimeSpan.FromSeconds(1))
            };

            var expectedEvent = new CommandEvent
            {
                CommandId = visualStudioCommandId,
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = event1.TriggeredAt
            };

            CollectionAssert.AreEquivalent(Sets.NewHashSet(expectedEvent), _uut.Map(event1));
            CollectionAssert.IsEmpty(_uut.Map(event2));
            CollectionAssert.IsEmpty(_uut.Map(event3));
            CollectionAssert.AreEquivalent(Sets.NewHashSet(lateEvent), _uut.Map(lateEvent));
        }

        [TestCase("Start Debugging", "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start", "Debug.Start"),
         TestCase("Add", "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Add"),
         TestCase("Exclude", "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Exclude"),
         TestCase("Include", "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Include")]
        public void ShouldInsertNewEventForSpecialMappings(string firstEvent, string secondEvent, string expectedString)
        {
            var triggeretAt = DateTimeFactory.SomeWorkingHoursDateTime();

            var event1 = new CommandEvent
            {
                CommandId = firstEvent,
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = triggeretAt
            };
            var event2 = new CommandEvent
            {
                CommandId = secondEvent,
                TriggeredAt = triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference
            };
            var lateEvent = new CommandEvent
            {
                CommandId = "Test",
                TriggeredAt =
                    triggeretAt + MapEquivalentCommandsProcessor.EventTimeDifference.Add(TimeSpan.FromSeconds(1))
            };

            var expectedEvent = new CommandEvent
            {
                CommandId = expectedString,
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = event1.TriggeredAt
            };

            CollectionAssert.AreEquivalent(Sets.NewHashSet(expectedEvent), _uut.Map(event1));
            CollectionAssert.IsEmpty(_uut.Map(event2));
            CollectionAssert.AreEquivalent(Sets.NewHashSet(lateEvent), _uut.Map(lateEvent));
        }

        public class TestResourceProvider : IResourceProvider
        {
            public List<SortedCommandPair> Mappings;

            public List<SortedCommandPair> GetCommandMappings()
            {
                return Mappings;
            }
        }
    }
}