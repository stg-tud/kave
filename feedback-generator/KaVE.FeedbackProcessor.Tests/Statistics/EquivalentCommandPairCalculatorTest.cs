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
            _uut = new EquivalentCommandPairCalculator();
            EquivalentCommandPairCalculator.Statistic.Clear();
        }

        [Test]
        public void ShouldAddMappingForTwoConcurrentCommandEvents()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();
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
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            var expectedPair = SortedCommandPair.NewSortedPair(expectedString1, expectedString2);
            CollectionAssert.Contains(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(expectedPair, 1));
        }

        [Test]
        public void ShouldNotAddMappingForNonConcurrentCommandEvents()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var commandEvent1 = new CommandEvent
            {
                CommandId = "Copy",
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = "Edit.Copy",
                TriggeredAt =
                    eventTime + ConcurrentEventHeuristic.EventTimeDifference +
                    TimeSpan.FromTicks(TimeSpan.TicksPerSecond)
            };

            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            CollectionAssert.IsEmpty(EquivalentCommandPairCalculator.Statistic);
        }

        [Test]
        public void ShouldNotRemoveFrequentMappingOnStreamEnd()
        {
            const string command1 = "Copy";
            const string command2 = "Edit.Copy";
            var frequencyThreshold = EquivalentCommandPairCalculator.FrequencyThreshold;
            var listOfCommandEvents = GenerateCommandPairWithFrequency(
                DateTimeFactory.SomeWorkingHoursDateTime(),
                command1,
                command2,
                frequencyThreshold);

            listOfCommandEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));

            var expectedPair = SortedCommandPair.NewSortedPair(command1, command2);
            CollectionAssert.Contains(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(expectedPair, frequencyThreshold));
        }

        [Test]
        public void ShouldRemoveNonFrequentMappingsOnStreamEnd()
        {
            const string command1 = "Copy";
            const string command2 = "Edit.Copy";
            var frequencyThreshold = EquivalentCommandPairCalculator.FrequencyThreshold;
            var listOfCommandEvents = GenerateCommandPairWithFrequency(
                DateTimeFactory.SomeWorkingHoursDateTime(),
                command1,
                command2,
                frequencyThreshold - 1);

            listOfCommandEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));
            _uut.OnStreamEnds();

            var expectedPair = SortedCommandPair.NewSortedPair(command1, command2);
            CollectionAssert.DoesNotContain(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(expectedPair, frequencyThreshold - 1));
        }

        [Test]
        public void ShouldKeepAllFrequentAndRemoveAllNonFrequentPairsOnStreamEnd()
        {
            var someDateTime = DateTimeFactory.SomeWorkingHoursDateTime();

            var frequencyThreshold = EquivalentCommandPairCalculator.FrequencyThreshold;
            var frequentEvents = GenerateCommandPairWithFrequency(
                someDateTime,
                "Copy",
                "Edit.Copy",
                frequencyThreshold);

            var nonFrequentEvents = GenerateCommandPairWithFrequency(
                someDateTime.AddYears(1),
                "Left",
                "Textcontrol.Left",
                frequencyThreshold - 1);

            var frequentEvents2 = GenerateCommandPairWithFrequency(
                someDateTime.AddYears(2),
                "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:224:File.SaveAll",
                "Save All",
                frequencyThreshold);

            var nonFrequentEvents2 = GenerateCommandPairWithFrequency(
                someDateTime.AddYears(3),
                "Right",
                "Textcontrol.Right",
                frequencyThreshold - 1);

            frequentEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));
            nonFrequentEvents.ForEach(commandEvent => _uut.OnEvent(commandEvent));
            frequentEvents2.ForEach(commandEvent => _uut.OnEvent(commandEvent));
            nonFrequentEvents2.ForEach(commandEvent => _uut.OnEvent(commandEvent));

            _uut.OnStreamEnds();

            var frequentPair1 = SortedCommandPair.NewSortedPair("Copy", "Edit.Copy");
            CollectionAssert.Contains(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(frequentPair1, frequencyThreshold));

            var nonFrequentPair = SortedCommandPair.NewSortedPair("Left", "Textcontrol.Left");
            CollectionAssert.DoesNotContain(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(nonFrequentPair, frequencyThreshold - 1));

            var frequentPair2 =
                SortedCommandPair.NewSortedPair("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:224:File.SaveAll", "Save All");
            CollectionAssert.Contains(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(frequentPair2, frequencyThreshold));

            var nonFrequentPair2 = SortedCommandPair.NewSortedPair("Right", "Textcontrol.Right");
            CollectionAssert.DoesNotContain(
                EquivalentCommandPairCalculator.Statistic,
                new KeyValuePair<Pair<string>, int>(nonFrequentPair2, frequencyThreshold - 1));
        }

        private List<CommandEvent> GenerateCommandPairWithFrequency(DateTime startTime,
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

        private IEnumerable<CommandEvent> GenerateCommandPair(DateTime startTime, string command1, string command2)
        {
            var commandEvent1 = new CommandEvent
            {
                CommandId = command1,
                TriggeredAt = startTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = command2,
                TriggeredAt = startTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            return new List<CommandEvent> {commandEvent1, commandEvent2};
        }

        [Test]
        public void ShouldNotAddMappingForIgnorableTextControlCommands()
        {
            var eventTime = DateTimeFactory.SomeWorkingHoursDateTime();
            var expectedString1 = ConcurrentEventHeuristic.IgnorableTextControlCommands[0];
            var expectedString2 = ConcurrentEventHeuristic.IgnorableTextControlCommands[1];

            var commandEvent1 = new CommandEvent
            {
                CommandId = expectedString1,
                TriggeredAt = eventTime
            };
            var commandEvent2 = new CommandEvent
            {
                CommandId = expectedString2,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);

            CollectionAssert.IsEmpty(EquivalentCommandPairCalculator.Statistic);
        }
    }
}