using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using KaVE.Utils;

namespace KaVE.Model.Events.CompletionEvent
{
    /// <summary>
    /// A completion event represents one cycle through the code completion
    /// process:
    /// <list type="number">
    /// <item><description>The completion is triggered.</description></item>
    /// <item><description>The user browses the code completion.</description></item>
    /// <item><description>The user applies a completion proposal or cancels the code completion.</description></item>
    /// </list>
    /// </summary>
    [DataContract]
    public class CompletionEvent : IDEEvent
    {
        public enum TerminationState
        {
            Applied,
            Cancelled,
            Filtered
        }

        /// <summary>
        /// Creates a new completion event, setting the
        /// <see cref="IDEEvent.TriggeredAt"/> property to the current time.
        /// The <see cref="ProposalSelection"/> of the new event will be empty.
        /// </summary>
        public CompletionEvent()
        {
            Selections = new List<ProposalSelection>();
        }

        /// <summary>
        /// The context in which the completion takes place. These information
        /// is what is used as the query to the intelligent code completion.
        /// </summary>
        [DataMember]
        public Context Context { get; set; }

        /// <summary>
        /// The completion proposals shown to the user.
        /// </summary>
        [DataMember]
        public ProposalCollection ProposalCollection { get; set; }

        /// <summary>
        /// The typed prefix that is used to filter the proposal list.
        /// </summary>
        [DataMember]
        public string Prefix { get; set; }

        /// <summary>
        /// The list of proposals from the ProposalCollection that where
        /// selected while the code completion was active. Proposals appear in
        /// the order of selection.
        /// 
        /// Add selections by <see cref="AddSelection"/>.
        /// </summary>
        [DataMember]
        public IList<ProposalSelection> Selections { get; set; }

        public void AddSelection(Proposal proposal)
        {
            Selections.Add(new ProposalSelection(proposal) {SelectedAt = DateTime.Now});
        }

        /// <summary>
        /// The kind of interaction that termined the completion, e.g., by a mouse click.
        /// </summary>
        [DataMember]
        public Trigger TerminatedBy { get; set; }

        /// <summary>
        /// The status with which the completion terminated, e.g., as cancelled.
        /// </summary>
        [DataMember]
        public TerminationState TerminatedAs { get; set; }

        protected bool Equals(CompletionEvent other)
        {
            return base.Equals(other) && Equals(Context, other.Context) &&
                   Equals(ProposalCollection, other.ProposalCollection) && string.Equals(Prefix, other.Prefix) &&
                   Selections.SequenceEqual(other.Selections) && TerminatedBy == other.TerminatedBy &&
                   TerminatedAs == other.TerminatedAs;
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
                hashCode = (hashCode*397) ^ (Context != null ? Context.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ProposalCollection != null ? ProposalCollection.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Prefix != null ? Prefix.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Selections != null ? Selections.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) TerminatedBy;
                hashCode = (hashCode*397) ^ (int) TerminatedAs;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return
                string.Format(
                    "{0}, Context: {1}, ProposalCollection: {2}, Prefix: {3}, Selections: [{4}], TerminatedBy: {5}, TerminatedAs: {6}",
                    base.ToString(),
                    Context,
                    ProposalCollection,
                    Prefix,
                    string.Join(", ", Selections),
                    TerminatedBy,
                    TerminatedAs);
        }
    }
}