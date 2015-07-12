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
using KaVE.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    public class UserProfileContextTest
    {
        private string _someGuid;

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
            _userSettings.HasBeenAskedtoProvideProfile = true;
            _exportSettings = new ExportSettings();
            _updatedProperties = new List<string>();

            var newGuid = Guid.NewGuid();
            _someGuid = newGuid.ToString();
            var rnd = Mock.Of<IRandomizationUtils>();
            Mock.Get(rnd).Setup(r => r.GetRandomGuid()).Returns(newGuid);

            _sut = new UserProfileContext(_exportSettings, _userSettings, rnd);

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
            _sut = new UserProfileContext(_exportSettings, _userSettings, new RandomizationUtils());
            Assert.False(_sut.IsProvidingProfile);
        }

        [Test]
        public void IsProviding_SetsUidOnFirstAccess()
        {
            _userSettings.HasBeenAskedtoProvideProfile = false;
            _sut.IsProvidingProfile = true;
            Assert.True(_userSettings.HasBeenAskedtoProvideProfile);
            Assert.AreEqual(_someGuid, _sut.ProfileId);
            AssertNotifications("ProfileId", "IsProvidingProfile");
        }

        [Test]
        public void IsProviding_DoesNotSetUidOnSecondAccess()
        {
            _userSettings.HasBeenAskedtoProvideProfile = true;
            _sut.IsProvidingProfile = true;
            Assert.True(_userSettings.HasBeenAskedtoProvideProfile);
            Assert.AreEqual("", _sut.ProfileId);
            AssertNotifications("IsProvidingProfile");
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
            _userSettings.Education = Educations.Training;
            Assert.AreEqual(Educations.Training, _sut.Education);
        }

        [Test]
        public void Education_PropagationToCode()
        {
            _sut.Education = Educations.Training;
            Assert.AreEqual(Educations.Training, _userSettings.Education);
        }

        [Test]
        public void Education_PropertyChange()
        {
            _sut.Education = Educations.Training;
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
        public void ProjectsNoAnswer_PropagationFromCode()
        {
            _userSettings.ProjectsNoAnswer = true;
            Assert.True(_sut.ProjectsNoAnswer);
            _userSettings.ProjectsNoAnswer = false;
            Assert.False(_sut.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsNoAnswer_PropagationToCode()
        {
            _sut.ProjectsCourses = true;
            _sut.ProjectsNoAnswer = true;
            Assert.True(_userSettings.ProjectsNoAnswer);

            // Cannot Be Disabled Directly
            _sut.ProjectsNoAnswer = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsNoAnswer_DisablesEverythingElse()
        {
            _sut.ProjectsCourses = true;
            _sut.ProjectsPersonal = true;
            _sut.ProjectsSharedSmall = true;
            _sut.ProjectsSharedLarge = true;
            _sut.ProjectsNoAnswer = true;
            Assert.False(_userSettings.ProjectsCourses);
            Assert.False(_userSettings.ProjectsPersonal);
            Assert.False(_userSettings.ProjectsSharedSmall);
            Assert.False(_userSettings.ProjectsSharedLarge);
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsNoAnswer_PropertyChange()
        {
            _sut.ProjectsNoAnswer = true;
            AssertNotifications(
                "ProjectsNoAnswer",
                "ProjectsCourses",
                "ProjectsPersonal",
                "ProjectsSharedSmall",
                "ProjectsSharedLarge");
        }

        [Test]
        public void ProjectsCourses_PropagationFromCode()
        {
            _userSettings.ProjectsCourses = true;
            Assert.True(_sut.ProjectsCourses);
            _userSettings.ProjectsCourses = false;
            Assert.False(_sut.ProjectsCourses);
        }

        [Test]
        public void ProjectsCourses_PropagationToCode()
        {
            _sut.ProjectsCourses = true;
            Assert.True(_userSettings.ProjectsCourses);
            _sut.ProjectsCourses = false;
            Assert.False(_userSettings.ProjectsCourses);
        }

        [Test]
        public void ProjectsCourses_DisablesNoAnswer()
        {
            _sut.ProjectsCourses = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
            _sut.ProjectsCourses = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsCourses_DisablesNoAnswer_CheckingPrivateHelperOnce()
        {
            _sut.ProjectsCourses = true;
            _sut.ProjectsPersonal = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
            _sut.ProjectsCourses = false;
            Assert.False(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsCourses_PropertyChange()
        {
            _sut.ProjectsCourses = true;
            AssertNotifications("ProjectsCourses", "ProjectsNoAnswer");
        }

        [Test]
        public void ProjectsPersonal_PropagationFromCode()
        {
            _userSettings.ProjectsPersonal = true;
            Assert.True(_sut.ProjectsPersonal);
            _userSettings.ProjectsPersonal = false;
            Assert.False(_sut.ProjectsPersonal);
        }

        [Test]
        public void ProjectsPersonal_PropagationToCode()
        {
            _sut.ProjectsPersonal = true;
            Assert.True(_userSettings.ProjectsPersonal);
            _sut.ProjectsPersonal = false;
            Assert.False(_userSettings.ProjectsPersonal);
        }

        [Test]
        public void ProjectsPersonal_DisablesNoAnswer()
        {
            _sut.ProjectsPersonal = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
            _sut.ProjectsPersonal = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsPersonal_PropertyChange()
        {
            _sut.ProjectsPersonal = true;
            AssertNotifications("ProjectsPersonal", "ProjectsNoAnswer");
        }

        [Test]
        public void ProjectsSharedSmall_PropagationFromCode()
        {
            _userSettings.ProjectsSharedSmall = true;
            Assert.True(_sut.ProjectsSharedSmall);
            _userSettings.ProjectsSharedSmall = false;
            Assert.False(_sut.ProjectsSharedSmall);
        }

        [Test]
        public void ProjectsSharedSmall_PropagationToCode()
        {
            _sut.ProjectsSharedSmall = true;
            Assert.True(_userSettings.ProjectsSharedSmall);
            _sut.ProjectsSharedSmall = false;
            Assert.False(_userSettings.ProjectsSharedSmall);
        }

        [Test]
        public void ProjectsSharedSmall_DisablesNoAnswer()
        {
            _sut.ProjectsSharedSmall = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
            _sut.ProjectsSharedSmall = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsSharedSmall_PropertyChange()
        {
            _sut.ProjectsSharedSmall = true;
            AssertNotifications("ProjectsSharedSmall", "ProjectsNoAnswer");
        }

        [Test]
        public void ProjectsSharedLarge_PropagationFromCode()
        {
            _userSettings.ProjectsSharedLarge = true;
            Assert.True(_sut.ProjectsSharedLarge);
            _userSettings.ProjectsSharedLarge = false;
            Assert.False(_sut.ProjectsSharedLarge);
        }

        [Test]
        public void ProjectsSharedLarge_PropagationToCode()
        {
            _sut.ProjectsSharedLarge = true;
            Assert.True(_userSettings.ProjectsSharedLarge);
            _sut.ProjectsSharedLarge = false;
            Assert.False(_userSettings.ProjectsSharedLarge);
        }

        [Test]
        public void ProjectsSharedLarge_DisablesNoAnswer()
        {
            _sut.ProjectsSharedLarge = true;
            Assert.False(_userSettings.ProjectsNoAnswer);
            _sut.ProjectsSharedLarge = false;
            Assert.True(_userSettings.ProjectsNoAnswer);
        }

        [Test]
        public void ProjectsSharedLarge_PropertyChange()
        {
            _sut.ProjectsSharedLarge = true;
            AssertNotifications("ProjectsSharedLarge", "ProjectsNoAnswer");
        }

        [Test]
        public void TeamNoAnswer_PropagationFromCode()
        {
            _userSettings.TeamsNoAnswer = true;
            Assert.True(_sut.TeamsNoAnswer);
            _userSettings.TeamsNoAnswer = false;
            Assert.False(_sut.TeamsNoAnswer);
        }

        [Test]
        public void TeamNoAnswer_PropagationToCode()
        {
            _sut.TeamsMedium = true;
            _sut.TeamsNoAnswer = true;
            Assert.True(_userSettings.TeamsNoAnswer);

            // Cannot Be Disabled Directly
            _sut.TeamsNoAnswer = false;
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamNoAnswer_DisablesEverythingElse()
        {
            _sut.TeamsSolo = true;
            _sut.TeamsSmall = true;
            _sut.TeamsMedium = true;
            _sut.TeamsLarge = true;
            _sut.TeamsNoAnswer = true;
            Assert.False(_userSettings.TeamsSolo);
            Assert.False(_userSettings.TeamsSmall);
            Assert.False(_userSettings.TeamsMedium);
            Assert.False(_userSettings.TeamsLarge);
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamNoAnswer_PropertyChange()
        {
            _sut.TeamsNoAnswer = true;
            AssertNotifications(
                "TeamsNoAnswer",
                "TeamsSolo",
                "TeamsSmall",
                "TeamsMedium",
                "TeamsLarge");
        }

        [Test]
        public void TeamSolo_PropagationFromCode()
        {
            _userSettings.TeamsSolo = true;
            Assert.True(_sut.TeamsSolo);
            _userSettings.TeamsSolo = false;
            Assert.False(_sut.TeamsSolo);
        }

        [Test]
        public void TeamSolo_PropagationToCode()
        {
            _sut.TeamsSolo = true;
            Assert.True(_userSettings.TeamsSolo);
            _sut.TeamsSolo = false;
            Assert.False(_userSettings.TeamsSolo);
        }

        [Test]
        public void TeamSolo_DisablesNoAnswer()
        {
            _sut.TeamsSolo = true;
            Assert.False(_userSettings.TeamsNoAnswer);
            _sut.TeamsSolo = false;
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamSolo_PropertyChange()
        {
            _sut.TeamsSolo = true;
            AssertNotifications("TeamsSolo", "TeamsNoAnswer");
        }

        [Test]
        public void TeamSmall_PropagationFromCode()
        {
            _userSettings.TeamsSmall = true;
            Assert.True(_sut.TeamsSmall);
            _userSettings.TeamsSmall = false;
            Assert.False(_sut.TeamsSmall);
        }

        [Test]
        public void TeamSmall_PropagationToCode()
        {
            _sut.TeamsSmall = true;
            Assert.True(_userSettings.TeamsSmall);
            _sut.TeamsSmall = false;
            Assert.False(_userSettings.TeamsSmall);
        }

        [Test]
        public void TeamSmall_DisablesNoAnswer()
        {
            _sut.TeamsSmall = true;
            Assert.False(_userSettings.TeamsNoAnswer);
            _sut.TeamsSmall = false;
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamSmall_PropertyChange()
        {
            _sut.TeamsSmall = true;
            AssertNotifications("TeamsSmall", "TeamsNoAnswer");
        }

        [Test]
        public void TeamMedium_PropagationFromCode()
        {
            _userSettings.TeamsMedium = true;
            Assert.True(_sut.TeamsMedium);
            _userSettings.TeamsMedium = false;
            Assert.False(_sut.TeamsMedium);
        }

        [Test]
        public void TeamMedium_PropagationToCode()
        {
            _sut.TeamsMedium = true;
            Assert.True(_userSettings.TeamsMedium);
            _sut.TeamsMedium = false;
            Assert.False(_userSettings.TeamsMedium);
        }

        [Test]
        public void TeamMedium_DisablesNoAnswer()
        {
            _sut.TeamsMedium = true;
            Assert.False(_userSettings.TeamsNoAnswer);
            _sut.TeamsMedium = false;
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamMedium_PropertyChange()
        {
            _sut.TeamsMedium = true;
            AssertNotifications("TeamsMedium", "TeamsNoAnswer");
        }

        [Test]
        public void TeamLarge_PropagationFromCode()
        {
            _userSettings.TeamsLarge = true;
            Assert.True(_sut.TeamsLarge);
            _userSettings.TeamsLarge = false;
            Assert.False(_sut.TeamsLarge);
        }

        [Test]
        public void TeamLarge_PropagationToCode()
        {
            _sut.TeamsLarge = true;
            Assert.True(_userSettings.TeamsLarge);
            _sut.TeamsLarge = false;
            Assert.False(_userSettings.TeamsLarge);
        }

        [Test]
        public void TeamLarge_DisablesNoAnswer()
        {
            _sut.TeamsLarge = true;
            Assert.False(_userSettings.TeamsNoAnswer);
            _sut.TeamsLarge = false;
            Assert.True(_userSettings.TeamsNoAnswer);
        }

        [Test]
        public void TeamLarge_PropertyChange()
        {
            _sut.TeamsLarge = true;
            AssertNotifications("TeamsLarge", "TeamsNoAnswer");
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

        [Test]
        public void GenerateNewProfileId()
        {
            _sut.ProfileId = "";
            _sut.GenerateNewProfileId();
            Assert.AreEqual(_someGuid, _sut.ProfileId);
        }

        private void AssertNotifications(params string[] expecteds)
        {
            CollectionAssert.AreEqual(expecteds, _updatedProperties);
        }
    }
}