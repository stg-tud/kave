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
using System.ComponentModel;
using System.Linq;
using JetBrains;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.ReSharper.Commons.Utils;
using KaVE.ReSharper.Commons.Utils.Logging;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NuGet;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public class SessionViewModel : ViewModelBase<SessionViewModel>
    {
        public ILog Log { get; private set; }
        private bool _isLoaded;
        private readonly BackgroundWorker<ICollection<EventViewModel>> _logLoader;

        private ICollection<EventViewModel> _events = new DispatchingObservableCollection<EventViewModel>();
        private ICollection<EventViewModel> _selectedEvents = new DispatchingObservableCollection<EventViewModel>();

        private readonly InteractionRequest<Confirmation> _confirmationRequest = new InteractionRequest<Confirmation>();

        private DelegateCommand _deleteEventsCommand;

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public SessionViewModel(ILog log)
        {
            Log = log;
            Log.EntriesRemoved += OnEntriesRemoved;
            Log.EntryAppended += OnEntryAdded;

            _logLoader = CreateBackgroundLogLoader();
        }

        private void OnEntryAdded(IDEEvent entry)
        {
            _events.Add(CreateEventViewModel(entry));
        }

        private void OnEntriesRemoved(IEnumerable<IDEEvent> entries)
        {
            _events.RemoveAll(vm => entries.Contains(vm.Event));
        }

        public DateTime StartDate
        {
            get { return Log.Date; }
        }

        public ICollection<EventViewModel> Events
        {
            private set
            {
                _events = value;
                RaisePropertyChanged(self => self.Events);
                RaisePropertyChanged(self => self.SingleSelectedEvent);
            }

            get
            {
                if (!_isLoaded)
                {
                    SetBusy(Messages.Loading);
                    _isLoaded = true;
                    _logLoader.RunWorkerAsync();
                }
                return _events;
            }
        }

        private BackgroundWorker<ICollection<EventViewModel>> CreateBackgroundLogLoader()
        {
            var logLoader = new BackgroundWorker<ICollection<EventViewModel>>();
            logLoader.DoWork += LoadLog;
            logLoader.WorkCompleted += OnLoadCompleted;
            logLoader.WorkFailed += OnLoadFailed;
            return logLoader;
        }

        private ICollection<EventViewModel> LoadLog(BackgroundWorker worker)
        {
            return Log.ReadAll().Select(CreateEventViewModel).ToList();
        }

        private static EventViewModel CreateEventViewModel(IDEEvent evt)
        {
            return new EventViewModel(evt);
        }

        private void OnLoadCompleted(ICollection<EventViewModel> result)
        {
            var previousSelection = new List<EventViewModel>(SelectedEvents);

            Events = new DispatchingObservableCollection<EventViewModel>(result);

            if (previousSelection.Any())
            {
                var selection = Events.Where(previousSelection.Contains);
                SelectedEvents = new DispatchingObservableCollection<EventViewModel>(selection);
            }
            SetIdle();
        }

        private void OnLoadFailed(Exception e)
        {
            Registry.GetComponent<ILogger>().Error(new Exception("could not read log", e));
            Events.Clear();
            SetIdle();
        }

        public ICollection<EventViewModel> SelectedEvents
        {
            set
            {
                _selectedEvents = value;
                RaisePropertyChanged(self => self.SelectedEvents);
                RaisePropertyChanged(self => self.SingleSelectedEvent);
                DeleteEventsCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedEvents; }
        }

        public EventViewModel SingleSelectedEvent
        {
            get { return _selectedEvents.Count == 1 ? _selectedEvents.First() : null; }
        }

        public void Refresh()
        {
            _isLoaded = false;
        }

        public DelegateCommand DeleteEventsCommand
        {
            get { return _deleteEventsCommand ?? (_deleteEventsCommand = new DelegateCommand(DeleteSelectedEvents, () => HasSelection)); }
        }

        public bool HasSelection
        {
            get { return SelectedEvents.Any(); }
        }

        public void DeleteSelectedEvents()
        {
            var numberOfEvents = SelectedEvents.Count;
            _confirmationRequest.Raise(
                new Confirmation
                {
                    Caption = Messages.EventDeleteConfirmTitle,
                    Message = numberOfEvents == 1
                        ? Messages.EventDeleteConfirmSingular
                        : Messages.EventDeleteConfirmPlural.FormatEx(numberOfEvents)
                },
                DoDeleteSelectedEvents);
        }

        private void DoDeleteSelectedEvents(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            Log.RemoveRange(SelectedEvents.Select(evm => evm.Event));
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