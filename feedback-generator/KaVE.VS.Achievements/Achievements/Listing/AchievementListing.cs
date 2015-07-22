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
using System.IO;
using JetBrains.Annotations;
using JetBrains.Application;
using KaVE.VS.Achievements.Achievements.BaseClasses.AchievementTypes;
using KaVE.VS.Achievements.Util;

namespace KaVE.VS.Achievements.Achievements.Listing
{
    [ShellComponent]
    public class AchievementListing : Listing<int, BaseAchievement>, IAchievementListing
    {
        private const string ProjectName = "KaVEAchievements";

        private const string FileName = "achievements";

        private static readonly string AppDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);

        private static readonly string DirectoryPath = Path.Combine(AppDataPath, ProjectName);

        public AchievementListing() : base(FileName, DirectoryPath) {}

        [CanBeNull]
        public BaseAchievement GetAchievement(int id)
        {
            try
            {
                return GetValue(id);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Implicitly persists the listing
        /// </summary>
        public void Update(BaseAchievement achievement)
        {
            Update(achievement.Id, achievement);
        }

        [NotNull]
        public Dictionary<int, BaseAchievement> GetAchievementDictionary()
        {
            return Dictionary;
        }
    }
}