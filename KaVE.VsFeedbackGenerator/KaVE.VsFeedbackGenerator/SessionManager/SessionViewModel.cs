using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using JetBrains;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionViewModel : ViewModelBase<SessionViewModel>
    {
        private readonly ILogFileManager<IDEEvent> _logFileManager;
        public string LogFileName { get; private set; }
        private readonly IList<EventView> _events = new ObservableCollection<EventView>();
        private readonly IList<EventView> _selectedEvents = new List<EventView>();
        private readonly InteractionRequest<Confirmation> _confirmationRequest = new InteractionRequest<Confirmation>();

        public SessionViewModel(ILogFileManager<IDEEvent> logFileManager, string logFileName)
        {
            _logFileManager = logFileManager;
            LogFileName = logFileName;
            DeleteEventsCommand = new DelegateCommand(OnDeleteSelectedEvents, CanDeleteEvents);
            // loading eagerly because lazy approaches led to UI display bugs
            // TODO if this should cause memory problems, we have to find a lazier solution...
            using (var logReader = logFileManager.NewLogReader(logFileName))
            {
                Events = logReader.ReadAll().Select(evt => new EventView(evt)).ToList();
            }
        }

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
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
                OnPropertyChanged(vm => vm.SingleSelectedEvent);
                DeleteEventsCommand.RaiseCanExecuteChanged();
            }
        }

        public EventView SingleSelectedEvent
        {
            get
            {
                return _selectedEvents.Count == 1 ? _selectedEvents.First() : null;
            }
        }

        public DelegateCommand DeleteEventsCommand { get; private set; }

        private void OnDeleteSelectedEvents()
        {
            var numberOfEvents = _selectedEvents.Count;
            _confirmationRequest.Raise(
                new Confirmation
                {
                    Caption = Messages.EventDeleteConfirmTitle,
                    Message = numberOfEvents == 1
                        ? Messages.EventDeleteConfirmSingular
                        : Messages.EventDeleteConfirmPlural.FormatEx(numberOfEvents)
                },
                DeleteEvents);
        }

        private bool CanDeleteEvents()
        {
            return _selectedEvents.Count > 0;
        }

        private void DeleteEvents(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            // TODO This logic should be moved out of here, but where to?
            var tempFileName = Path.GetTempFileName();
            using (var logWriter = new JsonLogWriter<IDEEvent>(new FileStream(tempFileName, FileMode.Append, FileAccess.Write)))
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
    }
}