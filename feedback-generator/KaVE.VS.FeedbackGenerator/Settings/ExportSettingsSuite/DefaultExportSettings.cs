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

namespace KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite
{
    [SettingsKey(typeof (FeedbackSettings), "Kave Feedback-Export Settings")]
    // WARNING: Do not change classname, as it is used to identify settings
    public class ExportSettings
    {
        [SettingsEntry("http://upload.kave.cc/", "KaVE FeedbackGenerator UploadUrl")]
        public string UploadUrl;

        [SettingsEntry("", "KaVE FeedbackGenerator WebAccessPrefix")]
        public string WebAccessPrefix;
    }
}