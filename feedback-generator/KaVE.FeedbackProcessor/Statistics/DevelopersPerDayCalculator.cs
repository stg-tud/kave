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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class DevelopersPerDayCalculator : BaseEventProcessor
    {
        private readonly Multiset<DateTime> _developerDays = new Multiset<DateTime>();
        private readonly IKaVESet<DateTime> _currentDeveloperDays = new KaVEHashSet<DateTime>(); 

        public DevelopersPerDayCalculator()
        {
            RegisterFor<IDEEvent>(OnAnyEvent);
        }

        private void OnAnyEvent(IDEEvent @event)
        {
            _currentDeveloperDays.Add(@event.GetTriggerDate());
        }

        public override void OnStreamEnds()
        {
            foreach (var currentDeveloperDay in _currentDeveloperDays)
            {
                _developerDays.Add(currentDeveloperDay);
            }
            _currentDeveloperDays.Clear();
        }

        public IDictionary<DateTime, int> GetStatistic()
        {
            return _developerDays.EntryDictionary;
        }

        public string GetStatisticAsCsv()
        {
            return GetStatistic().ToCsv();
        }
    }
}