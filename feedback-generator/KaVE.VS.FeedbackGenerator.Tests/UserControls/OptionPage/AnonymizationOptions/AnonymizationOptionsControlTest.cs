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
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.AnonymizationOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.AnonymizationOptions
{
    [RequiresSTA]
    internal class AnonymizationOptionsControlTest : BaseOptionPageUserControlTest
    {
        private AnonymizationOptionsControl _sut;

        private AnonymizationOptionsControl Open()
        {
            return
                OpenWindow(
                    new AnonymizationOptionsControl(
                        TestLifetime,
                        TestOptionsSettingsSmartContext,
                        MockSettingsStore.Object,
                        MockActionExecutor.Object,
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
            Assert.IsInstanceOf<AnonymizationContext>(_sut.DataContext);
        }

        [Test]
        public void ShouldExecuteResetOnClick()
        {
            SetConfirmationAnswerTo(true);

            Click(_sut.ResetButton);

            VerifyActionExecuted(Times.Once);
        }

        [Test]
        public void ShouldNotResetOnAbort()
        {
            SetConfirmationAnswerTo(false);

            Click(_sut.ResetButton);

            VerifyActionExecuted(Times.Never);
        }

        [Test]
        public void IsUsingAnonymizationResetType()
        {
            Assert.AreEqual(ResetTypes.AnonymizationSettings, AnonymizationOptionsControl.ResetType);
        }
    }
}