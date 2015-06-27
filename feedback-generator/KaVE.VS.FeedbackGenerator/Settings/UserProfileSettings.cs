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

using JetBrains.Application.Settings;
using KaVE.Commons.Model.Events;

namespace KaVE.VS.FeedbackGenerator.Settings
{
    [SettingsKey(typeof (FeedbackSettings), "KaVE UserProfile Settings")]
    public class UserProfileSettings
    {
        [SettingsEntry(false, "UserProfile: ProvideUserInformation")]
        public bool ProvideUserInformation;

        [SettingsEntry("", "UserProfile: Name")]
        public string Name;

        [SettingsEntry("", "UserProfile: Email")]
        public string Email;

        [SettingsEntry(WorkPosition.Unknown, "UserProfile: Position")]
        public WorkPosition Position;

        [SettingsEntry(0, "UserProfile: Numbers")]
        public string ExperienceYears;

        [SettingsEntry(ProjectExperience.Unknown, "UserProfile: Category")]
        public ProjectExperience ProjectExperience;

        [SettingsEntry(SelfEstimatedExperience.Unknown, "UserProfile: Category")]
        public SelfEstimatedExperience SelfEstimatedExperience;

        [SettingsEntry("", "UserProfile: Feedback")]
        public string Feedback;
    }
}