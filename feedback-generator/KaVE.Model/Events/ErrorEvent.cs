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
 * 
 * Contributors:
 *    - Sebastian Proksch
 */

using System.Linq;
using System.Runtime.Serialization;
using KaVE.Utils;

namespace KaVE.Model.Events
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
            return base.Equals(other) && string.Equals(Content, other.Content) && StackTrace.SequenceEqual(other.StackTrace);
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
                hashCode = (hashCode*397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (StackTrace != null ? StackTrace.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, Content: {1}, StackTrace: [{2}]", base.ToString(), Content, string.Join("\n", StackTrace));
        }
    }
}
