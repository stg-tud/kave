using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class FeedbackViewModel : ViewModelBase<FeedbackViewModel>
    {
        private readonly ILogFileManager<IDEEvent> _logFileManager;
        private readonly IList<SessionView> _sessions;
        private readonly IList<SessionView> _selectedSessions;
        private DelegateCommand _exportSessionsCommand;
        private DelegateCommand _sendSessionsCommand;
        private bool _refreshing;
        private DateTime _lastRefresh;
        private readonly ISettingsStore _store;

        private readonly InteractionRequest<Confirmation> _deleteSessionsConfirmationRequest; 

        public FeedbackViewModel(ILogFileManager<IDEEvent> logFileManager, ISettingsStore store)
        {
            _store = store;
            _logFileManager = logFileManager;
            _sessions = new ObservableCollection<SessionView>();
            _selectedSessions = new List<SessionView>();
            DeleteSessionsCommand = new DelegateCommand(OnDeleteSelectedSessions, CanDeleteSessions);
            _deleteSessionsConfirmationRequest = new InteractionRequest<Confirmation>();
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
                        _logFileManager.GetLogFileNames()
                                       .Select(logFileName => new SessionView(_logFileManager, logFileName));
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

        public IEnumerable<SessionView> Sessions
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

        public IEnumerable<SessionView> SelectedSessions
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
        public SessionView SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        public DelegateCommand ExportSessionsCommand
        {
            get
            {
                return _exportSessionsCommand ??
                       (_exportSessionsCommand =
                           CreateWithExportPolicy(new FileExport<IDEEvent>(AskForExportLocation)));
            }
        }

        public DelegateCommand SendSessionsCommand
        {
            get
            {
                return _sendSessionsCommand ??
                       (_sendSessionsCommand =
                           CreateWithExportPolicy(
                               new HttpExport<IDEEvent>("http://kave.st.informatik.tu-darmstadt.de:667/upload")));
            }
        }

        private DelegateCommand CreateWithExportPolicy(SessionExport<IDEEvent> policy)
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
        public IInteractionRequest DeleteSessionsConfirmationRequest { get { return _deleteSessionsConfirmationRequest; } }

        private bool CanDeleteSessions()
        {
            return _selectedSessions.Count > 0;
        }

        private void OnDeleteSelectedSessions()
        {
            var numberOfSessions = _selectedSessions.Count;
            _deleteSessionsConfirmationRequest.Raise(
                new Confirmation
                {
                    Title = Messages.SessionDeleteConfirmTitle,
                    Content = numberOfSessions == 1
                    ? Messages.SessionDeleteConfirmSingular
                    : Messages.SessionDeleteConfirmPlural.FormatEx(numberOfSessions)
                }, DeleteSessions);
        }

        private void DeleteSessions(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            foreach (var selectedSession in _selectedSessions)
            {
                File.Delete(selectedSession.LogFileName);
                _sessions.Remove(selectedSession);
            }
        }
    }
}
