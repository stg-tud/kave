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
 * Contributors:
 *    - Uli Fahrer
 */

using System.ComponentModel;
using System.Windows;
using JetBrains.ActionManagement;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.Export
{
    public partial class UploadWizard
    {
        public enum ExportType
        {
            ZipFile,
            HttpUpload
        }

        private readonly IActionManager _actionManager;
        private readonly UploadWizardViewModel _uploadWizardViewModel;

        public UploadWizard(IActionManager actionManager, UploadWizardViewModel uploadWizardViewModel)
        {
            _actionManager = actionManager;
            DataContext = uploadWizardViewModel;
            _uploadWizardViewModel = uploadWizardViewModel;
            _uploadWizardViewModel.PropertyChanged += OnViewModelPropertyChanged;
            _uploadWizardViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;
            _uploadWizardViewModel.SuccessNotificationRequest.Raised += new LinkNotificationRequestHandler(this).Handle;

            InitializeComponent();
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            Close();
            // TODO RS9
            //_actionManager.ExecuteActionGuarded(SessionManagerWindowActionHandler.ActionId, "AgentAction");
        }

        private void UploadButtonClicked(object sender, RoutedEventArgs e)
        {
            _uploadWizardViewModel.Export(ExportType.HttpUpload);
            // keep window open until processing finshes (see OnViewModelPropertyChanged)
        }

        private void ZipButtonClicked(object sender, RoutedEventArgs e)
        {
            _uploadWizardViewModel.Export(ExportType.ZipFile);
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
                global::JetBrains.Util.MessageBox.ShowInfo(
                    Properties.UploadWizard.ContinueInBackground,
                    Properties.UploadWizard.window_title);
            }
        }
   }
}