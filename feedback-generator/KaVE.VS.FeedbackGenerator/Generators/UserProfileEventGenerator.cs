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
        bool ShouldCreateEvent();
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
            var @event = Create<UserProfileEvent>();
            var s = _settingsStore.GetSettings<UserProfileSettings>();

            if (s.IsProvidingProfile)
            {
                @event.ProfileId = s.ProfileId;

                @event.Education = s.Education;
                @event.Position = s.Position;

                @event.ProjectsNoAnswer = s.ProjectsNoAnswer;
                @event.ProjectsCourses = s.ProjectsCourses;
                @event.ProjectsPersonal = s.ProjectsPersonal;
                @event.ProjectsSharedSmall = s.ProjectsSharedSmall;
                @event.ProjectsSharedLarge = s.ProjectsSharedLarge;

                @event.TeamsNoAnswer = s.TeamsNoAnswer;
                @event.TeamsSolo = s.TeamsSolo;
                @event.TeamsSmall = s.TeamsSmall;
                @event.TeamsMedium = s.TeamsMedium;
                @event.TeamsLarge = s.TeamsLarge;

                @event.ProgrammingGeneral = s.ProgrammingGeneral;
                @event.ProgrammingCSharp = s.ProgrammingCSharp;
            }

            @event.Comment = s.Comment;

            return @event;
        }

        public void ResetComment()
        {
            var s = _settingsStore.GetSettings<UserProfileSettings>();
            s.Comment = "";
            _settingsStore.SetSettings(s);
        }

        public bool ShouldCreateEvent()
        {
            var s = _settingsStore.GetSettings<UserProfileSettings>();
            var isProviding = s.IsProvidingProfile;
            var hasComment = s.Comment.Length > 0;
            return isProviding || hasComment;
        }
    }
}