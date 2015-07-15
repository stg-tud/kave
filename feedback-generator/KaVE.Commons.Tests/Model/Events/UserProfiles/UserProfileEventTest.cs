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

using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events.UserProfiles
{
    internal class UserProfileEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UserProfileEvent();
            Assert.AreEqual("", sut.ProfileId);
            Assert.AreEqual(Educations.Unknown, sut.Education);
            Assert.AreEqual(Positions.Unknown, sut.Position);
            Assert.False(sut.ProjectsCourses);
            Assert.False(sut.ProjectsPersonal);
            Assert.False(sut.ProjectsSharedSmall);
            Assert.False(sut.ProjectsSharedMedium);
            Assert.False(sut.ProjectsSharedLarge);
            Assert.False(sut.TeamsSolo);
            Assert.False(sut.TeamsSmall);
            Assert.False(sut.TeamsMedium);
            Assert.False(sut.TeamsLarge);
            Assert.AreEqual(YesNoUnknown.Unknown, sut.CodeReviews);
            Assert.AreEqual(Likert7Point.Unknown, sut.ProgrammingGeneral);
            Assert.AreEqual(Likert7Point.Unknown, sut.ProgrammingCSharp);
            Assert.AreEqual("", sut.Comment);
        }

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new UserProfileEvent());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new UserProfileEvent();
            var b = new UserProfileEvent();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new UserProfileEvent
            {
                ProfileId = "p",
                Education = Educations.Training,
                Position = Positions.HobbyProgrammer,
                ProjectsCourses = true,
                ProjectsPersonal = true,
                ProjectsSharedSmall = true,
                ProjectsSharedMedium = true,
                ProjectsSharedLarge = true,
                TeamsSolo = true,
                TeamsSmall = true,
                TeamsMedium = true,
                TeamsLarge = true,
                CodeReviews = YesNoUnknown.Yes,
                ProgrammingGeneral = Likert7Point.Negative1,
                ProgrammingCSharp = Likert7Point.Negative2,
                Comment = "f"
            };
            var b = new UserProfileEvent
            {
                ProfileId = "p",
                Education = Educations.Training,
                Position = Positions.HobbyProgrammer,
                ProjectsCourses = true,
                ProjectsPersonal = true,
                ProjectsSharedSmall = true,
                ProjectsSharedMedium = true,
                ProjectsSharedLarge = true,
                TeamsSolo = true,
                TeamsSmall = true,
                TeamsMedium = true,
                TeamsLarge = true,
                CodeReviews = YesNoUnknown.Yes,
                ProgrammingGeneral = Likert7Point.Negative1,
                ProgrammingCSharp = Likert7Point.Negative2,
                Comment = "f"
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProfileId()
        {
            var a = new UserProfileEvent
            {
                ProfileId = "p"
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentEducation()
        {
            var a = new UserProfileEvent
            {
                Education = Educations.Training
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentPosition()
        {
            var a = new UserProfileEvent
            {
                Position = Positions.HobbyProgrammer
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_Courses()
        {
            var a = new UserProfileEvent
            {
                ProjectsCourses = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_Personal()
        {
            var a = new UserProfileEvent
            {
                ProjectsPersonal = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_SharedSmall()
        {
            var a = new UserProfileEvent
            {
                ProjectsSharedSmall = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_SharedMedium()
        {
            var a = new UserProfileEvent
            {
                ProjectsSharedMedium = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_SharedLarge()
        {
            var a = new UserProfileEvent
            {
                ProjectsSharedLarge = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTeam_Solo()
        {
            var a = new UserProfileEvent
            {
                TeamsSolo = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTeam_Small()
        {
            var a = new UserProfileEvent
            {
                TeamsSmall = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTeam_Medium()
        {
            var a = new UserProfileEvent
            {
                TeamsMedium = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentTeam_Large()
        {
            var a = new UserProfileEvent
            {
                TeamsLarge = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentCodeReviews()
        {
            var a = new UserProfileEvent
            {
                CodeReviews = YesNoUnknown.Yes
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProgrammingGeneral()
        {
            var a = new UserProfileEvent
            {
                ProgrammingGeneral = Likert7Point.Negative1
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProgrammingCSharp()
        {
            var a = new UserProfileEvent
            {
                ProgrammingCSharp = Likert7Point.Negative1
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentComment()
        {
            var a = new UserProfileEvent
            {
                Comment = "f"
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferntBaseInformation()
        {
            var a = new UserProfileEvent
            {
                Id = "1"
            };
            var b = new UserProfileEvent
            {
                Id = "2"
            };
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}