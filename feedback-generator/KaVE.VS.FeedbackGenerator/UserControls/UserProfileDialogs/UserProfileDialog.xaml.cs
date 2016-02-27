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
using System.Windows;
using KaVE.Commons.Utils;
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

        public UserProfileDialog(IActionExecutor actionExec,
            KaVEISettingsStore settingsStore)
        {
            _actionExec = actionExec;
            _settingsStore = settingsStore;

            InitializeComponent();

            _userProfileSettings = settingsStore.GetSettings<UserProfileSettings>();

            var userProfileContext = new UserProfileContext(
                _userProfileSettings,
                new RandomizationUtils());
            DataContext = userProfileContext;

            userProfileContext.GenerateNewProfileId();
        }

        private void OnClickAbort(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnClickFinish(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UserProfileReminderWindow_OnClosed(object sender, EventArgs e)
        {
            _userProfileSettings.HasBeenAskedToFillProfile = true;
            OpenUploadWizard();
        }

        private void OpenUploadWizard()
        {
            _settingsStore.SetSettings(_userProfileSettings);
            _actionExec.ExecuteActionGuarded<UploadWizardAction>();
        }
    }
}