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
using System.Text;
using KaVE.Commons.Model.SSTs.Impl.Visitor;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using KaVE.Commons.Utils;

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

        [DataMember]
        public Trigger TerminatedBy { get; set; }

        [DataMember]
        public TerminationState TerminatedState { get; set; }

        public string Prefix
        {
            get
            {
                var builder = new StringBuilder();
                Context2.SST.Accept(new CompletionPrefixVisitor(), builder);
                return builder.ToString();
            }
        }

        public CompletionEvent()
        {
            Selections = Lists.NewList<IProposalSelection>();
            ProposalCollection = new ProposalCollection();
            Context2 = new Context();
            TerminatedState = TerminationState.Unknown;
        }

        public void AddSelection([NotNull] IProposal proposal)
        {
            var selectedAfter = DateTime.Now - TriggeredAt;
            Selections.Add(new ProposalSelection {Proposal = proposal, SelectedAfter = selectedAfter});
        }

        private bool Equals(CompletionEvent other)
        {
            return base.Equals(other) && Equals(Context2, other.Context2) &&
                   Equals(ProposalCollection, other.ProposalCollection) && string.Equals(Prefix, other.Prefix) &&
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
                hashCode = (hashCode*397) ^ Prefix.GetHashCode();
                hashCode = (hashCode*397) ^ Selections.GetHashCode();
                hashCode = (hashCode*397) ^ (int) TerminatedBy;
                hashCode = (hashCode*397) ^ (int) TerminatedState;
                return hashCode;
            }
        }
    }
}