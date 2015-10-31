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

using System.Collections.Generic;
using System.Windows.Input;
using KaVE.Commons.TestUtils.UserControls;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.CodeCompletion.Stores;
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
        private Mock<ILocalPBNRecommenderStore> _localStoreMock;
        private Mock<IUsageModelMergingStrategy> _mergingStrategyMock;

        private UsageModelOptionsViewModel ViewModel
        {
            get { return (UsageModelOptionsViewModel) _sut.DataContext; }
        }

        private static List<IUsageModelsTableRow> TestRows
        {
            get
            {
                return new List<IUsageModelsTableRow>
                {
                    GenerateMockedRow(true, true, true),
                    GenerateMockedRow(true, true, true),
                    GenerateMockedRow(true, true, true)
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            _proposalItemProviderMock = new Mock<IPBNProposalItemsProvider>();
            Registry.RegisterComponent(_proposalItemProviderMock.Object);

            _localStoreMock = new Mock<ILocalPBNRecommenderStore>();
            Registry.RegisterComponent(_localStoreMock.Object);

            _remoteStoreMock = new Mock<IRemotePBNRecommenderStore>();
            Registry.RegisterComponent(_remoteStoreMock.Object);

            _mergingStrategyMock = new Mock<IUsageModelMergingStrategy>();
            _mergingStrategyMock.Setup(
                merging => merging.MergeAvailableModels(_localStoreMock.Object, _remoteStoreMock.Object))
                                .Returns(TestRows);

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
                        MockMessageBoxCreator.Object,
                        _mergingStrategyMock.Object
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
        }

        private static IUsageModelsTableRow GenerateMockedRow(bool isInstallable, bool isUpdateable, bool isRemoveable)
        {
            var mockedRow = new Mock<IUsageModelsTableRow>();

            var installCommand = new Mock<ICommand>();
            installCommand.Setup(command => command.CanExecute(It.IsAny<object>())).Returns(isInstallable);
            var updateCommand = new Mock<ICommand>();
            updateCommand.Setup(command => command.CanExecute(It.IsAny<object>())).Returns(isUpdateable);
            var removeCommand = new Mock<ICommand>();
            removeCommand.Setup(command => command.CanExecute(It.IsAny<object>())).Returns(isRemoveable);

            mockedRow.Setup(row => row.InstallModel).Returns(installCommand.Object);
            mockedRow.Setup(row => row.UpdateModel).Returns(updateCommand.Object);
            mockedRow.Setup(row => row.RemoveModel).Returns(removeCommand.Object);

            return mockedRow.Object;
        }

        private static void VerifyExecuted(Mock<ICommand> commandMock)
        {
            commandMock.Verify(command => command.Execute(It.IsAny<object>()));
        }
    }
}