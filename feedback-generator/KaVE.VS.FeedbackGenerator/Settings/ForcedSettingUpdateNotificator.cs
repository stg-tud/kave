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

using JetBrains.Application;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.UserControls;

namespace KaVE.VS.FeedbackGenerator.Settings
{
    [ShellComponent]
    internal class ForcedSettingUpdateNotificator
    {
        private const string NewLine = "\n";

        private const string ThankYou = " You are using the KaVE plugin to capture your interactions with the IDE. " +
                                        "Thank you!" + NewLine + NewLine;

        private const string SessionIdText = ThankYou +
                                             "We noticed that you configured the anonymization options to remove the " +
                                             "session id in your submissions. Unfortunately, we had to remove this option, " +
                                             "because it made it incredibly hard to make sense of the data." +
                                             NewLine + NewLine +
                                             "Please visit the KaVE anonymization options now and update your preferences.";

        private const string ProfileIdText = ThankYou +
                                             "We noticed that you did not configure a profile id so far. " +
                                             "It is incredibly hard for us to make sense of the data without having this information," +
                                             "so we generated a random one for you." +
                                             NewLine + NewLine +
                                             "Please visit the KaVE options now and update your profile.";

        public ForcedSettingUpdateNotificator(ISettingsStore settingsStore,
            IUserProfileSettingsUtils profileUtils,
            ISimpleWindowOpener windows)
        {
            var anonSettings = settingsStore.GetSettings<AnonymizationSettings>();
            if (anonSettings.RemoveSessionIDs)
            {
                windows.OpenForcedSettingUpdateWindow(SessionIdText);

                anonSettings.RemoveSessionIDs = false;
                settingsStore.SetSettings(anonSettings);
            }

            var kaveSettings = settingsStore.GetSettings<KaVESettings>();
            var profileSettings = settingsStore.GetSettings<UserProfileSettings>();
            if (string.IsNullOrEmpty(profileSettings.ProfileId))
            {
                profileUtils.EnsureProfileId();
                if (!kaveSettings.IsFirstStart)
                {
                    windows.OpenForcedSettingUpdateWindow(SessionIdText);
                }
            }
        }
    }
}