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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    [DataContract]
    public class ProposalCollection : IProposalCollection
    {
        [DataMember]
        public IKaVEList<IProposal> Proposals { get; private set; }

        public int Count
        {
            get { return Proposals.Count; }
        }

        public ProposalCollection()
        {
            Proposals = Lists.NewList<IProposal>();
        }

        public ProposalCollection(IEnumerable<IProposal> proposals) : this()
        {
            foreach (var p in proposals)
            {
                Add(p);
            }
        }

        public int GetPosition(IProposal proposal)
        {
            return Proposals.IndexOf(proposal);
        }

        public void Add(IProposal proposal)
        {
            Proposals.Add(proposal);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IProposal> GetEnumerator()
        {
            return Proposals.GetEnumerator();
        }

        private bool Equals(ProposalCollection other)
        {
            return Proposals.SequenceEqual(other.Proposals);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return Proposals.GetHashCode();
        }
    }
}