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
using KaVE.RS.Commons;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.UploadWizard
{
    public partial class UploadWizardControl
    {
        private UploadWizardContext MyDataContext
        {
            get { return (UploadWizardContext) DataContext; }
        }

        public enum ExportType
        {
            ZipFile,
            HttpUpload
        }

        private readonly ISettingsStore _settingsStore;

        public UploadWizardControl(UploadWizardContext dataContext, ISettingsStore settingsStore)
        {
            InitializeComponent();

            _settingsStore = settingsStore;

            DataContext = dataContext;
            MyDataContext.PropertyChanged += OnViewModelPropertyChanged;
            MyDataContext.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;
            MyDataContext.SuccessNotificationRequest.Raised += new LinkNotificationRequestHandler(this).Handle;
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Registry.GetComponent<ActionExecutor>().ExecuteActionGuarded<SessionManagerWindowAction>();
        }

        private void UploadButtonClicked(object sender, RoutedEventArgs e)
        {
            Export(ExportType.HttpUpload);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void ZipButtonClicked(object sender, RoutedEventArgs e)
        {
            Export(ExportType.ZipFile);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void Export(ExportType exportType)
        {
            MyDataContext.Export(exportType);
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

        private void On_UserProfile_Click(object sender, RoutedEventArgs e)
        {
            var userProfileWindow = new UserProfile.UserProfileWindow(MyDataContext,_settingsStore);
            userProfileWindow.Show();
        }

        private void On_Anonymization_Click(object sender, RoutedEventArgs e)
        {
            var anonymizationWindow = new Anonymization.AnonymizationWindow(MyDataContext,_settingsStore);
            anonymizationWindow.Show();
        }
    }
}