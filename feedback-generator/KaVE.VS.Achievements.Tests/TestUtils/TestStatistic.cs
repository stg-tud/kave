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
using KaVE.VS.Statistics.Statistics;
using KaVE.VS.Statistics.UI;

namespace KaVE.VS.Achievements.Tests.TestUtils
{
    public class TestStatistic : IStatistic
    {
        private static readonly Random Rng = new Random();

        public int TestValue;

        public TestStatistic()
        {
            TestValue = Rng.Next();
        }

        public List<StatisticElement> GetCollection()
        {
            return new List<StatisticElement>
            {
                new StatisticElement
                {
                    Name = "TestValue",
                    Value = TestValue.ToString()
                }
            };
        }

        private bool Equals(TestStatistic other)
        {
            return TestValue.Equals(other.TestValue);
        }

#pragma warning disable 659
        public override bool Equals(object other)
        {
            return other is TestStatistic && Equals((TestStatistic) other);
        }
#pragma warning restore 659
    }
}