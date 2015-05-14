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
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class CommandMappingsCalculator : IEventProcessor
    {
        public static readonly Dictionary<Pair<string>, int> Statistic = new Dictionary<Pair<string>, int>();

        public static TimeSpan EventTimeDifference = new TimeSpan(0, 0, 0, 0, 10);

        public static int FrequencyThreshold = 10;

        private CommandEvent _lastCommandEvent;

        public void OnStreamStarts(Developer value)
        {
        }

        public void OnEvent(IDEEvent @event)
        {
            var commandEvent = @event as CommandEvent;
            if (commandEvent == null) return;

            if (_lastCommandEvent != null &&
                ConcurrentEventHeuristic.HaveSimiliarEventTime(
                _lastCommandEvent.TriggeredAt,
                commandEvent.TriggeredAt,
                EventTimeDifference))
            {
                AddEquivalentCommandsToStatistic(_lastCommandEvent.CommandId, commandEvent.CommandId);
            }

            _lastCommandEvent = commandEvent;
        }

        public void OnStreamEnds()
        {
            Statistic.
                Where(keyValuePair => keyValuePair.Value < FrequencyThreshold).ToList().
                ForEach(keyValuePair => Statistic.Remove(keyValuePair.Key));
        }

        private void AddEquivalentCommandsToStatistic(string command1, string command2)
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
    }
}