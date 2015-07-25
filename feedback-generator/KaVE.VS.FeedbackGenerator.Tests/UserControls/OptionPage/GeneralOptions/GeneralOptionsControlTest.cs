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

using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.GeneralOptions
{
    [RequiresSTA]
    internal class GeneralOptionsControlTest : BaseOptionPageUserControlTest
    {
        private GeneralOptionsControl _sut;

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

            Click(_sut.ResetGeneralSettingsButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotResetGeneralSettingsOnAbort()
        {
            SetConfirmationAnswerTo(false);

            Click(_sut.ResetGeneralSettingsButton);

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

            Click(_sut.ResetFeedbackSettingsButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotResetFeedbackSettingsOnAbort()
        {
            SetConfirmationAnswerTo(false);

            Click(_sut.ResetFeedbackSettingsButton);

            VerifyActionExecuted(Times.Never);
        }

        [Test]
        public void IsUsingCorrectResetTypeForFeedbackSettings()
        {
            Assert.AreEqual(ResetTypes.Feedback, GeneralOptionsControl.FeedbackResetType);
        }
    }
}