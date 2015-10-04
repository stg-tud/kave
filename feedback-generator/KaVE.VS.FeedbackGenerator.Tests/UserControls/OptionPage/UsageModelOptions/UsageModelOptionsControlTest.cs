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
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.OptionPage.UsageModelOptions
{
    [RequiresSTA]
    internal class UsageModelOptionsControlTest : BaseOptionPageUserControlTest
    {
        private UsageModelOptionsControl _sut;
        private Mock<IPBNProposalItemsProvider> _proposalItemProviderMock;

        [SetUp]
        public void Setup()
        {
            _proposalItemProviderMock = new Mock<IPBNProposalItemsProvider>();
            Registry.RegisterComponent(_proposalItemProviderMock.Object);

            _sut = Open();
        }

        [TearDown]
        public void ClearRegistry()
        {
            Registry.Clear();
        }

        private UsageModelOptionsControl Open()
        {
            return
                OpenWindow(
                    new UsageModelOptionsControl(
                        TestLifetime,
                        TestOptionsSettingsSmartContext,
                        MockSettingsStore.Object,
                        MockActionExecutor.Object,
                        TestDataContexts,
                        MockMessageBoxCreator.Object
                        ));
        }

        [Test]
        public void DataContextIsSetCorrectly()
        {
            Assert.IsInstanceOf<UsageModelOptionsViewModel>(_sut.DataContext);
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
        public void IsUsingModelStoreSettingsResetType()
        {
            Assert.AreEqual(ResetTypes.ModelStoreSettings, UsageModelOptionsControl.ResetType);
        }

        [Test]
        public void ShouldClearModelsOnReloadModels()
        {
            UserControlTestUtils.Click(_sut.ReloadModelsButton);

            _proposalItemProviderMock.Verify(provider => provider.Clear(), Times.Once);
        }
    }
}