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
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class ConcurrentSetsCalculator : BaseEventProcessor
    {
        public readonly IDictionary<ISet<string>, int> Statistic =
            new Dictionary<ISet<string>, int>();

        public ConcurrentSetsCalculator()
        {
            RegisterFor<ConcurrentEvent>(HandleConcurrentEvents);
        }

        public void HandleConcurrentEvents(ConcurrentEvent concurrentEvent)
        {
            var resultSet = Sets.NewHashSet<string>();

            foreach (var ideEvent in concurrentEvent.ConcurrentEventList)
            {
                resultSet.Add(EventMappingUtils.GetAbstractStringOf(ideEvent));
            }

            if (Statistic.ContainsKey(resultSet))
            {
                Statistic[resultSet]++;
            }
            else
            {
                if (resultSet.Count > 0)
                {
                    Statistic.Add(resultSet, 1);
                }
            }
        }

        public string StatisticAsCsv()
        {
            var csvBuilder = new CsvBuilder();
            var statistic = Statistic.OrderByDescending(keyValuePair => keyValuePair.Value);
            foreach (var stat in statistic)
            {
                csvBuilder.StartRow();

                var eventList = stat.Key.ToList();
                for (var i = 0; i < eventList.Count; i++)
                {
                    csvBuilder["Event" + i] = eventList[i];
                }
                csvBuilder["Count"] = stat.Value;
            }

            return csvBuilder.Build();
        }
    }
}