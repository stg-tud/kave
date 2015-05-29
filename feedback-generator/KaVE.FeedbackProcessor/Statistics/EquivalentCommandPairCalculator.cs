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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class EquivalentCommandPairCalculator : BaseEventProcessor
    {
        public static readonly Dictionary<Pair<string>, int> Statistic = new Dictionary<Pair<string>, int>();

        public static int FrequencyThreshold;

        private CommandEvent _lastCommandEvent;

        public EquivalentCommandPairCalculator(int frequencyThreshold)
        {
            FrequencyThreshold = frequencyThreshold;

            RegisterFor<CommandEvent>(CalculateEquivalentPairs);
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
                AddEquivalentCommandsToStatistic(_lastCommandEvent.CommandId, commandEvent.CommandId);
            }

            _lastCommandEvent = commandEvent;
        }

        public override void OnStreamEnds()
        {
            Statistic.
                Where(keyValuePair => keyValuePair.Value < FrequencyThreshold).ToList().
                ForEach(keyValuePair => Statistic.Remove(keyValuePair.Key));
        }

        private static void AddEquivalentCommandsToStatistic(string command1, string command2)
        {
            var keyPair = SortedCommandPair.NewSortedPair(command1, command2);
            if (Statistic.ContainsKey(keyPair))
            {
                Statistic[keyPair]++;
            }
            else
            {
                Statistic.Add(keyPair, 1);
            }
        }

        public string StatisticAsCsv()
        {
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
    }
}