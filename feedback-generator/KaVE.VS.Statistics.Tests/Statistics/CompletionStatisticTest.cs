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
using KaVE.VS.Statistics.Properties;
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.UI;
using NUnit.Framework;

namespace KaVE.VS.Statistics.Tests.Statistics
{
    [TestFixture]
    public class CompletionStatisticTest
    {
        private readonly CompletionStatistic _uut = new CompletionStatistic
        {
            SavedKeystrokes = 10000,
            TotalCompletions = 1100,
            TotalCancelled = 100,
            TotalCompleted = 1000,
            TotalProposals = BigInteger.Parse("100000000000"),
            TotalTime = TimeSpan.FromDays(100) + TimeSpan.FromHours(12),
            TotalTimeCancelled = TimeSpan.FromHours(12),
            TotalTimeCompleted = TimeSpan.FromDays(100)
        };

        [Test]
        public void GetCollectionTest()
        {
            var actualCollection = _uut.GetCollection();

            var expectedCollection = new List<StatisticElement>
            {
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalCompleted,
                    Value = "1.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalCancelled,
                    Value = "100"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.SavedKeystrokes,
                    Value = "10.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalTimeCompleted,
                    Value = "100 d"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalTimeCancelled,
                    Value = "12 hr"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalProposals,
                    Value = "100.000.000.000"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalCompletions,
                    Value = "1.100"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.AverageSavedKeystrokes,
                    Value = "10"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.AverageTimeCompleted,
                    Value = "2,4 hr"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.AverageTimeCancelled,
                    Value = "7,2 min"
                },
                new StatisticElement
                {
                    Name = StatisticPropertyNames.TotalTime,
                    Value = "100,5 d"
                }
            };

            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }
    }
}