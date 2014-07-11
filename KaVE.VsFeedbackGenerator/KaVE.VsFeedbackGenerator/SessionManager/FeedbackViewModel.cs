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
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Export;
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
        private readonly IActionManager _actionManager;
        private readonly IList<SessionViewModel> _sessions;
        private readonly IList<SessionViewModel> _selectedSessions;
        private BackgroundWorker _refreshWorker;

        private DelegateCommand _exportCommand;
        private DelegateCommand _deleteCommand;

        private readonly InteractionRequest<Confirmation> _confirmationRequest;

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public FeedbackViewModel(ILogManager<IDEEvent> logManager, IActionManager actionManager)
        {
            _logManager = logManager;
            _actionManager = actionManager;
            _sessions = new ObservableCollection<SessionViewModel>();
            _selectedSessions = new List<SessionViewModel>();
            _confirmationRequest = new InteractionRequest<Confirmation>();

            _logManager.LogsChanged += delegate { Invoke.OnSTA(Refresh); };
            SetupRefresh();
        }

        private void SetupRefresh()
        {
            // TODO Maybe introduce an specific worker that capsules the boilerplate?
            _refreshWorker = new BackgroundWorker {WorkerSupportsCancellation = false};
            _refreshWorker.DoWork += OnRefresh;
            _refreshWorker.RunWorkerCompleted += OnRefreshCompleted;
        }

        /// <summary>
        ///     Reloads the contents of the model from the underlying log manager. Must be called from STA!
        /// </summary>
        public void Refresh()
        {
            SetBusy(Properties.SessionManager.Refreshing);
            _refreshWorker.RunWorkerAsync();
        }

        private void OnRefresh(object worker, DoWorkEventArgs workArgs)
        {
            workArgs.Result =
                _logManager.GetLogs().Select(
                    log =>
                    {
                        var vm = new SessionViewModel(log);
                        vm.ConfirmationRequest.Raised +=
                            (sender, args) => _confirmationRequest.Delegate(args);
                        return vm;
                    });
        }

        private void OnRefreshCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            if (runWorkerCompletedEventArgs.Error != null)
            {
                var logEventGenerator = Registry.GetComponent<Generators.ILogger>();
                logEventGenerator.Error(runWorkerCompletedEventArgs.Error);
                Sessions = null;
            }
            else
            {
                Sessions = (IEnumerable<SessionViewModel>) runWorkerCompletedEventArgs.Result;
            }
            SetIdle();
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
                ExportCommand.RaiseCanExecuteChanged();
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
                OnPropertyChanged(vm => vm.SingleSelectedSession);
                DeleteSessionsCommand.RaiseCanExecuteChanged();
            }
        }

        [CanBeNull]
        public SessionViewModel SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
        }

        public DelegateCommand ExportCommand
        {
            get { return _exportCommand ?? (_exportCommand = new DelegateCommand(OnExport, AreAnyEventsPresent)); }
        }

        private void OnExport()
        {
            UploadWizardActionHandler.Execute(_actionManager);
        }

        private bool AreAnyEventsPresent()
        {
            return _sessions.Any(s => s.Events.Any());
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
            }
        }
    }
}