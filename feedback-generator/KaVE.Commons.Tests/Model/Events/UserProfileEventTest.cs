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

using System.Net.Mail;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Events
{
    internal class UserProfileEventTest
    {
        [Test]
        public void DefaultValues()
        {
            var sut = new UserProfileEvent();
            Assert.AreEqual("", sut.Name);
            Assert.AreEqual(new MailAddress("anonymous@acme.com"), sut.Email);
            Assert.AreEqual(WorkPosition.Unknown, sut.Position);
            Assert.AreEqual(ProjectExperience.Unknown, sut.ProjectExperience);
            Assert.False(sut.ExperienceYears.HasValue);
            Assert.AreEqual(SelfEstimatedExperience.Unknown, sut.SelfEstimatedExperience);
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
                Name = "a",
                Email = new MailAddress("a@b.c"),
                Position = WorkPosition.Student,
                ProjectExperience = ProjectExperience.OnlyIfRequired,
                ExperienceYears = 1,
                SelfEstimatedExperience = SelfEstimatedExperience.Neutral,
                Feedback = "f"
            };
            var b = new UserProfileEvent
            {
                Name = "a",
                Email = new MailAddress("a@b.c"),
                Position = WorkPosition.Student,
                ProjectExperience = ProjectExperience.OnlyIfRequired,
                ExperienceYears = 1,
                SelfEstimatedExperience = SelfEstimatedExperience.Neutral,
                Feedback = "f"
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferntName()
        {
            var a = new UserProfileEvent
            {
                Name = "a"
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferntEmail()
        {
            var a = new UserProfileEvent
            {
                Email = new MailAddress("a@b.c")
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
                Position = WorkPosition.Professional
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExperienceProjects()
        {
            var a = new UserProfileEvent
            {
                ProjectExperience = ProjectExperience.OnlyPrivateProjects
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentExperienceYears()
        {
            var a = new UserProfileEvent
            {
                ExperienceYears = 1
            };
            var b = new UserProfileEvent();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSelfEstimation()
        {
            var a = new UserProfileEvent
            {
                SelfEstimatedExperience = SelfEstimatedExperience.Neutral
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