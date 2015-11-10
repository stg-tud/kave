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
using System.IO;
using System.Linq;
using System.Windows.Input;
using JetBrains.Util;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.UsageModelOptions
{
    public class ModelStoreValidation
    {
        public ModelStoreValidation(bool isPathValid)
        {
            IsPathValid = isPathValid;
        }

        public bool IsPathValid { get; private set; }

        public bool IsValidModelStoreInformation
        {
            get { return IsPathValid; }
        }
    }

    public class UsageModelOptionsViewModel : ViewModelBase<UsageModelOptionsViewModel>
    {
        public IInteractionRequest<Notification> ErrorNotificationRequest
        {
            get { return _errorNotificationRequest; }
        }

        public ModelStoreSettings ModelStoreSettings { get; set; }

        public string ModelPath
        {
            get { return ModelStoreSettings.ModelStorePath; }
            set
            {
                ModelStoreSettings.ModelStorePath = value;
                RaisePropertyChanged(self => self.ModelPath);
            }
        }

        public string ModelUri
        {
            get { return ModelStoreSettings.ModelStoreUri; }
            set
            {
                ModelStoreSettings.ModelStoreUri = value;
                RaisePropertyChanged(self => self.ModelUri);
            }
        }

        public IEnumerable<IUsageModelsTableRow> UsageModelsTableContent
        {
            get { return _usageModelsTableContent; }
            set
            {
                _usageModelsTableContent = value;
                RaisePropertyChanged(self => self.UsageModelsTableContent);
            }
        }

        private IEnumerable<IUsageModelsTableRow> _usageModelsTableContent;

        public AsyncCommand InstallAllModelsCommand
        {
            get { return new AsyncCommand(InstallAllModels, _usageModelCommandsWorker); }
        }

        public AsyncCommand UpdateAllModelsCommand
        {
            get { return new AsyncCommand(UpdateAllModels, _usageModelCommandsWorker); }
        }

        public AsyncCommand RemoveAllModelsCommand
        {
            get { return new AsyncCommand(RemoveAllModels, _usageModelCommandsWorker); }
        }

        public AsyncCommand<IUsageModelsTableRow> InstallModel
        {
            get
            {
                var installCommand = new AsyncCommand<IUsageModelsTableRow>(
                    row =>
                    {
                        RunningCommandMessage = string.Format("Installing model for {0}", row.TypeName);
                        row.LoadModel();
                    },
                    row => row != null && row.IsInstallable,
                    _usageModelCommandsWorker);
                return installCommand;
            }
        }

        public AsyncCommand<IUsageModelsTableRow> UpdateModel
        {
            get
            {
                var updateCommand = new AsyncCommand<IUsageModelsTableRow>(
                    row =>
                    {
                        RunningCommandMessage = string.Format("Updating model for {0}", row.TypeName);
                        row.LoadModel();
                    },
                    row => row != null && row.IsUpdateable,
                    _usageModelCommandsWorker);
                return updateCommand;
            }
        }

        public AsyncCommand<IUsageModelsTableRow> RemoveModel
        {
            get
            {
                var removeCommand = new AsyncCommand<IUsageModelsTableRow>(
                    row =>
                    {
                        RunningCommandMessage = string.Format("Removing model for {0}", row.TypeName);;
                        row.RemoveModel();
                    },
                    row => row != null && row.IsRemoveable,
                    _usageModelCommandsWorker);
                return removeCommand;
            }
        }

        public DelegateCommand CancelCurrentCommand
        {
            get
            {
                return new DelegateCommand(
                    () => { _cancelCurrentCommand = true; },
                    () => true);
            }
        }

        public int CurrentUsageModelCommandProgressValue
        {
            get { return _currentUsageModelCommandProgressValue; }
            private set
            {
                _currentUsageModelCommandProgressValue = value;
                RaisePropertyChanged(self => self.CurrentUsageModelCommandProgressValue);
            }
        }

        private int _currentUsageModelCommandProgressValue;

        public int MaximumUsageModelCommandProgressValue
        {
            get { return _maximumUsageModelCommandProgressValue; }
            set
            {
                _maximumUsageModelCommandProgressValue = value;
                RaisePropertyChanged(self => self.MaximumUsageModelCommandProgressValue);
            }
        }

        private int _maximumUsageModelCommandProgressValue;

        public bool RunningUsageModelCommand
        {
            get { return _runningUsageModelCommand; }
            set
            {
                _runningUsageModelCommand = value;
                RaisePropertyChanged(self => self.RunningUsageModelCommand);
                RaisePropertyChanged(self => self.RunnerIsIdle);
            }
        }
        
        private bool _runningUsageModelCommand;

        public bool RunnerIsIdle
        {
            get { return !RunningUsageModelCommand; }
        }

        public string RunningCommandMessage
        {
            get
            {
                return _runningCommandMessage;
            }
            set
            {
                _runningCommandMessage = value;
                RaisePropertyChanged(self => self.RunningCommandMessage);
            }
        }

        private string _runningCommandMessage;

        private bool _cancelCurrentCommand;

        [CanBeNull]
        private static ILocalPBNRecommenderStore LocalStore
        {
            get
            {
                try
                {
                    return Registry.GetComponent<ILocalPBNRecommenderStore>();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        [CanBeNull]
        private static IRemotePBNRecommenderStore RemoteStore
        {
            get
            {
                try
                {
                    return Registry.GetComponent<IRemotePBNRecommenderStore>();
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        private readonly InteractionRequest<Notification> _errorNotificationRequest;

        [NotNull]
        private readonly IKaVEBackgroundWorker _usageModelCommandsWorker;

        public UsageModelOptionsViewModel([NotNull] IUsageModelMergingStrategy mergingStrategy,
            [NotNull] IKaVEBackgroundWorker usageModelCommandsWorker)
        {
            _errorNotificationRequest = new InteractionRequest<Notification>();
            _usageModelCommandsWorker = usageModelCommandsWorker;
            _usageModelCommandsWorker.DoWork += delegate
            {
                _cancelCurrentCommand = false;
                RunningUsageModelCommand = true;
            };
            _usageModelCommandsWorker.RunWorkerCompleted += delegate
            {
                _cancelCurrentCommand = false;
                RunningUsageModelCommand = false;
                ReloadUsageModelsTableContent(mergingStrategy);
            };

            ReloadUsageModelsTableContent(mergingStrategy);
        }
        
        private void ReloadUsageModelsTableContent(IUsageModelMergingStrategy mergingStrategy)
        {
            UsageModelsTableContent = mergingStrategy.MergeAvailableModels(LocalStore, RemoteStore);
        }

        private void InstallAllModels()
        {
            var installableRows = UsageModelsTableContent.Where(row => row.IsInstallable).AsArray();
            MaximumUsageModelCommandProgressValue = installableRows.Length;
            CurrentUsageModelCommandProgressValue = 0;
            foreach (var row in installableRows)
            {
                if (_cancelCurrentCommand)
                {
                    return;
                }

                RunningCommandMessage = "Installing " + row.TypeName;
                row.LoadModel();
                CurrentUsageModelCommandProgressValue++;
            }
        }

        private void UpdateAllModels()
        {
            var updateableRows = UsageModelsTableContent.Where(row => row.IsUpdateable).AsArray();
            MaximumUsageModelCommandProgressValue = updateableRows.Length;
            CurrentUsageModelCommandProgressValue = 0;
            foreach (var row in updateableRows)
            {
                if (_cancelCurrentCommand)
                {
                    return;
                }

                RunningCommandMessage = "Updating " + row.TypeName;
                row.LoadModel();
                CurrentUsageModelCommandProgressValue++;
            }
        }

        private void RemoveAllModels()
        {
            var removeableRows = UsageModelsTableContent.Where(row => row.IsRemoveable).AsArray();
            MaximumUsageModelCommandProgressValue = removeableRows.Length;
            CurrentUsageModelCommandProgressValue = 0;
            foreach (var row in removeableRows)
            {
                if (_cancelCurrentCommand)
                {
                    return;
                }

                RunningCommandMessage = "Removing " + row.TypeName;
                row.RemoveModel();
                CurrentUsageModelCommandProgressValue++;
            }
        }

        public ModelStoreValidation ValidateModelStoreInformation(string path)
        {
            var pathIsValid = ValidatePath(path);

            if (!pathIsValid)
            {
                ShowInformationInvalidMessage(Properties.SessionManager.OptionPageInvalidModelStorePathMessage);
            }

            return new ModelStoreValidation(pathIsValid);
        }

        private void ShowInformationInvalidMessage(string message)
        {
            _errorNotificationRequest.Raise(
                new Notification
                {
                    Caption = Properties.SessionManager.Options_Title,
                    Message = message
                });
        }

        private static bool ValidatePath(string path)
        {
            try
            {
                return new DirectoryInfo(path).Exists;
            }
            catch
            {
                return false;
            }
        }
    }
}