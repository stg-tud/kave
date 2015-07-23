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
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO;
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Achievements.Listing;
using KaVE.VS.Achievements.Statistics.Listing;
using KaVE.VS.Achievements.Statistics.Statistics;
using KaVE.VS.Achievements.Util;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.AchievementCalculators
{
    internal abstract class CalculatorTestBase
    {
        public static DateTime TestTime = DateTime.Now;

        protected Mock<IAchievementListing> AchievementListingMock;
        protected Mock<IDateUtils> ClockMock;
        protected Mock<IObservable<IStatistic>> ObservableMock;
        protected Mock<IStatisticListing> StatisticListingMock;
        protected Mock<ITargetValueProvider> TargetValueProviderMock;
        protected Mock<IDisposable> UnsubscriberMock;
        protected int AchievementId;

        protected CalculatorTestBase(int achievementId)
        {
            AchievementId = achievementId;
        }

        [SetUp]
        public void SetUp()
        {
            AchievementListingMock = new Mock<IAchievementListing>();
            StatisticListingMock = new Mock<IStatisticListing>();
            ObservableMock = new Mock<IObservable<IStatistic>>();
            UnsubscriberMock = new Mock<IDisposable>();

            var ioUtilMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(ioUtilMock.Object);
            Registry.RegisterComponent(new Mock<IErrorHandler>().Object);

            ObservableMock.Setup(obs => obs.Subscribe(It.IsAny<IObserver<IStatistic>>()))
                          .Returns(UnsubscriberMock.Object);

            ClockMock = new Mock<IDateUtils>();
            SetTimeTo(TestTime);
            Registry.RegisterComponent(ClockMock.Object);

            TargetValueProviderMock = new Mock<ITargetValueProvider>();
            TargetValueProviderMock.Setup(tvp => tvp.GetTargetValue(It.IsAny<int>())).Returns(int.MaxValue);
            Registry.RegisterComponent(TargetValueProviderMock.Object);
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        protected void ReturnTargetValueForId(object value, int id)
        {
            TargetValueProviderMock.Setup(tvp => tvp.GetTargetValue(It.Is<int>(i => i == id))).Returns(value);
        }

        protected void ReturnTargetValueForIds(object value, int[] ids)
        {
            foreach (var id in ids)
            {
                ReturnTargetValueForId(value, id);
            }
        }

        protected void SetTimeTo(DateTime dateTime)
        {
            ClockMock.Setup(c => c.Now).Returns(dateTime);
        }

        protected void BaseVerifyInitialized()
        {
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<BaseAchievement>(
                            actualAchievement =>
                                !actualAchievement.IsCompleted &&
                                actualAchievement.Id == AchievementId)));
        }

        protected void ProgressVerifyInitialized()
        {
            BaseVerifyInitialized();

            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<ProgressAchievement>(
                            actualAchievement =>
                                actualAchievement.CurrentProgress.Equals(0) &&
                                actualAchievement.Id == AchievementId)));
        }

        protected void StagedVerifyInitialized()
        {
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<StagedAchievement>(
                            actualAchievement =>
                                !actualAchievement.IsCompleted &&
                                actualAchievement.CurrentStage == 0 &&
                                actualAchievement.CurrentStageAchievement.CurrentProgress.Equals(0) &&
                                actualAchievement.Id == AchievementId)));
        }

        protected void VerifyCompleted()
        {
            AchievementListingMock.Verify(
                l =>
                    l.Update(
                        It.Is<BaseAchievement>(
                            actualAchievement =>
                                actualAchievement.Id == AchievementId &&
                                actualAchievement.IsCompleted)));
        }
    }
}