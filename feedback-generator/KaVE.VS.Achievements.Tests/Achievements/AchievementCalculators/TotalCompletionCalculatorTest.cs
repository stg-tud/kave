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

using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Achievements.Calculators;
using KaVE.VS.Achievements.Statistics.Statistics;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.AchievementCalculators
{
    [TestFixture]
    internal class TotalCompletionCalculatorTest : CalculatorTestBase
    {
        private TotalCompletionCalculator _uut;

        public TotalCompletionCalculatorTest() : base(TotalCompletionCalculator.Ids[0]) {}

        [SetUp]
        public void Init()
        {
            _uut = new TotalCompletionCalculator(
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
        public void ResetTest()
        {
            var completionStatistic = new CompletionStatistic
            {
                TotalCompleted = 5
            };

            _uut.OnNext(completionStatistic);

            _uut.ResetAchievement();

            StagedVerifyInitialized();
        }

        [Test]
        public void SetsNextStageCorrectly()
        {
            ReturnTargetValueForIds(2, TotalCompletionCalculator.Ids);
            ReturnTargetValueForId(1, TotalCompletionCalculator.Ids[0]);
            ReturnTargetValueForId(1, TotalCompletionCalculator.Ids[1]);

            var completionStatistic = new CompletionStatistic
            {
                TotalCompleted = 1
            };

            _uut.OnNext(completionStatistic);

            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.CurrentStage == 2)));
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.Stages[0].IsCompleted)));
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.Stages[1].IsCompleted)));
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.Id == AchievementId)));
        }

        [Test]
        public void InitializeTest()
        {
            StagedVerifyInitialized();
        }
    }
}