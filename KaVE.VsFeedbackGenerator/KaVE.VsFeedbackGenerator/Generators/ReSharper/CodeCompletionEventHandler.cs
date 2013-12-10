using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal interface ICodeCompletionLifecycleHandler
    {
        /// <summary>
        /// Invoked when the code completion is opened.
        /// </summary>
        /// <param name="prefix">The prefix, the completion is triggered on.</param>
        void OnOpened(string prefix);

        void SetLookupItems(IEnumerable<ILookupItem> items);

        /// <summary>
        /// Invoked when the prefix changes (typing or deletion of a character), while the code completion is opened.
        /// </summary>
        /// <param name="newPrefix">The prefix after it was changed.</param>
        void OnPrefixChanged(string newPrefix);

        /// <summary>
        /// Invoked for the initial selection, any manual selection changes (using the arrow keys), selection changes
        /// caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion).
        /// </summary>
        void OnSelectionChanged(ILookupItem selectedItem);

        /// <summary>
        /// Called when the completion is closed. This happens for every completion, regardless of whether is is
        /// applied or cancelled. This is invoked before <see cref="OnApplication"/>.
        /// </summary>
        void OnClosed();

        /// <summary>
        /// Invoked when the code completion is closed due to the application of an item.
        /// </summary>
        /// <param name="appliedItem">The item that is applied.</param>
        void OnApplication(ILookupItem appliedItem);

        /// <summary>
        /// Maybe invoked with additional information about how the event was terminated. Invocation occurs after
        /// <see cref="OnClosed"/> and <see cref="OnApplication"/>.
        /// </summary>
        /// <param name="trigger">The kind of trigger that terminated the event</param>
        void SetTerminatedBy(IDEEvent.Trigger trigger);

        /// <summary>
        /// Returns the created event.
        /// </summary>
        /// <returns>The completion event constructed by this builder</returns>
        void OnFinished();
    }

    internal class CodeCompletionEventHandler : AbstractEventGenerator, ICodeCompletionLifecycleHandler
    {
        private CompletionEvent _event;
        private bool _beforeShownCalled;
        private bool _terminated;

        public CodeCompletionEventHandler(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _beforeShownCalled = false;
            _terminated = false;
        }

        public void OnOpened(string prefix)
        {
            _event = Create<CompletionEvent>();
            _event.Prefix = prefix;
        }

        public void SetLookupItems(IEnumerable<ILookupItem> items)
        {
            // sometimes, the lookup is empty before this call
            // TODO test whether this is sometimes not called
            _beforeShownCalled = true;
            _event.ProposalCollection = items.ToProposalCollection();
        }

        public void OnSelectionChanged(ILookupItem selectedItem)
        {
            _event.AddSelection(selectedItem.ToProposal());
        }

        public void OnPrefixChanged(string newPrefix)
        {
            // TODO fix this
            //FireCompletionEvent(CompletionEvent.TerminationState.Filtered, DateTime.Now);
        }

        public void OnClosed()
        {
            _event.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            _event.TerminatedAt = DateTime.Now;
            _event.TerminatedBy = IDEEvent.Trigger.Unknown;
        }

        public void OnApplication(ILookupItem appliedItem)
        {
            _event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            _event.TerminatedBy = IDEEvent.Trigger.Typing;
            _terminated = true;
        }

        public void SetTerminatedBy(IDEEvent.Trigger trigger)
        {
            _event.TerminatedBy = trigger;
        }

        public void OnFinished()
        {
            Asserts.That(_beforeShownCalled, "beforeShown not called");
            Asserts.That(
                _event.TerminatedAs == CompletionEvent.TerminationState.Cancelled || _terminated,
                "event not terminated");
            Fire(_event);
        }
    }
}