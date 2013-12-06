using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionView : INotifyPropertyChanged
    {
        private readonly string _logFileName;
        private readonly JsonLogFileManager _logFileManager;
        private DateTime _sessionStartTime;
        private IList<EventView> _events; 
        private readonly IList<EventView> _selectedEvents;

        public SessionView(JsonLogFileManager logFileManager, string logFileName)
        {
            _logFileName = logFileName;
            _logFileManager = logFileManager;
            _selectedEvents = new List<EventView>();
        }

        public DateTime StartDate
        { 
            // TODO include date in log file name and extract it here
            get
            {
                if (_sessionStartTime == default(DateTime))
                {
                    _sessionStartTime = Events.First().StartDateTime;
                }
                return _sessionStartTime;
            }
        }

        public IEnumerable<EventView> Events
        {
            get
            {
                // TODO make this lazier again or else it might blow the memory...
                if (_events == null)
                {
                    var logReader = _logFileManager.NewLogReader(_logFileName);
                    _events = logReader.GetEnumeration<IDEEvent>().Select(evt => new EventView(evt)).ToList();
                }
                return _events;
            }
        }

        public IEnumerable<EventView> SelectedEvents
        {
            set
            {
                _selectedEvents.Clear();
                _selectedEvents.AddRange(value);
                OnPropertyChanged("SelectedEvents");
                OnPropertyChanged("SingleSelectedEvent");
            }
        }

        public EventView SingleSelectedEvent
        {
            get
            {
                return _selectedEvents.Count == 1 ? _selectedEvents.First() : null;
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