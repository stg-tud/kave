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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
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
            var inputEvent = new CommandEvent {CommandId = _saveAllPair.Item1, TriggeredBy = EventTrigger.Click};

            var actualSet = _uut.Map(inputEvent);

            var expectedCommandEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredBy = EventTrigger.Click
            };
            CollectionAssert.AreEquivalent(actualSet, Sets.NewHashSet<IDEEvent>(expectedCommandEvent));
        }

        [Test]
        public void ShouldDropLeftSideIfItHasAnUnknownTrigger()
        {
            var leftSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item1,
                TriggeredBy = EventTrigger.Unknown
            };

            var actualSet = _uut.Map(leftSideEvent);

            CollectionAssert.IsEmpty(actualSet);
        }

        [Test]
        public void ShouldDropRightSideIfItHasAnUnknownTrigger()
        {
            var leftSideEvent = new CommandEvent
            {
                CommandId = _reSharperToVsPair.Item2,
                TriggeredBy = EventTrigger.Unknown
            };

            var actualSet = _uut.Map(leftSideEvent);

            CollectionAssert.IsEmpty(actualSet);
        }

        [Test]
        public void ShouldKeepRightSideIfItDoesNotHaveAnUnknownTrigger()
        {
            var inputEvent = new CommandEvent
            {
                CommandId = _saveAllPair.Item2,
                TriggeredBy = EventTrigger.Shortcut
            };

            var actualSet = _uut.Map(inputEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(inputEvent), actualSet);
        }

        [Test]
        public void ShouldKeepEventsWithoutMapping()
        {
            var inputEvent = new CommandEvent {CommandId = "Test"};

            var actualSet = _uut.Map(inputEvent);

            CollectionAssert.AreEquivalent(Sets.NewHashSet<IDEEvent>(inputEvent), actualSet);
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