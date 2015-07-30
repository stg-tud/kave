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
using System.IO;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO;
using KaVE.VS.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Utils;
using KaVE.VS.Statistics.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.Achievements.Tests
{
    [TestFixture]
    internal class AchievementListingTest
    {
        private static readonly DateTime TestTime = DateTime.Now;

        private const int TestId = 0;

        private AchievementListing _uut;
        private BaseAchievement _baseAchievement;
        private Mock<IIoUtils> _ioUtilMock;

        [SetUp]
        public void Init()
        {
            _ioUtilMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioUtilMock.Object);

            Registry.RegisterComponent(new Mock<IErrorHandler>().Object);

            var clockMock = new Mock<IDateUtils>();
            Registry.RegisterComponent(clockMock.Object);
            clockMock.Setup(c => c.Now).Returns(TestTime);

            var targetValueProviderMock = new Mock<ITargetValueProvider>();
            Registry.RegisterComponent(targetValueProviderMock.Object);
            targetValueProviderMock.Setup(tvp => tvp.GetTargetValue(It.IsAny<int>())).Returns(1984);

            _uut = new AchievementListing();
            _baseAchievement = new BaseAchievement(TestId);
        }

        [TearDown]
        public void TearDown()
        {
            BaseAchievement.CompletedEventHandler -= FailOnCompletedEvent;
            Registry.Clear();
        }

        [Test]
        public void AddNewObjectToDictionaryAndGetAchievementSuccess()
        {
            _uut.Update(_baseAchievement);
            var actual = _uut.GetAchievement(TestId);

            Assert.AreEqual(_baseAchievement, actual);
        }

        [Test]
        public void CreateFileOnFirstUpdateWhenFileDoesNotExist()
        {
            _uut.Update(_baseAchievement);

            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void DeleteDataTest()
        {
            _uut.Update(_baseAchievement);

            _uut.DeleteData();

            var actualAchievement = _uut.GetAchievement(0);

            Assert.AreEqual(null, actualAchievement);

            _ioUtilMock.Verify(io => io.DeleteFile(It.IsAny<string>()));
        }

        [Test]
        public void DeserializingCompletedAchievementsShouldntRaiseEvent()
        {
            var progressAchievement = new ProgressAchievement(TestId + 1);
            var stagedAchievement = new StagedAchievement(new[] {TestId + 2});

            _baseAchievement.Unlock();
            progressAchievement.Unlock();
            stagedAchievement.CurrentStageAchievement.Unlock();

            _uut.Update(_baseAchievement);
            _uut.Update(progressAchievement);
            _uut.Update(stagedAchievement);

            var serializedDictionary = JsonSerialization.JsonSerializeObject(_uut.GetAchievementDictionary());

            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _ioUtilMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(serializedDictionary);

            // Deserializing shouldn't cause completed events
            BaseAchievement.CompletedEventHandler += FailOnCompletedEvent;

            // Constructor implicitly deserializes serializedDictionary
            _uut = new AchievementListing();
        }

        [Test]
        public void FileIsRecreatedOnExceptionWhileInitializing()
        {
            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _ioUtilMock.Setup(x => x.ReadFile(It.IsAny<string>())).Throws<Exception>();

            _uut = new AchievementListing();

            _ioUtilMock.Verify(x => x.DeleteFile(It.IsAny<string>()));
            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void GetAchievementFail()
        {
            Assert.AreEqual(null, _uut.GetAchievement(0));
        }

        [Test]
        public void ReadStatisticsFromFileOnInitalize()
        {
            var stagedAchievement = new StagedAchievement(new[] {TestId + 2});
            stagedAchievement.CurrentStageAchievement.IsCompleted = true;
            stagedAchievement.CurrentStageAchievement.CurrentProgress = 50;

            _uut.Update(stagedAchievement);
            _uut.Update(_baseAchievement);

            var serializedDictionary = JsonSerialization.JsonSerializeObject(_uut.GetAchievementDictionary());

            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);
            _ioUtilMock.Setup(x => x.ReadFile(It.IsAny<string>())).Returns(serializedDictionary);

            _uut = new AchievementListing();

            Assert.AreEqual(2, _uut.GetAchievementDictionary().Count);

            var actualAchievement = (StagedAchievement) _uut.GetAchievement(TestId + 2);
            Assert.NotNull(actualAchievement);
            Assert.IsTrue(actualAchievement.CurrentStageAchievement.IsCompleted);
            Assert.AreEqual(50, actualAchievement.CurrentStageAchievement.CurrentProgress);
        }

        [Test]
        public void UpdateCreatesFileWhenFileDoesNotExist()
        {
            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(false);

            _uut.Update(_baseAchievement);

            _ioUtilMock.Verify(x => x.CreateFile(It.IsAny<string>()));
        }

        [Test]
        public void UpdateWriteAchievementsToFileTest()
        {
            _ioUtilMock.Setup(x => x.FileExists(It.IsAny<string>())).Returns(true);

            _uut.Update(_baseAchievement);

            _ioUtilMock.Verify(x => x.OpenFile(It.IsAny<string>(), FileMode.Create, FileAccess.Write));
        }

        private static void FailOnCompletedEvent(object achievement, EventArgs args)
        {
            Assert.Fail("Completed Event was raised by" + achievement);
        }
    }
}