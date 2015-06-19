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
using KaVE.Commons.Model.Events.Export;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation.UserSetting
{
    [SettingsKey(typeof (FeedbackSettings), "Kave Feedback-User Settings")]
    public class UserSettings
    {
        [SettingsEntry(true, "KaVE FeedbackGenerator ProvideUserInformation")]
        public bool ProvideUserInformation;

        [SettingsEntry("", "KaVE FeedbackGenerator Username")]
        public string Username;

        [SettingsEntry("", "KaVE FeedbackGenerator E-Mail")]
        public string Mail;

        [SettingsEntry(Category.Unknown, "KaVE FeedbackGenerator Category")]
        public Category Category;

        [SettingsEntry("", "KaVE FeedbackGenerator Numbers")]
        public string NumberField;

        [SettingsEntry(Valuation.Unknown, "KaVE FeedbackGenerator Valuation")]
        public Valuation Valuation;

        [SettingsEntry("", "KaVE FeedbackGenerator Feedback")]
        public string Feedback;
    }
}