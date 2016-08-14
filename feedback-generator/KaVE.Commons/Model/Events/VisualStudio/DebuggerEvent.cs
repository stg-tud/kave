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

namespace KaVE.Commons.Model.Events.VisualStudio
{
    [DataContract]
    public class DebuggerEvent : IDEEvent
    {
        [DataMember]
        public DebuggerMode Mode { get; set; }

        [DataMember]
        public string Reason { get; set; }

        [DataMember]
        public string Action { get; set; }

        protected bool Equals(DebuggerEvent other)
        {
            return base.Equals(other) && Mode == other.Mode && string.Equals(Reason, other.Reason) &&
                   string.Equals(Action, other.Action);
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
                hashCode = (hashCode*397) ^ (int) Mode;
                hashCode = (hashCode*397) ^ (Reason != null ? Reason.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Action != null ? Action.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public enum DebuggerMode
    {
        Design,
        Run,
        Break,
        ExceptionThrown,
        ExceptionNotHandled
    }
}