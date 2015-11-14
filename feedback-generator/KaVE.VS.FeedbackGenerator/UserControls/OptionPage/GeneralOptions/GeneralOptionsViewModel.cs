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

using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.SessionManager;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.ValidationRules;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions
{
    public class GeneralOptionsViewModel : ViewModelBase<GeneralOptionsViewModel>
    {
        public string UploadUrl
        {
            get { return _uploadUrl; }
            set
            {
                _uploadUrl = value;
                RaisePropertyChanged(self => self.UploadUrl);
            }
        }

        private string _uploadUrl;

        public string WebAccessPrefix
        {
            get { return _webAccessPrefix; }
            set
            {
                _webAccessPrefix = value;
                RaisePropertyChanged(self => self.WebAccessPrefix);
            }
        }

        private string _webAccessPrefix;

        public GeneralOptionsViewModel([NotNull] ISettingsStore settingsStore)
        {
            _uploadUrl = settingsStore.GetSettings<ExportSettings>().UploadUrl;
            _webAccessPrefix = settingsStore.GetSettings<ExportSettings>().WebAccessPrefix;
        }

        public bool SaveSettings(ExportSettings settings)
        {
            var urlIsValid = UploadUrlValidationRule.Validate(UploadUrl).IsValid;
            if (urlIsValid)
            {
                settings.UploadUrl = UploadUrl;
            }

            var prefixIsValid = UploadUrlValidationRule.Validate(WebAccessPrefix).IsValid;
            if (prefixIsValid)
            {
                settings.WebAccessPrefix = WebAccessPrefix;
            }

            return urlIsValid && prefixIsValid;
        }
    }
}