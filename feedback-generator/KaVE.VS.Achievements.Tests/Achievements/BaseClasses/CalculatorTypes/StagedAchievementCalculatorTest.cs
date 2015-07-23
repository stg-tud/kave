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
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Achievements.BaseClasses.CalculatorTypes;
using KaVE.VS.Achievements.Achievements.Listing;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Tests.Achievements.AchievementCalculators;
using KaVE.VS.Achievements.Tests.TestUtils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.BaseClasses.CalculatorTypes
{
    [TestFixture]
    internal class StagedAchievementCalculatorTest : CalculatorTestBase
    {
        private const int TestId = 1;

        private StagedCalculatorTestImplementation _uut;
        private StagedAchievement _testAchievement;

        public StagedAchievementCalculatorTest() : base(TestId) {}

        [SetUp]
        public void Init()
        {
            _testAchievement = new StagedAchievement(new[] {TestId, 2, 3, 4, 5});
            _uut = new StagedCalculatorTestImplementation(
                _testAchievement,
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);
        }

        public class StagedCalculatorTestImplementation : IntegerStagedAchievementCalculator
        {
            public StagedCalculatorTestImplementation(StagedAchievement testAchievement,
                IAchievementListing achievementListing,
                IStatisticListing statisticListing,
                IObservable<IStatistic> observable)
                : base(testAchievement, achievementListing, statisticListing, observable) {}

            protected override bool Calculate(IStatistic statistic)
            {
                var testStatistic = statistic as TestStatistic;
                if (testStatistic == null)
                {
                    return false;
                }

                StagedCalculation(testStatistic.TestValue);
                return true;
            }
        }

        [Test]
        public void ResetsAchievementCorrectly()
        {
            _uut.OnNext(new TestStatistic {TestValue = int.MaxValue});

            _uut.ResetAchievement();

            Assert.IsFalse(_testAchievement.IsCompleted);
            foreach (var stage in _testAchievement.Stages)
            {
                Assert.IsFalse(stage.IsCompleted);
            }
        }

        [Test]
        public void StagedCalculationTest()
        {
            ReturnTargetValueForId(5, TestId);
            ReturnTargetValueForId(50, 2);
            ReturnTargetValueForId(100, 3);
            ReturnTargetValueForId(500, 4);
            ReturnTargetValueForId(1000, 5);

            var stagedAchievement = new StagedAchievement(new[] {TestId, 2, 3, 4, 5});

            var calculator = new StagedCalculatorTestImplementation(
                stagedAchievement,
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);

            calculator.OnNext(new TestStatistic {TestValue = 250});

            Assert.AreEqual(100, stagedAchievement.Stages[0].CurrentProgress);
            Assert.AreEqual(100, stagedAchievement.Stages[TestId].CurrentProgress);
            Assert.AreEqual(100, stagedAchievement.Stages[2].CurrentProgress);
            Assert.AreEqual(50, stagedAchievement.Stages[3].CurrentProgress);
            Assert.AreEqual(25, stagedAchievement.Stages[4].CurrentProgress);

            Assert.AreEqual(3, stagedAchievement.CurrentStage);

            calculator.OnNext(new TestStatistic {TestValue = 1200});

            Assert.AreEqual(100, stagedAchievement.Stages[3].CurrentProgress);
            Assert.AreEqual(100, stagedAchievement.Stages[4].CurrentProgress);
            Assert.AreEqual(4, stagedAchievement.CurrentStage);
        }

        [Test]
        public void InitializeTest()
        {
            StagedVerifyInitialized();
        }
    }
}