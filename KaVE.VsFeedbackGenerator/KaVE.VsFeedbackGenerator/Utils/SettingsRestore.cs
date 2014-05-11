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
 * 
 * Contributors:
 *    - Uli Fahrer
 */

using JetBrains.Application;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils.Json;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Utils
{
    [ShellComponent]
    class SettingsRestore
    {
        private readonly ISettingsStore _settings;
        private readonly IDEEventLogFileManager _logManager;

        public SettingsRestore(ISettingsStore settings, IDEEventLogFileManager logManager)
        {
            _settings = settings;
            _logManager = logManager;
        }

        public void RestoreDefaultSettings()
        {
           _settings.ResetSettings<UploadSettings>();
           _settings.ResetSettings<ExportSettings>();
           _settings.ResetSettings<IDESessionSettings>();

           _logManager.DeleteLogFileDirectory();
        } 
    }
}
