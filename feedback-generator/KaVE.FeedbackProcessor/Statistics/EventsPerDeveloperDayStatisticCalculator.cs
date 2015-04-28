﻿/*
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
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class EventsPerDeveloperDayStatisticCalculator : IEventProcessor
    {
        public static readonly IDictionary<Developer, IList<DeveloperDay>> Statistic = new Dictionary<Developer, IList<DeveloperDay>>(); 
        private readonly IDictionary<DateTime, DeveloperDay> _developerDays = new Dictionary<DateTime, DeveloperDay>();

        private Developer _developer;

        public void OnStreamStarts(Developer value)
        {
            _developer = value;
        }

        public void OnEvent(IDEEvent @event)
        {
            Asserts.That(@event.TriggeredAt.HasValue, "event without trigger time cannot be assigned to date");
            var day = @event.TriggeredAt.Value.Date;
            var developerDay = FindOrCreateDeveloperDay(day);
            developerDay.NumberOfEvents++;
        }

        private DeveloperDay FindOrCreateDeveloperDay(DateTime day)
        {
            DeveloperDay developerDay;
            if (!_developerDays.TryGetValue(day, out developerDay))
            {
                developerDay = new DeveloperDay {Date = day};
                _developerDays.Add(day, developerDay);
            }
            return developerDay;
        }

        public IList<DeveloperDay> GetStatistic()
        {
            return _developerDays.Values.ToList();
        }

        public void OnStreamEnds()
        {
            Statistic[_developer] = GetStatistic();
        }

        public class DeveloperDay
        {
            public int NumberOfEvents { get; set; }
            public DateTime Date { get; set; }
        }
    }
}