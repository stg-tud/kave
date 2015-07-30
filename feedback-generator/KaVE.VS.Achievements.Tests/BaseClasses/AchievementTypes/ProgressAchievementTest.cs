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
using KaVE.VS.Achievements.Utils;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.BaseClasses.AchievementTypes
{
    [TestFixture]
    internal class ProgressAchievementTest : AchievementTestBase
    {
        [Test]
        public void InitializeTest()
        {
            const int targetValue = 1984;
            const int testId = 42;

            ReturnTargetValueForId(targetValue, testId);

            var uut = new ProgressAchievement(testId);
            Assert.AreEqual(testId, uut.Id);
            Assert.IsFalse(uut.IsCompleted);
            Assert.AreEqual("Not Completed", uut.CompletionDate);
            Assert.AreEqual(new DateTime(), uut.CompletionDateTime);
            Assert.AreEqual("0/" + targetValue.Format(), uut.ProgressString);
        }

        [Test]
        public void MinusAsInititalProgressStringValueForNonConstructableObjects()
        {
            ReturnTargetValueForId(1, 0);
            var baseAchievementAsTargetValue = new BaseAchievement(0);
            ReturnTargetValueForId(baseAchievementAsTargetValue, 1);
            var uut = new ProgressAchievement(1);

            Assert.AreEqual("-/" + baseAchievementAsTargetValue, uut.ProgressString);
        }

        [Test]
        public void PropertyChangedTest()
        {
            var propertyChangedWasRaised = false;
            var progressAchievement = new ProgressAchievement(1);
            progressAchievement.PropertyChanged += delegate { propertyChangedWasRaised = true; };

            progressAchievement.CurrentProgress = 50;

            Assert.IsTrue(propertyChangedWasRaised);
        }
    }
}