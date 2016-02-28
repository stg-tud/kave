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
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UploadWizard.Anonymization;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfileDialogs;
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
        private readonly IActionExecutor _actionExec;

        public UploadWizardControl(UploadWizardContext dataContext,
            ISettingsStore settingsStore,
            IActionExecutor actionExec)
        {
            InitializeComponent();

            _settingsStore = settingsStore;
            _actionExec = actionExec;

            DataContext = dataContext;
            MyDataContext.PropertyChanged += OnViewModelPropertyChanged;
            MyDataContext.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;
            MyDataContext.SuccessNotificationRequest.Raised += new LinkNotificationRequestHandler(this).Handle;
        }

        private void OnClickReview(object sender, RoutedEventArgs e)
        {
            Close();
            Registry.GetComponent<ActionExecutor>().ExecuteActionGuarded<SessionManagerWindowAction>();
        }

        private void OnClickDirectUpload(object sender, RoutedEventArgs e)
        {
            ExportAndUpdateSettings(ExportType.HttpUpload);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void OnClickZipExport(object sender, RoutedEventArgs e)
        {
            ExportAndUpdateSettings(ExportType.ZipFile);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void ExportAndUpdateSettings(ExportType exportType)
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

        private void OnClickUserProfile(object sender, RoutedEventArgs e)
        {
            new UserProfileDialog(_actionExec, _settingsStore, UploadWizardPolicy.DoNotOpenUploadWizardOnFinish).Show();
        }

        private void OnClickAnonymization(object sender, RoutedEventArgs e)
        {
            var anonymizationWindow = new AnonymizationWindow(_settingsStore);
            anonymizationWindow.Show();
        }

        private void OnClickWebsite(object sender, RoutedEventArgs e)
        {
            OpenWebsite("http://www.kave.cc/");
        }

        private void OnClickEventDetails(object sender, RoutedEventArgs e)
        {
            OpenWebsite("http://www.kave.cc/documentation/event-generation");
        }

        private void OnClickContact(object sender, RoutedEventArgs e)
        {
            OpenWebsite("http://www.kave.cc/community");
        }

        private void OnClickFeedback(object sender, RoutedEventArgs e)
        {
            OpenWebsite("http://www.kave.cc/feedback");
        }

        private void OnClickManualUpload(object sender, RoutedEventArgs e)
        {
            OpenWebsite("http://kave.st.informatik.tu-darmstadt.de/");
        }

        private void OpenWebsite(string url)
        {
            var prefix = _settingsStore.GetSettings<ExportSettings>().WebAccessPrefix;
            System.Diagnostics.Process.Start(prefix + url);
        }
    }
}