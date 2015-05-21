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
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class AverageBreakAfterEventsCalculator : BaseEventProcessor
    {
        public readonly IDictionary<string, Tuple<TimeSpan, int>> Statistic = new Dictionary<string, Tuple<TimeSpan, int>>();

        private readonly IDictionary<string, List<TimeSpan>> _breaks = new Dictionary<string, List<TimeSpan>>();

        private IDEEvent _lastEvent;

        public AverageBreakAfterEventsCalculator()
        {
            RegisterFor<IDEEvent>(HandleEvent);
        }

        public void HandleEvent(IDEEvent @event)
        {
            if (_lastEvent != null)
            {
                var breakAfterLastEvent = @event.GetTriggeredAt() - _lastEvent.GetTriggeredAt();
                var lastEventString = EventMappingUtils.GetAbstractStringOf(_lastEvent);
                AddToBreaks(lastEventString, breakAfterLastEvent);
            }

            _lastEvent = @event;
        }

        private void AddToBreaks(string lastEventString, TimeSpan breakAfterLastEvent)
        {
            if (_breaks.ContainsKey(lastEventString))
            {
                _breaks[lastEventString].Add(breakAfterLastEvent);
            }
            else
            {
                _breaks.Add(lastEventString, new List<TimeSpan> {breakAfterLastEvent});
            }
        }

        public override void OnStreamEnds()
        {
            foreach (var @break in _breaks)
            {
                var key = @break.Key;
                var averageBreakTime = GetAverageTime(@break.Value);
                var occurences = @break.Value.Count;

                if (Statistic.ContainsKey(key))
                {
                    averageBreakTime = GetAverageTime(new List<TimeSpan> {Statistic[key].Item1, averageBreakTime});
                    var sumOfOccurences = Statistic[key].Item2 + occurences;

                    Statistic[key] = new Tuple<TimeSpan, int>(averageBreakTime, sumOfOccurences);
                }
                else
                {
                    Statistic.Add(key, new Tuple<TimeSpan, int>(averageBreakTime, occurences));
                }
            }
        }

        private static TimeSpan GetAverageTime(IEnumerable<TimeSpan> timeSpans)
        {
            return TimeSpan.FromTicks(Convert.ToInt64(timeSpans.Average(timeSpan => timeSpan.Ticks)));
        }

        public string StatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            var statistic = Statistic.OrderByDescending(keyValuePair => keyValuePair.Value.Item1);
            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();

                csvBuilder["Event"] = stat.Key;
                csvBuilder["AverageTime"] = stat.Value.Item1.ToString("g");
                csvBuilder["Occurences"] = stat.Value.Item2;
            }

            return csvBuilder.Build();
        }
    }
}