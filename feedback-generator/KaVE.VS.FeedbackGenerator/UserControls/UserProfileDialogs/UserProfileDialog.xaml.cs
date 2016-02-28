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

using System.Windows;
using KaVE.RS.Commons;
using KaVE.VS.FeedbackGenerator.Menu;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;


namespace KaVE.VS.FeedbackGenerator.UserControls.UserProfileDialogs
{
    public partial class UserProfileDialog
    {
        private readonly UserProfileSettings _userProfileSettings;
        private readonly IActionExecutor _actionExec;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly UploadWizardPolicy _policy;

        public UserProfileDialog(IActionExecutor actionExec,
            KaVEISettingsStore settingsStore,
            UploadWizardPolicy policy)
        {
            _actionExec = actionExec;
            _settingsStore = settingsStore;
            _policy = policy;

            InitializeComponent();

            _userProfileSettings = settingsStore.GetSettings<UserProfileSettings>();

            var userProfileContext = new UserProfileContext(_userProfileSettings);
            DataContext = userProfileContext;
        }

        private void UpdateUserProfileSettings()
        {
            _userProfileSettings.HasBeenAskedToFillProfile = true;
            _settingsStore.SetSettings(_userProfileSettings);
        }

        private void OnClickAbort(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClickFinish(object sender, RoutedEventArgs e)
        {
            Close();
            UpdateUserProfileSettings();
            if (_policy == UploadWizardPolicy.OpenUploadWizardOnFinish)
            {
                OpenUploadWizard();
            }
        }

        private void OpenUploadWizard()
        {
            _actionExec.ExecuteActionGuarded<UploadWizardAction>();
        }
    }

    public enum UploadWizardPolicy
    {
        OpenUploadWizardOnFinish,
        DoNotOpenUploadWizardOnFinish
    }
}