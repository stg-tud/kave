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
using System.Net.Mail;
using JetBrains.Application;
using KaVE.Commons.Model.Events.Export;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.SessionManager.Presentation.UserSetting;
using KaVE.VS.FeedbackGenerator.Utils;

namespace KaVE.VS.FeedbackGenerator.Generators.Export
{
    [ShellComponent]
    public class ExportEventGenerator : EventGeneratorBase, IExportEventGenerator
    {
        private readonly ISettingsStore _settingsStore;

        public ExportEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ISettingsStore settingsStore) : base(env, messageBus, dateUtils)
        {
            _settingsStore = settingsStore;
        }

        public ExportEvent CreateExportEvent()
        {
            var exportEvent = Create<ExportEvent>();
            var userSettings = _settingsStore.GetSettings<UserSettings>();

            if (userSettings.ProvideUserInformation)
            {
                AddUserInformationTo(exportEvent);
            }

            exportEvent.Feedback = userSettings.Feedback;

            return exportEvent;
        }

        private void AddUserInformationTo(ExportEvent exportEvent)
        {
            var userSettings = _settingsStore.GetSettings<UserSettings>();

            exportEvent.UserName = userSettings.Username;
            exportEvent.Category = userSettings.Category;
            exportEvent.Valuation = userSettings.Valuation;

            try
            {
                exportEvent.Mail = new MailAddress(userSettings.Mail);
            }
            catch
            {
                exportEvent.Mail = null;
            }

            try
            {
                exportEvent.Number = long.Parse(userSettings.NumberField);
            }
            catch (Exception)
            {
                exportEvent.Number = null;
            }
        }
    }

    public interface IExportEventGenerator
    {
        ExportEvent CreateExportEvent();
    }
}