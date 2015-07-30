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
    internal class TotalCommitsCalculatorTest : CalculatorTestBase
    {
        private TotalCommitsCalculator _uut;

        public TotalCommitsCalculatorTest() : base(TotalCommitsCalculator.Ids[0]) {}

        [SetUp]
        public void Init()
        {
            _uut = new TotalCommitsCalculator(
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
            var commandStatistic = new CommandStatistic();
            commandStatistic.CommandTypeValues.Add("Team.Git.Commit", 1);
            commandStatistic.CommandTypeValues.Add("Team.Git.CommitAndPush", 2);
            commandStatistic.CommandTypeValues.Add("Team.Git.CommitAndSync", 3);

            _uut.OnNext(commandStatistic);

            _uut.ResetAchievement();

            StagedVerifyInitialized();
        }

        [Test]
        public void SetsNextStageCorrectly()
        {
            ReturnTargetValueForIds(7, TotalCommitsCalculator.Ids);
            ReturnTargetValueForId(6, TotalCommitsCalculator.Ids[0]);
            ReturnTargetValueForId(6, TotalCommitsCalculator.Ids[1]);

            var commandStatistic = new CommandStatistic();
            commandStatistic.CommandTypeValues.Add("Team.Git.Commit", 1);
            commandStatistic.CommandTypeValues.Add("Team.Git.CommitAndPush", 1);
            commandStatistic.CommandTypeValues.Add("Team.Git.CommitAndSync", 2);
            commandStatistic.CommandTypeValues.Add("Comm_it", 2);

            _uut.OnNext(commandStatistic);

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
    }
}