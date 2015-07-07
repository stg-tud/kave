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

using System.ComponentModel;
using System.Windows;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Interactivity;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.UploadWizard
{
    public partial class UploadWizardControl
    {
        private UploadWizardViewModel MyDataContext
        {
            get { return (UploadWizardViewModel) DataContext; }
        }

        public enum ExportType
        {
            ZipFile,
            HttpUpload
        }

        private readonly ISettingsStore _settingsStore;

        public UploadWizardControl(UploadWizardViewModel uploadWizardViewModel, ISettingsStore settingsStore)
        {
            InitializeComponent();

            _settingsStore = settingsStore;

            DataContext = uploadWizardViewModel;
            MyDataContext.PropertyChanged += OnViewModelPropertyChanged;
            MyDataContext.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;
            MyDataContext.SuccessNotificationRequest.Raised += new LinkNotificationRequestHandler(this).Handle;
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            StoreSettings();

            /* var isValidEmail = _userSettingsViewModel.ValidateEmail(UserSettingsGrid.EmailTextBox.Text);
            if (isValidEmail)
            {
                _userSettingsViewModel.SetUserSettings();
                _uploadWizardViewModel.SetSettings();
                Close();
                Registry.GetComponent<ActionExecutor>().ExecuteActionGuarded<SessionManagerWindowAction>();
            }*/
        }

        private void StoreSettings()
        {
            _settingsStore.SetSettings(MyDataContext.ExportSettings);
            _settingsStore.SetSettings(MyDataContext.UserProfileSettings);
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

        private void ExportAndUpdateSettings(ExportType exportType)
        {
            StoreSettings();
            
            /*var isValidEmail = _userSettingsViewModel.ValidateEmail(UserSettingsGrid.EmailTextBox.Text);
                      if (isValidEmail)
                      {
                          _userSettingsViewModel.SetUserSettings();
                          _uploadWizardViewModel.SetSettings();*/
            MyDataContext.Export(exportType);
            //}
        }

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsBusy") && !MyDataContext.IsBusy)
            {
                Close();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (MyDataContext.IsBusy)
            {
                MessageBox.ShowInfo(
                    Properties.UploadWizard.ContinueInBackground,
                    UploadWizardMessages.Title);
            }
        }
    }
}