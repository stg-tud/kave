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
 * 
 */

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using JetBrains.ActionManagement;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVE.VS.FeedbackGenerator.Utils;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.Export
{
    public partial class UploadWizard
    {
        public enum ExportType
        {
            ZipFile,
            HttpUpload
        }

        private readonly UploadWizardViewModel _uploadWizardViewModel;
        private readonly UserProfileViewModel _userSettingsViewModel;

        public UploadWizard(IActionManager actionManager,
            UploadWizardViewModel uploadWizardViewModel,
            ISettingsStore settingsStore)
        {
            DataContext = uploadWizardViewModel;
            _uploadWizardViewModel = uploadWizardViewModel;
            _uploadWizardViewModel.ExportSettings = settingsStore.GetSettings<ExportSettings>();
            _uploadWizardViewModel.UserSettings = settingsStore.GetSettings<UserProfileSettings>();
            _uploadWizardViewModel.PropertyChanged += OnViewModelPropertyChanged;
            _uploadWizardViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;
            _uploadWizardViewModel.SuccessNotificationRequest.Raised += new LinkNotificationRequestHandler(this).Handle;

            InitializeComponent();

            var exportSettings = settingsStore.GetSettings<ExportSettings>();
            if (exportSettings.IsDatev)
            {
                UserSettingsGrid.ProvideUserInformationCheckBox.IsEnabled = false;
                UserSettingsGrid.DatevDeactivationLabel.Visibility = Visibility.Visible;
            }

            _userSettingsViewModel = (UserProfileViewModel) UserSettingsGrid.DataContext;
            _userSettingsViewModel.UserSettings = settingsStore.GetSettings<UserProfileSettings>();
            _userSettingsViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;

            UserSettingsGrid.CategoryComboBox.SetBinding(Selector.SelectedItemProperty, new Binding("Category"));
            UserSettingsGrid.RadioButtonListBox.SetBinding(Selector.SelectedItemProperty, new Binding("Valuation"));
            
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            var isValidEmail = _userSettingsViewModel.ValidateEmail(UserSettingsGrid.EmailTextBox.Text);
            if (isValidEmail)
            {
                _userSettingsViewModel.SetUserSettings();
                _uploadWizardViewModel.SetSettings();
                Close();
                Registry.GetComponent<ActionExecutor>().ExecuteActionGuarded<SessionManagerWindowAction>();
            }
        }

        private void UploadButtonClicked(object sender, RoutedEventArgs e)
        {
            ExportAndUpdateSettings(ExportType.HttpUpload);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void ZipButtonClicked(object sender, RoutedEventArgs e)
        {
            ExportAndUpdateSettings(ExportType.ZipFile);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsBusy") && !_uploadWizardViewModel.IsBusy)
            {
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (_uploadWizardViewModel.IsBusy)
            {
                MessageBox.ShowInfo(
                    Properties.UploadWizard.ContinueInBackground,
                    Properties.UploadWizard.window_title);
            }
        }

        private void ExportAndUpdateSettings(ExportType exportType)
        {
            var isValidEmail = _userSettingsViewModel.ValidateEmail(UserSettingsGrid.EmailTextBox.Text);
            if (isValidEmail)
            {
                _userSettingsViewModel.SetUserSettings();
                _uploadWizardViewModel.SetSettings();
                _uploadWizardViewModel.Export(exportType);
            }
        }
    }
}