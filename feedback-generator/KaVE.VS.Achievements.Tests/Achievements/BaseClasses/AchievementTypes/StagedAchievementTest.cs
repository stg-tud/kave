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
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests.Achievements.BaseClasses.AchievementTypes
{
    [TestFixture]
    internal class StagedAchievementTest : AchievementTestBase
    {
        [SetUp]
        public void Init()
        {
            _uut = new StagedAchievement(new[] {1, 2, 3});
        }

        private static StagedAchievement _uut;

        [Test]
        public void CurrentStageTest()
        {
            Assert.AreEqual(_uut.Stages[_uut.CurrentStage], _uut.CurrentStageAchievement);
        }

        [Test]
        public void FirstStageTest()
        {
            Assert.AreEqual(_uut.Stages[0], _uut.FirstStageAchievement);
        }

        [Test]
        public void HighestCompletedStageOrFirstTest()
        {
            Assert.AreEqual(_uut.Stages[0], _uut.HighestCompletedStageOrFirst);

            _uut.CurrentStage = 1;
            Assert.AreEqual(_uut.Stages[0], _uut.HighestCompletedStageOrFirst);

            _uut.CurrentStage = 2;
            Assert.AreEqual(_uut.Stages[1], _uut.HighestCompletedStageOrFirst);

            _uut.IsCompleted = true;
            Assert.AreEqual(_uut.Stages[2], _uut.HighestCompletedStageOrFirst);
        }

        [Test]
        public void InitializeStagedAchievement()
        {
            ReturnTargetValueForId(5, 1);
            ReturnTargetValueForId(50, 2);
            ReturnTargetValueForId(100, 3);

            Assert.AreEqual(1, _uut.Stages[0].Id);
            Assert.AreEqual(2, _uut.Stages[1].Id);
            Assert.AreEqual(3, _uut.Stages[2].Id);
            Assert.AreEqual("0/5", _uut.Stages[0].ProgressString);
            Assert.AreEqual("0/50", _uut.Stages[1].ProgressString);
            Assert.AreEqual("0/100", _uut.Stages[2].ProgressString);
        }

        [Test]
        public void LastStageTest()
        {
            Assert.AreEqual(_uut.Stages[_uut.Stages.Length - 1], _uut.LastStageAchievement);
        }
    }
}