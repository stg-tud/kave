using System.Collections.Generic;
using System.Linq;
using KaVE.Model.Names;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// An ordered collection of completion proposals as, for example,
    /// presented to the code-completion user in the code-completion
    /// dropdown.
    /// </summary>
    public class ProposalCollection
    {
        /// <summary>
        /// The proposals contained in this collection.
        /// </summary>
        public IList<Proposal> Proposals { get; private set; }

        /// <summary>
        /// Creates a collection from a list of proposals.
        /// </summary>
        public ProposalCollection(IEnumerable<Proposal> proposals)
        {
            Proposals = proposals.ToList();
        }

        /// <returns>The 0-based position of the given proposal in this collection.</returns>
        public int GetPosition(Proposal proposal)
        {
            return Proposals.IndexOf(proposal);
        }

        /// <returns>The name of the completion engine that recommended the given proposal.</returns>
        public IName GetCompletionEngine(Proposal proposal)
        {
            return null;
        }

        /// <returns>The names of all completion engines that provided proposals to this collection.</returns>
        public ISet<IName> GetActiveCompletionEngines()
        {
            return null;
        }


        /// <returns>All proposals contributed by a specific completion engine, in the same order as in this collection.</returns>
        public IList<Proposal> GetProposalsFrom(IName engine)
        {
            return null;
        }

        protected bool Equals(ProposalCollection other)
        {
            return Proposals != null && Proposals.SequenceEqual(other.Proposals);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ProposalCollection) obj);
        }

        public override int GetHashCode()
        {
            return (Proposals != null ? Proposals.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Join(",", Proposals);
        }
    }
}