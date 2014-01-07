using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public sealed class FeedbackView : INotifyPropertyChanged
    {
        private readonly JsonLogFileManager _logFileManager;
        private readonly IList<SessionView> _sessions;
        private readonly IList<SessionView> _selectedSessions;
        private DelegateCommand _deleteSessionsCommand;

        public FeedbackView(JsonLogFileManager logFileManager)
        {
            _logFileManager = logFileManager;
            _sessions = new ObservableCollection<SessionView>();
            _selectedSessions = new List<SessionView>();

            RefreshSessions();
        }

        public void RefreshSessions()
        {
            Invoke.OnDispatcher(
                () =>
                {
                    Sessions =
                        _logFileManager.GetLogFileNames()
                            .Select(logFileName => new SessionView(_logFileManager, logFileName));
                });
            Released = false;
        }

        public void Release()
        {
            Invoke.OnDispatcher(() => _sessions.Clear());
            Released = true;
        }

        public bool Released { get; private set; }

        public IEnumerable<SessionView> Sessions
        {
            private set
            {
                _sessions.Clear();
                _sessions.AddRange(value);
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

        #region Delete-Sessions Command implementation

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
            foreach (var logFileName in _selectedSessions.Select(session => session.LogFileName))
            {
                File.Delete(logFileName);
            }
            RefreshSessions();
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