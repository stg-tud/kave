using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application;
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

        public FeedbackView(JsonLogFileManager logFileManager)
        {
            _logFileManager = logFileManager;
            _sessions = new List<SessionView>();
            _selectedSessions = new List<SessionView>();
            RefreshSessions();
        }

        public void RefreshSessions()
        {
            // TODO fix refresh (breaks if log files are deleted)
            Sessions = _logFileManager.GetLogFileNames().Select(logFileName => new SessionView(_logFileManager, logFileName));
        }

        public IEnumerable<SessionView> Sessions
        {
            get
            {
                return _sessions;
            }

            set
            {
                _sessions.Clear();
                _sessions.AddRange(value);
                OnPropertyChanged("Sessions");
            }
        }

        public IEnumerable<SessionView> SelectedSessions
        {
            set
            {
                _selectedSessions.Clear();
                _selectedSessions.AddRange(value);
                OnPropertyChanged("SelectedSessions");
                OnPropertyChanged("SingleSelectedSession");
            }
        }

        [CanBeNull]
        public SessionView SingleSelectedSession
        {
            get
            {
                return _selectedSessions.Count == 1 ? _selectedSessions.First() : null;
            }
        }

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
