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
using KaVE.VS.Statistics.UI;
using KaVE.VS.Statistics.Utils;

namespace KaVE.VS.Statistics.Statistics
{
    /// <summary>
    ///     Data structure for Statistics computed using SolutionEvents
    /// </summary>
    public class SolutionStatistic : Statistic<SolutionStatistic>
    {
        public int SolutionsOpened { get; set; }

        public int SolutionsRenamed { get; set; }

        public int SolutionsClosed { get; set; }

        public int SolutionItemsAdded { get; set; }

        public int SolutionItemsRenamed { get; set; }

        public int SolutionItemsRemoved { get; set; }

        public int ProjectsAdded { get; set; }

        public int ProjectsRenamed { get; set; }

        public int ProjectsRemoved { get; set; }

        public int ProjectItemsAdded { get; set; }

        public int ProjectItemsRenamed { get; set; }

        public int ProjectItemsRemoved { get; set; }

        public int TestClassesCreated { get; set; }

        /// <summary>
        ///     Gets a List of StatisticElements containing each statistic of this data structure
        /// </summary>
        public override List<StatisticElement> GetCollection()
        {
            return new List<StatisticElement>
            {
                NewElement(self => self.SolutionsOpened, SolutionsOpened.Format()),
                NewElement(self => self.SolutionsRenamed, SolutionsRenamed.Format()),
                NewElement(self => self.SolutionsClosed, SolutionsClosed.Format()),
                NewElement(self => self.SolutionItemsAdded, SolutionItemsAdded.Format()),
                NewElement(self => self.SolutionItemsRenamed, SolutionItemsRenamed.Format()),
                NewElement(self => self.SolutionItemsRemoved, SolutionItemsRemoved.Format()),
                NewElement(self => self.ProjectsAdded, ProjectsAdded.Format()),
                NewElement(self => self.ProjectsRenamed, ProjectsRenamed.Format()),
                NewElement(self => self.ProjectsRemoved, ProjectsRemoved.Format()),
                NewElement(self => self.ProjectItemsAdded, ProjectItemsAdded.Format()),
                NewElement(self => self.ProjectItemsRenamed, ProjectItemsRenamed.Format()),
                NewElement(self => self.ProjectItemsRemoved, ProjectItemsRemoved.Format()),
                NewElement(self => self.TestClassesCreated, TestClassesCreated.Format())
            };
        }
    }
}