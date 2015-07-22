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

using KaVE.VS.Achievements.UI.StatisticUI;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.UI.StatisticUI
{
    [TestFixture]
    public class StatisticElementTest
    {
        private static readonly StatisticElement TestStatisticElement = new StatisticElement
        {
            Name = "SameRefernce",
            Value = "0"
        };

        private static readonly object[][] StatisticElements =
        {
            new object[]
            {
                new StatisticElement {Name = "Test", Value = "10"},
                new StatisticElement {Name = "Test2", Value = "10"},
                false
            },
            new object[]
            {
                new StatisticElement {Name = "Test", Value = "10"},
                new StatisticElement {Name = "Test", Value = "10"},
                true
            },
            new object[]
            {
                TestStatisticElement,
                TestStatisticElement,
                true
            },
            new object[]
            {
                TestStatisticElement,
                10,
                false
            }
        };

        [TestCaseSource("StatisticElements")]
        public void EqualsTest(object statisticElement1, object statisticElement2, bool expectedResult)
        {
            Assert.AreEqual(expectedResult, statisticElement1.Equals(statisticElement2));
        }
    }
}