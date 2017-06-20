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
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UserProfileOptions;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UserProfileOptions
{
    [RequiresSTA]
    internal class UserProfileOptionsControlTest : BaseOptionPageUserControlTest
    {
        private IUserProfileSettingsUtils upUtil;
        private UserProfileOptionsControl _sut;

        [SetUp]
        public void Setup()
        {
            upUtil = Mock.Of<IUserProfileSettingsUtils>();
            MockSettingsStore.Setup(settingStore => settingStore.GetSettings<ExportSettings>())
                              .Returns(new ExportSettings());
            MockSettingsStore.Setup(settingStore => settingStore.GetSettings<UserProfileSettings>())
                              .Returns(new UserProfileSettings());
            
            _sut = Open();
        }

        private UserProfileOptionsControl Open()
        {
            return
                OpenWindow(
                    new UserProfileOptionsControl(
                        TestLifetime,
                        TestOptionsSettingsSmartContext,
                        MockSettingsStore.Object,
                        MockActionExecutor.Object,
                        TestDataContexts,
                        MockMessageBoxCreator.Object,
                        upUtil));
        }

        [Test]
        public void DataContextIsSetCorrectly()
        {
            Assert.IsInstanceOf<UserProfileContext>(_sut.DataContext);
        }

        [Test]
        public void ShouldExecuteActionOnResetClick()
        {
            SetConfirmationAnswerTo(true);

            UserControlTestUtils.Click(_sut.ResetButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotExecuteActionOnAbort()
        {
            SetConfirmationAnswerTo(false);

            UserControlTestUtils.Click(_sut.ResetButton);

            VerifyActionExecuted(Times.Never);
        }

        [Test]
        public void IsUsingUserProfileSettingsResetType()
        {
            Assert.AreEqual(ResetTypes.UserProfileSettings, UserProfileOptionsControl.ResetType);
        }
    }
}