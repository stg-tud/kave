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
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events
{
    [DataContract]
    public abstract class IDEEvent : IIDEEvent
    {
        /// <summary>
        ///     Unique id of this event, used for tracking in feedback processing.
        /// </summary>
        public string Id { get; set; }

        [DataMember]
        public string IDESessionUUID { get; set; }

        [DataMember]
        public string KaVEVersion { get; set; }

        [DataMember]
        public DateTime? TriggeredAt { get; set; }

        [DataMember]
        public EventTrigger TriggeredBy { get; set; }

        public DateTime? TerminatedAt
        {
            get { return TriggeredAt + Duration; }
            set { Duration = value - TriggeredAt; }
        }

        [DataMember]
        public TimeSpan? Duration { get; set; }

        [DataMember]
        public IWindowName ActiveWindow { get; set; }

        [DataMember]
        public IDocumentName ActiveDocument { get; set; }

        private bool Equals(IDEEvent other)
        {
            return string.Equals(Id, other.Id) && string.Equals(IDESessionUUID, other.IDESessionUUID) &&
                   string.Equals(KaVEVersion, other.KaVEVersion) && TriggeredAt.Equals(other.TriggeredAt) &&
                   TriggeredBy == other.TriggeredBy && Duration.Equals(other.Duration) &&
                   Equals(ActiveWindow, other.ActiveWindow) && Equals(ActiveDocument, other.ActiveDocument);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Id != null ? Id.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (IDESessionUUID != null ? IDESessionUUID.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (KaVEVersion != null ? KaVEVersion.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ TriggeredAt.GetHashCode();
                hashCode = (hashCode*397) ^ (int) TriggeredBy;
                hashCode = (hashCode*397) ^ Duration.GetHashCode();
                hashCode = (hashCode*397) ^ (ActiveWindow != null ? ActiveWindow.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ActiveDocument != null ? ActiveDocument.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}