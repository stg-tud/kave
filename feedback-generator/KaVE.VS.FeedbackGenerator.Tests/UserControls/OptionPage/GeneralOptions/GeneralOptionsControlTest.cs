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
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.GeneralOptions
{
    [RequiresSTA]
    internal class GeneralOptionsControlTest : BaseOptionPageUserControlTest
    {
        private GeneralOptionsControl _sut;
        private ExportSettings _testSettings;

        private GeneralOptionsControl Open()
        {
            return
                OpenWindow(
                    new GeneralOptionsControl(
                        TestLifetime,
                        TestOptionsSettingsSmartContext,
                        MockActionExecutor.Object,
                        MockSettingsStore.Object,
                        TestDataContexts,
                        MockMessageBoxCreator.Object));
        }

        [SetUp]
        public void Setup()
        {
            _testSettings = new ExportSettings();
            MockSettingsStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(_testSettings);

            _sut = Open();
        }

        [Test]
        public void DataContextIsSetCorrectly()
        {
            Assert.IsInstanceOf<GeneralOptionsViewModel>(_sut.DataContext);
        }

        [Test]
        public void ShouldResetGeneralSettingsOnClick()
        {
            SetConfirmationAnswerTo(true);

            UserControlTestUtils.Click(_sut.ResetGeneralSettingsButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotResetGeneralSettingsOnAbort()
        {
            SetConfirmationAnswerTo(false);

            UserControlTestUtils.Click(_sut.ResetGeneralSettingsButton);

            VerifyActionExecuted(Times.Never);
        }

        [Test]
        public void IsUsingCorrectResetTypeForGeneralSettings()
        {
            Assert.AreEqual(ResetTypes.GeneralSettings, GeneralOptionsControl.GeneralSettingsResetType);
        }

        [Test]
        public void ShouldResetFeedbackSettingsOnClick()
        {
            SetConfirmationAnswerTo(true);

            UserControlTestUtils.Click(_sut.ResetFeedbackSettingsButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotResetFeedbackSettingsOnAbort()
        {
            SetConfirmationAnswerTo(false);

            UserControlTestUtils.Click(_sut.ResetFeedbackSettingsButton);

            VerifyActionExecuted(Times.Never);
        }

        [Test]
        public void IsUsingCorrectResetTypeForFeedbackSettings()
        {
            Assert.AreEqual(ResetTypes.Feedback, GeneralOptionsControl.FeedbackResetType);
        }

        [Test]
        public void ShouldSetSettingsOnOk()
        {
            const string newUrl = "http://www.kave.cc/";
            const string newPrefix = "http://www";
            _sut.UploadUrlTextBox.Text = newUrl;
            _sut.WebPraefixTextBox.Text = newPrefix;

            _sut.OnOk();

            Assert.AreEqual(newUrl, _testSettings.UploadUrl);
            Assert.AreEqual(newPrefix, _testSettings.WebAccessPrefix);
            MockSettingsStore.Verify(store => store.SetSettings(_testSettings), Times.Once);
        }

        [Test]
        public void ShouldNotSetSettingsOnInvalidValues()
        {
            const string invalidUrl = "invalid url";
            const string invalidPrefix = "invalid prefix";
            _sut.UploadUrlTextBox.Text = invalidUrl;
            _sut.WebPraefixTextBox.Text = invalidPrefix;

            _sut.OnOk();

            Assert.AreEqual(new ExportSettings().UploadUrl, _testSettings.UploadUrl);
            Assert.AreEqual(new ExportSettings().WebAccessPrefix, _testSettings.WebAccessPrefix);
        }

        [Test]
        public void ShouldNotCloseWithUnsavedChanges()
        {
            const string invalidUrl = "invalid url";
            const string invalidPrefix = "invalid prefix";
            _sut.UploadUrlTextBox.Text = invalidUrl;
            _sut.WebPraefixTextBox.Text = invalidPrefix;

            Assert.IsFalse(_sut.OnOk());
            MockMessageBoxCreator.Verify(messageBox => messageBox.ShowError(It.IsAny<string>()), Times.Once);
        }
    }
}