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
 *    - Sven Aman
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class EventsPerDeveloperDayStatisticCalculator : IEventProcessor
    {
        public readonly IDictionary<Developer, IList<DeveloperDay>> Statistic =
            new Dictionary<Developer, IList<DeveloperDay>>();

        private IDictionary<DateTime, DeveloperDay> _currentDeveloperDays;

        private Developer _currentDeveloper;

        public void OnStreamStarts(Developer value)
        {
            _currentDeveloper = value;
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
                developerDay = CreateDeveloperDay(day);
                _currentDeveloperDays.Add(day, developerDay);
            }
            return developerDay;
        }

        protected virtual DeveloperDay CreateDeveloperDay(DateTime day)
        {
            return new DeveloperDay(day);
        }

        public void OnStreamEnds()
        {
            Statistic[_currentDeveloper] =
                _currentDeveloperDays.OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        public class DeveloperDay
        {
            public static readonly TimeSpan CountInactivityAsBreakThreshold = TimeSpan.FromMinutes(10);

            public DateTime Day { get; private set; }
            public DateTime FirstActivityAt { get; private set; }
            public DateTime LastActivityAt { get; private set; }
            public int NumberOfEvents { get; private set; }
            public int NumberOfBreaks { get; private set; }

            public DeveloperDay(DateTime day)
            {
                Day = day;
            }

            public void AddEvent(IDEEvent @event)
            {
                Asserts.That(@event.TriggeredAt.HasValue, "event without trigger time cannot be assigned to day");
                var triggeredAt = @event.TriggeredAt.Value;
                if (IsFirstEventOfDay())
                {
                    FirstActivityAt = triggeredAt;
                }
                else if ((triggeredAt - LastActivityAt) >= CountInactivityAsBreakThreshold)
                {
                    NumberOfBreaks++;
                }
                LastActivityAt = triggeredAt;
                NumberOfEvents++;
            }

            private bool IsFirstEventOfDay()
            {
                return FirstActivityAt == default(DateTime);
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
    }
}