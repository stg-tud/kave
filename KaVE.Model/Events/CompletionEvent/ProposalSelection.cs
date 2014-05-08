using System;
using System.Runtime.Serialization;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    [DataContract]
    public class ProposalSelection
    {
        public ProposalSelection(Proposal proposal)
        {
            Proposal = proposal;
        }

        [DataMember]
        public TimeSpan? SelectedAfter { get; set; }

        [DataMember]
        public Proposal Proposal { get; private set; }

        public override string ToString()
        {
            return Proposal + "@" + SelectedAfter;
        }

        protected bool Equals(ProposalSelection other)
        {
            return SelectedAfter.Equals(other.SelectedAfter) && Equals(Proposal, other.Proposal);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SelectedAfter.GetHashCode();
                hashCode = (hashCode*397) ^ (Proposal != null ? Proposal.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}