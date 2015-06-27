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
using System.Runtime.Serialization;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events
{
    public interface IUserProfileEvent
    {
        [NotNull]
        string Name { get; }

        [NotNull]
        MailAddress Email { get; }

        WorkPosition Position { get; }

        byte? ExperienceYears { get; }

        ProjectExperience ProjectExperience { get; }

        SelfEstimatedExperience SelfEstimatedExperience { get; }

        [NotNull]
        string Feedback { get; }
    }

    public enum WorkPosition
    {
        Unknown,
        Student,
        Researcher,
        Professional
    }

    public enum ProjectExperience
    {
        Unknown,
        ProfessionalProjects,
        OpenSourceButNotProfessional,
        OnlyPrivateProjects,
        OnlyIfRequired
    }

    public enum SelfEstimatedExperience
    {
        Unknown,
        VeryBad,
        Bad,
        Neutral,
        Good,
        VeryGood
    }

    [DataContract]
    public class UserProfileEvent : IDEEvent, IUserProfileEvent
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public MailAddress Email { get; set; }

        [DataMember]
        public WorkPosition Position { get; set; }

        [DataMember]
        public byte? ExperienceYears { get; set; }

        [DataMember]
        public ProjectExperience ProjectExperience { get; set; }

        [DataMember]
        public SelfEstimatedExperience SelfEstimatedExperience { get; set; }

        [DataMember]
        public string Feedback { get; set; }

        public UserProfileEvent()
        {
            Name = "";
            Email = new MailAddress("anonymous@acme.com");
            Feedback = "";
        }

        private bool Equals(UserProfileEvent other)
        {
            return base.Equals(other) && string.Equals(Name, other.Name) && Email.Equals(other.Email) &&
                   Position == other.Position && ExperienceYears == other.ExperienceYears &&
                   ProjectExperience == other.ProjectExperience &&
                   SelfEstimatedExperience == other.SelfEstimatedExperience && string.Equals(Feedback, other.Feedback);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ Name.GetHashCode();
                hashCode = (hashCode*397) ^ Email.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Position;
                hashCode = (hashCode*397) ^ ExperienceYears.GetHashCode();
                hashCode = (hashCode*397) ^ (int) ProjectExperience;
                hashCode = (hashCode*397) ^ (int) SelfEstimatedExperience;
                hashCode = (hashCode*397) ^ Feedback.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}