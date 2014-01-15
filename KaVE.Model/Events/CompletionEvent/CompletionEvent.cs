using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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

        public override string ToString()
        {
            return IDESessionUUID + "@" + TriggeredAt + " {\n" +
                   "  Context : " + Context + "\n" +
                   "  Proposals : " + ProposalCollection + "\n" +
                   "  Actions : " + string.Join(",", Selections) + "\n" +
                   "  TerminatedBy : " + TerminatedBy + "\n" +
                   "}";
        }
    }
}