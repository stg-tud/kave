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

using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Settings
{
    [RequiresSTA]
    internal class ForcedSettingUpdateNotificatorTest
    {
        private ISettingsStore _store;
        private IUserProfileSettingsUtils _profileUtils;
        private AnonymizationSettings _settings;
        private UserProfileSettings _profile;
        private KaVESettings _kaveSettings;
        private ISimpleWindowOpener _windows;

        [SetUp]
        public void SetUp()
        {
            _windows = Mock.Of<ISimpleWindowOpener>();

            _store = Mock.Of<ISettingsStore>();
            _profileUtils = Mock.Of<IUserProfileSettingsUtils>();
            _kaveSettings = new KaVESettings();
            _settings = new AnonymizationSettings();
            _profile = new UserProfileSettings();
            Mock.Get(_store).Setup(s => s.GetSettings<KaVESettings>()).Returns(_kaveSettings);
            Mock.Get(_store).Setup(s => s.GetSettings<AnonymizationSettings>()).Returns(_settings);
            Mock.Get(_store).Setup(s => s.GetSettings<UserProfileSettings>()).Returns(_profile);

            // setting valid values that should not be notified
            _settings.RemoveSessionIDs = false;
            _profile.ProfileId = "x";
        }

        private void Init()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new ForcedSettingUpdateNotificator(_store, _profileUtils, _windows);
        }

        [Test]
        public void NothingHappensIfSettingIsNotActivated()
        {
            Init();

            Mock.Get(_store).Verify(s => s.GetSettings<AnonymizationSettings>());
            Mock.Get(_store).Verify(s => s.SetSettings(_settings), Times.Never);
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void DisablesActivatedSettingAndUpdatesConfig()
        {
            _settings.RemoveSessionIDs = true;

            Init();

            Assert.False(_settings.RemoveSessionIDs);
            Mock.Get(_store).Verify(s => s.GetSettings<AnonymizationSettings>());
            Mock.Get(_store).Verify(s => s.SetSettings(_settings));
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()));
        }

        [Test]
        public void NothinHappensIfProfileIdIsSet()
        {
            Init();

            Assert.AreEqual("x", _profile.ProfileId);
            Mock.Get(_store).Verify(s => s.GetSettings<UserProfileSettings>());
            Mock.Get(_store).Verify(s => s.SetSettings(_settings), Times.Never);
            Mock.Get(_profileUtils).Verify(s => s.EnsureProfileId(), Times.Never);
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void EnsuresProfileIdIfNotSet_Null()
        {
            _profile.ProfileId = null;

            Init();

            Mock.Get(_store).Verify(s => s.GetSettings<UserProfileSettings>());
            Mock.Get(_profileUtils).Verify(s => s.EnsureProfileId());
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()));
        }

        [Test]
        public void EnsuresProfileIdIfNotSet_Empty()
        {
            _profile.ProfileId = "";

            Init();

            Mock.Get(_store).Verify(s => s.GetSettings<UserProfileSettings>());
            Mock.Get(_profileUtils).Verify(s => s.EnsureProfileId());
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()));
        }

        [Test]
        public void ShouldNotOpenOnFirstStart()
        {
            _kaveSettings.IsFirstStart = true;
            _profile.ProfileId = "";

            Init();

            Mock.Get(_store).Verify(s => s.GetSettings<UserProfileSettings>());
            Mock.Get(_profileUtils).Verify(s => s.EnsureProfileId());
            Mock.Get(_windows).Verify(s => s.OpenForcedSettingUpdateWindow(It.IsAny<string>()), Times.Never);
        }
    }
}