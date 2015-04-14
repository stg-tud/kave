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
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.VisualStudio
{
    [DataContract]
    public class BuildEvent : IDEEvent
    {
        public BuildEvent()
        {
            Targets = new List<BuildTarget>();
        }

        [DataMember]
        public string Scope { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember, NotNull]
        public IList<BuildTarget> Targets { get; set; }

        protected bool Equals(BuildEvent other)
        {
            return base.Equals(other) && string.Equals(Scope, other.Scope) && string.Equals(Action, other.Action) &&
                   Targets.SequenceEqual(other.Targets);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (Scope != null ? Scope.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Action != null ? Action.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (HashCodeUtils.For(397, Targets));
                return hashCode;
            }
        }
    }

    [DataContract]
    public class BuildTarget
    {
        [DataMember]
        public string Project { get; set; }

        [DataMember]
        public string ProjectConfiguration { get; set; }

        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public string SolutionConfiguration { get; set; }

        [DataMember]
        public DateTime? StartedAt { get; set; }

        [DataMember]
        public TimeSpan? Duration { get; set; }

        [DataMember]
        public bool Successful { get; set; }

        protected bool Equals(BuildTarget other)
        {
            return string.Equals(Project, other.Project) &&
                   string.Equals(ProjectConfiguration, other.ProjectConfiguration) &&
                   string.Equals(Platform, other.Platform) &&
                   string.Equals(SolutionConfiguration, other.SolutionConfiguration) &&
                   StartedAt.Equals(other.StartedAt) && Duration.Equals(other.Duration) &&
                   Successful.Equals(other.Successful);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Project != null ? Project.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ProjectConfiguration != null ? ProjectConfiguration.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Platform != null ? Platform.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (SolutionConfiguration != null ? SolutionConfiguration.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ StartedAt.GetHashCode();
                hashCode = (hashCode*397) ^ Duration.GetHashCode();
                hashCode = (hashCode*397) ^ Successful.GetHashCode();
                return hashCode;
            }
        }
    }
}