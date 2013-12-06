using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

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
        /// Invoked when the code completion is cancelled.
        /// </summary>
        /// <param name="timeOfCancellation">The cancellation time. May be in the past.</param>
        void OnCancellation(DateTime timeOfCancellation);

        /// <summary>
        /// Invoked when the code completion is closed due to the application of an item.
        /// </summary>
        /// <param name="timeOfApplication">The application time. May be in the past.</param>
        /// <param name="appliedItem">The item that is applied.</param>
        void OnApplication(DateTime timeOfApplication, ILookupItem appliedItem);
    }

    internal class CodeCompletionEventHandler : AbstractEventGenerator, ICodeCompletionLifecycleHandler
    {
        private const Key EnterKey = Key.Enter;
        private const Key EscapeKey = Key.Escape;

        private CompletionEvent _event;
        private bool _beforeShownCalled;

        public CodeCompletionEventHandler(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _beforeShownCalled = false;
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
            FireCompletionEvent(CompletionEvent.TerminationState.Filtered, DateTime.Now);
        }

        public void OnCancellation(DateTime timeOfCancellation)
        {
            FireCompletionEvent(CompletionEvent.TerminationState.Cancelled, timeOfCancellation);
        }

        public void OnApplication(DateTime timeOfApplication, ILookupItem appliedItem)
        {
            FireCompletionEvent(CompletionEvent.TerminationState.Applied, timeOfApplication);
        }

        private void FireCompletionEvent(CompletionEvent.TerminationState state, DateTime finishedAt)
        {
            _event.TerminatedAt = finishedAt;
            _event.TerminatedBy = CompletionTerminator;
            _event.TerminatedAs = state;
            Asserts.That(_beforeShownCalled, "beforeShown not called");
            Fire(_event);
        }

        private static IDEEvent.Trigger CompletionTerminator
        {
            get
            {
                return EnterKey.IsPressed() || EscapeKey.IsPressed()
                    ? IDEEvent.Trigger.Typing
                    : IDEEvent.Trigger.Click;
            }
        }
    }
}