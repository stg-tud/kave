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
using System.Runtime.Serialization;
using KaVE.VS.Achievements.UI.StatisticUI;
using KaVE.VS.Achievements.Util.ToStringFormatting.StatisticFormatting;

namespace KaVE.VS.Achievements.Statistics.Statistics
{
    /// <summary>
    ///     Data structure for Statistics computed using BuildEvents
    /// </summary>
    [DataContract]
    public class BuildStatistic : Statistic<BuildStatistic>
    {
        [DataMember]
        public int FailedBuilds { get; set; }

        [DataMember]
        public int SuccessfulBuilds { get; set; }

        public int TotalBuilds
        {
            get { return FailedBuilds + SuccessfulBuilds; }
        }

        /// <summary>
        ///     Gets a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public override List<StatisticElement> GetCollection()
        {
            return new List<StatisticElement>
            {
                NewElement(self => self.FailedBuilds, FailedBuilds.Format()),
                NewElement(self => self.SuccessfulBuilds, SuccessfulBuilds.Format()),
                NewElement(self => self.TotalBuilds, TotalBuilds.Format())
            };
        }
    }
}