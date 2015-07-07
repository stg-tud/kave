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
using System.Collections.Generic;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    public class UserProfileContextTest
    {
        private ExportSettings _exportSettings;
        private UserProfileSettings _userSettings;
        private List<string> _updatedProperties;

        private UserProfileContext _sut;

        [SetUp]
        public void SetUp()
        {
            _userSettings = new UserProfileSettings();
            _userSettings.ProfileId = "";
            _userSettings.Comment = "";
            _userSettings.ProjectsNoAnswer = true;
            _exportSettings = new ExportSettings();
            _updatedProperties = new List<string>();

            _sut = new UserProfileContext(_exportSettings, _userSettings);

            _sut.PropertyChanged += (sender, args) => { _updatedProperties.Add(args.PropertyName); };
        }

        [Test]
        public void IsDatev()
        {
            _exportSettings.IsDatev = false;
            Assert.False(_sut.IsDatev);

            _exportSettings.IsDatev = true;
            Assert.True(_sut.IsDatev);
        }

        [Test]
        public void IsProviding_PropagationFromCode()
        {
            _userSettings.IsProvidingProfile = false;
            Assert.False(_sut.IsProvidingProfile);

            _userSettings.IsProvidingProfile = true;
            Assert.True(_sut.IsProvidingProfile);
        }

        [Test]
        public void IsProviding_PropagationToCode()
        {
            _sut.IsProvidingProfile = false;
            Assert.False(_userSettings.IsProvidingProfile);

            _sut.IsProvidingProfile = true;
            Assert.True(_userSettings.IsProvidingProfile);
        }

        [Test]
        public void IsProviding_PropertyChange()
        {
            _sut.IsProvidingProfile = false;
            AssertNotifications("IsProvidingProfile");
        }

        [Test]
        public void IsProviding_DisabledForDatev()
        {
            _userSettings.IsProvidingProfile = true;
            _exportSettings.IsDatev = true;
            _sut = new UserProfileContext(_exportSettings, _userSettings);
            Assert.False(_sut.IsProvidingProfile);
        }

        [Test]
        public void ProfileId_PropagationFromCode()
        {
            _userSettings.ProfileId = "x";
            Assert.AreEqual("x", _sut.ProfileId);
        }

        [Test]
        public void ProfileId_PropagationToCode()
        {
            _sut.ProfileId = "x";
            Assert.AreEqual("x", _userSettings.ProfileId);
        }

        [Test]
        public void ProfileId_PropertyChange()
        {
            _sut.ProfileId = "x";
            AssertNotifications("ProfileId");
        }

        [Test]
        public void Education_PropagationFromCode()
        {
            _userSettings.Education = Educations.Apprenticeship;
            Assert.AreEqual(Educations.Apprenticeship, _sut.Education);
        }

        [Test]
        public void Education_PropagationToCode()
        {
            _sut.Education = Educations.Apprenticeship;
            Assert.AreEqual(Educations.Apprenticeship, _userSettings.Education);
        }

        [Test]
        public void Education_PropertyChange()
        {
            _sut.Education = Educations.Apprenticeship;
            AssertNotifications("Education");
        }

        [Test]
        public void Position_PropagationFromCode()
        {
            _userSettings.Position = Positions.HobbyProgrammer;
            Assert.AreEqual(Positions.HobbyProgrammer, _sut.Position);
        }

        [Test]
        public void Position_PropagationToCode()
        {
            _sut.Position = Positions.HobbyProgrammer;
            Assert.AreEqual(Positions.HobbyProgrammer, _userSettings.Position);
        }

        [Test]
        public void Position_PropertyChange()
        {
            _sut.Position = Positions.HobbyProgrammer;
            AssertNotifications("Position");
        }

        [Test]
        public void Projects_None_PropagationFromCode()
        {
            _userSettings.ProjectsNoAnswer = true;
            Assert.True(_sut.ProjectsNoAnswer);
            _userSettings.ProjectsNoAnswer = false;
            Assert.False(_sut.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_None_PropagationToCode()
        {
            _sut.ProjectsCourses = true;
            _sut.ProjectsNoAnswer = true;
            Assert.True(_userSettings.ProjectsNoAnswer);

            // Cannot Be Disabled Directly
            _sut.ProjectsNoAnswer = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_None_DisablesEverythingElse()
        {
            _sut.ProjectsCourses = true;
            _sut.ProjectsPrivate = true;
            _sut.ProjectsTeamSmall = true;
            _sut.ProjectsTeamLarge = true;
            _sut.ProjectsCommercial = true;
            _sut.ProjectsNoAnswer = true;
            Assert.False(_userSettings.ProjectsCourses);
            Assert.False(_userSettings.ProjectsPrivate);
            Assert.False(_userSettings.ProjectsTeamSmall);
            Assert.False(_userSettings.ProjectsTeamLarge);
            Assert.False(_userSettings.ProjectsCommercial);
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_None_PropertyChange()
        {
            _sut.ProjectsNoAnswer = true;
            AssertNotifications(
                "ProjectsNoAnswer",
                "ProjectsCourses",
                "ProjectsPrivate",
                "ProjectsTeamSmall",
                "ProjectsTeamLarge",
                "ProjectsCommercial");
        }

        [Test]
        public void Projects_Courses_PropagationFromCode()
        {
            _userSettings.ProjectsCourses = true;
            Assert.True(_sut.ProjectsCourses);
            _userSettings.ProjectsCourses = false;
            Assert.False(_sut.ProjectsCourses);
        }

        [Test]
        public void Projects_Courses_PropagationToCode()
        {
            _sut.ProjectsCourses = true;
            Assert.True(_userSettings.ProjectsCourses);
            _sut.ProjectsCourses = false;
            Assert.False(_userSettings.ProjectsCourses);
        }

        [Test]
        public void Projects_Courses_DisablesNoAnswer()
        {
            _sut.ProjectsCourses = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_Courses_PropertyChange()
        {
            _sut.ProjectsCourses = true;
            AssertNotifications("ProjectsCourses", "ProjectsNoAnswer");
        }

        [Test]
        public void Projects_Private_PropagationFromCode()
        {
            _userSettings.ProjectsPrivate = true;
            Assert.True(_sut.ProjectsPrivate);
            _userSettings.ProjectsPrivate = false;
            Assert.False(_sut.ProjectsPrivate);
        }

        [Test]
        public void Projects_Private_PropagationToCode()
        {
            _sut.ProjectsPrivate = true;
            Assert.True(_userSettings.ProjectsPrivate);
            _sut.ProjectsPrivate = false;
            Assert.False(_userSettings.ProjectsPrivate);
        }

        [Test]
        public void Projects_Private_DisablesNoAnswer()
        {
            _sut.ProjectsPrivate = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_Private_PropertyChange()
        {
            _sut.ProjectsPrivate = true;
            AssertNotifications("ProjectsPrivate", "ProjectsNoAnswer");
        }

        [Test]
        public void Projects_SmallTeam_PropagationFromCode()
        {
            _userSettings.ProjectsTeamSmall = true;
            Assert.True(_sut.ProjectsTeamSmall);
            _userSettings.ProjectsTeamSmall = false;
            Assert.False(_sut.ProjectsTeamSmall);
        }

        [Test]
        public void Projects_SmallTeam_PropagationToCode()
        {
            _sut.ProjectsTeamSmall = true;
            Assert.True(_userSettings.ProjectsTeamSmall);
            _sut.ProjectsTeamSmall = false;
            Assert.False(_userSettings.ProjectsTeamSmall);
        }

        [Test]
        public void Projects_SmallTeam_DisablesNoAnswer()
        {
            _sut.ProjectsTeamSmall = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_SmallTeam_PropertyChange()
        {
            _sut.ProjectsTeamSmall = true;
            AssertNotifications("ProjectsTeamSmall", "ProjectsNoAnswer");
        }

        [Test]
        public void Projects_LargeTeam_PropagationFromCode()
        {
            _userSettings.ProjectsTeamLarge = true;
            Assert.True(_sut.ProjectsTeamLarge);
            _userSettings.ProjectsTeamLarge = false;
            Assert.False(_sut.ProjectsTeamLarge);
        }

        [Test]
        public void Projects_LargeTeam_PropagationToCode()
        {
            _sut.ProjectsTeamLarge = true;
            Assert.True(_userSettings.ProjectsTeamLarge);
            _sut.ProjectsTeamLarge = false;
            Assert.False(_userSettings.ProjectsTeamLarge);
        }

        [Test]
        public void Projects_LargeTeam_DisablesNoAnswer()
        {
            _sut.ProjectsTeamLarge = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_LargeTeam_PropertyChange()
        {
            _sut.ProjectsTeamLarge = true;
            AssertNotifications("ProjectsTeamLarge", "ProjectsNoAnswer");
        }

        [Test]
        public void Projects_Commercial_PropagationFromCode()
        {
            _userSettings.ProjectsCommercial = true;
            Assert.True(_sut.ProjectsCommercial);
            _userSettings.ProjectsCommercial = false;
            Assert.False(_sut.ProjectsCommercial);
        }

        [Test]
        public void Projects_Commercial_PropagationToCode()
        {
            _sut.ProjectsCommercial = true;
            Assert.True(_userSettings.ProjectsCommercial);
            _sut.ProjectsCommercial = false;
            Assert.False(_userSettings.ProjectsCommercial);
        }

        [Test]
        public void Projects_Commercial_DisablesNoAnswer()
        {
            _sut.ProjectsCommercial = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void Projects_Commercial_PropertyChange()
        {
            _sut.ProjectsCommercial = true;
            AssertNotifications("ProjectsCommercial", "ProjectsNoAnswer");
        }

        [Test]
        public void ProgrammingGeneral_PropagationFromCode()
        {
            _userSettings.ProgrammingGeneral = Likert7Point.Negative1;
            Assert.AreEqual(Likert7Point.Negative1, _sut.ProgrammingGeneral);
        }

        [Test]
        public void ProgrammingGeneral_PropagationToCode()
        {
            _sut.ProgrammingGeneral = Likert7Point.Negative1;
            Assert.AreEqual(Likert7Point.Negative1, _userSettings.ProgrammingGeneral);
        }

        [Test]
        public void ProgrammingGeneral_PropertyChange()
        {
            _sut.ProgrammingGeneral = Likert7Point.Negative1;
            AssertNotifications("ProgrammingGeneral");
        }

        [Test]
        public void ProgrammingCSharp_PropagationFromCode()
        {
            _userSettings.ProgrammingCSharp = Likert7Point.Negative1;
            Assert.AreEqual(Likert7Point.Negative1, _sut.ProgrammingCSharp);
        }

        [Test]
        public void ProgrammingCSharp_PropagationToCode()
        {
            _sut.ProgrammingCSharp = Likert7Point.Negative1;
            Assert.AreEqual(Likert7Point.Negative1, _userSettings.ProgrammingCSharp);
        }

        [Test]
        public void ProgrammingCSharp_PropertyChange()
        {
            _sut.ProgrammingCSharp = Likert7Point.Negative1;
            AssertNotifications("ProgrammingCSharp");
        }

        [Test]
        public void EducationOptions()
        {
            var actuals = _sut.EducationOptions;
            var expecteds = Enum.GetValues(typeof (Educations));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void PositionOptions()
        {
            var actuals = _sut.PositionOptions;
            var expecteds = Enum.GetValues(typeof (Positions));
            Assert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void LickertOptions()
        {
            var actuals = _sut.LikertOptions;
            var expecteds = Enum.GetValues(typeof (Likert7Point));
            Assert.AreEqual(expecteds, actuals);
        }

        private void AssertNotifications(params string[] expecteds)
        {
            CollectionAssert.AreEqual(expecteds, _updatedProperties);
        }
    }
}