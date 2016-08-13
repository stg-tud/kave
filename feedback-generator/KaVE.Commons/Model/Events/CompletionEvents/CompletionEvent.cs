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
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Model.Events.CompletionEvents
{
    [DataContract]
    public class CompletionEvent : IDEEvent, ICompletionEvent
    {
        [DataMember]
        public Context Context2 { get; set; }

        [DataMember]
        public IProposalCollection ProposalCollection { get; set; }

        [DataMember]
        public IKaVEList<IProposalSelection> Selections { get; set; }

        public IProposal LastSelectedProposal
        {
            get
            {
                return Selections.Count > 0
                    ? Selections.Last().Proposal
                    : ProposalCollection.Proposals.FirstOrDefault();
            }
        }

        [DataMember]
        public EventTrigger TerminatedBy { get; set; }

        [DataMember]
        public TerminationState TerminatedState { get; set; }

        [DataMember]
        public int ProposalCount { get; set; }

        public CompletionEvent()
        {
            Selections = Lists.NewList<IProposalSelection>();
            ProposalCollection = new ProposalCollection();
            Context2 = new Context();
            TerminatedState = TerminationState.Unknown;
            ProposalCount = 0;
        }

        public void AddSelection([NotNull] IProposal proposal, int index = -1)
        {
            var selectedAfter = DateTime.Now - TriggeredAt;
            Selections.Add(new ProposalSelection {Proposal = proposal, SelectedAfter = selectedAfter, Index = index});
        }

        private bool Equals(CompletionEvent other)
        {
            return base.Equals(other) && Equals(Context2, other.Context2) &&
                   Equals(ProposalCollection, other.ProposalCollection) &&
                   Selections.SequenceEqual(other.Selections) && TerminatedBy == other.TerminatedBy &&
                   TerminatedState == other.TerminatedState;
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
                hashCode = (hashCode*397) ^ Context2.GetHashCode();
                hashCode = (hashCode*397) ^ ProposalCollection.GetHashCode();
                hashCode = (hashCode*397) ^ Selections.GetHashCode();
                hashCode = (hashCode*397) ^ (int) TerminatedBy;
                hashCode = (hashCode*397) ^ (int) TerminatedState;
                return hashCode;
            }
        }
    }
}