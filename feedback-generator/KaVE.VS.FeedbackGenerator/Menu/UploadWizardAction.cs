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

using System.IO;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using NuGet;

namespace KaVE.VS.FeedbackGenerator.Menu
{
    [Action(Id, "Export Feedback...", Id = 12193486)]
    public class UploadWizardAction : IExecutableAction
    {
        internal const string Id = "KaVE.VS.FeedbackGenerator.UploadWizard";

        private readonly ISettingsStore _settingsStore;
        private readonly IUploadWizardWindowCreator _uploadWizardWindowCreator;
        private readonly ILogManager _logManager;
        
        public UploadWizardAction()
        {
            _settingsStore = Registry.GetComponent<ISettingsStore>();
            _uploadWizardWindowCreator = Registry.GetComponent<IUploadWizardWindowCreator>();
            _logManager = Registry.GetComponent<ILogManager>();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            if (ShouldShowUserProfileReminder())
            {
                _uploadWizardWindowCreator.OpenUserProfileReminderDialog();
            }
            else
            {
                if (HasContentToExport())
                {
                    _uploadWizardWindowCreator.OpenUploadWizardControl();
                }
                else
                {
                    _uploadWizardWindowCreator.OpenNothingToExportDialog();
                }
            }
        }

        private bool HasContentToExport()
        {
            try
            {
                var logs = _logManager.Logs.ToList();
                var noLogs = EnumerableExtensions.IsEmpty(logs);
                var singleEmptyLog = logs.Count == 1 && logs[0].IsEmpty();
                return !noLogs && !singleEmptyLog;
            }
            catch (DirectoryNotFoundException)
            {
                // directly after the reset, before any event is generated
                return false;
            }
        }

        private bool ShouldShowUserProfileReminder()
        {
            var userProfileSettings = _settingsStore.GetSettings<UserProfileSettings>();
            var exportSettings= _settingsStore.GetSettings<ExportSettings>();
            return !userProfileSettings.HasBeenAskedtoProvideProfile && !userProfileSettings.IsProvidingProfile && !exportSettings.IsDatev;
        }
    }
}