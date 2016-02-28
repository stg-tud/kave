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
using KaVE.Commons.Utils;

namespace KaVE.RS.Commons.Settings
{
    public interface IUserProfileSettingsUtils
    {
        void EnsureProfileId();
        bool HasBeenAskedToFillProfile();
        string CreateNewProfileId();
        UserProfileSettings GetSettings();
        void StoreSettings(UserProfileSettings userProfileSettings);
    }

    [ShellComponent]
    public class UserProfileSettingsUtils : IUserProfileSettingsUtils
    {
        private readonly ISettingsStore _settingsStore;
        private readonly IRandomizationUtils _rnd;

        public UserProfileSettingsUtils(ISettingsStore settingsStore, IRandomizationUtils rnd)
        {
            _settingsStore = settingsStore;
            _rnd = rnd;
        }

        public void EnsureProfileId()
        {
            var settings = GetSettings();
            if (!HasProfileId)
            {
                settings.ProfileId = CreateNewProfileId();
                StoreSettings(settings);
            }
        }

        public bool HasBeenAskedToFillProfile()
        {
            return GetSettings().HasBeenAskedToFillProfile;
        }

        private bool HasProfileId
        {
            get
            {
                var profileId = GetSettings().ProfileId;
                return !"".Equals(profileId);
            }
        }

        public string CreateNewProfileId()
        {
            return _rnd.GetRandomGuid().ToString();
        }

        public UserProfileSettings GetSettings()
        {
            return _settingsStore.GetSettings<UserProfileSettings>();
        }

        public void StoreSettings(UserProfileSettings settings)
        {
            _settingsStore.SetSettings(settings);
        }
    }
}