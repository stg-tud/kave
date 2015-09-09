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
using System.Runtime.Serialization;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events.GitEvents
{
    [DataContract]
    public class GitAction
    {
        [DataMember]
        public DateTime? ExecutedAt { get; set; }

        [DataMember]
        public GitActionType ActionType { get; set; }

        private bool Equals(GitAction other)
        {
            return Equals(ExecutedAt, other.ExecutedAt) && Equals(ActionType, other.ActionType);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ExecutedAt != null ? ExecutedAt.GetHashCode() : 1;
                hashCode = (hashCode * 397) ^ (int) ActionType;
                return hashCode;
            }
        }
    }

    public enum GitActionType
    {
        Unknown = 0,
        Commit
    }
}