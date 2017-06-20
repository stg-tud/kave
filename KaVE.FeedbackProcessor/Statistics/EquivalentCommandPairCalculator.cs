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
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class EquivalentCommandPairCalculator : BaseEventProcessor
    {
        public Dictionary<SortedCommandPair, int> Statistic { get; private set; }
        private IDictionary<SortedCommandPair, int> _currentDeveloperStatistic;

        public readonly Dictionary<SortedCommandPair, int> UnknownTriggerMappings =
            new Dictionary<SortedCommandPair, int>();

        public int FrequencyThreshold;

        private CommandEvent _lastCommandEvent;

        public EquivalentCommandPairCalculator(int frequencyThreshold)
        {
            FrequencyThreshold = frequencyThreshold;
            Statistic = new Dictionary<SortedCommandPair, int>();
            RegisterFor<CommandEvent>(CalculateEquivalentPairs);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloperStatistic = new Dictionary<SortedCommandPair, int>();
        }

        public void CalculateEquivalentPairs(CommandEvent commandEvent)
        {
            if (ConcurrentEventHeuristic.IsIgnorableTextControlCommand(commandEvent.CommandId))
            {
                return;
            }

            if (_lastCommandEvent != null &&
                ConcurrentEventHeuristic.AreConcurrent(_lastCommandEvent, commandEvent) &&
                _lastCommandEvent.CommandId != commandEvent.CommandId)
            {
                if (_lastCommandEvent.TriggeredBy != EventTrigger.Unknown ||
                    commandEvent.TriggeredBy != EventTrigger.Unknown)
                {
                    AddEquivalentCommandsToStatistic(
                        _lastCommandEvent.CommandId,
                        commandEvent.CommandId);
                }
                else
                {
                    AddEquivalentCommandsToUnknownTriggerMappings(
                        _lastCommandEvent.CommandId,
                        commandEvent.CommandId);
                }
            }

            _lastCommandEvent = commandEvent;
        }

        public override void OnStreamEnds()
        {
            var cleanStats = MappingCleaner.GetCleanMappings(_currentDeveloperStatistic, FrequencyThreshold);
            cleanStats.ToList().ForEach(newMapping => AddOrUpdate(Statistic, newMapping));
        }

        private static void AddOrUpdate(IDictionary<SortedCommandPair, int> statistic,
            KeyValuePair<SortedCommandPair, int> entry)
        {
            if (statistic.ContainsKey(entry.Key))
            {
                statistic[entry.Key] += entry.Value;
            }
            else
            {
                statistic.Add(entry.Key, entry.Value);
            }
        }

        private void AddEquivalentCommandsToStatistic(string command1, string command2)
        {
            var keyPair = SortedCommandPair.NewSortedPair(command1, command2);
            if (_currentDeveloperStatistic.ContainsKey(keyPair))
            {
                _currentDeveloperStatistic[keyPair]++;
            }
            else
            {
                _currentDeveloperStatistic.Add(keyPair, 1);
            }
        }

        private void AddEquivalentCommandsToUnknownTriggerMappings(string command1, string command2)
        {
            var keyPair = SortedCommandPair.NewSortedPair(command1, command2);
            if (UnknownTriggerMappings.ContainsKey(keyPair))
            {
                UnknownTriggerMappings[keyPair]++;
            }
            else
            {
                UnknownTriggerMappings.Add(keyPair, 1);
            }
        }

        public string StatisticAsCsv()
        {
            RemoveInfrequentMappingsFromStatistic();

            var csvBuilder = new CsvBuilder();
            var statistic = Statistic.OrderByDescending(keyValuePair => keyValuePair.Value);
            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();

                csvBuilder["FirstCommand"] = stat.Key.Item1;
                csvBuilder["SecondCommand"] = stat.Key.Item2;
                csvBuilder["Count"] = stat.Value;
            }
            return csvBuilder.Build();
        }

        private void RemoveInfrequentMappingsFromStatistic()
        {
            Statistic.
                Where(keyValuePair => keyValuePair.Value < FrequencyThreshold).ToList().
                ForEach(keyValuePair => Statistic.Remove(keyValuePair.Key));
        }
    }

    internal static class MappingCleaner
    {
        public static readonly Dictionary<SortedCommandPair, SortedCommandPair> SpecialMappings = new Dictionary
            <SortedCommandPair, SortedCommandPair>
        {
            {
                SortedCommandPair.NewSortedPair(
                    "Continue",
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start"),
                SortedCommandPair.NewSortedPair("Continue", "Debug.Continue")
            },
            {
                SortedCommandPair.NewSortedPair(
                    "Add",
                    "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove"),
                SortedCommandPair.NewSortedPair("Add", "Git.Add")
            },
            {
                SortedCommandPair.NewSortedPair(
                    "Exclude",
                    "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove"),
                SortedCommandPair.NewSortedPair("Exclude", "Git.Exclude")
            },
            {
                SortedCommandPair.NewSortedPair(
                    "Include",
                    "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove"),
                SortedCommandPair.NewSortedPair("Include", "Git.Include")
            }
        };

        public static Dictionary<SortedCommandPair, int> GetCleanMappings(IDictionary<SortedCommandPair, int> mappings,
            int frequencyThreshold)
        {
            var cleanMappings = new Dictionary<SortedCommandPair, int>();

            foreach (var mapping in mappings)
            {
                var specialMappings = GetSpecialMappings(mapping);
                if (specialMappings.Any())
                {
                    foreach (var specialMapping in specialMappings)
                    {
                        cleanMappings.Add(specialMapping.Value, mapping.Value);
                    }
                }
                else
                {
                    cleanMappings.Add(mapping.Key, mapping.Value);
                }
            }


            return cleanMappings;
        }

        private static IList<KeyValuePair<SortedCommandPair, SortedCommandPair>> GetSpecialMappings(
            KeyValuePair<SortedCommandPair, int> mapping)
        {
            return SpecialMappings.Where(specialMapping => mapping.Key.Equals(specialMapping.Key)).ToList();
        }
    }
}