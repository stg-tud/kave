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
using System.Text;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class AverageBreakAfterEventsCalculator : BaseEventProcessor
    {
        public const char StatisticStringSeparator = ';';
        public const string EventAfterBreakSeparator = " || Next Event: ";

        public int MaxEventsBeforeBreak;

        public TimeSpan MinBreakTime;

        public bool AddEventAfterBreakToStatistic;

        public readonly IDictionary<string, Tuple<TimeSpan, int>> Statistic =
            new Dictionary<string, Tuple<TimeSpan, int>>();

        private readonly IDictionary<string, List<TimeSpan>> _breaks = new Dictionary<string, List<TimeSpan>>();

        private readonly FixedSizeQueue<IDEEvent> _eventCache = new FixedSizeQueue<IDEEvent>(0);

        public AverageBreakAfterEventsCalculator(int maxEventsBeforeBreak,
            TimeSpan minBreakTime,
            bool addEventAfterBreakToStatistic = true)
        {
            MaxEventsBeforeBreak = maxEventsBeforeBreak;
            MinBreakTime = minBreakTime;
            AddEventAfterBreakToStatistic = addEventAfterBreakToStatistic;

            RegisterFor<IDEEvent>(HandleEvent);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _breaks.Clear();

            _eventCache.Limit = MaxEventsBeforeBreak;
            _eventCache.Clear();

            base.OnStreamStarts(developer);
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

        public void HandleEvent(IDEEvent @event)
        {
            var windowEvent = @event as WindowEvent;
            if (windowEvent != null && windowEvent.Action.Equals(WindowAction.Move))
            {
                return;
            }

            if (_eventCache.Count > 0 && BreakOccured(@event))
            {
                AddCacheToBreaks(@event);
                _eventCache.Clear();
            }

            _eventCache.Enqueue(@event);
        }

        private bool BreakOccured(IDEEvent nextEvent)
        {
            return (nextEvent.GetTriggeredAt() - _eventCache.Last().GetTriggeredAt()) >= MinBreakTime;
        }

        private void AddCacheToBreaks(IDEEvent lateEvent)
        {
            var breakAfterLastEvent = lateEvent.GetTriggeredAt() - _eventCache.Last().GetTriggeredAt();

            var lastEventsString = GenerateLastEventsString(lateEvent);

            if (_breaks.ContainsKey(lastEventsString))
            {
                _breaks[lastEventsString].Add(breakAfterLastEvent);
            }
            else
            {
                _breaks.Add(lastEventsString, new List<TimeSpan> {breakAfterLastEvent});
            }
        }

        private string GenerateLastEventsString(IDEEvent lateEvent)
        {
            return AddEventAfterBreakToStatistic
                ? String.Format(
                    "{0}{1}{2}",
                    GenerateStringFromCache(),
                    EventAfterBreakSeparator,
                    EventMappingUtils.GetAbstractStringOf(lateEvent))
                : GenerateStringFromCache();
        }

        private string GenerateStringFromCache()
        {
            var cacheStringBuilder = new StringBuilder();

            _eventCache.ToList().ForEach(
                ideEvent =>
                    cacheStringBuilder.Append(
                        EventMappingUtils.GetAbstractStringOf(ideEvent) + StatisticStringSeparator));

            return cacheStringBuilder.ToString().TrimEnd(StatisticStringSeparator);
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