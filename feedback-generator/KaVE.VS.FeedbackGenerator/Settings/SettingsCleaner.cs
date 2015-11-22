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

using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using KaVE.RS.Commons.Settings;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.Utils.Logging;

namespace KaVE.VS.FeedbackGenerator.Settings
{
    [Action(ActionId)]
    public class SettingsCleaner : IExecutableAction
    {
        public const string ActionId = "KaVE.VsFeedbackGenerator.ResetAll";

        private readonly ISettingsStore _settings;
        private readonly ILogManager _logManager;

        public SettingsCleaner()
        {
            _settings = Registry.GetComponent<ISettingsStore>();
            _logManager = Registry.GetComponent<ILogManager>();
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true; // always active
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            var restoreType = context.GetData(SettingDataContextCreator.DataConstant);
            if (restoreType != null)
            {
                if (restoreType.ResetType == ResetTypes.Feedback)
                {
                    ResetFeedback();
                }
                else
                {
                    RestoreSettings(restoreType.ResetType);
                }
            }
        }

        private void ResetFeedback()
        {
            _logManager.DeleteAllLogs();
        }

        private void RestoreSettings(ResetTypes settingType)
        {
            // WARNING: Do not reset FeedbackSettings, as it is used to store entries that have to be consistent over time
            switch (settingType)
            {
                case ResetTypes.GeneralSettings:
                    _settings.ResetSettings<UploadSettings>();
                    _settings.ResetSettings<ExportSettings>();
                    break;
                case ResetTypes.UserProfileSettings:
                    _settings.ResetSettings<UserProfileSettings>();
                    break;
                case ResetTypes.AnonymizationSettings:
                    _settings.ResetSettings<AnonymizationSettings>();
                    break;
                case ResetTypes.ModelStoreSettings:
                    _settings.ResetSettings<ModelStoreSettings>();
                    break;
            }
        }
    }
}