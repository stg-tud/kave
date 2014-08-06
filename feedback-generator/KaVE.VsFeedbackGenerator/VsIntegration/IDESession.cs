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
using EnvDTE;
using JetBrains.Application;
using JetBrains.Application.Components;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent(ProgramConfigurations.VS_ADDIN)]
    public class IDESession : IIDESession
    {
        private readonly DTE _dte;
        private readonly ISettingsStore _settingsStore;

        public IDESession([NotNull] DTE dte, ISettingsStore settingsStore)
        {
            _dte = dte;
            _settingsStore = settingsStore;
        }

        public string UUID
        {
            get
            {
                var settings = _settingsStore.GetSettings<IDESessionSettings>();
                var dateUtils = Registry.GetComponent<IDateUtils>();
                if (settings.SessionUUIDCreationDate != dateUtils.Today)
                {
                    settings.SessionUUID = Guid.NewGuid().ToString();
                    settings.SessionUUIDCreationDate = dateUtils.Today;
                    _settingsStore.SetSettings(settings);
                }
                return settings.SessionUUID;
            }
        }

        public DTE DTE
        {
            get
            {
                return _dte;
            }
        }
    }
}