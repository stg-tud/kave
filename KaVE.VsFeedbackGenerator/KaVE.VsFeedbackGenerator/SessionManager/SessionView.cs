using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using KaVE.Model.Events;
#if !DEBUG
using System.IO.Compression;
#endif
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionView : INotifyPropertyChanged
    {
        private readonly string _logFileName;
        private DateTime _sessionStartTime;
        private IList<EventView> _events; 
        private readonly IList<EventView> _selectedEvents;

        public SessionView(string logFileName)
        {
            _logFileName = logFileName;
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
                    var logReader = NewLogReader(_logFileName);
                    _events = logReader.GetEnumeration<IDEEvent>().Select(evt => new EventView(evt)).ToList();
                }
                return _events;
            }
        }

        private static JsonLogReader NewLogReader(string logFilePath)
        {
            Stream logStream = new FileStream(logFilePath, FileMode.Open);
            try
            {
#if !DEBUG
                logStream = new GZipStream(logStream, CompressionMode.Decompress);
#endif
                return new JsonLogReader(logStream);
            }
            catch (Exception)
            {
                logStream.Close();
                throw;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}