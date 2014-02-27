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
        public DateTime SelectedAt { get; internal set; }

        [DataMember]
        public Proposal Proposal { get; private set; }

        public override string ToString()
        {
            return Proposal + "@" + SelectedAt;
        }

        protected bool Equals(ProposalSelection other)
        {
            return SelectedAt.Equals(other.SelectedAt) && Equals(Proposal, other.Proposal);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SelectedAt.GetHashCode();
                hashCode = (hashCode*397) ^ (Proposal != null ? Proposal.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}