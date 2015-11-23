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
using System.Text;
using System.Windows;
using Avalon.Windows.Dialogs;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.ValidationRules;
using KaVE.VS.FeedbackGenerator.Utils;
using IKaVESettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    [OptionsPage(PID, "Usage Models", typeof (OptionsThemedIcons.ImportLayer),
        ParentId = RootOptionPage.PID, Sequence = 4.0)]
    public partial class UsageModelOptionsControl : IOptionsPage
    {
        private const string PID =
            "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions.UsageModelOptionsControl";

        public const ResetTypes ResetType = ResetTypes.ModelStoreSettings;

        private readonly Lifetime _lifetime;
        private readonly IKaVESettingsStore _settingsStore;
        private readonly IActionExecutor _actionExecutor;
        private readonly DataContexts _dataContexts;
        private readonly ModelStoreSettings _modelStoreSettings;

        private readonly IMessageBoxCreator _messageBoxCreator;

        public UsageModelOptionsControl([NotNull] Lifetime lifetime,
            // ReSharper disable once SuggestBaseTypeForParameter
            [CanBeNull] OptionsSettingsSmartContext jetBrainsContext,
            [NotNull] IKaVESettingsStore settingsStore,
            [NotNull] IActionExecutor actionExecutor,
            [NotNull] DataContexts dataContexts,
            [NotNull] IMessageBoxCreator messageBoxCreator,
            [NotNull] IUsageModelOptionsViewModel usageModelOptionsViewModel)
        {
            _messageBoxCreator = messageBoxCreator;
            _lifetime = lifetime;
            _actionExecutor = actionExecutor;
            _settingsStore = settingsStore;
            _dataContexts = dataContexts;
            DataContext = usageModelOptionsViewModel;

            InitializeComponent();

            _modelStoreSettings = settingsStore.GetSettings<ModelStoreSettings>();
            ModelStorePathTextBox.Text = _modelStoreSettings.ModelStorePath;
            ModelStoreUriTextBox.Text = _modelStoreSettings.ModelStoreUri;

            if (jetBrainsContext != null)
            {
                jetBrainsContext.SetBinding(
                    lifetime,
                    (ModelStoreSettings s) => s.ModelStorePath,
                    ModelStorePathTextBox,
                    TextBox.TextProperty);

                jetBrainsContext.SetBinding(
                    lifetime,
                    (ModelStoreSettings s) => s.ModelStoreUri,
                    ModelStoreUriTextBox,
                    TextBox.TextProperty);
            }
        }

        private void OnBrowse_Path(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModelStorePathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void OnBrowse_Uri(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ModelStoreUriTextBox.Text = dialog.SelectedPath;
            }
        }

        private void OnResetSettings(object sender, RoutedEventArgs e)
        {
            var result = _messageBoxCreator.ShowYesNo(UsageModelOptionsMessages.SettingResetDialog);
            if (result)
            {
                var settingResetType = new SettingResetType {ResetType = ResetType};
                _actionExecutor.ExecuteActionGuarded<SettingsCleaner>(
                    settingResetType.GetDataContextForSettingResultType(_dataContexts, _lifetime));

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        private void OnReloadModels(object sender, RoutedEventArgs e)
        {
            try
            {
                Registry.GetComponent<IPBNProposalItemsProvider>().Clear();
            }
            catch (InvalidOperationException) {}
        }

        public bool OnOk()
        {
            var errorMessage = new StringBuilder();

            var pathValidationResult = UsageModelsPathValidationRule.Validate(ModelStorePathTextBox.Text);
            if (pathValidationResult.IsValid)
            {
                _modelStoreSettings.ModelStorePath = ModelStorePathTextBox.Text;
            }
            else
            {
                errorMessage.AppendLine(
                    ValidationErrorMessage(
                        Properties.SessionManager.Options_ModelPath,
                        pathValidationResult.ErrorContent));
            }

            var uriValidationResult = UsageModelsUriValidationRule.Validate(ModelStoreUriTextBox.Text);
            if (uriValidationResult.IsValid)
            {
                _modelStoreSettings.ModelStoreUri = ModelStoreUriTextBox.Text;
            }
            else
            {
                errorMessage.AppendLine(
                    ValidationErrorMessage(Properties.SessionManager.Options_ModelUri, uriValidationResult.ErrorContent));
            }

            _settingsStore.SetSettings(_modelStoreSettings);

            if (!pathValidationResult.IsValid || !uriValidationResult.IsValid)
            {
                _messageBoxCreator.ShowError(errorMessage.ToString());
                return false;
            }

            return true;
        }

        public bool ValidatePage()
        {
            return true;
        }

        public EitherControl Control
        {
            get { return this; }
        }

        public string Id
        {
            get { return PID; }
        }

        private static string ValidationErrorMessage(string invalidItemName, object errorContent)
        {
            return invalidItemName + ": " + errorContent;
        }
    }
}