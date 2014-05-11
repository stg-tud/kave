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
 *    - Sven Amann
 */
using System;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [SettingsKey(typeof(KaVESettings), "Settings of the current IDE session")]
    class IDESessionSettings
    {
        [SettingsEntry("0001-01-01T00:00:00", "The generation time of the current session id.")]
        public DateTime SessionUUIDCreationDate;

        [SettingsEntry("", "The current session id.")]
        public string SessionUUID;
    }
}
