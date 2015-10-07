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
using KaVE.VS.Statistics.Properties;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.UI;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Statistics
{
    [TestFixture]
    public class BuildStatisticTest
    {
        [Test]
        public void DefaultValues()
        {
            var uut = new BuildStatistic();
            Assert.AreEqual(0, uut.FailedBuilds);
            Assert.AreEqual(0, uut.SuccessfulBuilds);
            Assert.AreEqual(0, uut.TotalBuilds);
        }

        [Test]
        public void GetCollectionTest()
        {
            var actualCollection = new BuildStatistic
            {
                FailedBuilds = 10000,
                SuccessfulBuilds = 10
            }.GetCollection();

            var expectedCollection = new List<StatisticElement>
            {
                new StatisticElement
                {
                    Name = StatisticPropertyNames.FailedBuilds,
                    Value = "10.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SuccessfulBuilds,
                    Value = "10"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalBuilds,
                    Value = "10.010"
                }
            };

            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }
    }
}