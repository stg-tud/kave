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
using KaVE.VsFeedbackGenerator.Utils;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class FeedbackViewModel : ViewModelBase<FeedbackViewModel>
    {
        private readonly ILogFileManager<IDEEvent> _logFileManager;
        private readonly IList<SessionViewModel> _sessions;
        private readonly IList<SessionViewModel> _selectedSessions;
        private DelegateCommand _exportSessionsCommand;
        private DelegateCommand _sendSessionsCommand;
        private bool _refreshing;
        private DateTime _lastRefresh;
        private readonly ISettingsStore _store;

        private readonly InteractionRequest<Confirmation> _confirmationRequest;

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public FeedbackViewModel(ILogFileManager<IDEEvent> logFileManager, ISettingsStore store)
        {
            _store = store;
            _logFileManager = logFileManager;
            _sessions = new ObservableCollection<SessionViewModel>();
            _selectedSessions = new List<SessionViewModel>();
            DeleteSessionsCommand = new DelegateCommand(OnDeleteSelectedSessions, CanDeleteSessions);
            _confirmationRequest = new InteractionRequest<Confirmation>();
            Released = true;
        }

        public void Refresh()
        {
            Refreshing = true;
            Invoke.Async(
                () =>
                {
                    _lastRefresh = DateTime.Now;
                    Sessions =
                        _logFileManager.GetLogFileNames().Select(
                            logFileName =>
                            {
                                var vm = new SessionViewModel(_logFileManager, logFileName);
                                vm.ConfirmationRequest.Raised += (sender, args) => _confirmationRequest.Delegate(args);
                                return vm;
                            });
                    Refreshing = false;
                    Released = false;
                });
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
                        SendSessionsCommand.RaiseCanExecuteChanged();
                        ExportSessionsCommand.RaiseCanExecuteChanged();
                    });
            }
            get { return _sessions; }
        }

        public bool AnySessionsPresent
        {
            get { return _sessions.Count > 0; }
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

        public DelegateCommand ExportSessionsCommand
        {
            get
            {
                return _exportSessionsCommand ??
                       (_exportSessionsCommand =
                           CreateWithExportPolicy(new SessionExport(new FilePublisher(AskForExportLocation))));
            }
        }

        public DelegateCommand SendSessionsCommand
        {
            get
            {
                return _sendSessionsCommand ??
                       (_sendSessionsCommand =
                           CreateWithExportPolicy(
                               new SessionExport(
                                   new HttpPublisher("http://kave.st.informatik.tu-darmstadt.de:667/upload"))));
            }
        }

        private DelegateCommand CreateWithExportPolicy(SessionExport policy)
        {
            return ExportCommand.Create(
                policy,
                ExtractEventsForExport,
                _logFileManager.NewLogWriter,
                o => AnySessionsPresent,
                res =>
                {
                    if (res.Status == State.Ok)
                    {
                        _logFileManager.DeleteLogsOlderThan(_lastRefresh);
                        var settings = _store.GetSettings<UploadSettings>();
                        settings.LastUploadDate = DateTime.Now;
                        _store.SetSettings(settings);
                        MessageBox.Show(string.Format(Messages.ExportSuccess, res.Data.Count));
                        Refresh();
                    }
                    else
                    {
                        MessageBox.Show(
                            Messages.ExportFail + (string.IsNullOrWhiteSpace(res.Message) ? "" : "\n" + res.Message));
                    }
                });
        }

        private IEnumerable<IDEEvent> ExtractEventsForExport()
        {
            return _sessions.SelectMany(session => session.Events.Select(events => events.Event));
        }

        private static string AskForExportLocation()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = Properties.SessionManager.SaveFileDialogFilter
            };
            if (saveFileDialog.ShowDialog().Equals(DialogResult.Cancel))
            {
                return null;
            }
            return saveFileDialog.FileName;
        }

        public DelegateCommand DeleteSessionsCommand { get; private set; }

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

            // Changing _sessions implicitly changes _selectedSessions, what leads to a concurrent modification, if we change _session in the loop.
            // Therefore we collect what has been successfully deleted and update the UI afterwards.
            var deletedSessions = new List<SessionViewModel>();
            try
            {
                foreach (var selectedSession in _selectedSessions)
                {
                    _logFileManager.DeleteLogs(selectedSession.LogFileName);
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