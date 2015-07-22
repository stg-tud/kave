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
using KaVE.VS.Achievements.Properties;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.UI.StatisticUI;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Statistics.Statistics
{
    [TestFixture]
    public class SolutionStatisticTest
    {
        private readonly SolutionStatistic _uut = new SolutionStatistic
        {
            ProjectItemsAdded = 1,
            ProjectItemsRemoved = 10,
            ProjectItemsRenamed = 100,
            ProjectsAdded = 1000,
            ProjectsRemoved = 1000000,
            ProjectsRenamed = 100000,
            SolutionItemsAdded = 2,
            SolutionItemsRemoved = 20,
            SolutionItemsRenamed = 200,
            SolutionsClosed = 2000,
            SolutionsOpened = 20000,
            SolutionsRenamed = 200000,
            TestClassesCreated = 9999
        };

        [Test]
        public void GetCollection()
        {
            var actualCollection = _uut.GetCollection();

            var expectedCollection = new List<StatisticElement>
            {
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionsOpened,
                    Value = "20.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionsRenamed,
                    Value = "200.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionsClosed,
                    Value = "2.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionItemsAdded,
                    Value = "2"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionItemsRenamed,
                    Value = "200"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SolutionItemsRemoved,
                    Value = "20"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectsAdded,
                    Value = "1.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectsRenamed,
                    Value = "100.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectsRemoved,
                    Value = "1.000.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectItemsAdded,
                    Value = "1"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectItemsRenamed,
                    Value = "100"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.ProjectItemsRemoved,
                    Value = "10"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TestClassesCreated,
                    Value = "9.999"
                }
            };

            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }
    }
}