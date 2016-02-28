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
using KaVE.RS.Commons.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Settings
{
    internal class UserProfileSettingsUtilsTest
    {
        private ISettingsStore _settingsStore;
        private UserProfileSettings _settings;
        private IRandomizationUtils _rnd;
        private Guid _rndGuid;

        private UserProfileSettingsUtils _sut;

        [SetUp]
        public void Setup()
        {
            _settings = new UserProfileSettings();
            _settingsStore = Mock.Of<ISettingsStore>();
            Mock.Get(_settingsStore).Setup(ss => ss.GetSettings<UserProfileSettings>()).Returns(_settings);

            _rndGuid = Guid.NewGuid();
            _rnd = Mock.Of<IRandomizationUtils>();
            Mock.Get(_rnd).Setup(r => r.GetRandomGuid()).Returns(_rndGuid);

            _sut = new UserProfileSettingsUtils(_settingsStore, _rnd);
        }

        [Test]
        public void EnsureIdCreatesInitialValue()
        {
            _settings.ProfileId = "";
            _sut.EnsureProfileId();

            Assert.AreEqual(_rndGuid.ToString(), _settings.ProfileId);
            Mock.Get(_settingsStore).Verify(ss => ss.SetSettings(_settings));
        }

        [Test]
        public void EnsureIdDoesNotOverwriteExistingValue()
        {
            _settings.ProfileId = "x";
            _sut.EnsureProfileId();

            Assert.AreEqual("x", _settings.ProfileId);
            Mock.Get(_settingsStore).Verify(ss => ss.SetSettings(_settings), Times.Never);
        }

        [Test]
        public void HasBeenAsked()
        {
            Assert.False(_sut.HasBeenAskedToFillProfile());
            _settings.HasBeenAskedToFillProfile = true;
            Assert.True(_sut.HasBeenAskedToFillProfile());
        }

        [Test]
        public void CreateProfile()
        {
            Assert.AreEqual(_rndGuid.ToString(), _sut.CreateNewProfileId());
        }

        [Test]
        public void GetSettings()
        {
            var actual = _sut.GetSettings();
            Mock.Get(_settingsStore).Verify(ss => ss.GetSettings<UserProfileSettings>());
            Assert.AreSame(_settings, actual);
        }

        [Test]
        public void StoreSettings()
        {
            _sut.StoreSettings(_settings);

            Assert.True(_settings.HasBeenAskedToFillProfile);
            Mock.Get(_settingsStore).Verify(ss => ss.SetSettings(_settings));
        }
    }
}