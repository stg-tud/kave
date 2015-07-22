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
using KaVE.VS.Achievements.Achievements.Calculators;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Tests.Statistics.Calculators;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.AchievementCalculators
{
    [TestFixture]
    internal class NightWorkerCalculatorTest : CalculatorTestBase
    {
        private NightWorkerCalculator _uut;

        public NightWorkerCalculatorTest() : base(NightWorkerCalculator.Id) {}

        [SetUp]
        public void Init()
        {
            _uut = new NightWorkerCalculator(
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);
        }

        [TestCase(0, true), TestCase(1, true), TestCase(2, true), TestCase(3, true), TestCase(4, true),
         TestCase(5, false), TestCase(6, false), TestCase(7, false), TestCase(8, false), TestCase(9, false),
         TestCase(10, false), TestCase(11, false), TestCase(12, false), TestCase(13, false), TestCase(14, false),
         TestCase(15, false), TestCase(16, false), TestCase(17, false), TestCase(18, false), TestCase(19, false),
         TestCase(20, false), TestCase(21, false), TestCase(22, true), TestCase(23, true)]
        public void SetsCompletedCorrectly(int hour, bool shouldBeCompleted)
        {
            var testStatistic = new GlobalStatistic
            {
                EarliestEventTime = new DateTime(1984, 1, 1, hour, 0, 0),
                LatestEventTime = new DateTime(1984, 1, 1, hour, 0, 0)
            };

            _uut.OnNext(testStatistic);

            if (shouldBeCompleted)
            {
                VerifyCompleted();
            }
            else
            {
                BaseVerifyInitialized();
            }
        }

        [Test]
        public void InitializeTest()
        {
            BaseVerifyInitialized();
        }

        [Test]
        public void ResetTest()
        {
            var testStatistic = new StatisticCalculatorTest.TestStatistic();
            SetTimeTo(new DateTime(1984, 1, 1, 0, 0, 0));

            _uut.OnNext(testStatistic);

            _uut.ResetAchievement();

            BaseVerifyInitialized();
        }
    }
}