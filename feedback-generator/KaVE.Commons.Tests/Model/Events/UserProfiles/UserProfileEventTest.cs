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
            Assert.True(sut.ProjectsNoAnswer);
            Assert.False(sut.ProjectsCourses);
            Assert.False(sut.ProjectsPrivate);
            Assert.False(sut.ProjectsTeamInsignificant);
            Assert.False(sut.ProjectsTeamSignificant);
            Assert.False(sut.ProjectsCommercial);
            Assert.AreEqual(Likert7Point.Unknown, sut.ProgrammingGeneral);
            Assert.AreEqual(Likert7Point.Unknown, sut.ProgrammingCSharp);
            Assert.AreEqual("", sut.Feedback);
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
                Education = Educations.Apprenticeship,
                Position = Positions.HobbyProgrammer,
                ProjectsNoAnswer = true,
                ProjectsCourses = true,
                ProjectsPrivate = true,
                ProjectsTeamInsignificant = true,
                ProjectsTeamSignificant = true,
                ProjectsCommercial = true,
                ProgrammingGeneral = Likert7Point.Negative1,
                ProgrammingCSharp = Likert7Point.Negative2,
                Feedback = "f"
            };
            var b = new UserProfileEvent
            {
                ProfileId = "p",
                Education = Educations.Apprenticeship,
                Position = Positions.HobbyProgrammer,
                ProjectsNoAnswer = true,
                ProjectsCourses = true,
                ProjectsPrivate = true,
                ProjectsTeamInsignificant = true,
                ProjectsTeamSignificant = true,
                ProjectsCommercial = true,
                ProgrammingGeneral = Likert7Point.Negative1,
                ProgrammingCSharp = Likert7Point.Negative2,
                Feedback = "f"
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
                Education = Educations.Apprenticeship
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
        public void Equality_DifferentProjects_NoAnswer()
        {
            var a = new UserProfileEvent
            {
                ProjectsNoAnswer = false
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
        public void Equality_DifferentProjects_Private()
        {
            var a = new UserProfileEvent
            {
                ProjectsPrivate = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_TeamInsignificant()
        {
            var a = new UserProfileEvent
            {
                ProjectsTeamInsignificant = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_TeamSignificant()
        {
            var a = new UserProfileEvent
            {
                ProjectsTeamSignificant = true
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentProjects_Commercial()
        {
            var a = new UserProfileEvent
            {
                ProjectsCommercial = true
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
        public void Equality_DifferentFeedback()
        {
            var a = new UserProfileEvent
            {
                Feedback = "f"
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