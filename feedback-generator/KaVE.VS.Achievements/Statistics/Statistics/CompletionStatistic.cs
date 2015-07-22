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
using System.Runtime.Serialization;
using KaVE.VS.Achievements.UI.StatisticUI;
using KaVE.VS.Achievements.Util.ToStringFormatting.StatisticFormatting;

namespace KaVE.VS.Achievements.Statistics.Statistics
{
    /// <summary>
    ///     Data structure for Statistics computed using CompletionEvents
    /// </summary>
    [DataContract]
    public class CompletionStatistic : Statistic<CompletionStatistic>
    {
        [DataMember]
        public int TotalCompletions { get; set; }

        [DataMember]
        public int TotalCompleted { get; set; }

        [DataMember]
        public int TotalCancelled { get; set; }

        [DataMember]
        public TimeSpan TotalTime { get; set; }

        [DataMember]
        public TimeSpan TotalTimeCompleted { get; set; }

        public TimeSpan AverageTimeCompleted
        {
            get { return TotalCompleted > 0 ? new TimeSpan(TotalTimeCompleted.Ticks/TotalCompleted) : TimeSpan.Zero; }
        }

        [DataMember]
        public TimeSpan TotalTimeCancelled { get; set; }

        public TimeSpan AverageTimeCancelled
        {
            get { return TotalCancelled > 0 ? new TimeSpan(TotalTimeCancelled.Ticks/TotalCancelled) : TimeSpan.Zero; }
        }

        [DataMember]
        public int SavedKeystrokes { get; set; }

        public double AverageSavedKeystrokes
        {
            get
            {
                if (TotalCompleted > 0)
                {
                    return (double) SavedKeystrokes/TotalCompleted;
                }
                return 0;
            }
        }

        [DataMember]
        public BigInteger TotalProposals { get; set; }

        /// <summary>
        ///     Gets a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public override List<StatisticElement> GetCollection()
        {
            return new List<StatisticElement>
            {
                NewElement(self => self.TotalCompleted, TotalCompleted.Format()),
                NewElement(self => self.TotalCancelled, TotalCancelled.Format()),
                NewElement(self => self.SavedKeystrokes, SavedKeystrokes.Format()),
                NewElement(self => self.TotalTimeCompleted, TotalTimeCompleted.Format()),
                NewElement(self => self.TotalTimeCancelled, TotalTimeCancelled.Format()),
                NewElement(self => self.TotalProposals, TotalProposals.Format()),
                NewElement(self => self.TotalCompletions, TotalCompletions.Format()),
                NewElement(self => self.AverageSavedKeystrokes, AverageSavedKeystrokes.Format()),
                NewElement(self => self.AverageTimeCompleted, AverageTimeCompleted.Format()),
                NewElement(self => self.AverageTimeCancelled, AverageTimeCancelled.Format()),
                NewElement(self => self.TotalTime, TotalTime.Format())
            };
        }
    }
}