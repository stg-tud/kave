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
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Settings
{
    [RequiresSTA]
    internal class FirstStartNotificatorTest
    {
        private KaVESettings _kaveSettings;
        private ISimpleWindowOpener _windows;
        private ISettingsStore _store;
        private IUserProfileSettingsUtils _profileUtils;
        private UserProfileSettings _profileSettings;
        private AnonymizationSettings _anonSettings;
        private ExportSettings _exportSettings;


        [SetUp]
        public void Setup()
        {
            _kaveSettings = new KaVESettings();
            _anonSettings = new AnonymizationSettings();
            _profileSettings = new UserProfileSettings();
            _exportSettings = new ExportSettings();
            _profileUtils = Mock.Of<IUserProfileSettingsUtils>();

            _store = Mock.Of<ISettingsStore>();
            Mock.Get(_store).Setup(ss => ss.GetSettings<KaVESettings>()).Returns(_kaveSettings);
            Mock.Get(_store).Setup(s => s.GetSettings<AnonymizationSettings>()).Returns(_anonSettings);
            Mock.Get(_store).Setup(s => s.GetSettings<UserProfileSettings>()).Returns(_profileSettings);
            Mock.Get(_store).Setup(s => s.GetSettings<ExportSettings>()).Returns(_exportSettings);

            _windows = Mock.Of<ISimpleWindowOpener>();

            // setting valid values that should not be notified
            _anonSettings.RemoveSessionIDs = false;
            _profileSettings.ProfileId = "x";
            _exportSettings.UploadUrl = "http://upload.kave.cc/";
        }

        private void Init()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new FirstStartNotificator(_windows, _store, _profileUtils);
        }

        [Test]
        public void SettingIsSetOnFirstStart()
        {
            _kaveSettings.IsFirstStart = true;
            _anonSettings.RemoveSessionIDs = true;
            _profileSettings.ProfileId = null;

            Init();

            AssertIsFirstStart(true, true);
            AssertProfileId(true, false);
            AssertSessionId(true, false);
        }

        [Test]
        public void NothingIsDoneIfEverythingIsCorrect()
        {
            Init();

            AssertIsFirstStart(false, false);
            AssertProfileId(false, false);
            AssertSessionId(false, false);
        }


        [Test]
        public void SessionIdIsSetAndReminded()
        {
            _anonSettings.RemoveSessionIDs = true;

            Init();

            AssertIsFirstStart(false, false);
            AssertProfileId(false, false);
            AssertSessionId(true, true);
        }

        [Test]
        public void ProfileIdIsSetAndReminded_null()
        {
            _profileSettings.ProfileId = null;

            Init();

            AssertIsFirstStart(false, false);
            AssertProfileId(true, true);
            AssertSessionId(false, false);
        }

        [Test]
        public void ProfileIdIsSetAndReminded_empty()
        {
            _profileSettings.ProfileId = "";

            Init();

            AssertIsFirstStart(false, false);
            AssertProfileId(true, true);
            AssertSessionId(false, false);
        }

        [TestCase("http://kave.st.informatik.tu-darmstadt.de/"), TestCase("http://kave.st.informatik.tu-darmstadt.de"),
         TestCase("http://130.83.165.51/"), TestCase("http://130.83.165.51")]
        public void OldServerIsUpdated(string url)
        {
            _exportSettings.UploadUrl = url;

            Init();

            AssertUrlSettings(true, "http://upload.kave.cc/");
        }

        [TestCase("http://kave.st.informatik.tu-darmstadt.de/test/"),
         TestCase("http://kave.st.informatik.tu-darmstadt.de/test"),
         TestCase("http://130.83.165.51/test/"), TestCase("http://130.83.165.51/test")]
        public void OldServerIsUpdated_test(string url)
        {
            _exportSettings.UploadUrl = url;

            Init();

            AssertUrlSettings(true, "http://upload.kave.cc/test/");
        }

        [Test]
        public void OldServerIsNotUpdatedForOtherValues()
        {
            _exportSettings.UploadUrl = "http://other.url/";

            Init();

            AssertUrlSettings(false, "http://other.url/");
        }

        private void AssertIsFirstStart(bool shouldSet, bool shouldOpen)
        {
            var timesSet = shouldSet ? Times.Once() : Times.Never();
            var timesOpen = shouldOpen ? Times.Once() : Times.Never();
            Mock.Get(_store).Verify(ss => ss.SetSettings(_kaveSettings), timesSet);
            Mock.Get(_windows).Verify(w => w.OpenFirstStartWindow(), timesOpen);
            Assert.False(_kaveSettings.IsFirstStart);
        }


        private void AssertSessionId(bool shouldSet, bool shouldOpen)
        {
            var timesSet = shouldSet ? Times.Once() : Times.Never();
            var timesOpen = shouldOpen ? Times.Once() : Times.Never();
            Mock.Get(_store).Verify(ss => ss.SetSettings(_anonSettings), timesSet);
            Mock.Get(_windows)
                .Verify(w => w.OpenForcedSettingUpdateWindow(FirstStartNotificator.SessionIdText), timesOpen);
            Assert.False(_anonSettings.RemoveSessionIDs);
        }

        private void AssertProfileId(bool shouldSet, bool shouldOpen)
        {
            var timesSet = shouldSet ? Times.Once() : Times.Never();
            var timesOpen = shouldOpen ? Times.Once() : Times.Never();
            Mock.Get(_profileUtils).Verify(u => u.EnsureProfileId(), timesSet);
            Mock.Get(_windows)
                .Verify(w => w.OpenForcedSettingUpdateWindow(FirstStartNotificator.ProfileIdText), timesOpen);
        }

        private void AssertUrlSettings(bool shouldUpdate, string expectedUrl)
        {
            var timesSet = shouldUpdate ? Times.Once() : Times.Never();
            var timesOpen = shouldUpdate ? Times.Once() : Times.Never();
            Mock.Get(_store).Verify(ss => ss.SetSettings(_exportSettings), timesSet);
            Mock.Get(_windows)
                .Verify(w => w.OpenForcedSettingUpdateWindow(FirstStartNotificator.UploadUrlText), timesOpen);
            Assert.AreEqual(expectedUrl, _exportSettings.UploadUrl);
        }
    }
}