using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class FeedbackView : INotifyPropertyChanged
    {
        private readonly JsonLogFileManager _logFileManager;
        private readonly IList<SessionView> _sessions;
        private readonly IList<SessionView> _selectedSessions;
        private DelegateCommand _deleteSessionsCommand;
        private bool _refreshing;

        public FeedbackView(JsonLogFileManager logFileManager)
        {
            _logFileManager = logFileManager;
            _sessions = new ObservableCollection<SessionView>();
            _selectedSessions = new List<SessionView>();
            Released = true;
        }

        public void Refresh()
        {
            Refreshing = true;
            Invoke.Async(
                () =>
                {
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
                OnPropertyChanged("Refreshing");
            }
        }

        public IEnumerable<SessionView> Sessions
        {
            private set
            {
                Invoke.OnDispatcher(
                    () =>
                    {
                        _sessions.Clear();
                        if (value != null)
                        {
                            _sessions.AddRange(value);
                        }
                    });
            }
            get { return _sessions; }
        }

        public IEnumerable<SessionView> SelectedSessions
        {
            set
            {
                _selectedSessions.Clear();
                _selectedSessions.AddRange(value);
                // single selected session depends on selected sessions
                OnPropertyChanged("SingleSelectedSession");
                _deleteSessionsCommand.RaiseCanExecuteChanged();
            }
        }

        [CanBeNull]
        public SessionView SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        #region DeleteSessionsCommand implementation

        public DelegateCommand DeleteSessionsCommand
        {
            get { return _deleteSessionsCommand ?? (_deleteSessionsCommand = CreateDeleteSessionsCommand()); }
        }

        private DelegateCommand CreateDeleteSessionsCommand()
        {
            Func<string> confirmationTitle = () => Messages.SessionDeleteConfirmTitle;
            Func<string> confirmationText = () =>
            {
                var numberOfSessions = _selectedSessions.Count;
                return numberOfSessions == 1
                    ? Messages.SessionDeleteConfirmSingular
                    : Messages.SessionDeleteConfirmPlural.FormatEx(numberOfSessions);
            };
            return ConfirmedCommand.Create(DeleteSessions, confirmationTitle, confirmationText, CanDeleteSessions);
        }

        private bool CanDeleteSessions()
        {
            return _selectedSessions.Count > 0;
        }

        private void DeleteSessions()
        {
            for (var i = 0; i < _sessions.Count; i++)
            {
                var session = _sessions[i];
                if (_selectedSessions.Contains(session))
                {
                    _sessions.Remove(session);
                    File.Delete(session.LogFileName);
                    i--;
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}