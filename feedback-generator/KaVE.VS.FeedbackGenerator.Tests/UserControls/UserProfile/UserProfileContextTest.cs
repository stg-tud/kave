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
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.UserControls.UserProfile
{
    public class UserProfileContextTest
    {
        private string _someGuid;

        private UserProfileSettings _userSettings;
        private IUserProfileSettingsUtils _userSettingsUtil;
        private List<string> _updatedProperties;

        private UserProfileContext _sut;

        [SetUp]
        public void SetUp()
        {
            _userSettings = new UserProfileSettings
            {
                ProfileId = ""
            };
            _updatedProperties = new List<string>();


            var newGuid = Guid.NewGuid();
            _someGuid = newGuid.ToString();
            var rnd = Mock.Of<IRandomizationUtils>();
            Mock.Get(rnd).Setup(r => r.GetRandomGuid()).Returns(newGuid);

            _userSettingsUtil = Mock.Of<IUserProfileSettingsUtils>();
            Mock.Get(_userSettingsUtil).Setup(u => u.CreateNewProfileId()).Returns(_someGuid);

            _sut = new UserProfileContext(_userSettings, _userSettingsUtil);

            _sut.PropertyChanged += (sender, args) => { _updatedProperties.Add(args.PropertyName); };
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
        public void ProjectsCourses_PropertyChange()
        {
            _sut.ProjectsCourses = true;
            AssertNotifications("ProjectsCourses");
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
        public void ProjectsPersonal_PropertyChange()
        {
            _sut.ProjectsPersonal = true;
            AssertNotifications("ProjectsPersonal");
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
        public void ProjectsSharedSmall_PropertyChange()
        {
            _sut.ProjectsSharedSmall = true;
            AssertNotifications("ProjectsSharedSmall");
        }

        [Test]
        public void ProjectsSharedMedium_PropagationFromCode()
        {
            _userSettings.ProjectsSharedMedium = true;
            Assert.True(_sut.ProjectsSharedMedium);
            _userSettings.ProjectsSharedMedium = false;
            Assert.False(_sut.ProjectsSharedMedium);
        }

        [Test]
        public void ProjectsSharedMedium_PropagationToCode()
        {
            _sut.ProjectsSharedMedium = true;
            Assert.True(_userSettings.ProjectsSharedMedium);
            _sut.ProjectsSharedMedium = false;
            Assert.False(_userSettings.ProjectsSharedMedium);
        }

        [Test]
        public void ProjectsSharedMedium_PropertyChange()
        {
            _sut.ProjectsSharedMedium = true;
            AssertNotifications("ProjectsSharedMedium");
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
        public void ProjectsSharedLarge_PropertyChange()
        {
            _sut.ProjectsSharedLarge = true;
            AssertNotifications("ProjectsSharedLarge");
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
        public void TeamSolo_PropertyChange()
        {
            _sut.TeamsSolo = true;
            AssertNotifications("TeamsSolo");
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
        public void TeamSmall_PropertyChange()
        {
            _sut.TeamsSmall = true;
            AssertNotifications("TeamsSmall");
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
        public void TeamMedium_PropertyChange()
        {
            _sut.TeamsMedium = true;
            AssertNotifications("TeamsMedium");
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
        public void TeamLarge_PropertyChange()
        {
            _sut.TeamsLarge = true;
            AssertNotifications("TeamsLarge");
        }

        [Test]
        public void CodeReviews_PropagationFromCode()
        {
            _userSettings.CodeReviews = YesNoUnknown.Yes;
            Assert.AreEqual(YesNoUnknown.Yes, _sut.CodeReviews);
        }

        [Test]
        public void CodeReviews_PropagationToCode()
        {
            _sut.CodeReviews = YesNoUnknown.Yes;
            Assert.AreEqual(YesNoUnknown.Yes, _userSettings.CodeReviews);
        }

        [Test]
        public void CodeReviews_PropertyChange()
        {
            _sut.CodeReviews = YesNoUnknown.Yes;
            AssertNotifications("CodeReviews");
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
        public void YesNoOptions()
        {
            var actuals = _sut.YesNoOptions;
            var expecteds = Enum.GetValues(typeof (YesNoUnknown));
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