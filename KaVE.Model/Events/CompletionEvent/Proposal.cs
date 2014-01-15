using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// A completion proposal.
    /// </summary>
    public class Proposal
    {
        /// <summary>
        /// The name of the element that is proposed for completion.
        /// </summary>
        public IName Name { get; set; }

        /// <summary>
        /// The relevance this proposal was ranked with by the code completion. Bigger better.
        /// Optional if no relevance is known.
        /// </summary>
        public int? Relevance { get; set; }

        private bool Equals(Proposal other)
        {
            return Equals(Name, other.Name) && Relevance == other.Relevance;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Proposal) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^ Relevance.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("Proposal: {0} ({1})", Name, Relevance);
        }
    }
}