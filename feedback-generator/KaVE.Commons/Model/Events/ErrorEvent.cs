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

using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events
{
    [DataContract]
    public class ErrorEvent : IDEEvent
    {
        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string[] StackTrace { get; set; }

        protected bool Equals(ErrorEvent other)
        {
            var isBaseEqual = base.Equals(other);
            var hasEqualContent = string.Equals(Content, other.Content);

            if (StackTrace == other.StackTrace)
            {
                return isBaseEqual && hasEqualContent;
            }

            if (StackTrace == null || other.StackTrace == null)
            {
                return false;
            }
            var hasEqualStacktrace = StackTrace.SequenceEqual(other.StackTrace);
            return isBaseEqual && hasEqualContent && hasEqualStacktrace;
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
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 17);
                hashCode = (hashCode*397) ^ (StackTrace != null ? HashCodeUtils.For(397, StackTrace) : 13);
                return hashCode;
            }
        }
    }
}