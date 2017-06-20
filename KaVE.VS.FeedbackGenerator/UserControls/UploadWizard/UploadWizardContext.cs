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
using System.ComponentModel;
using System.Windows.Forms;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Utils.Export;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.UserControls.UploadWizard
{
    public class UploadWizardContext : ViewModelBase<UploadWizardContext>
    {
        private readonly IExporter _exporter;
        private readonly ILogManager _logManager;
        private readonly ISettingsStore _settingsStore;
        private readonly IDateUtils _dateUtils;
        private readonly ILogger _logger;
        private readonly BackgroundWorker _exportWorker;
        private DateTime _exportTime;
        private UploadWizardControl.ExportType _exportType;

        private readonly InteractionRequest<Notification> _errorNotificationRequest;
        private readonly InteractionRequest<LinkNotification> _successNotificationRequest;

        public UserProfileSettings UserProfileSettings { get; private set; }

        public IInteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        public IInteractionRequest<LinkNotification> SuccessNotificationRequest
        {
            get { return _successNotificationRequest; }
        }

        public UploadWizardContext(IExporter exporter,
            ILogManager logManager,
            ISettingsStore settingsStore,
            IDateUtils dateUtils,
            ILogger logger)
        {
            _exporter = exporter;
            _logManager = logManager;
            _settingsStore = settingsStore;
            _dateUtils = dateUtils;
            _logger = logger;
            _errorNotificationRequest = new InteractionRequest<Notification>();
            _successNotificationRequest = new InteractionRequest<LinkNotification>();
            _exportWorker = new BackgroundWorker {WorkerSupportsCancellation = false, WorkerReportsProgress = true};
            _exportWorker.DoWork += OnExport;
            _exportWorker.ProgressChanged += OnProgressChanged;
            _exportWorker.RunWorkerCompleted += OnExportCompleted;

            UserProfileSettings = _settingsStore.GetSettings<UserProfileSettings>();
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            Asserts.NotNull(progressChangedEventArgs.UserState);
            BusyMessage = string.Format(
                "{0} ({1})",
                Properties.UploadWizard.Export_BusyMessage,
                progressChangedEventArgs.UserState);
        }

        private bool _hasCheckedDeclaration;

        public bool HasCheckedDeclaration
        {
            get { return _hasCheckedDeclaration; }
            set
            {
                _hasCheckedDeclaration = value;
                RaisePropertyChanged(self => self.HasCheckedDeclaration);
                RaisePropertyChanged(self => self.IsShowingDeclarationWarning);
            }
        }

        public bool IsShowingDeclarationWarning
        {
            get { return !_hasCheckedDeclaration; }
        }

        public void Export(UploadWizardControl.ExportType exportType)
        {
            SetBusy(Properties.UploadWizard.Export_BusyMessage);
            _exportTime = _dateUtils.Now;
            _exportWorker.RunWorkerAsync(exportType);
        }

        private void OnExport(object sender, DoWorkEventArgs args)
        {
            var worker = (BackgroundWorker) sender;
            _exportType = (UploadWizardControl.ExportType) args.Argument;

            Action<int> reportExportStatusChange =
                percentage => worker.ReportProgress(percentage, string.Format("{0}%", percentage));
            _exporter.ExportProgressChanged += reportExportStatusChange;

            try
            {
                var publisher = CreatePublisher();
                args.Result = _exporter.Export(_exportTime, publisher);
                _logManager.DeleteLogsOlderThan(_exportTime);
            }
            finally
            {
                _exporter.ExportProgressChanged -= reportExportStatusChange;
            }
        }

        private IPublisher CreatePublisher()
        {
            IPublisher publisher;
            if (_exportType == UploadWizardControl.ExportType.ZipFile)
            {
                publisher = new FilePublisher(AskForExportLocation);
            }
            else
            {
                publisher = new HttpPublisher(GetUploadUrl());
            }
            return publisher;
        }

        private static string AskForExportLocation()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.SessionManager.SaveFileDialogFilter,
                AddExtension = true
            };
            var result = Invoke.OnSTA(() => saveFileDialog.ShowDialog());
            return result == DialogResult.Cancel ? null : saveFileDialog.FileName;
        }

        private Uri GetUploadUrl()
        {
            var exportSettings = _settingsStore.GetSettings<ExportSettings>();
            return new Uri(exportSettings.UploadUrl);
        }

        private void OnExportCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                UpdateLastUploadDate();
                ShowExportSucceededMessage((int) e.Result);
            }
            else
            {
                _logger.Error(e.Error, "export failed");
                ShowExportFailedMessage(e.Error.Message);
            }
            SetIdle();
        }

        private void UpdateLastUploadDate()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            settings.LastUploadDate = _dateUtils.Now;
            _settingsStore.SetSettings(settings);
        }

        private void ShowExportSucceededMessage(int numberOfExportedEvents)
        {
            var message = string.Format(Properties.UploadWizard.ExportSuccess, numberOfExportedEvents);

            switch (_exportType)
            {
                case UploadWizardControl.ExportType.ZipFile:
                    var export = _settingsStore.GetSettings<ExportSettings>();
                    RaiseLinkNotificationRequest(message, export.UploadUrl);
                    break;
                case UploadWizardControl.ExportType.HttpUpload:
                    RaiseNotificationRequest(message);
                    break;
            }
        }

        private void ShowExportFailedMessage(string message)
        {
            RaiseNotificationRequest(
                Properties.UploadWizard.ExportFail + (string.IsNullOrWhiteSpace(message) ? "" : ":\n" + message));
        }


        private void RaiseNotificationRequest(string text)
        {
            _errorNotificationRequest.Raise(
                new Notification
                {
                    Caption = UploadWizardMessages.Title,
                    Message = text
                });
        }

        private void RaiseLinkNotificationRequest(string text, string url)
        {
            _successNotificationRequest.Raise(
                new LinkNotification
                {
                    Caption = UploadWizardMessages.Title,
                    Message = text,
                    LinkDescription = Properties.UploadWizard.ExportSuccessLinkDescription,
                    Link = url
                });
        }
    }
}