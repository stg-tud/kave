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
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.BaseClasses.CalculatorTypes;
using KaVE.VS.Achievements.Tests.AchievementCalculators;
using KaVE.VS.Achievements.Tests.TestUtils;
using KaVE.VS.Statistics.StatisticListing;
using KaVE.VS.Statistics.Statistics;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.BaseClasses.CalculatorTypes
{
    [TestFixture]
    internal class IntegerStagedAchievementCalculatorTest : CalculatorTestBase
    {
        private const int TestId = 1;

        protected IntegerStagedCalculatorTestImplementation Uut;

        public IntegerStagedAchievementCalculatorTest() : base(TestId) {}

        [SetUp]
        public void Init()
        {
            Uut = new IntegerStagedCalculatorTestImplementation(
                new StagedAchievement(new[] {TestId}),
                AchievementListingMock.Object,
                StatisticListingMock.Object,
                ObservableMock.Object);
        }

        public class IntegerStagedCalculatorTestImplementation : IntegerStagedAchievementCalculator
        {
            public IntegerStagedCalculatorTestImplementation(StagedAchievement achievement,
                IAchievementListing achievementListing,
                IStatisticListing statisticListing,
                IObservable<IStatistic> observable)
                : base(achievement, achievementListing, statisticListing, observable) {}

            protected override bool Calculate(IStatistic statistic)
            {
                StagedCalculation(int.MaxValue);
                return true;
            }

            public IDisposable GetUnsubscriber()
            {
                return Unsubscriber;
            }
        }

        [Test, ExpectedException(typeof (InvalidCastException))]
        public void WrongTargetValuesCausesAnException()
        {
            ReturnTargetValueForId(new object(), TestId);

            var calculatorWithWrongTargetValueType =
                new IntegerStagedCalculatorTestImplementation(
                    new StagedAchievement(new[] {TestId}),
                    AchievementListingMock.Object,
                    StatisticListingMock.Object,
                    ObservableMock.Object);

            calculatorWithWrongTargetValueType.OnNext(new TestStatistic());
        }

        [Test]
        public void InitializeTest()
        {
            StagedVerifyInitialized();
        }
    }
}