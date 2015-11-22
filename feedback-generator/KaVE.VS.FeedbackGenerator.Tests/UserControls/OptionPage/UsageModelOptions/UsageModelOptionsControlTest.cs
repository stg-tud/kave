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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using JetBrains.UI.Extensions.Commands;
using KaVE.Commons.TestUtils.UserControls;
using KaVE.Commons.Utils.Reflection;
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

        private IUsageModelOptionsViewModel _viewModel;
        private ModelStoreSettings _testSettings;

        #region command mocks

        private readonly ICommand<IUsageModelsTableRow> _installModelCommand = Mock.Of<ICommand<IUsageModelsTableRow>>();
        private readonly ICommand<IUsageModelsTableRow> _updateModelCommand = Mock.Of<ICommand<IUsageModelsTableRow>>();
        private readonly ICommand<IUsageModelsTableRow> _removeModelCommand = Mock.Of<ICommand<IUsageModelsTableRow>>();
        private readonly ICommand<object> _installAllModelsCommand = Mock.Of<ICommand<object>>();
        private readonly ICommand<object> _updateAllModelsCommand = Mock.Of<ICommand<object>>();
        private readonly ICommand<object> _removeAllModelsCommand = Mock.Of<ICommand<object>>();
        private readonly ICommand _cancelCurrentCommand = Mock.Of<ICommand>();

        #endregion

        [SetUp]
        public void Setup()
        {
            _proposalItemProviderMock = new Mock<IPBNProposalItemsProvider>();
            Registry.RegisterComponent(_proposalItemProviderMock.Object);

            _testSettings = new ModelStoreSettings();
            MockSettingsStore.Setup(settingsStore => settingsStore.GetSettings<ModelStoreSettings>())
                             .Returns(_testSettings);

            _viewModel = Mock.Of<IUsageModelOptionsViewModel>();
            Mock.Get(_viewModel)
                .Setup(viewModel => viewModel.UsageModelsTableContent)
                .Returns(
                    new List<IUsageModelsTableRow>
                    {
                        Mock.Of<IUsageModelsTableRow>(),
                        Mock.Of<IUsageModelsTableRow>(),
                        Mock.Of<IUsageModelsTableRow>()
                    });
            Mock.Get(_viewModel).Setup(viewModel => viewModel.InstallModel).Returns(_installModelCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.UpdateModel).Returns(_updateModelCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.RemoveModel).Returns(_removeModelCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.InstallAllModelsCommand).Returns(_installAllModelsCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.UpdateAllModelsCommand).Returns(_updateAllModelsCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.RemoveAllModelsCommand).Returns(_removeAllModelsCommand);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.CancelCurrentCommand).Returns(_cancelCurrentCommand);

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
                        _viewModel));
        }

        [Test]
        public void IsUsingModelStoreSettingsResetType()
        {
            Assert.AreEqual(ResetTypes.ModelStoreSettings, UsageModelOptionsControl.ResetType);
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
        public void ShouldClearModelsOnReloadModels()
        {
            UserControlTestUtils.Click(_sut.ReloadModelsButton);
            Thread.Sleep(100);

            _proposalItemProviderMock.Verify(provider => provider.Clear(), Times.Once);
        }

        [Test]
        public void Binding_UsageModelsTable_ItemsSource()
        {
            Assert.AreSame(_viewModel.UsageModelsTableContent, _sut.UsageModelsTable.ItemsSource);
        }

        [Test]
        public void Binding_InstallSelectedModelButton_Command()
        {
            Assert.AreSame(_viewModel.InstallModel, _sut.InstallSelectedModelButton.Command);
        }

        [Test]
        public void Binding_UpdateSelectedModelButton_Command()
        {
            Assert.AreSame(_viewModel.UpdateModel, _sut.UpdateSelectedModelButton.Command);
        }

        [Test]
        public void Binding_RemoveSelectedModelButton_Command()
        {
            Assert.AreSame(_viewModel.RemoveModel, _sut.RemoveSelectedModelButton.Command);
        }

        [Test]
        public void Binding_InstallAllModelsButton()
        {
            Assert.AreSame(_viewModel.InstallAllModelsCommand, _sut.InstallAllModelsButton.Command);
        }

        [Test]
        public void Binding_UpdateAllModelsButton()
        {
            Assert.AreSame(_viewModel.UpdateAllModelsCommand, _sut.UpdateAllModelsButton.Command);
        }

        [Test]
        public void Binding_RemoveAllModelsButton()
        {
            Assert.AreSame(_viewModel.RemoveAllModelsCommand, _sut.RemoveAllModelsButton.Command);
        }

        [Test]
        public void Binding_CancelModelsAction()
        {
            Assert.AreSame(_viewModel.CancelCurrentCommand, _sut.CancelModelsAction.Command);
        }

        [Test]
        public void Binding_InstallSelectedModelButton_CommandParameter()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;
            Assert.AreSame(_sut.UsageModelsTable.SelectedItem, _sut.InstallSelectedModelButton.CommandParameter);
        }

        [Test]
        public void Binding_UpdateSelectedModelButton_CommandParameter()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;
            Assert.AreSame(_sut.UsageModelsTable.SelectedItem, _sut.UpdateSelectedModelButton.CommandParameter);
        }

        [Test]
        public void Binding_RemoveSelectedModelButton_CommandParameter()
        {
            _sut.UsageModelsTable.SelectedIndex = 1;
            Assert.AreSame(_sut.UsageModelsTable.SelectedItem, _sut.RemoveSelectedModelButton.CommandParameter);
        }

        [Test]
        public void Binding_UsageModelsTableBusyOverlay_Visibility()
        {
            SetCommandIsRunning(true);
            Assert.AreEqual(Visibility.Visible, _sut.UsageModelsTableBusyOverlay.Visibility);
            SetCommandIsRunning(false);
            Assert.AreEqual(Visibility.Collapsed, _sut.UsageModelsTableBusyOverlay.Visibility);
        }

        [Test]
        public void Binding_ModelManagementCommandButtons_Visibility()
        {
            SetCommandIsRunning(false);
            Assert.AreEqual(Visibility.Visible, _sut.ModelManagementCommandButtons.Visibility);
            SetCommandIsRunning(true);
            Assert.AreEqual(Visibility.Collapsed, _sut.ModelManagementCommandButtons.Visibility);
        }

        [Test]
        public void Binding_ModelCommandsProgressBar_Maximum()
        {
            Mock.Get(_viewModel).Setup(viewModel => viewModel.MaximumUsageModelCommandProgressValue).Returns(259);
            RaisePropertyChanged(viewModel => viewModel.MaximumUsageModelCommandProgressValue);
            Assert.AreEqual(_viewModel.MaximumUsageModelCommandProgressValue, _sut.ModelCommandsProgressBar.Maximum);
        }

        [Test]
        public void Binding_ModelCommandsProgressBar_Value()
        {
            Mock.Get(_viewModel).Setup(viewModel => viewModel.MaximumUsageModelCommandProgressValue).Returns(259);
            RaisePropertyChanged(viewModel => viewModel.MaximumUsageModelCommandProgressValue);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.CurrentUsageModelCommandProgressValue).Returns(104);
            RaisePropertyChanged(viewModel => viewModel.CurrentUsageModelCommandProgressValue);
            Assert.AreEqual(_viewModel.CurrentUsageModelCommandProgressValue, _sut.ModelCommandsProgressBar.Value);
        }

        [Test]
        public void Binding_ModelCommandsProgressBarText()
        {
            Mock.Get(_viewModel).Setup(viewModel => viewModel.RunningCommandMessage).Returns("some message");
            RaisePropertyChanged(viewModel => viewModel.RunningCommandMessage);
            Assert.AreEqual(_viewModel.RunningCommandMessage, _sut.RunningCommandsProgressBarTextBlock.Text);
        }

        [Test]
        public void ShouldSetModelStoreSettingsOnOk()
        {
            // TODO mock verification to make sure the validation is successful for these settings
            const string testUri = "http://www.kave.cc/";
            _sut.ModelStoreUriTextBox.Text = testUri;

            _sut.OnOk();

            Assert.AreEqual(_testSettings.ModelStoreUri, testUri);
            MockSettingsStore.Verify(store => store.SetSettings(_testSettings));
        }

        [Test]
        public void ShouldNotSetInvalidSettingsOnOk()
        {
            // TODO mock verification to make sure the validation is not successful for these settings
            _sut.ModelStorePathTextBox.Text = "invalid path";

            _sut.OnOk();

            Assert.AreEqual(_testSettings.ModelStorePath, new ModelStoreSettings().ModelStorePath);
            MockSettingsStore.Verify(store => store.SetSettings(_testSettings));
        }

        [Test]
        public void ShouldNotCloseWithInvalidChangesIfUserAnswersNo()
        {
            _sut.ModelStorePathTextBox.Text = "invalid path";

            MockMessageBoxCreator.Setup(box => box.ShowYesNo(It.IsAny<string>())).Returns(false);

            Assert.IsFalse(_sut.OnOk());
        }

        [Test]
        public void ShouldCloseWithInvalidChangesIfUserAnswersYes()
        {
            _sut.ModelStorePathTextBox.Text = "invalid path";

            MockMessageBoxCreator.Setup(box => box.ShowYesNo(It.IsAny<string>())).Returns(true);

            Assert.IsTrue(_sut.OnOk());
        }

        private void SetCommandIsRunning(bool value)
        {
            Mock.Get(_viewModel).Setup(viewModel => viewModel.RunningUsageModelCommand).Returns(value);
            Mock.Get(_viewModel).Setup(viewModel => viewModel.RunnerIsIdle).Returns(!value);
            RaisePropertyChanged(viewModel => viewModel.RunningUsageModelCommand);
            RaisePropertyChanged(viewModel => viewModel.RunnerIsIdle);
        }

        protected void RaisePropertyChanged<TProperty>(
            Expression<Func<IUsageModelOptionsViewModel, TProperty>> expression)
        {
            var propertyName = TypeExtensions<IUsageModelOptionsViewModel>.GetPropertyName(expression);
            Mock.Get(_viewModel)
                .Raise(
                    viewModel => viewModel.PropertyChanged += null,
                    new PropertyChangedEventArgs(propertyName));
        }
    }
}