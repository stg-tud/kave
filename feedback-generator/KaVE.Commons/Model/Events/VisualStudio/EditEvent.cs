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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events.VisualStudio
{
    [DataContract]
    public class EditEvent : IDEEvent
    {
        [DataMember]
        public Context Context2 { get; set; }

        [DataMember]
        public int NumberOfChanges { get; set; }

        [DataMember]
        public int SizeOfChanges { get; set; }

        protected bool Equals(EditEvent other)
        {
            return base.Equals(other) && NumberOfChanges == other.NumberOfChanges &&
                   SizeOfChanges == other.SizeOfChanges;
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
                hashCode = (hashCode*397) ^ NumberOfChanges;
                hashCode = (hashCode*397) ^ SizeOfChanges;
                return hashCode;
            }
        }
    }
}