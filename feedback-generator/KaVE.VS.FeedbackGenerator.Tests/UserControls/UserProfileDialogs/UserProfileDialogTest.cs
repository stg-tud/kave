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

using KaVE.Commons.TestUtils.UserControls;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Menu;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfileDialogs;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfileDialogs
{
    [RequiresSTA]
    internal class UserProfileDialogTest : BaseUserControlTest
    {
        private UserProfileDialog _sut;
        private UserProfileSettings _settings;
        private IUserProfileSettingsUtils _util;
        private IActionExecutor _actionExecutor;
        private UploadWizardPolicy _uploadWizardPolicy;

        [SetUp]
        public void SetUp()
        {
            _settings = new UserProfileSettings();
            _util = Mock.Of<IUserProfileSettingsUtils>();
            Mock.Get(_util).Setup(u => u.GetSettings()).Returns(_settings);

            _actionExecutor = Mock.Of<IActionExecutor>();
            _uploadWizardPolicy = UploadWizardPolicy.OpenUploadWizardOnFinish;
        }

        private UserProfileDialog Open()
        {
            _sut = new UserProfileDialog(
                _actionExecutor,
                _uploadWizardPolicy,
                _util);
            _sut.Show();
            return _sut;
        }

        [Test]
        public void DataContextIsSetCorrectly()
        {
            Open();
            Assert.IsInstanceOf<UserProfileContext>(_sut.DataContext);
        }

        [Test]
        public void NothingIsSavedOnClose()
        {
            _sut = Open();
            Mock.Get(_util).Verify(u => u.GetSettings());
            _sut.Close();
            Mock.Get(_util).Verify(u => u.StoreSettings(It.IsAny<UserProfileSettings>()), Times.Never);
        }

        [Test]
        public void NothingIsSavedOnAbort()
        {
            _sut = Open();
            Mock.Get(_util).Verify(u => u.GetSettings());
            UserControlTestUtils.Click(_sut.ButtonAbort);
            Mock.Get(_util).Verify(u => u.StoreSettings(It.IsAny<UserProfileSettings>()), Times.Never);
        }

        [Test]
        public void ProfileIsSavedOnFinish()
        {
            _sut = Open();
            _sut.UserSettingsGrid.ProjectsCoursesCheckBox.Toggle();
            UserControlTestUtils.Click(_sut.ButtonOk);

            Assert.True(_settings.ProjectsCourses);
            Mock.Get(_util).Verify(u => u.StoreSettings(_settings), Times.Once);
        }

        [Test]
        public void UploadWizardIsOpenedWhenRequested()
        {
            _uploadWizardPolicy = UploadWizardPolicy.OpenUploadWizardOnFinish;
            _sut = Open();
            UserControlTestUtils.Click(_sut.ButtonOk);
            Mock.Get(_actionExecutor).Verify(a => a.ExecuteActionGuarded<UploadWizardAction>(), Times.Once);
        }

        [Test]
        public void UploadWizardIsNotOpenedWhenNotRequested()
        {
            _uploadWizardPolicy = UploadWizardPolicy.DoNotOpenUploadWizardOnFinish;
            _sut = Open();
            UserControlTestUtils.Click(_sut.ButtonOk);
            Mock.Get(_actionExecutor).Verify(a => a.ExecuteActionGuarded<UploadWizardAction>(), Times.Never);
        }
    }
}