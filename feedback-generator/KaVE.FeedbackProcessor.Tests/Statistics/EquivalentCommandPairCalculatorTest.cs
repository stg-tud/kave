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
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    [TestFixture]
    internal class EquivalentCommandPairCalculatorTest
    {
        private EquivalentCommandPairCalculator _uut;

        [SetUp]
        public void Setup()
        {
            _uut = new EquivalentCommandPairCalculator(10);
        }

        [Test]
        public void ShouldAddMappingForTwoConcurrentCommandEvents()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            var expectedPair = SortedCommandPair.NewSortedPair(commandEvent1.CommandId, commandEvent2.CommandId);
            CollectionAssert.Contains(
                _uut.Statistic,
                new KeyValuePair<SortedCommandPair, int>(expectedPair, 1));
        }

        [Test]
        public void ShouldNotAddMappingForNonConcurrentCommandEvents()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredAt =
                    eventTime + ConcurrentEventHeuristic.EventTimeDifference +
                    TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldNotRemoveFrequentMappingOnStreamEnd()
        {
            const string command1 = "Copy";
            const string command2 = "Edit.Copy";
            var frequencyThreshold = _uut.FrequencyThreshold;
            var listOfCommandEvents = GenerateCommandPairWithFrequency(
                DateTimeFactory.SomeWorkingHoursDateTime(),
                command1,
                command2,
                frequencyThreshold);

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            listOfCommandEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));

            var expectedPair = SortedCommandPair.NewSortedPair(command1, command2);
            CollectionAssert.Contains(
                _uut.Statistic,
                new KeyValuePair<SortedCommandPair, int>(expectedPair, frequencyThreshold));
        }

        [Test]
        public void ShouldRemoveNonFrequentMappingsOnStreamEnd()
        {
            const string command1 = "Copy";
            const string command2 = "Edit.Copy";
            var frequencyThreshold = _uut.FrequencyThreshold;
            var listOfCommandEvents = GenerateCommandPairWithFrequency(
                DateTimeFactory.SomeWorkingHoursDateTime(),
                command1,
                command2,
                frequencyThreshold - 1);

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            listOfCommandEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));
            _uut.OnStreamEnds();

            var expectedPair = SortedCommandPair.NewSortedPair(command1, command2);
            CollectionAssert.DoesNotContain(
                _uut.Statistic,
                new KeyValuePair<SortedCommandPair, int>(expectedPair, frequencyThreshold - 1));
        }

        [Test]
        public void ShouldKeepAllFrequentAndRemoveAllNonFrequentPairsOnStreamEnd()
        {
            var frequencyThreshold = _uut.FrequencyThreshold;

            var frequentPair1 = SortedCommandPair.NewSortedPair("Copy", "Edit.Copy");
            var frequentPair2 =
                SortedCommandPair.NewSortedPair("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:224:File.SaveAll", "Save All");

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.Statistic.Add(frequentPair1, frequencyThreshold);
            _uut.Statistic.Add(SortedCommandPair.NewSortedPair("Left", "Textcontrol.Left"), frequencyThreshold - 1);
            _uut.Statistic.Add(frequentPair2, frequencyThreshold);
            _uut.Statistic.Add(SortedCommandPair.NewSortedPair("Right", "Textcontrol.Right"), frequencyThreshold - 1);

            _uut.OnStreamEnds();

            CollectionAssert.AreEquivalent(
                _uut.Statistic,
                new Dictionary<SortedCommandPair, int>
                {
                    {frequentPair1, frequencyThreshold},
                    {frequentPair2, frequencyThreshold}
                });
        }

        [Test]
        public void ShouldNotAddMappingForIgnorableTextControlCommands()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var ignorableCommand1 = ConcurrentEventHeuristic.IgnorableTextControlCommands[0];
            var ignorableCommand2 = ConcurrentEventHeuristic.IgnorableTextControlCommands[1];

            var commandEvent1 = new CommandEvent
            {
                CommandId = ignorableCommand1,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = ignorableCommand2,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldNotMapEventsOnThemselves()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();

            const string sameId = "Test";
            var commandEvent1 = new CommandEvent
            {
                CommandId = sameId,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = sameId,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void ShouldReplaceMappingsWithMappingsFromMappingCleanerOnStreamEnds()
        {
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            MappingCleaner.SpecialMappings.Keys.ToList()
                          .ForEach(mappingToReplace => _uut.Statistic.Add(mappingToReplace, _uut.FrequencyThreshold));

            _uut.OnStreamEnds();

            var expectedDictionary = new Dictionary<SortedCommandPair, int>();
            MappingCleaner.SpecialMappings.Values.ToList()
                          .ForEach(replacedMapping => expectedDictionary.Add(replacedMapping, _uut.FrequencyThreshold));
            CollectionAssert.AreEquivalent(expectedDictionary, _uut.Statistic);
        }

        [Test]
        public void ShouldAddUnknownTriggerMappingsToSeparateStatistic()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Test1",
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Test2",
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };
            var commandEvent3 = new CommandEvent
            {
                CommandId = commandEvent1.CommandId,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference +
                                        TimeSpan.FromSeconds(1)
            };
            var commandEvent4 = new CommandEvent
            {
                CommandId = commandEvent2.CommandId,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference +
                                        TimeSpan.FromSeconds(1) +
                                        ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            _uut.OnEvent(commandEvent3);
            _uut.OnEvent(commandEvent4);

            var expectedPair = SortedCommandPair.NewSortedPair(commandEvent1.CommandId, commandEvent2.CommandId);
            CollectionAssert.AreEquivalent(
                new Dictionary<SortedCommandPair, int>
                {
                    {expectedPair, 2}
                },
                _uut.UnknownTriggerMappings);

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        [Test]
        public void CleansStatisticForNewDeveloper()
        {
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.Statistic.Add(SortedCommandPair.NewSortedPair("1", "2"), 3);
            _uut.OnStreamEnds();
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());

            CollectionAssert.IsEmpty(_uut.Statistic);
        }

        private static List<CommandEvent> GenerateCommandPairWithFrequency(DateTime startTime,
            string command1,
            string command2,
            int frequency)
        {
            var returnList = new List<CommandEvent>();
            for (var i = 0; i < frequency; i++)
            {
                returnList.AddRange(GenerateCommandPair(startTime, command1, command2));
                startTime = startTime.Add(ConcurrentEventHeuristic.EventTimeDifference + TimeSpan.FromSeconds(1));
            }
            return returnList;
        }

        private static IEnumerable<CommandEvent> GenerateCommandPair(DateTime startTime,
            string command1,
            string command2)
        {
            var commandEvent1 = new CommandEvent
            {
                CommandId = command1,
                TriggeredBy = IDEEvent.Trigger.Shortcut,
                TriggeredAt = startTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = command2,
                TriggeredAt = startTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            return new List<CommandEvent> {commandEvent1, commandEvent2};
        }
    }
}