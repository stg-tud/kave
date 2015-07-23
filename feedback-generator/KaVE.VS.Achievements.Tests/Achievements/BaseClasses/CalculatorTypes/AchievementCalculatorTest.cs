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
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.BaseClasses.CalculatorTypes
{
    [TestFixture]
    internal class AchievementCalculatorTest : CalculatorTestBase
    {
        private const int TestId = 1;

        private CalculatorTestImplementation _uut;
        private BaseAchievement _testAchievement;

        public AchievementCalculatorTest() : base(TestId) {}

        [SetUp]
        public void Init()
        {
            _testAchievement = new BaseAchievement(TestId);
            _uut = new CalculatorTestImplementation(
                _testAchievement,
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);
        }

        public class CalculatorTestImplementation : AchievementCalculator
        {
            public CalculatorTestImplementation(BaseAchievement testAchievement,
                IAchievementListing achievementListing,
                IStatisticListing statisticListing,
                IObservable<IStatistic> observable)
                : base(testAchievement, achievementListing, statisticListing, observable)
            {
                AchievementListing.Update(testAchievement);
            }

            protected override bool Calculate(IStatistic statistic)
            {
                Achievement.Unlock();
                return true;
            }

            public BaseAchievement GetAchievement()
            {
                return Achievement;
            }
        }

        [Test]
        public void OnCompletedTest()
        {
            _uut.OnCompleted();

            Assert.Pass();
        }

        [Test]
        public void OnErrorTest()
        {
            _uut.OnError(new Exception());

            Assert.Pass();
        }

        [Test]
        public void InitializeTest()
        {
            BaseVerifyInitialized();
        }

        [Test]
        public void ResetAchievementTest()
        {
            var progressAchievement = new ProgressAchievement(TestId);
            var calculator = new CalculatorTestImplementation(
                progressAchievement,
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);

            calculator.OnNext(new TestStatistic());

            calculator.ResetAchievement();

            Assert.IsFalse(calculator.GetAchievement().IsCompleted);
            AchievementListingMock.Verify(
                l => l.Update(It.Is<ProgressAchievement>(actualAchievement => !actualAchievement.IsCompleted)));
        }

        [Test]
        public void UpdatesAchievementListingTest()
        {
            _uut.OnNext(new TestStatistic());

            AchievementListingMock.Verify(l => l.Update(_testAchievement));
        }
    }
}