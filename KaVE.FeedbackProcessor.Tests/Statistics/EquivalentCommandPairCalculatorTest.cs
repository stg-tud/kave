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

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.DateTimes;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using KaVE.FeedbackProcessor.Tests.TestUtils;
using KaVE.FeedbackProcessor.Utils;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
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
                TriggeredBy = EventTrigger.Click,
                TriggeredAt = eventTime + ConcurrentEventHeuristic.EventTimeDifference
            };

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(commandEvent1);
            _uut.OnEvent(commandEvent2);
            _uut.OnStreamEnds();

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
                TriggeredBy = EventTrigger.Click,
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
        public void AddsFrequentPairToExport()
        {
            var frequencyThreshold = _uut.FrequencyThreshold;

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.Statistic.Add(SortedCommandPair.NewSortedPair("Copy", "Edit.Copy"), frequencyThreshold);
            _uut.OnStreamEnds();

            StringAssert.Contains(string.Format("Copy,Edit.Copy,{0}", frequencyThreshold), _uut.StatisticAsCsv());
        }

        [Test]
        public void RemovesInfrequentPairToExport()
        {
            var frequencyThreshold = _uut.FrequencyThreshold;

            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.Statistic.Add(SortedCommandPair.NewSortedPair("Left", "Textcontrol.Left"), frequencyThreshold - 1);
            _uut.OnStreamEnds();

            StringAssert.DoesNotContain(
                string.Format("Left,Textcontrol.Left,{0}", frequencyThreshold - 1),
                _uut.StatisticAsCsv());
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
            var triggeredAt = DateTime.Now;
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            MappingCleaner.SpecialMappings.Keys.ToList()
                          .ForEach(
                              mappingToReplace =>
                              {
                                  _uut.OnEvent(
                                      new CommandEvent
                                      {
                                          CommandId = mappingToReplace.Item1,
                                          TriggeredAt = triggeredAt,
                                          TriggeredBy = EventTrigger.Click
                                      });
                                  triggeredAt += ConcurrentEventHeuristic.EventTimeDifference;
                                  _uut.OnEvent(
                                      new CommandEvent {CommandId = mappingToReplace.Item2, TriggeredAt = triggeredAt});
                                  triggeredAt += ConcurrentEventHeuristic.EventTimeDifference.Times(2);
                              });
            _uut.OnStreamEnds();

            var expectedDictionary = new Dictionary<SortedCommandPair, int>();
            MappingCleaner.SpecialMappings.Values.ToList()
                          .ForEach(replacedMapping => expectedDictionary.Add(replacedMapping, 1));
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
    }
}