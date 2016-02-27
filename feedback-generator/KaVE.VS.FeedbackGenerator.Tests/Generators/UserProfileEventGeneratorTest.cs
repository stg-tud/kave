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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Generators;
using KaVE.VS.FeedbackGenerator.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    internal class UserProfileEventGeneratorTest : EventGeneratorTestBase
    {
        private UserProfileSettings _userSettings;
        private ISettingsStore _settingsStore;

        private UserProfileEventGenerator _sut;

        [SetUp]
        public void Setup()
        {
            _userSettings = new UserProfileSettings
            {
                HasBeenAskedToFillProfile = false,
                ProfileId = "",
                Education = Educations.Unknown,
                Position = Positions.Unknown,
                ProjectsCourses = false,
                ProjectsPersonal = false,
                ProjectsSharedSmall = false,
                ProjectsSharedMedium = false,
                ProjectsSharedLarge = false,
                TeamsSolo = false,
                TeamsSmall = false,
                TeamsMedium = false,
                TeamsLarge = false,
                CodeReviews = YesNoUnknown.Unknown,
                ProgrammingGeneral = Likert7Point.Unknown,
                ProgrammingCSharp = Likert7Point.Unknown
            };

            _settingsStore = Mock.Of<ISettingsStore>();
            Mock.Get(_settingsStore).Setup(s => s.GetSettings<UserProfileSettings>()).Returns(_userSettings);

            _sut = new UserProfileEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, _settingsStore);
        }

        private void AssertEvent(UserProfileEvent expected)
        {
            var actual = _sut.CreateEvent();

            // remove information from base event...
            actual.KaVEVersion = null;
            actual.TriggeredAt = null;
            actual.TriggeredBy = IDEEvent.Trigger.Unknown;
            actual.Duration = null;
            actual.ActiveWindow = null;
            actual.ActiveDocument = null;

            // .. but keep one to make sure it was actually invoked
            expected.IDESessionUUID = TestRSEnv.IDESession.UUID;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SettingsAreNotReadOnInit()
        {
            Mock.Get(_settingsStore).Verify(s => s.GetSettings<UserProfileSettings>(), Times.Never);
        }

        [Test]
        public void ShouldNotPublishAnyEvents()
        {
            _sut.CreateEvent();
            AssertNoEvent();
        }

        [Test]
        public void CopiesProfileId()
        {
            _userSettings.ProfileId = "P";
            AssertEvent(
                new UserProfileEvent
                {
                    ProfileId = "P"
                });
        }

        [Test]
        public void CopiesEducation()
        {
            _userSettings.Education = Educations.Autodidact;
            AssertEvent(
                new UserProfileEvent
                {
                    Education = Educations.Autodidact
                });
        }

        [Test]
        public void CopiesPosition()
        {
            _userSettings.Position = Positions.HobbyProgrammer;
            AssertEvent(
                new UserProfileEvent
                {
                    Position = Positions.HobbyProgrammer
                });
        }

        [Test]
        public void CopiesProjectsCourses()
        {
            _userSettings.ProjectsCourses = true;
            AssertEvent(
                new UserProfileEvent
                {
                    ProjectsCourses = true
                });
        }

        [Test]
        public void CopiesProjectsPersonal()
        {
            _userSettings.ProjectsPersonal = true;
            AssertEvent(
                new UserProfileEvent
                {
                    ProjectsPersonal = true
                });
        }

        [Test]
        public void CopiesProjectsSharedSmall()
        {
            _userSettings.ProjectsSharedSmall = true;
            AssertEvent(
                new UserProfileEvent
                {
                    ProjectsSharedSmall = true
                });
        }

        [Test]
        public void CopiesProjectsSharedMedium()
        {
            _userSettings.ProjectsSharedMedium = true;
            AssertEvent(
                new UserProfileEvent
                {
                    ProjectsSharedMedium = true
                });
        }

        [Test]
        public void CopiesProjectsSharedLarge()
        {
            _userSettings.ProjectsSharedLarge = true;
            AssertEvent(
                new UserProfileEvent
                {
                    ProjectsSharedLarge = true
                });
        }

        [Test]
        public void CopiesTeamsSolo()
        {
            _userSettings.TeamsSolo = true;
            AssertEvent(
                new UserProfileEvent
                {
                    TeamsSolo = true
                });
        }

        [Test]
        public void CopiesTeamsSmall()
        {
            _userSettings.TeamsSmall = true;
            AssertEvent(
                new UserProfileEvent
                {
                    TeamsSmall = true
                });
        }

        [Test]
        public void CopiesTeamsMedium()
        {
            _userSettings.TeamsMedium = true;
            AssertEvent(
                new UserProfileEvent
                {
                    TeamsMedium = true
                });
        }

        [Test]
        public void CopiesTeamsLarge()
        {
            _userSettings.TeamsLarge = true;
            AssertEvent(
                new UserProfileEvent
                {
                    TeamsLarge = true
                });
        }

        [Test]
        public void CopiesCodeReviews()
        {
            _userSettings.CodeReviews = YesNoUnknown.Yes;
            AssertEvent(
                new UserProfileEvent
                {
                    CodeReviews = YesNoUnknown.Yes
                });
        }

        [Test]
        public void CopiesProgrammingGeneral()
        {
            _userSettings.ProgrammingGeneral = Likert7Point.Neutral;
            AssertEvent(
                new UserProfileEvent
                {
                    ProgrammingGeneral = Likert7Point.Neutral
                });
        }

        [Test]
        public void CopiesProgrammingCSharp()
        {
            _userSettings.ProgrammingCSharp = Likert7Point.Negative1;
            AssertEvent(
                new UserProfileEvent
                {
                    ProgrammingCSharp = Likert7Point.Negative1
                });
        }
    }
}