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

using JetBrains.Application;
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
        UserProfileEvent CreateEvent();
        void ResetComment();
    }

    [ShellComponent]
    public class UserProfileEventGenerator : EventGeneratorBase, IUserProfileEventGenerator
    {
        private readonly ISettingsStore _settingsStore;

        public UserProfileEventGenerator([NotNull] IRSEnv env,
            [NotNull] IMessageBus messageBus,
            [NotNull] IDateUtils dateUtils,
            [NotNull] ISettingsStore settingsStore) : base(env, messageBus, dateUtils)
        {
            _settingsStore = settingsStore;
        }

        public UserProfileEvent CreateEvent()
        {
            var exportEvent = Create<UserProfileEvent>();
            var s = _settingsStore.GetSettings<UserProfileSettings>();

            exportEvent.ProfileId = s.ProfileId;

            exportEvent.Education = s.Education;
            exportEvent.Position = s.Position;

            exportEvent.ProjectsNoAnswer = s.ProjectsNoAnswer;
            exportEvent.ProjectsCourses = s.ProjectsCourses;
            exportEvent.ProjectsPersonal = s.ProjectsPersonal;
            exportEvent.ProjectsSharedSmall = s.ProjectsSharedSmall;
            exportEvent.ProjectsSharedLarge = s.ProjectsSharedLarge;

            exportEvent.TeamsNoAnswer = s.TeamsNoAnswer;
            exportEvent.TeamsSolo = s.TeamsSolo;
            exportEvent.TeamsSmall = s.TeamsSmall;
            exportEvent.TeamsMedium = s.TeamsMedium;
            exportEvent.TeamsLarge = s.TeamsLarge;

            exportEvent.ProgrammingGeneral = s.ProgrammingGeneral;
            exportEvent.ProgrammingCSharp = s.ProgrammingCSharp;

            exportEvent.Comment = s.Comment;

            return exportEvent;
        }

        public void ResetComment()
        {
            var s = _settingsStore.GetSettings<UserProfileSettings>();
            s.Comment = "";
            _settingsStore.SetSettings(s);
        }
    }
}