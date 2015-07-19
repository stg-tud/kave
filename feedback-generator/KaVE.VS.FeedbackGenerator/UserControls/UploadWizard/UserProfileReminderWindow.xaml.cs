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
using JetBrains.ActionManagement;
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Menu;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;


namespace KaVE.VS.FeedbackGenerator.UserControls.UploadWizard
{

    public partial class UserProfileReminderWindow : Window
    {
        private readonly IActionManager _actionManager;
        private readonly UserProfileSettings _userProfileSettings;
        private readonly KaVEISettingsStore _settingsStore;

        public UserProfileReminderWindow(IActionManager actionManager, KaVEISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            _actionManager = actionManager;

            InitializeComponent();
            
            _userProfileSettings = settingsStore.GetSettings<UserProfileSettings>();

            var userProfileContext = new UserProfileContext(
                settingsStore.GetSettings<ExportSettings>(),
                _userProfileSettings,
                new RandomizationUtils());
            UserSettingsGrid.DataContext = userProfileContext;

            userProfileContext.GenerateNewProfileId();
        }

        private void On_No_Participation_Click(object sender, RoutedEventArgs e)
        {
            _userProfileSettings.IsProvidingProfile = false;
            Close();
        }

        private void On_Ok_Click(object sender, RoutedEventArgs e)
        {
            _userProfileSettings.IsProvidingProfile = true;
           Close();
        }

        private void UserProfileReminderWindow_OnClosed(object sender, EventArgs e)
        {
            _userProfileSettings.HasBeenAskedtoProvideProfile = true;
            OpenUploadWizard();
        }

        private void OpenUploadWizard()
        {
            _settingsStore.SetSettings(_userProfileSettings);
            _actionManager.ExecuteAction<UploadWizardAction>();
        }
    }
}
