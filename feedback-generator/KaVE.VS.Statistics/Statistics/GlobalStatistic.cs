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
using System.Numerics;
using KaVE.VS.Statistics.UI;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Statistics
{
    /// <summary>
    ///     Data structure for generic Statistics
    /// </summary>
    public class GlobalStatistic : Statistic<GlobalStatistic>
    {
        public GlobalStatistic()
        {
            LatestEventTime = DateTime.Now;
            EarliestEventTime = DateTime.Now;
        }

        public BigInteger TotalEvents { get; set; }

        public int TotalNumberOfEdits { get; set; }

        public int CurrentNumberOfEditsBetweenCommits { get; set; }

        public int MaxNumberOfEditsBetweenCommits { get; set; }

        public TimeSpan TimeInDebugSession { get; set; }

        public TimeSpan TotalWorkTime { get; set; }

        public DateTime LatestEventTime { get; set; }

        public DateTime EarliestEventTime { get; set; }

        /// <summary>
        ///     Gets a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public override List<StatisticElement> GetCollection()
        {
            return new List<StatisticElement>
            {
                NewElement((self => self.TotalEvents), TotalEvents.Format()),
                NewElement(self => self.TotalNumberOfEdits, TotalNumberOfEdits.Format()),
                NewElement(
                    self => self.CurrentNumberOfEditsBetweenCommits,
                    CurrentNumberOfEditsBetweenCommits.Format()),
                NewElement(
                    self => self.MaxNumberOfEditsBetweenCommits,
                    MaxNumberOfEditsBetweenCommits.Format()),
                NewElement(self => self.TimeInDebugSession, TimeInDebugSession.Format()),
                NewElement(self => self.TotalWorkTime, TotalWorkTime.Format()),
                NewElement(self => self.EarliestEventTime, EarliestEventTime.ToShortTimeString()),
                NewElement(self => self.LatestEventTime, LatestEventTime.ToShortTimeString())
            };
        }
    }
}