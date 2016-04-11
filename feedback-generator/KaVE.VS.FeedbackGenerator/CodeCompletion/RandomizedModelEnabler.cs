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
using JetBrains.Application;
using KaVE.RS.Commons.Settings;

namespace KaVE.VS.FeedbackGenerator.CodeCompletion
{
    [ShellComponent]
    public class RandomizedModelEnabler
    {
        public const int AvailabilityChance = 90;
        private readonly IDictionary<string, IDictionary<string, bool>> _cache;
        private readonly ISettingsStore _settings;

        public RandomizedModelEnabler(ISettingsStore settings)
        {
            _settings = settings;
            _cache = new Dictionary<string, IDictionary<string, bool>>();
        }

        public bool IsEnabled(string typeName)
        {
            var profileId = _settings.GetSettings<UserProfileSettings>().ProfileId;

            if (!_cache.ContainsKey(profileId))
            {
                _cache.Add(profileId, new Dictionary<string, bool>());
            }

            if (!_cache[profileId].ContainsKey(typeName))
            {
                _cache[profileId].Add(typeName, IsEnabledForUserAndType(profileId, typeName, AvailabilityChance));
            }

            return _cache[profileId][typeName];
        }

        public static bool IsEnabledForUserAndType(string userProfileId,
            string typeName,
            int availabilityChanceInPercent)
        {
            var seed = userProfileId.GetHashCode() ^ typeName.GetHashCode();
            var random = new Random(seed);
            return random.Next(100) < availabilityChanceInPercent;
        }
    }
}