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
using KaVE.VS.Achievements.Achievements.Calculators;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util.ToStringFormatting.AchievementFormatting;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.AchievementCalculators
{
    [TestFixture]
    internal class TimeInDebugCalculatorTest : CalculatorTestBase
    {
        private TimeInDebugCalculator _uut;

        public TimeInDebugCalculatorTest() : base(TimeInDebugCalculator.Ids[0]) {}

        [SetUp]
        public void Init()
        {
            ReturnTargetValueForIds(new TimeSpan(0, 0, 1), TimeInDebugCalculator.Ids);
            _uut = new TimeInDebugCalculator(
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
            var globalStatistic = new GlobalStatistic
            {
                TimeInDebugSession = new TimeSpan(0, 1, 0)
            };

            _uut.OnNext(globalStatistic);

            _uut.ResetAchievement();

            StagedVerifyInitialized();
        }

        [Test]
        public void SetsNextStageCorrectly()
        {
            ReturnTargetValueForIds(new TimeSpan(0, 0, 2), TimeInDebugCalculator.Ids);
            ReturnTargetValueForId(new TimeSpan(0, 0, 1), TimeInDebugCalculator.Ids[0]);
            ReturnTargetValueForId(new TimeSpan(0, 0, 1), TimeInDebugCalculator.Ids[1]);

            var timeInDebugSession = new TimeSpan(0, 0, 1);
            var globalStatistic = new GlobalStatistic
            {
                TimeInDebugSession = timeInDebugSession
            };

            _uut.OnNext(globalStatistic);

            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.CurrentStage == 2 &&
                                actualAchievement.Stages[0].IsCompleted &&
                                actualAchievement.Id == AchievementId)));
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                actualAchievement.FirstStageAchievement.ProgressString ==
                                string.Format("{0}/{0}", timeInDebugSession.Format()))));
        }
    }
}