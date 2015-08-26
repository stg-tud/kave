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
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using JetBrains;
using JetBrains.Annotations;
using KaVE.Commons.Utils.Collections;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;
using KaVE.VS.FeedbackGenerator.Utils.Export;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using NuGet;

namespace KaVE.VS.FeedbackGenerator.SessionManager
{
    public sealed class FeedbackViewModel : ViewModelBase<FeedbackViewModel>
    {
        private readonly ILogManager _logManager;
        private ICollection<SessionViewModel> _sessions = new DispatchingObservableCollection<SessionViewModel>();

        private ICollection<SessionViewModel> _selectedSessions =
            new DispatchingObservableCollection<SessionViewModel>();

        private BackgroundWorker<IList<SessionViewModel>> _refreshWorker;
        private DelegateCommand _deleteCommand;
        private readonly InteractionRequest<Confirmation> _confirmationRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest<Confirmation> ConfirmationRequest
        {
            get { return _confirmationRequest; }
        }

        public FeedbackViewModel(ILogManager logManager, IExporter exporter)
        {
            _logManager = logManager;

            SetupRefresh();
            _logManager.LogCreated += OnLogCreated;

            exporter.ExportStarted += () => SetBusy("");
            exporter.ExportEnded += SetIdle;
            exporter.StatusChanged += SetBusy;
        }

        private void OnLogCreated(ILog log)
        {
            _sessions.Add(CreateOrRefreshSessionViewModel(log));
        }

        private void SetupRefresh()
        {
            // TODO Maybe introduce an specific worker that capsules the boilerplate?
            _refreshWorker = new BackgroundWorker<IList<SessionViewModel>> {WorkerReportsProgress = true};
            _refreshWorker.DoWork += OnRefresh;
            _refreshWorker.ProgressChanged += OnRefreshProgress;
            _refreshWorker.WorkCompleted += OnRefreshCompleted;
            _refreshWorker.WorkFailed += OnRefreshFailed;
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

        private IList<SessionViewModel> OnRefresh(BackgroundWorker worker)
        {
            var logs = _logManager.Logs.ToList();
            var progressPerFile = logs.IsEmpty() ? 0 : 100/logs.Count;
            var progress = 0;
            worker.ReportProgress(progress);
            return
                logs.Select(
                    log =>
                    {
                        var vm = CreateOrRefreshSessionViewModel(log);
                        progress += progressPerFile;
                        worker.ReportProgress(progress);
                        return vm;
                    }).ToList();
        }

        private SessionViewModel CreateOrRefreshSessionViewModel(ILog log)
        {
            log.Deleted += OnLogDeleted;
            var viewModel = Sessions.FirstOrDefault(session => session.Log.Equals(log));
            if (viewModel == null)
            {
                viewModel = new SessionViewModel(log);
                viewModel.ConfirmationRequest.Raised += (sender, args) => _confirmationRequest.Delegate(args);
                RegisterSubViewModel(viewModel);
            }
            else
            {
                viewModel.Refresh();
            }
            return viewModel;
        }

        private void OnRefreshProgress(int percentageProgressed)
        {
            BusyMessage = Properties.SessionManager.Refreshing + " " + percentageProgressed + "%";
        }

        private void OnRefreshCompleted(IList<SessionViewModel> result)
        {
            var previousSelection = new List<SessionViewModel>(_selectedSessions);

            Sessions = new DispatchingObservableCollection<SessionViewModel>(result);

            SetIdle();
            if (previousSelection.Any())
            {
                var newSelectedSessions = Sessions.Where(previousSelection.Contains).ToList();
                SelectedSessions = new DispatchingObservableCollection<SessionViewModel>(newSelectedSessions);
            }
        }

        private void OnRefreshFailed(Exception e)
        {
            var logEventGenerator = Registry.GetComponent<Commons.Utils.Exceptions.ILogger>();
            logEventGenerator.Error(new Exception("refresh failed", e));
            // TODO send error notification event to inform user!
            Sessions.Clear();
            SetIdle();
        }

        [NotNull]
        public ICollection<SessionViewModel> Sessions
        {
            private set
            {
                _sessions = value;
                RaisePropertyChanged(self => self.Sessions);
            }
            get { return _sessions; }
        }

        [NotNull]
        public ICollection<SessionViewModel> SelectedSessions
        {
            set
            {
                _selectedSessions = value;
                RaisePropertyChanged(self => self.SelectedSessions);
                RaisePropertyChanged(self => self.SingleSelectedSession);
                DeleteSessionsCommand.RaiseCanExecuteChanged();
            }
            get { return _selectedSessions; }
        }

        [CanBeNull]
        public SessionViewModel SingleSelectedSession
        {
            get { return _selectedSessions.Count == 1 ? _selectedSessions.First() : null; }
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
                    Caption = Properties.SessionManager.SessionDeleteConfirmTitle,
                    Message = numberOfSessions == 1
                        ? Properties.SessionManager.SessionDeleteConfirmSingular
                        : Properties.SessionManager.SessionDeleteConfirmPlural.FormatEx(numberOfSessions)
                },
                DeleteSessions);
        }

        private bool _bulkDelete;

        private void DeleteSessions(Confirmation confirmation)
        {
            if (!confirmation.Confirmed)
            {
                return;
            }

            // Removing sessions while iterating here, leads to strange
            // Schroeding bugs in production mode (seems some UI-update
            // events interfere with the deletion).
            _bulkDelete = true;
            var deletedLogs = new List<ILog>();
            foreach (var selectedSession in _selectedSessions)
            {
                selectedSession.Log.Delete();
                deletedLogs.Add(selectedSession.Log);
            }
            _bulkDelete = false;

            deletedLogs.ForEach(OnLogDeleted);
        }

        private void OnLogDeleted(ILog log)
        {
            if (!_bulkDelete)
            {
                _sessions.RemoveAll(svm => svm.Log.Equals(log));
                log.Deleted -= OnLogDeleted;
            }
        }
    }
}