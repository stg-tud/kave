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
using KaVE.Commons.Utils.CodeCompletion.Stores;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.SessionManager;

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
        private readonly InteractionRequest<Notification> _errorNotificationRequest;

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
                if (!value.EndsWith("\\"))
                {
                    value = value + "\\";
                }
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

        [NotNull]
        private readonly IUsageModelMergingStrategy _mergingStrategy;

        public UsageModelOptionsViewModel([NotNull] IUsageModelMergingStrategy mergingStrategy)
        {
            _mergingStrategy = mergingStrategy;
            _errorNotificationRequest = new InteractionRequest<Notification>();
            ReloadUsageModelsTableContent();
        }

        public void ReloadUsageModelsTableContent()
        {
            if (LocalStore != null)
            {
                LocalStore.ReloadAvailableModels();
            }
            if (RemoteStore != null)
            {
                RemoteStore.ReloadAvailableModels();
            }

            UsageModelsTableContent = _mergingStrategy.MergeAvailableModels(LocalStore, RemoteStore);
        }

        public void UpdateAllModels()
        {
            foreach (var row in UsageModelsTableContent)
            {
                if (row.UpdateModel.CanExecute(null))
                {
                    row.UpdateModel.Execute(null);
                }
                if (row.InstallModel.CanExecute(null))
                {
                    row.InstallModel.Execute(null);
                }
            }

            ReloadUsageModelsTableContent();
        }

        public void RemoveAllModels()
        {
            foreach (var row in UsageModelsTableContent)
            {
                if (row.RemoveModel.CanExecute(null))
                {
                    row.RemoveModel.Execute(null);
                }
            }

            ReloadUsageModelsTableContent();
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