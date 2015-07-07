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

        bool ProjectsNoAnswer { get; }
        bool ProjectsCourses { get; }
        bool ProjectsPrivate { get; }
        bool ProjectsTeamSmall { get; }
        bool ProjectsTeamLarge { get; }
        bool ProjectsCommercial { get; }

        Likert7Point ProgrammingGeneral { get; }

        Likert7Point ProgrammingCSharp { get; }

        [NotNull]
        string Comment { get; }
    }

    // TODO @seb: write (de-) serialization tests!!
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
        public bool ProjectsNoAnswer { get; set; }

        [DataMember]
        public bool ProjectsCourses { get; set; }

        [DataMember]
        public bool ProjectsPrivate { get; set; }

        [DataMember]
        public bool ProjectsTeamSmall { get; set; }

        [DataMember]
        public bool ProjectsTeamLarge { get; set; }

        [DataMember]
        public bool ProjectsCommercial { get; set; }

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
            ProjectsNoAnswer = true;
        }

        private bool Equals(UserProfileEvent other)
        {
            return base.Equals(other) && string.Equals(ProfileId, other.ProfileId) && Education == other.Education &&
                   Position == other.Position && ProjectsNoAnswer == other.ProjectsNoAnswer &&
                   ProjectsCourses == other.ProjectsCourses && ProjectsPrivate == other.ProjectsPrivate &&
                   ProjectsTeamSmall == other.ProjectsTeamSmall &&
                   ProjectsTeamLarge == other.ProjectsTeamLarge &&
                   ProjectsCommercial == other.ProjectsCommercial && ProgrammingGeneral == other.ProgrammingGeneral &&
                   ProgrammingCSharp == other.ProgrammingCSharp && string.Equals(Comment, other.Comment);
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
                hashCode = (hashCode*397) ^ ProfileId.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Education;
                hashCode = (hashCode*397) ^ (int) Position;
                hashCode = (hashCode*397) ^ ProjectsNoAnswer.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsCourses.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsPrivate.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsTeamSmall.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsTeamLarge.GetHashCode();
                hashCode = (hashCode*397) ^ ProjectsCommercial.GetHashCode();
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