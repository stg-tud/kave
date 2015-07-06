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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.MessageBus;
using KaVE.VS.FeedbackGenerator.Settings;

namespace KaVE.VS.FeedbackGenerator.Generators
{
    public interface IUserProfileEventGenerator
    {
        UserProfileEvent CreateExportEvent();
    }

    [ShellComponent]
    public class UserProfileEventGenerator : EventGeneratorBase, IUserProfileEventGenerator
    {
        private readonly ISettingsStore _settingsStore;

        private UserProfileSettings Settings
        {
            get { return _settingsStore.GetSettings<UserProfileSettings>(); }
        }

        public UserProfileEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ISettingsStore settingsStore) : base(env, messageBus, dateUtils)
        {
            _settingsStore = settingsStore;
        }

        public UserProfileEvent CreateExportEvent()
        {
            var exportEvent = Create<UserProfileEvent>();

            if (Settings.IsProvidingProfile)
            {
                AddUserInformationTo(exportEvent);
            }

            exportEvent.Feedback = Settings.Feedback;

            return exportEvent;
        }

        private void AddUserInformationTo(UserProfileEvent exportEvent)
        {
            //exportEvent.Name = Settings.Name;
            //exportEvent.Category = Settings.Category;
            //exportEvent.Valuation = Settings.Valuation;

            try
            {
                //exportEvent.Email = new MailAddress(Settings.Email);
            }
            catch
            {
                //exportEvent.Email = null;
            }

            try
            {
                // exportEvent.Number = long.Parse(Settings.NumberField);
            }
            catch (Exception)
            {
                //  exportEvent.Number = null;
            }
        }
    }
}