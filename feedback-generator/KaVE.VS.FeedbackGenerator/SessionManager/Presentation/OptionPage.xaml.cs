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
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using JetBrains.ActionManagement;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Navigation.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionPages.ToolsPages;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation.UserSetting;
using KaVE.VS.FeedbackGenerator.Utils;
using ISettingsStore = KaVE.VS.FeedbackGenerator.Utils.ISettingsStore;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    [OptionsPage(PID, "KaVE Feedback", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = ToolsPage.PID)]
    public partial class OptionPage : IOptionsPage
    {
        private readonly OptionPageViewModel _optionPageViewModel;
        private readonly Lifetime _lifetime;
        private readonly IActionManager _actionManager;
        private readonly UserSettingsViewModel _userSettingsViewModel;
        private const string PID = "FeedbackGenerator.OptionPage";

        public OptionPage(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            IActionManager actionManager,
            ISettingsStore settingsStore)
        {
            _optionPageViewModel = new OptionPageViewModel
            {
                ExportSettings = settingsStore.GetSettings<ExportSettings>()
            };
            _optionPageViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;

            DataContext = _optionPageViewModel;

            _lifetime = lifetime;
            _actionManager = actionManager;

            InitializeComponent();

            _userSettingsViewModel = (UserSettingsViewModel) UserSettingsGrid.DataContext;
            _userSettingsViewModel.UserSettings = settingsStore.GetSettings<UserSettings>();
            _userSettingsViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;

            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveCodeNames, RemoveCodeNamesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveStartTimes, RemoveStartTimesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveDurations, RemoveDurationsCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveSessionIDs, RemoveSessionUUIDCheckBox);
            ctx.SetBinding(lifetime, (ExportSettings s) => s.UploadUrl, UploadUrlTextBox, TextBox.TextProperty);
            ctx.SetBinding(lifetime, (ExportSettings s) => s.WebAccessPrefix, WebPraefixTextBox, TextBox.TextProperty);

            ctx.SetBinding(lifetime, (UserSettings s) => (bool?) s.ProvideUserInformation, UserSettingsGrid.ProvideUserInformationCheckBox, ToggleButton.IsCheckedProperty);
            ctx.SetBinding(lifetime, (UserSettings s) => s.Username, UserSettingsGrid.UsernameTextBox, TextBox.TextProperty);
            ctx.SetBinding(lifetime, (UserSettings s) => s.Mail, UserSettingsGrid.EmailTextBox, TextBox.TextProperty);
            ctx.SetBinding(lifetime, (UserSettings s) => s.NumberField, UserSettingsGrid.NumberTextBox, TextBox.TextProperty);
            ctx.SetBinding(lifetime, (UserSettings s) => s.Category, _userSettingsViewModel, UserSettingsViewModel.SelectedCategoryOptionProperty);
            ctx.SetBinding(lifetime, (UserSettings s) => s.Valuation, _userSettingsViewModel, UserSettingsViewModel.SelectedValuationOptionProperty);

            var exportSettings = settingsStore.GetSettings<ExportSettings>();
            if (exportSettings.IsDatev)
            {
                UserSettingsGrid.ProvideUserInformationCheckBox.IsEnabled = false;
                UserSettingsGrid.DatevDeactivationLabel.Visibility = Visibility.Visible;
            }

            UserSettingsGrid.CategoryComboBox.SetBinding(Selector.SelectedItemProperty, new Binding("SelectedCategoryOption"));
            UserSettingsGrid.RadioButtonListBox.SetBinding(Selector.SelectedItemProperty, new Binding("SelectedValuationOption"));
        }

        private static void SetToggleButtonBinding(IContextBoundSettingsStore ctx,
            Lifetime lifetime,
            Expression<Func<ExportSettings, bool?>> settingProperty,
            ToggleButton toggleButton)
        {
            ctx.SetBinding(lifetime, settingProperty, toggleButton, ToggleButton.IsCheckedProperty);
        }

        private void RestoreSettings_OnClick(object sender, RoutedEventArgs e)
        {
            var message = Properties.SessionManager.ResourceManager.GetString("Option_SettingsCleaner_Dialog");
            var result = MessageBox.ShowYesNo(message);

            if (result)
            {
                _actionManager.ExecuteActionGuarded<SettingsCleaner>(_lifetime);
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            Asserts.NotNull(window, "option page has no option window");
            window.Close();
        }

        public bool OnOk()
        {
            var uploadInformationVerification = _optionPageViewModel.ValidateUploadInformation(
                UploadUrlTextBox.Text,
                WebPraefixTextBox.Text);

            var isValidEmail = _userSettingsViewModel.ValidateEmail(UserSettingsGrid.EmailTextBox.Text);

            return uploadInformationVerification.IsValidUploadInformation && isValidEmail;
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

    }
}