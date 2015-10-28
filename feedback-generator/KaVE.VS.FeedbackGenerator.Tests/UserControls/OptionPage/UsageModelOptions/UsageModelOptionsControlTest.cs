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
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
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
        private Mock<IRemotePBNRecommenderStore> _remoteStoreMock;
        private Mock<IPBNRecommenderStore> _localStoreMock;

        private UsageModelOptionsViewModel ViewModel
        {
            get { return (UsageModelOptionsViewModel) _sut.DataContext; }
        }

        [SetUp]
        public void Setup()
        {
            _proposalItemProviderMock = new Mock<IPBNProposalItemsProvider>();
            Registry.RegisterComponent(_proposalItemProviderMock.Object);

            _localStoreMock = new Mock<IPBNRecommenderStore>();
            Registry.RegisterComponent(_localStoreMock.Object);

            _remoteStoreMock = new Mock<IRemotePBNRecommenderStore>();
            Registry.RegisterComponent(_remoteStoreMock.Object);

            MockSettingsStore.Setup(settingsStore => settingsStore.GetSettings<ModelStoreSettings>())
                             .Returns(new ModelStoreSettings());

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

        [Test]
        public void ShouldLoadAllModelsOnUpdateAllModels()
        {
            UserControlTestUtils.Click(_sut.UpdateModelsButton);

            _remoteStoreMock.Verify(remoteStore => remoteStore.LoadAll(), Times.Once);
        }

        [Test]
        public void ShouldRemoveAllModelsOnRemoveAllModels()
        {
            UserControlTestUtils.Click(_sut.RemoveModelsButton);

            _localStoreMock.Verify(localStore => localStore.RemoveAll(), Times.Once);
        }

        [Test]
        public void Binding_UsageModelsTableItemsSource()
        {
            Assert.AreSame(ViewModel.UsageModelsTableContent, _sut.UsageModelsTable.ItemsSource);
            ViewModel.UsageModelsTableContent = new UsageModelsTableRow[0];
            Assert.AreSame(ViewModel.UsageModelsTableContent, _sut.UsageModelsTable.ItemsSource);
        }
    }
}