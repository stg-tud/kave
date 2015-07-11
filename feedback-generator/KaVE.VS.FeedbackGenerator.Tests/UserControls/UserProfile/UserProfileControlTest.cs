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

using System.Windows;
using KaVE.Commons.TestUtils.UserControls;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    internal class UserProfileControlTest : BaseUserControlTest
    {
        private ExportSettings _exportSettings;
        private UserProfileSettings _userProfileSettings;
        private UserProfileContext _dataContext;

        [SetUp]
        public void SetUp()
        {
            _exportSettings = new ExportSettings();
            _userProfileSettings = new UserProfileSettings();
            _dataContext = new UserProfileContext(_exportSettings, _userProfileSettings, new RandomizationUtils());
        }

        private UserProfileControl Create()
        {
            return Create(new UserProfileControl {DataContext = _dataContext});
        }

        [Test]
        public void ProfilePanelIsNotVisibleByDefault()
        {
            var sut = Create();
            Assert.AreEqual(Visibility.Collapsed, sut.ProfilePanel.Visibility);
        }

        [Test]
        public void ProfilePanelIsVisibleIfActivated()
        {
            _userProfileSettings.IsProvidingProfile = true;
            var sut = Create();
            Assert.AreEqual(Visibility.Visible, sut.ProfilePanel.Visibility);
        }

        [Test]
        public void ProfilePanelGetsVisibleOnActivation()
        {
            var sut = Create();
            sut.IsProvidingProfileCheckBox.Toggle();
            Assert.AreEqual(Visibility.Visible, sut.ProfilePanel.Visibility);
        }
    }
}