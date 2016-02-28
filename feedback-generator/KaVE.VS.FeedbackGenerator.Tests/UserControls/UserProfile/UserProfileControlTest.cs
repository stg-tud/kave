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
using KaVE.Commons.TestUtils.UserControls;
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    [RequiresSTA]
    internal class UserProfileControlTest : BaseUserControlTest
    {
        private UserProfileSettings _userProfileSettings;
        private IUserProfileSettingsUtils _userProfileSettingsUtil;
        private UserProfileContext _dataContext;
        private IRandomizationUtils _randomizationUtils;

        private Guid _rndGuid;

        [SetUp]
        public void SetUp()
        {
            _userProfileSettings = new UserProfileSettings();
            _userProfileSettingsUtil = Mock.Of<IUserProfileSettingsUtils>();
            _randomizationUtils = Mock.Of<IRandomizationUtils>();
            _rndGuid = Guid.NewGuid();
            Mock.Get(_randomizationUtils).Setup(r => r.GetRandomGuid()).Returns(_rndGuid);
            _dataContext = new UserProfileContext(_userProfileSettings, _userProfileSettingsUtil);
        }

        private UserProfileControl Open()
        {
            return OpenWindow(new UserProfileControl {DataContext = _dataContext});
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