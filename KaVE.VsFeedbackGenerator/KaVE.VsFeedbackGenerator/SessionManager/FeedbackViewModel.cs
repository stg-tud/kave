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
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using JetBrains;
using JetBrains.Annotations;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NuGet;
using Messages = KaVE.VsFeedbackGenerator.Properties.SessionManager;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public sealed class FeedbackViewModel : ViewModelBase<FeedbackViewModel>
    {
        private readonly ILogManager<IDEEvent> _logManager;
        private readonly IList<SessionViewModel> _sessions;
        private readonly IList<SessionViewModel> _selectedSessions;
        private BackgroundWorker _refreshWorker;

        private DelegateCommand _deleteCommand;

        private readonly InteractionRequest<Confirmation> _confirmationRequest;

        public delegate void SessionSelectionHandler(object sender, List<SessionViewModel> model);

        public event SessionSelectionHandler SessionSelection = delegate { };

        public delegate void EventSelectionHandler(object sender, List<EventViewModel> model);

        public event EventSelectionHandler EventSelection = delegate { };

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public FeedbackViewModel(ILogManager<IDEEvent> logManager)
        {
            _logManager = logManager;
            _sessions = new ObservableCollection<SessionViewModel>();
            _selectedSessions = new List<SessionViewModel>();
            _confirmationRequest = new InteractionRequest<Confirmation>();

            _logManager.LogsChanged += delegate { Invoke.OnSTA(Refresh); };
            SetupRefresh();
        }

        private void SetupRefresh()
        {
            // TODO Maybe introduce an specific worker that capsules the boilerplate?
            _refreshWorker = new BackgroundWorker {WorkerSupportsCancellation = false, WorkerReportsProgress = true};
            _refreshWorker.DoWork += OnRefresh;
            _refreshWorker.RunWorkerCompleted += OnRefreshCompleted;
            _refreshWorker.ProgressChanged += OnRefreshProgress;
        }

        /// <summary>
        ///     Reloads the contents of the model from the underlying log manager. Must be called from STA!
        /// </summary>
        public void Refresh()
        {
            if (!_refreshWorker.IsBusy)
            {
                SetBusy(Properties.SessionManager.Refreshing);
                _refreshWorker.RunWorkerAsync();
            }
        }

        private void OnRefresh(object worker, DoWorkEventArgs workArgs)
        {
            var bgWorker = (BackgroundWorker) worker;
            var logs = _logManager.GetLogs().ToList();
            if (logs.Any())
            {
                var progressPerFile = 100/logs.Count;
                var progress = 0;
                workArgs.Result =
                    logs.Select(
                        log =>
                        {
                            bgWorker.ReportProgress(progress);
                            var vm = new SessionViewModel(log);
                            vm.ConfirmationRequest.Raised +=
                                (sender, args) => _confirmationRequest.Delegate(args);
                            progress += progressPerFile;
                            return vm;
                        }).ToList();
            }
            bgWorker.ReportProgress(100);
        }

        private void OnRefreshProgress(object sender, ProgressChangedEventArgs e)
        {
            BusyMessage = Properties.SessionManager.Refreshing + " " + e.ProgressPercentage + "%";
        }

        private void OnRefreshCompleted(object sender, RunWorkerCompletedEventArgs args)
        {
            if (args.Error != null)
            {
                HandleRefreshError(args.Error);
            }
            else
            {
                var sessions = (IEnumerable<SessionViewModel>) args.Result;
                HandleSuccessfulRefresh(sessions);
            }
            SetIdle();
        }

        private void HandleRefreshError(Exception error)
        {
            var logEventGenerator = Registry.GetComponent<Generators.ILogger>();
            logEventGenerator.Error(error);
            Sessions = null;
        }

        private void HandleSuccessfulRefresh(IEnumerable<SessionViewModel> sessions)
        {
            var oldSelectedSessions = _selectedSessions.Select(s => s.Log).ToList();
            var oldSelectedEvents = CollectSelectedEvents();

            Sessions = sessions;

            var newSelectedSessions = Sessions.Where(s => oldSelectedSessions.Contains(s.Log)).ToList();
            if (newSelectedSessions.Any())
            {
                SessionSelection(this, newSelectedSessions);
                if (SingleSelectedSession != null)
                {
                    var newSelectedEvents =
                        SingleSelectedSession.Events.Where(e => oldSelectedEvents.Contains(e.Event)).ToList();
                    if (newSelectedEvents.Any())
                    {
                        EventSelection(this, newSelectedEvents);
                    }
                }
            }
        }

        private List<IDEEvent> CollectSelectedEvents()
        {
            if (SingleSelectedSession == null)
            {
                return new List<IDEEvent>();
            }
            return SingleSelectedSession.SelectedEvents.Select(e => e.Event).ToList();
        }

        public IEnumerable<SessionViewModel> Sessions
        {
            private set
            {
                _sessions.Clear();
                if (value != null)
                {
                    _sessions.AddRange(value);
                }
                RaisePropertyChanged(self => self.AreAnyEventsPresent);
            }
            get { return _sessions; }
        }

        public IEnumerable<SessionViewModel> SelectedSessions
        {
            set
            {
                _selectedSessions.Clear();
                _selectedSessions.AddRange(value);
                // single selected session depends on selected session
                RaisePropertyChanged(vm => vm.SingleSelectedSession);
                DeleteSessionsCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedSessions; }
        }

        [CanBeNull]
        public SessionViewModel SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        public bool AreAnyEventsPresent
        {
            get { return _sessions.Any(s => s.Events.Any()); }
        }

        public DelegateCommand DeleteSessionsCommand
        {
            get
            {
                return _deleteCommand ??
                       (_deleteCommand = new DelegateCommand(OnDeleteSelectedSessions, CanDeleteSessions));
            }
        }

        private bool CanDeleteSessions()
        {
            return _selectedSessions.Count > 0;
        }

        private void OnDeleteSelectedSessions()
        {
            var numberOfSessions = _selectedSessions.Count;
            _confirmationRequest.Raise(
                new Confirmation
                {
                    Caption = Messages.SessionDeleteConfirmTitle,
                    Message = numberOfSessions == 1
                        ? Messages.SessionDeleteConfirmSingular
                        : Messages.SessionDeleteConfirmPlural.FormatEx(numberOfSessions)
                },
                DeleteSessions);
        }

        private void DeleteSessions(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            // Changing _sessions implicitly changes _selectedSessions, what
            // leads to concurrent modification problems, if we change _session
            // in the loop. Therefore, we collect what has been successfully
            // deleted and update the UI afterwards.
            var deletedSessions = new List<SessionViewModel>();
            try
            {
                foreach (var selectedSession in _selectedSessions)
                {
                    selectedSession.Log.Delete();
                    deletedSessions.Add(selectedSession);
                }
            }
            finally
            {
                _sessions.RemoveAll(deletedSessions.Contains);
                RaisePropertyChanged(self => self.AreAnyEventsPresent);
            }
        }
    }
}