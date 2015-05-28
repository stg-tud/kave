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
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class CompletionDurationStatistic : BaseEventProcessor
    {
        public readonly IDictionary<DateTime, TimeSpan> Statistic = new Dictionary<DateTime, TimeSpan>();

        public int NumberOfDeveloperDaysWithCompletionUsage { get; private set; }

        private DateTime _today;
        private bool _developerCountedToday;

        public CompletionDurationStatistic()
        {
            RegisterFor<IDEEvent>(OnAnyEvent);
            RegisterFor<CompletionEvent>(OnCompletionEvent);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _today = new DateTime();
            _developerCountedToday = false;
        }

        private void OnAnyEvent(IDEEvent @event)
        {
            EnsureEntryForDay(@event.GetTriggerDate());
        }

        private void EnsureEntryForDay(DateTime date)
        {
            if (!Statistic.ContainsKey(date))
            {
                Statistic[date] = TimeSpan.Zero;
            }
        }

        private void OnCompletionEvent(CompletionEvent @event)
        {
            var date = @event.GetTriggerDate();
            EnsureEntryForDay(date);

            // ReSharper disable once PossibleInvalidOperationException
            Statistic[date] += @event.Duration.Value;

            if (!_developerCountedToday && date != _today)
            {
                _today = date;
                NumberOfDeveloperDaysWithCompletionUsage++;
            }
        }

        public string StatisticAsCsv()
        {
            return Statistic.Select(kvp => new KeyValuePair<string, int>(kvp.Key.ToString("yyyy-MM-dd"), (int) kvp.Value.TotalMilliseconds)).ToCsv();
        }
    }
}