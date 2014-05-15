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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using EnvDTE;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.VsIntegration
{
    [TestFixture]
    public class IDESessionTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private IDESessionSettings _settings;
        private IDESession _uut;

        [SetUp]
        public void MockEnvironment()
        {
            Registry.RegisterComponent<IDateUtils>(new DateUtils());

            _settings = new IDESessionSettings();

            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockSettingsStore.Setup(store => store.GetSettings<IDESessionSettings>()).Returns(_settings);

            var dte = new Mock<DTE>().Object;

            _uut = new IDESession(dte, _mockSettingsStore.Object);
        }

        [TearDown]
        public void TearDown()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfNoneExisted()
        {
            _settings.SessionUUID = "";

            var actualSessionId = _uut.UUID;

            _mockSettingsStore.Verify(store => store.SetSettings(_settings));
            var storedSessionUUID = _settings.SessionUUID;
            Assert.NotNull(storedSessionUUID);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfStoredOneWasGeneratedInThePast()
        {
            _settings.SessionUUID = "OutdatedID";
            _settings.SessionUUIDCreationDate = DateTime.Today.AddDays(-1);

            var actualSessionId = _uut.UUID;

            _mockSettingsStore.Verify(store => store.SetSettings(_settings));
            var storedSessionUUID = _settings.SessionUUID;
            Assert.NotNull(actualSessionId);
            Assert.AreNotEqual("OutdatedID", actualSessionId);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfStoredOneWasGeneratedOnFutureDate()
        {
            _settings.SessionUUID = "FutureID";
            _settings.SessionUUIDCreationDate = DateTime.Today.AddDays(-1);

            var actualSessionId = _uut.UUID;

            _mockSettingsStore.Verify(store => store.SetSettings(_settings));
            var storedSessionUUID = _settings.SessionUUID;
            Assert.NotNull(actualSessionId);
            Assert.AreNotEqual("FutureID", actualSessionId);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldReturnExistingSessionUUIDIfItWasGeneratedToday()
        {
            _settings.SessionUUID = "CurrentID";
            _settings.SessionUUIDCreationDate = DateTime.Today;

            var actualSessionId = _uut.UUID;

            _mockSettingsStore.Verify(store => store.SetSettings(It.IsAny<IDESessionSettings>()), Times.Never);
            Assert.AreEqual("CurrentID", actualSessionId);
        }
    }
}