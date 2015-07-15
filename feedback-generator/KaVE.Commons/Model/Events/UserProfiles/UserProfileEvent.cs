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

using System.Runtime.Serialization;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.UserProfiles
{
    public interface IUserProfileEvent
    {
        [NotNull]
        string ProfileId { get; }

        Educations Education { get; }

        Positions Position { get; }

        bool ProjectsCourses { get; }
        bool ProjectsPersonal { get; }
        bool ProjectsSharedSmall { get; }
        bool ProjectsSharedMedium { get; }
        bool ProjectsSharedLarge { get; }

        bool TeamsSolo { get; }
        bool TeamsSmall { get; }
        bool TeamsMedium { get; }
        bool TeamsLarge { get; }

        YesNoUnknown CodeReviews { get; }

        Likert7Point ProgrammingGeneral { get; }

        Likert7Point ProgrammingCSharp { get; }

        [NotNull]
        string Comment { get; }
    }

    [DataContract]
    public class UserProfileEvent : IDEEvent, IUserProfileEvent
    {
        [DataMember]
        public string ProfileId { get; set; }

        [DataMember]
        public Educations Education { get; set; }

        [DataMember]
        public Positions Position { get; set; }

        [DataMember]
        public bool ProjectsCourses { get; set; }

        [DataMember]
        public bool ProjectsPersonal { get; set; }

        [DataMember]
        public bool ProjectsSharedSmall { get; set; }

        [DataMember]
        public bool ProjectsSharedMedium { get; set; }

        [DataMember]
        public bool ProjectsSharedLarge { get; set; }

        [DataMember]
        public bool TeamsSolo { get; set; }

        [DataMember]
        public bool TeamsSmall { get; set; }

        [DataMember]
        public bool TeamsMedium { get; set; }

        [DataMember]
        public bool TeamsLarge { get; set; }

        [DataMember]
        public YesNoUnknown CodeReviews { get; set; }

        [DataMember]
        public Likert7Point ProgrammingGeneral { get; set; }

        [DataMember]
        public Likert7Point ProgrammingCSharp { get; set; }

        [DataMember]
        public string Comment { get; set; }

        public UserProfileEvent()
        {
            ProfileId = "";
            Comment = "";
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(UserProfileEvent other)
        {
            return base.Equals(other) && string.Equals(ProfileId, other.ProfileId) && Education == other.Education &&
                   Position == other.Position && ProjectsCourses == other.ProjectsCourses &&
                   ProjectsPersonal == other.ProjectsPersonal && ProjectsSharedSmall == other.ProjectsSharedSmall &&
                   ProjectsSharedMedium == other.ProjectsSharedMedium &&
                   ProjectsSharedLarge == other.ProjectsSharedLarge && TeamsSolo == other.TeamsSolo &&
                   TeamsSmall == other.TeamsSmall && TeamsMedium == other.TeamsMedium && TeamsLarge == other.TeamsLarge &&
                   CodeReviews == other.CodeReviews && ProgrammingGeneral == other.ProgrammingGeneral &&
                   ProgrammingCSharp == other.ProgrammingCSharp && string.Equals(Comment, other.Comment);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ ProfileId.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Education;
                hashCode = (hashCode*397) ^ (int) Position;
                hashCode = (hashCode*397) ^ ProjectsCourses.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsPersonal.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsSharedSmall.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsSharedMedium.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsSharedLarge.GetHashCode();
                hashCode = (hashCode*397) ^ TeamsSolo.GetHashCode();
                hashCode = (hashCode*397) ^ TeamsSmall.GetHashCode();
                hashCode = (hashCode*397) ^ TeamsMedium.GetHashCode();
                hashCode = (hashCode*397) ^ TeamsLarge.GetHashCode();
                hashCode = (hashCode*397) ^ (int) CodeReviews;
                hashCode = (hashCode*397) ^ (int) ProgrammingGeneral;
                hashCode = (hashCode*397) ^ (int) ProgrammingCSharp;
                hashCode = (hashCode*397) ^ Comment.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}