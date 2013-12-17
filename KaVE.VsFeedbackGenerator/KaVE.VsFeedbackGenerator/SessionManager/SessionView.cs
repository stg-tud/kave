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
        private readonly IList<EventView> _events; 
        private readonly IList<EventView> _selectedEvents;

        public SessionView(JsonLogFileManager logFileManager, string logFileName)
        {
            _selectedEvents = new List<EventView>();
            // loading eagerly because lazy approaches led to UI display bugs
            // TODO if this should cause memory problems, we have to find a lazier solution...
            using (var logReader = logFileManager.NewLogReader(logFileName))
            {
                _events = logReader.GetEnumeration<IDEEvent>().Select(evt => new EventView(evt)).ToList();
            }
        }

        public DateTime StartDate
        {
            // TODO include date in log file name and extract it here
            get
            {
                return _events.First().StartDateTime;
            }
        }

        public IEnumerable<EventView> Events
        {
            get
            {
                return _events;
            }
        }

        public IEnumerable<EventView> SelectedEvents
        {
            set
            {
                _selectedEvents.Clear();
                _selectedEvents.AddRange(value);
                // notify listeners about dependent property cange
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