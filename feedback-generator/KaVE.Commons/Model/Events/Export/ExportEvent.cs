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
using System.Net.Mail;
using System.Runtime.Serialization;

namespace KaVE.Commons.Model.Events.Export
{
    [DataContract]
    public class ExportEvent : IDEEvent
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public MailAddress Mail { get; set; }

        [DataMember]
        public Category Category { get; set; }

        [DataMember]
        public long? Number { get; set; }

        [DataMember]
        public Valuation Valuation { get; set; }

        [DataMember]
        public string Feedback { get; set; }

        protected bool Equals(ExportEvent other)
        {
            return base.Equals(other) && Equals(UserName, other.UserName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ UserName.GetHashCode();
                hashCode = (hashCode*397) ^ Mail.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Category;
                hashCode = (hashCode*397) ^ Number.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Valuation;
                hashCode = (hashCode*397) ^ Feedback.GetHashCode();
                return hashCode;
            }
        }
    }
}