/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains;
using JetBrains.UI.Extensions.Commands;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionViewModel : ViewModelBase<SessionViewModel>
    {
        public ILog<IDEEvent> Log { get; private set; }
        private readonly IList<EventViewModel> _events = new ObservableCollection<EventViewModel>();
        private readonly IList<EventViewModel> _selectedEvents = new List<EventViewModel>();
        private readonly InteractionRequest<Confirmation> _confirmationRequest = new InteractionRequest<Confirmation>();

        public SessionViewModel(ILog<IDEEvent> log)
        {
            Log = log;
            // loading eagerly because lazy approaches led to UI display bugs
            using (var logReader = log.NewLogReader())
            {
                Events = logReader.ReadAll().Select(evt => new EventViewModel(evt)).ToList();
            }
        }

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public DateTime StartDate
        {
            get { return Log.Date; }
        }

        public IEnumerable<EventViewModel> Events
        {
            set
            {
                _events.Clear();
                CollectionExtensions.AddRange(_events, value);
            }

            get { return _events; }
        }

        public IEnumerable<EventViewModel> SelectedEvents
        {
            set
            {
                _selectedEvents.Clear();
                CollectionExtensions.AddRange(_selectedEvents, value);
                // notify listeners about dependent property cange
                RaisePropertyChanged(vm => vm.SingleSelectedEvent);
                DeleteEventsCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedEvents; }
        }

        public EventViewModel SingleSelectedEvent
        {
            get { return _selectedEvents.Count == 1 ? _selectedEvents.First() : null; }
        }

        private DelegateCommand _deleteEventsCommand;

        public DelegateCommand DeleteEventsCommand
        {
            get
            {
                return
                    _deleteEventsCommand ??
                    (_deleteEventsCommand = new DelegateCommand(OnDeleteSelectedEvents, CanDeleteEvents));
            }
        }

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
            var selection = new List<EventViewModel>(_selectedEvents);
            Log.RemoveRange(selection.Select(evm => evm.Event));
            _events.RemoveRange(selection);
        }

        protected bool Equals(SessionViewModel other)
        {
            return string.Equals(Log, other.Log);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return Log.GetHashCode();
        }

        public override string ToString()
        {
            return "[SessionViewModel: " + Log + "]";
        }
    }
}