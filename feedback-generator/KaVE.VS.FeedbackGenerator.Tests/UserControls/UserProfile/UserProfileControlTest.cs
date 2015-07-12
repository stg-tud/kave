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
using System.Windows;
using KaVE.Commons.TestUtils.UserControls;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    [RequiresSTA]
    internal class UserProfileControlTest : BaseUserControlTest
    {
        private ExportSettings _exportSettings;
        private UserProfileSettings _userProfileSettings;
        private UserProfileContext _dataContext;
        private IRandomizationUtils _randomizationUtils;

        private Guid _rndGuid;

        [SetUp]
        public void SetUp()
        {
            _exportSettings = new ExportSettings();
            _userProfileSettings = new UserProfileSettings();
            _randomizationUtils = Mock.Of<IRandomizationUtils>();
            _rndGuid = Guid.NewGuid();
            Mock.Get(_randomizationUtils).Setup(r => r.GetRandomGuid()).Returns(_rndGuid);
            _dataContext = new UserProfileContext(_exportSettings, _userProfileSettings, _randomizationUtils);
        }

        private UserProfileControl Open()
        {
            return OpenWindow(new UserProfileControl {DataContext = _dataContext});
        }

        [Test]
        public void Startup_ProfilePanelIsNotVisibleByDefault()
        {
            var sut = Open();
            Assert.AreEqual(Visibility.Collapsed, sut.ProfilePanel.Visibility);
        }

        [Test]
        public void Startup_ProfilePanelIsVisibleIfActivated()
        {
            _userProfileSettings.IsProvidingProfile = true;
            var sut = Open();
            Assert.AreEqual(Visibility.Visible, sut.ProfilePanel.Visibility);
        }

        [Test]
        public void Startup_ProfilePanelIsNotVisibleAndCheckboxisDisabledForDatev()
        {
            _exportSettings.IsDatev = true;
            var sut = Open();

            UserControlAssert.IsNotVisible(sut.ProfilePanel);
            UserControlAssert.IsVisible(sut.DatevLabel);
            UserControlAssert.IsDisabled(sut.IsProvidingProfileCheckBox);
        }

        [Test]
        public void ProfilePanelGetsVisibleOnActivation()
        {
            var sut = Open();
            sut.IsProvidingProfileCheckBox.Toggle();
            UserControlAssert.IsVisible(sut.ProfilePanel);
        }

        [Test]
        public void Binding_IsProviding()
        {
            var sut = Open();

            sut.IsProvidingProfileCheckBox.Toggle();
            Assert.True(_userProfileSettings.IsProvidingProfile);

            _dataContext.IsProvidingProfile = false;
            UserControlAssert.IsNotChecked(sut.IsProvidingProfileCheckBox);
            UserControlAssert.IsNotVisible(sut.ProfilePanel);
        }

        [Test]
        public void Binding_ProfileId()
        {
            var sut = Open();

            sut.ProfileIdTextBox.Text = "p";
            Assert.AreEqual("p", _userProfileSettings.ProfileId);

            _dataContext.ProfileId = "q";
            Assert.AreEqual("q", sut.ProfileIdTextBox.Text);
        }

        [Test]
        public void Binding_ProfileIdCanBeGeneratedOnClick()
        {
            var sut = Open();
            UserControlTestUtils.Click(sut.RefreshProfileIdButton);

            var expected = _rndGuid.ToString();
            Assert.AreEqual(expected, sut.ProfileIdTextBox.Text);
            Assert.AreEqual(expected, _userProfileSettings.ProfileId);
        }

        // TODO add binding tests for the remaining properties
    }
}