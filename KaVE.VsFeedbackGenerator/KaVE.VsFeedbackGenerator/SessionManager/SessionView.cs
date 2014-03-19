using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using JetBrains;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Utils;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionView : INotifyPropertyChanged
    {
        private readonly ILogFileManager<IDEEvent> _logFileManager;
        public string LogFileName { get; private set; }
        private readonly IList<EventView> _events; 
        private readonly IList<EventView> _selectedEvents;
        private DelegateCommand _deleteEventsCommand;

        public SessionView(ILogFileManager<IDEEvent> logFileManager, string logFileName)
        {
            _logFileManager = logFileManager;
            LogFileName = logFileName;
            _events = new ObservableCollection<EventView>();
            _selectedEvents = new List<EventView>();
            // loading eagerly because lazy approaches led to UI display bugs
            // TODO if this should cause memory problems, we have to find a lazier solution...
            using (var logReader = logFileManager.NewLogReader(logFileName))
            {
                Events = logReader.ReadAll().Select(evt => new EventView(evt));
            }
        }

        public DateTime StartDate
        {
            // TODO include date in log file name and extract it here
            get
            {
                return _events.First().StartDateTime.Date;
            }
        }

        public IEnumerable<EventView> Events
        {
            set
            {
                _events.Clear();
                _events.AddRange(value);
            }

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
                _deleteEventsCommand.RaiseCanExecuteChanged();
            }
        }

        public EventView SingleSelectedEvent
        {
            get
            {
                return _selectedEvents.Count == 1 ? _selectedEvents.First() : null;
            }
        }

        // TODO remove these regions
        #region Delete-Events Command implementation
        public DelegateCommand DeleteEventsCommand
        {
            get { return _deleteEventsCommand ?? (_deleteEventsCommand = CreateDeleteEventsCommand()); }
        }

        private DelegateCommand CreateDeleteEventsCommand()
        {
            Func<string> confirmationTitle = () => Messages.EventDeleteConfirmTitle;
            Func<string> confirmationText = () =>
            {
                var numberOfSessions = _selectedEvents.Count;
                return numberOfSessions == 1
                    ? Messages.EventDeleteConfirmSingular
                    : Messages.EventDeleteConfirmPlural.FormatEx(numberOfSessions);
            };
            return ConfirmedCommand.Create(DeleteEvents, confirmationTitle, confirmationText, CanDeleteEvents);
        }

        private bool CanDeleteEvents()
        {
            return _selectedEvents.Count > 0;
        }

        private void DeleteEvents()
        {
            var tempFileName = Path.GetTempFileName();
            using (var logWriter = _logFileManager.NewLogWriter(tempFileName))
            {
                for (var i = 0; i < _events.Count; i++)
                {
                    var @event = _events[i];
                    if (_selectedEvents.Contains(@event))
                    {
                        _events.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        logWriter.Write(@event.Event);
                    }
                }
            }
            File.Delete(LogFileName);
            File.Move(tempFileName, LogFileName);
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