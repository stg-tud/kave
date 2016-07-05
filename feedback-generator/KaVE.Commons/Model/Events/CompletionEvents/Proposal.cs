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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    [DataContract]
    public class Proposal : IProposal
    {
        [DataMember]
        public IName Name { get; set; }

        [DataMember]
        public int? Relevance { get; set; }

        private bool Equals(Proposal other)
        {
            return Equals(Name, other.Name) && Relevance == other.Relevance;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hcRelevance = Relevance.HasValue ? Relevance.Value : -1;
                return 129487 + ((Name != null ? Name.GetHashCode() : 0)*397) ^ hcRelevance;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}