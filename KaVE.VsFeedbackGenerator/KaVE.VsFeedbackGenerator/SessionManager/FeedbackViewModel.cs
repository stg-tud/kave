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
 *    - Sven Amann
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public interface IFeedbackViewModelDialog
    {
        bool AreAnyEventsPresent { get; }
        IList<IDEEvent> ExtractEventsForExport();
        void ShowExportSucceededMessage(int count);
        void ShowExportFailedMessage(string message);
    }

    [ShellComponent]
    public sealed class FeedbackViewModel : ViewModelBase<FeedbackViewModel>, IFeedbackViewModelDialog
    {
        private readonly ILogManager<IDEEvent> _logManager;
        private readonly IList<SessionViewModel> _sessions;
        private readonly IList<SessionViewModel> _selectedSessions;
        private DelegateCommand _exportCommand;
        private DelegateCommand _deleteCommand;
        private bool _refreshing;
        private DateTime _lastRefresh;
        private readonly ISettingsStore _settingsStore;
        private readonly IDateUtils _dateUtils;
        private readonly IExporter _exporter;

        private readonly InteractionRequest<Confirmation> _confirmationRequest;

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        private readonly InteractionRequest<UploadWizard.UploadOptions> _uploadOptionsRequest;

        public IInteractionRequest<UploadWizard.UploadOptions> UploadOptionsRequest
        {
            get { return _uploadOptionsRequest; }
        }

        private readonly InteractionRequest<Notification> _notificationRequest;

        public IInteractionRequest<Notification> NotificationRequest
        {
            get { return _notificationRequest; }
        }

        public FeedbackViewModel(ILogManager<IDEEvent> logManager, IExporter exporter, ISettingsStore settingsStore, IDateUtils dateUtils)
        {
            _settingsStore = settingsStore;
            _dateUtils = dateUtils;
            _exporter = exporter;
            _logManager = logManager;
            _sessions = new ObservableCollection<SessionViewModel>();
            _selectedSessions = new List<SessionViewModel>();
            _confirmationRequest = new InteractionRequest<Confirmation>();
            _uploadOptionsRequest = new InteractionRequest<UploadWizard.UploadOptions>();
            _notificationRequest = new InteractionRequest<Notification>();
            Released = true;
        }

        public void Refresh()
        {
            Refreshing = true;
            Invoke.Async(
                () =>
                {
                    _lastRefresh = DateTime.Now;
                    DoRefresh();
                    Refreshing = false;
                    Released = false;
                });
        }

        private void DoRefresh()
        {
            try
            {
                Sessions =
                    _logManager.GetLogs().Select(
                        log =>
                        {
                            var vm = new SessionViewModel(log);
                            vm.ConfirmationRequest.Raised +=
                                (sender, args) => _confirmationRequest.Delegate(args);
                            return vm;
                        });
            }
            catch (Exception e)
            {
                var logEventGenerator = Registry.GetComponent<Generators.ILogger>();
                logEventGenerator.Error(e);
            }
        }

        public void Release()
        {
            Sessions = null;
            Released = true;
        }

        public bool Released { get; private set; }

        public bool Refreshing
        {
            get { return _refreshing; }
            private set
            {
                _refreshing = value;
                OnPropertyChanged(vm => vm.Refreshing);
            }
        }

        public IEnumerable<SessionViewModel> Sessions
        {
            private set
            {
                Invoke.OnSTA(
                    () =>
                    {
                        _sessions.Clear();
                        if (value != null)
                        {
                            _sessions.AddRange(value);
                        }
                        ExportCommand.RaiseCanExecuteChanged();
                    });
            }
            get { return _sessions; }
        }

        public bool AreAnyEventsPresent
        {
            get { return _sessions.Any(s => s.Events.Any()); }
        }

        public IEnumerable<SessionViewModel> SelectedSessions
        {
            set
            {
                _selectedSessions.Clear();
                _selectedSessions.AddRange(value);
                // single selected session depends on selected session
                OnPropertyChanged(vm => vm.SingleSelectedSession);
                DeleteSessionsCommand.RaiseCanExecuteChanged();
            }
        }

        [CanBeNull]
        public SessionViewModel SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        public DelegateCommand ExportCommand
        {
            get
            {
                // TODO @Sven: make AreAnyEventsPresent a method after code review with Dennis finished
                return _exportCommand ?? (_exportCommand = new DelegateCommand(OnExport, () => AreAnyEventsPresent));
            }
        }

        public void OnExport()
        {
            _uploadOptionsRequest.Raise(new UploadWizard.UploadOptions(), Export);
        }

        private void Export(UploadWizard.UploadOptions options)
        {
            if (options.Type.HasValue)
            {
                try
                {
                    var eventsForExport = ExtractEventsForExport();

                    if (options.Type == UploadWizard.UploadOptions.ExportType.ZipFile)
                    {
                        _exporter.Export(eventsForExport, new FilePublisher(AskForExportLocation));
                    }
                    else
                    {
                        _exporter.Export(eventsForExport, new HttpPublisher(GetUploadUrl()));
                    }

                    _logManager.DeleteLogsOlderThan(_lastRefresh);
                    UpdateLastUploadDate();
                    ShowExportSucceededMessage(eventsForExport.Count);
                }
                catch (Exception e)
                {
                    ShowExportFailedMessage(e.Message);
                }
                Refresh();
            }
        }

        private Uri GetUploadUrl()
        {
            var exportSettings = _settingsStore.GetSettings<ExportSettings>();
            return new Uri(exportSettings.UploadUrl);
        }

        public void ShowExportSucceededMessage(int numberOfExportedEvents)
        {
            _notificationRequest.Raise(
                new Notification
                {
                    Caption = Properties.UploadWizard.window_title,
                    Message = string.Format(Messages.ExportSuccess, numberOfExportedEvents)
                });
        }

        public void ShowExportFailedMessage(string message)
        {
            _notificationRequest.Raise(
                new Notification
                {
                    Caption = Properties.UploadWizard.window_title,
                    Message = Messages.ExportFail + (string.IsNullOrWhiteSpace(message) ? "" : ":\n" + message)
                });
        }

        private void UpdateLastUploadDate()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            settings.LastUploadDate = _dateUtils.Now;
            _settingsStore.SetSettings(settings);
        }

        public IList<IDEEvent> ExtractEventsForExport()
        {
            return _sessions.SelectMany(session => session.Events.Select(events => events.Event)).ToList();
        }

        private static string AskForExportLocation()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.SessionManager.SaveFileDialogFilter,
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog().Equals(DialogResult.Cancel))
            {
                return null;
            }
            return saveFileDialog.FileName;
        }

        public DelegateCommand DeleteSessionsCommand
        {
            get
            {
                return _deleteCommand ??
                       (_deleteCommand = new DelegateCommand(OnDeleteSelectedSessions, CanDeleteSessions));
            }
        }

        private bool CanDeleteSessions()
        {
            return _selectedSessions.Count > 0;
        }

        private void OnDeleteSelectedSessions()
        {
            var numberOfSessions = _selectedSessions.Count;
            _confirmationRequest.Raise(
                new Confirmation
                {
                    Caption = Messages.SessionDeleteConfirmTitle,
                    Message = numberOfSessions == 1
                        ? Messages.SessionDeleteConfirmSingular
                        : Messages.SessionDeleteConfirmPlural.FormatEx(numberOfSessions)
                },
                DeleteSessions);
        }

        private void DeleteSessions(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            // Changing _sessions implicitly changes _selectedSessions, what
            // leads to concurrent modification problems, if we change _session
            // in the loop. Therefore, we collect what has been successfully
            // deleted and update the UI afterwards.
            var deletedSessions = new List<SessionViewModel>();
            try
            {
                foreach (var selectedSession in _selectedSessions)
                {
                    selectedSession.Log.Delete();
                    deletedSessions.Add(selectedSession);
                }
            }
            finally
            {
                _sessions.RemoveAll(deletedSessions.Contains);
            }
        }
    }
}