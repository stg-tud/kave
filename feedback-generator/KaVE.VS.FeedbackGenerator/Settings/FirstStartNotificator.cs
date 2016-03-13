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
    public interface IFirstStartNotificator {}

    [ShellComponent]
    public class FirstStartNotificator : IFirstStartNotificator
    {
        private const string NewLine = "\n";

        private const string ThankYou = " You are using the KaVE plugin to capture your interactions with the IDE. " +
                                        "Thank you!" + NewLine + NewLine;

        public const string SessionIdText = ThankYou +
                                            "We noticed that you configured the anonymization options to remove the " +
                                            "session id in your submissions. Unfortunately, we had to remove this option, " +
                                            "because it made it incredibly hard to make sense of the data." +
                                            NewLine + NewLine +
                                            "Please visit the KaVE anonymization options now and update your preferences.";

        public const string ProfileIdText = ThankYou +
                                            "We noticed that you did not configure a profile id so far. " +
                                            "It is incredibly hard for us to make sense of the data without having this information," +
                                            "so we generated a random one for you." +
                                            NewLine + NewLine +
                                            "Please visit the KaVE options now and update your profile.";


        private readonly ISettingsStore _settingsStore;
        private readonly IUserProfileSettingsUtils _profileUtils;
        private readonly ISimpleWindowOpener _windows;

        public FirstStartNotificator(ISimpleWindowOpener windows,
            ISettingsStore ss,
            IUserProfileSettingsUtils profileUtils)
        {
            _settingsStore = ss;
            _profileUtils = profileUtils;
            _windows = windows;

            var s = _settingsStore.GetSettings<KaVESettings>();
            if (s.IsFirstStart)
            {
                s.IsFirstStart = false;
                _settingsStore.SetSettings(s);

                _windows.OpenFirstStartWindow();
                Assert(false);
            }
            else
            {
                Assert(true);
            }
        }

        public void Assert(bool isOpeningWindows)
        {
            var anonSettings = _settingsStore.GetSettings<AnonymizationSettings>();
            if (anonSettings.RemoveSessionIDs)
            {
                anonSettings.RemoveSessionIDs = false;
                _settingsStore.SetSettings(anonSettings);

                if (isOpeningWindows)
                {
                    _windows.OpenForcedSettingUpdateWindow(SessionIdText);
                }
            }

            var profileSettings = _settingsStore.GetSettings<UserProfileSettings>();
            if (string.IsNullOrEmpty(profileSettings.ProfileId))
            {
                _profileUtils.EnsureProfileId();

                if (isOpeningWindows)
                {
                    _windows.OpenForcedSettingUpdateWindow(ProfileIdText);
                }
            }
        }
    }
}