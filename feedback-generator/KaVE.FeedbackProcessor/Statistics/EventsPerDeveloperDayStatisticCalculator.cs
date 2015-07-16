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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class EventsPerDeveloperDayStatisticCalculator : IEventProcessor
    {
        public readonly IDictionary<Developer, IList<DeveloperDay>> Statistic =
            new Dictionary<Developer, IList<DeveloperDay>>();

        private IDictionary<DateTime, DeveloperDay> _currentDeveloperDays;

        private Developer _currentDeveloper;
        private readonly TimeSpan _minBreakSpan;

        public EventsPerDeveloperDayStatisticCalculator(TimeSpan minBreakSpan)
        {
            _minBreakSpan = minBreakSpan;
        }

        public void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            _currentDeveloperDays = new Dictionary<DateTime, DeveloperDay>();
        }

        public void OnEvent(IDEEvent @event)
        {
            Asserts.That(@event.TriggeredAt.HasValue, "event without trigger time cannot be assigned to date");
            var triggeredAt = @event.TriggeredAt.Value;
            var day = triggeredAt.Date;
            var developerDay = FindOrCreateDeveloperDay(day);
            developerDay.AddEvent(@event);
        }

        private DeveloperDay FindOrCreateDeveloperDay(DateTime day)
        {
            DeveloperDay developerDay;
            if (!_currentDeveloperDays.TryGetValue(day, out developerDay))
            {
                developerDay = new DeveloperDay(day, _minBreakSpan);
                _currentDeveloperDays.Add(day, developerDay);
            }
            return developerDay;
        }

        public void OnStreamEnds()
        {
            foreach (var developerDay in _currentDeveloperDays)
            {
                developerDay.Value.EndDay();
            }
            Statistic[_currentDeveloper] =
                _currentDeveloperDays.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        public class DeveloperDay
        {
            private readonly TimeSpan _minBreakSpan;

            public DateTime Day { get; private set; }
            public DateTime FirstActivityAt { get; private set; }
            public DateTime LastActivityAt { get; private set; }
            public int NumberOfEvents { get; private set; }

            public DateTime _startOfSpree;
            public IList<TimeSpan> _sprees = new List<TimeSpan>();

            public int NumberOfBreaks
            {
                get { return Breaks.Count; }
            }

            public readonly IList<TimeSpan> Breaks = new List<TimeSpan>();

            public DeveloperDay(DateTime day, TimeSpan minBreakSpan)
            {
                _minBreakSpan = minBreakSpan;
                Day = day;
            }

            public void AddEvent(IDEEvent @event)
            {
                Asserts.That(@event.TriggeredAt.HasValue, "event without trigger time cannot be assigned to day");
                var triggeredAt = @event.TriggeredAt.Value;
                if (IsFirstEventOfDay())
                {
                    FirstActivityAt = triggeredAt;
                    LastActivityAt = triggeredAt;
                    _startOfSpree = triggeredAt;
                }
                var timeSinceLastEvent = triggeredAt - LastActivityAt;
                if (timeSinceLastEvent >= _minBreakSpan)
                {
                    Breaks.Add(timeSinceLastEvent);
                    _sprees.Add(LastActivityAt - _startOfSpree);
                    _startOfSpree = triggeredAt;
                }
                LastActivityAt = triggeredAt;
                NumberOfEvents++;
            }

            public void EndDay()
            {
                _sprees.Add(LastActivityAt - _startOfSpree);
            }

            private bool IsFirstEventOfDay()
            {
                return FirstActivityAt == default(DateTime);
            }

            public TimeSpan TotalBreakTime
            {
                get { return Breaks.Count == 0 ? TimeSpan.Zero : Breaks.Aggregate((b1, b2) => b1 + b2); }
            }

            public TimeSpan AverageSpreeTime
            {
                get
                {
                    if (!_sprees.Any())
                    {
                        return TimeSpan.Zero;
                    }
                    var doubleAverageTicks = _sprees.Average(timeSpan => timeSpan.TotalSeconds);
                    var longAverageTicks = Convert.ToInt64(doubleAverageTicks);
                    return TimeSpan.FromSeconds(longAverageTicks);
                }
            }

            public TimeSpan AverageBreakTime
            {
                get
                {
                    if (!Breaks.Any())
                    {
                        return TimeSpan.Zero;
                    }
                    var doubleAverageTicks = Breaks.Average(timeSpan => timeSpan.TotalSeconds);
                    var longAverageTicks = Convert.ToInt64(doubleAverageTicks);
                    return TimeSpan.FromSeconds(longAverageTicks);
                }
            }

            protected bool Equals(DeveloperDay other)
            {
                return NumberOfEvents == other.NumberOfEvents && Day.Equals(other.Day) &&
                       FirstActivityAt.Equals(other.FirstActivityAt) && LastActivityAt.Equals(other.LastActivityAt);
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj, Equals);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = NumberOfEvents;
                    hashCode = (hashCode*397) ^ Day.GetHashCode();
                    hashCode = (hashCode*397) ^ FirstActivityAt.GetHashCode();
                    hashCode = (hashCode*397) ^ LastActivityAt.GetHashCode();
                    return hashCode;
                }
            }

            public override string ToString()
            {
                return this.ToStringReflection();
            }
        }

        public string BreakAndSpreeStatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            foreach (var stat in Statistic)
            {
                csvBuilder.StartRow();
                csvBuilder["Developer"] = stat.Key.Id;

                if (stat.Value.Count > 0)
                {
                    var avgBreakTime = stat.Value.Average(day => day.AverageBreakTime.TotalSeconds);
                    var avgSpreeTime = stat.Value.Average(day => day.AverageSpreeTime.TotalSeconds);

                    csvBuilder["break"] = avgBreakTime;
                    csvBuilder["spree"] = avgSpreeTime;
                }
            }
            return csvBuilder.Build(CsvBuilder.SortFields.ByNameLeaveFirst);
        }

        public string StatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            foreach (var stat in Statistic)
            {
                csvBuilder.StartRow();
                csvBuilder["Developer"] = stat.Key.Id;
                foreach (var day in stat.Value)
                {
                    var dayString = day.Day.ToString("yyyy-MM-dd");
                    csvBuilder[dayString + " 1 Start"] = day.FirstActivityAt.TimeOfDay;
                    csvBuilder[dayString + " 2 End"] = day.LastActivityAt.TimeOfDay;
                    csvBuilder[dayString + " 3 Events"] = day.NumberOfEvents;
                    csvBuilder[dayString + " 4 Breaks"] = day.NumberOfBreaks;
                    csvBuilder[dayString + " 5 Total Break"] = (int) day.TotalBreakTime.TotalSeconds;
                }
                csvBuilder["#Days"] = stat.Value.Count;
                csvBuilder["Total Break"] = stat.Value.Select(day => (int) day.TotalBreakTime.TotalSeconds).Sum();
            }
            return csvBuilder.Build(CsvBuilder.SortFields.ByNameLeaveFirst);
        }

        public string EventsPerDeveloperStatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            foreach (var stat in Statistic)
            {
                csvBuilder.StartRow();
                csvBuilder["Developer"] = stat.Key.Id;
                csvBuilder["Events"] = stat.Value.Sum(day => day.NumberOfEvents);
            }
            return csvBuilder.Build();
        }

        public string EventsPerDeveloperDayStatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            foreach (var stat in Statistic)
            {
                foreach (var developerDay in stat.Value)
                {
                    csvBuilder.StartRow();
                    csvBuilder["DevDay"] = developerDay.Day.ToString("yyyy-MM-dd") + "_" + stat.Key.Id;
                    csvBuilder["Events"] = developerDay.NumberOfEvents;
                }
            }
            return csvBuilder.Build();
        }
    }
}