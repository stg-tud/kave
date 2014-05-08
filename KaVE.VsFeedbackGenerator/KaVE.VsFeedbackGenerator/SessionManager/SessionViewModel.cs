using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains;
using JetBrains.UI.Extensions.Commands;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionViewModel : ViewModelBase<SessionViewModel>
    {
        public ILog<IDEEvent> Log { get; private set; }
        private readonly IList<EventView> _events = new ObservableCollection<EventView>();
        private readonly IList<EventView> _selectedEvents = new List<EventView>();
        private readonly InteractionRequest<Confirmation> _confirmationRequest = new InteractionRequest<Confirmation>();

        public SessionViewModel(ILog<IDEEvent> log)
        {
            Log = log;
            DeleteEventsCommand = new DelegateCommand(OnDeleteSelectedEvents, CanDeleteEvents);
            // loading eagerly because lazy approaches led to UI display bugs
            // TODO if this should cause memory problems, we have to find a lazier solution...
            using (var logReader = log.NewLogReader())
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
            get
            {
                return Log.Date;
            }
        }

        public IEnumerable<EventView> Events
        {
            set
            {
                _events.Clear();
                CollectionExtensions.AddRange(_events, value);
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
                CollectionExtensions.AddRange(_selectedEvents, value);
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

            // Changing _events implicitly changes _selectedEvents, what
            // leads to concurrent modification problems, since RemoveRange()
            // loops over the selection. Therefore, we copy the selection.
            // TODO this can lead to events being shown in the UI that don't exist in the log file! Somehow force refresh on error?
            var selection = new List<EventView>(_selectedEvents);
            Log.RemoveRange(selection.Select(evm => evm.Event));
            _events.RemoveRange(selection);
        }

        public override string ToString()
        {
            return "[SessionViewModel: " + Log + "]";
        }
    }
}