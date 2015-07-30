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

using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Calculators;
using KaVE.VS.Statistics.Statistics;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.AchievementCalculators
{
    [TestFixture]
    internal class FailedCompileCalculatorTest : CalculatorTestBase
    {
        private FailedCompileCalculator _uut;

        public FailedCompileCalculatorTest() : base(FailedCompileCalculator.Ids[0]) {}

        [SetUp]
        public void Init()
        {
            _uut = new FailedCompileCalculator(
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);
        }


        [Test]
        public void IgnoresOtherStatistics()
        {
            _uut.OnNext(new Mock<IStatistic>().Object);

            StatisticListingMock.Verify(l => l.Update(It.IsAny<IStatistic>()), Times.Never);
        }

        [Test]
        public void InitializeTest()
        {
            StagedVerifyInitialized();
        }

        [Test]
        public void ResetTest()
        {
            var buildStatistic = new BuildStatistic
            {
                FailedBuilds = 10
            };

            _uut.OnNext(buildStatistic);

            _uut.ResetAchievement();

            StagedVerifyInitialized();
        }

        [Test]
        public void SetsNextStageCorrectly()
        {
            ReturnTargetValueForIds(2, FailedCompileCalculator.Ids);
            ReturnTargetValueForId(1, FailedCompileCalculator.Ids[0]);
            ReturnTargetValueForId(1, FailedCompileCalculator.Ids[1]);

            var buildStatistic = new BuildStatistic
            {
                FailedBuilds = 10
            };

            _uut.OnNext(buildStatistic);

            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.CurrentStage == 2 &&
                                actualAchievement.Stages[0].IsCompleted &&
                                actualAchievement.Id == AchievementId)));
        }
    }
}