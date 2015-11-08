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
using System.Linq;
using System.Threading;
using KaVE.Commons.TestUtils.UserControls;
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

        private IEnumerable<IUsageModelsTableRow> _testRows;

        [SetUp]
        public void Setup()
        {
            _testRows = new[]
            {
                GenerateMockedRow(true, true, true),
                GenerateMockedRow(true, true, true),
                GenerateMockedRow(true, true, true)
            };

            _proposalItemProviderMock = new Mock<IPBNProposalItemsProvider>();
            Registry.RegisterComponent(_proposalItemProviderMock.Object);

            _localStoreMock = new Mock<ILocalPBNRecommenderStore>();
            Registry.RegisterComponent(_localStoreMock.Object);

            _remoteStoreMock = new Mock<IRemotePBNRecommenderStore>();
            Registry.RegisterComponent(_remoteStoreMock.Object);

            _mergingStrategyMock = new Mock<IUsageModelMergingStrategy>();
            _mergingStrategyMock.Setup(
                merging => merging.MergeAvailableModels(_localStoreMock.Object, _remoteStoreMock.Object))
                                .Returns(() => _testRows);

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
        public void InstallSelectedModelOnInstallClick()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;

            UserControlTestUtils.Execute(_sut.InstallSelectedModelButton);
            Thread.Sleep(100);

            var selectedRow = Mock.Get((IUsageModelsTableRow)_sut.UsageModelsTable.SelectedItem);
            selectedRow.Verify(row => row.LoadModel(), Times.Once);
        }

        [Test]
        public void UpdateSelectedModelOnUpdateClick()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;

            UserControlTestUtils.Execute(_sut.UpdateSelectedModelButton);
            Thread.Sleep(100);

            var selectedRow = Mock.Get((IUsageModelsTableRow)_sut.UsageModelsTable.SelectedItem);
            selectedRow.Verify(row => row.LoadModel(), Times.Once);
        }

        [Test]
        public void RemoveSelectedModelOnRemoveClick()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;

            UserControlTestUtils.Execute(_sut.RemoveSelectedModelButton);
            Thread.Sleep(100);

            var selectedRow = Mock.Get((IUsageModelsTableRow)_sut.UsageModelsTable.SelectedItem);
            selectedRow.Verify(row => row.RemoveModel(), Times.Once);
        }

        [Test]
        public void ShouldClearModelsOnReloadModels()
        {
            UserControlTestUtils.Click(_sut.ReloadModelsButton);
            Thread.Sleep(100);

            _proposalItemProviderMock.Verify(provider => provider.Clear(), Times.Once);
        }

        [Test]
        public void ShouldLoadAllUpdateableModelsOnUpdateAllModels()
        {
            _testRows = new[]
            {
                GenerateMockedRow(false, true, false),
                GenerateMockedRow(false, true, false),
                GenerateMockedRow(false, true, false)
            };
            _sut = Open();

            UserControlTestUtils.Execute(_sut.UpdateModelsButton);
            Thread.Sleep(250);

            foreach (var row in _sut.UsageModelsTable.Items.Cast<IUsageModelsTableRow>())
            {
                Mock.Get(row).Verify(r => r.IsUpdateable);
                Mock.Get(row).Verify(r => r.LoadModel(), Times.Once);
            }
        }

        [Test]
        public void ShouldRemoveAllModelsOnRemoveAllModels()
        {
            _testRows = new[]
            {
                GenerateMockedRow(false, false, true),
                GenerateMockedRow(false, false, true),
                GenerateMockedRow(false, false, true)
            };
            _sut = Open();

            UserControlTestUtils.Execute(_sut.RemoveModelsButton);
            Thread.Sleep(250);

            foreach (var row in _sut.UsageModelsTable.Items.Cast<IUsageModelsTableRow>())
            {
                Mock.Get(row).Verify(r => r.IsRemoveable);
                Mock.Get(row).Verify(r => r.RemoveModel(), Times.Once);
            }
        }

        [Test]
        public void ButtonsShouldBeDisabledWhenNothingIsSelected()
        {
            Assert.IsNull(_sut.UsageModelsTable.SelectedItem);
            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.InstallSelectedModelButton));
            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.UpdateSelectedModelButton));
            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.RemoveSelectedModelButton));
        }

        [Test]
        public void InstallSelectedShouldBeEnabledIfSelectedIsInstallable()
        {
            _testRows = new[] {GenerateMockedRow(true, false, false)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsTrue(UserControlTestUtils.CanExecute(_sut.InstallSelectedModelButton));
        }

        [Test]
        public void InstallSelectedShouldBeDisabledIfSelectedIsNotInstallable()
        {
            _testRows = new[] {GenerateMockedRow(false, true, true)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.InstallSelectedModelButton));
        }

        [Test]
        public void UpdateSelectedShouldBeEnabledIfSelectedIsUpdateable()
        {
            _testRows = new[] {GenerateMockedRow(false, true, false)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsTrue(UserControlTestUtils.CanExecute(_sut.UpdateSelectedModelButton));
        }

        [Test]
        public void UpdateSelectedShouldBeDisabledIfSelectedIsNotUpdateable()
        {
            _testRows = new[] {GenerateMockedRow(true, false, true)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.UpdateSelectedModelButton));
        }

        [Test]
        public void RemoveSelectedShouldBeEnabledIfSelectedIsRemoveable()
        {
            _testRows = new[] {GenerateMockedRow(false, false, true)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsTrue(UserControlTestUtils.CanExecute(_sut.RemoveSelectedModelButton));
        }

        [Test]
        public void RemoveSelectedShouldBeDisabledIfSelectedIsNotRemoveable()
        {
            _testRows = new[] {GenerateMockedRow(true, true, false)};
            _sut = Open();
            _sut.UsageModelsTable.SelectedIndex = 0;

            Assert.IsFalse(UserControlTestUtils.CanExecute(_sut.RemoveSelectedModelButton));
        }

        [Test]
        public void Binding_UsageModelsTableItemsSource()
        {
            Assert.AreSame(ViewModel.UsageModelsTableContent, _sut.UsageModelsTable.ItemsSource);
        }

        private static IUsageModelsTableRow GenerateMockedRow(bool isInstallable, bool isUpdateable, bool isRemoveable)
        {
            var mockedRow = new Mock<IUsageModelsTableRow>();
            mockedRow.Setup(row => row.IsInstallable).Returns(isInstallable);
            mockedRow.Setup(row => row.IsUpdateable).Returns(isUpdateable);
            mockedRow.Setup(row => row.IsRemoveable).Returns(isRemoveable);
            return mockedRow.Object;
        }
    }
}