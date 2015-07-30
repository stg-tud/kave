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
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.BaseClasses.AchievementTypes
{
    [TestFixture]
    internal class BaseAchievementTest : AchievementTestBase
    {
        [SetUp]
        public void Init()
        {
            _completionEventWasRaised = false;
            BaseAchievement.CompletedEventHandler += delegate { _completionEventWasRaised = true; };
        }

        private static bool _completionEventWasRaised;

        [Test]
        public void BaseAchievementIsCompletedTest()
        {
            var baseAchievement = new BaseAchievement(0);

            baseAchievement.Unlock();

            Assert.IsTrue(baseAchievement.IsCompleted);
            Assert.IsTrue(_completionEventWasRaised);
            Assert.AreEqual(baseAchievement.CompletionDateTime, TestTime);
        }

        [Test]
        public void InitializeBaseAchievement()
        {
            var baseAchievement = new BaseAchievement(0);
            Assert.AreEqual(0, baseAchievement.Id);
            Assert.IsFalse(baseAchievement.IsCompleted);
            Assert.AreEqual("Not Completed", baseAchievement.CompletionDate);
            Assert.AreEqual(new DateTime(), baseAchievement.CompletionDateTime);
        }

        [Test]
        public void BaseAchievementCantBeCompletedTwice()
        {
            var baseAchievement = new BaseAchievement(0);

            baseAchievement.Unlock();
            _completionEventWasRaised = false;

            SetTimeTo(Now.AddDays(1));

            baseAchievement.Unlock();

            Assert.IsFalse(_completionEventWasRaised);
            Assert.IsTrue(baseAchievement.IsCompleted);
            Assert.AreEqual(baseAchievement.CompletionDateTime, TestTime);
        }
    }
}