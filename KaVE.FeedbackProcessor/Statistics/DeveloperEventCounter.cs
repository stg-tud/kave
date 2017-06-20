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
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class DeveloperEventCounter : BaseEventProcessor
    {
        private Developer _currentDeveloper;
        public readonly IDictionary<Developer, int> EventCount = new Dictionary<Developer, int>();

        public DeveloperEventCounter()
        {
            RegisterFor<IDEEvent>(OnAnyEvent);
        }

        public override void OnStreamStarts(Developer developer)
        {
            _currentDeveloper = developer;
            EventCount[_currentDeveloper] = 0;
        }

        private void OnAnyEvent(IDEEvent @event)
        {
            EventCount[_currentDeveloper]++;
        }

        public string StatisticAsCsv()
        {
            return EventCount.Select(kvp => new KeyValuePair<string, int>(kvp.Key.Id.ToString(), kvp.Value)).ToCsv();
        }
    }
}