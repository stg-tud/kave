using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils;
using KaVE.Utils.Assertion;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    internal class CodeCompletionEventHandler : AbstractEventGenerator
    {
        private const Key EnterKey = Key.Enter;
        private const Key EscapeKey = Key.Escape;

        private readonly CompletionEvent _event;
        private bool _isApplied;
        private bool _beforeShownCalled;

        public CodeCompletionEventHandler(string lookupPrefix, IIDESession session, IMessageBus messageBus)
            : base(session, messageBus)
        {
            _event = CreateEvent(lookupPrefix);
            _isApplied = false;
            _beforeShownCalled = false;
        }

        private CompletionEvent CreateEvent(string lookupPrefix)
        {
            var @event = Create<CompletionEvent>();
            @event.Prefix = lookupPrefix;
            return @event;
        }

        public void OnBeforeShownItemsUpdated(IEnumerable<ILookupItem> items)
        {
            // sometimes, the lookup is empty before this call
            // TODO test whether this is sometimes not called
            _beforeShownCalled = true;
            _event.ProposalCollection = items.ToProposalCollection();
        }

        /// <summary>
        /// This is invoked for the initial selection, any manual selection change (using the arrow keys), selection
        /// change caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion).
        /// </summary>
        public void OnSelectionChanged(ILookupItem selectedItem)
        {
            _event.AddSelection(selectedItem.ToProposal());
        }

        public void OnFiltering()
        {
            FireCurrentCompletionEvent(CompletionEvent.TerminationState.Filtered, DateTime.Now);
        }

        /// <summary>
        /// This is invoked when code completion is terminated either by appying a proposal or by cancelation. In the
        /// former case, it is invoked before <see cref="OnCompletionApplied"/>. However, unregistering from the lookup
        /// here does not prevent the call to <see cref="OnCompletionApplied"/>. Therefore, we do all the cleanup here
        /// and schedule <see cref="OnCompletionCancelled"/>, which will fire the event if
        /// <see cref="OnCompletionApplied"/> has not been invoked in the meanwhile.
        /// </summary>
        public void OnLookupClosed()
        {
            var terminatedAt = DateTime.Now;
            Invoke.Later(() => OnCompletionCancelled(terminatedAt), 10000);
        }

        private void OnCompletionCancelled(DateTime terminatedAt)
        {
            lock (_event)
            {
                if (!_isApplied)
                {
                    FireCurrentCompletionEvent(CompletionEvent.TerminationState.Cancelled, terminatedAt);
                }
            }
        }

        public void OnCompletionApplied()
        {
            lock (_event)
            {
                Asserts.Null(_event.TerminatedAt, "event was terminated earlier");
                _isApplied = true;
                FireCurrentCompletionEvent(CompletionEvent.TerminationState.Applied, DateTime.Now);
            }
        }

        private void FireCurrentCompletionEvent(CompletionEvent.TerminationState state, DateTime finishedAt)
        {
            _event.TerminatedAt = finishedAt;
            _event.TerminatedBy = CompletionTerminationTrigger;
            _event.TerminatedAs = state;
            Asserts.That(_beforeShownCalled, "beforeShown not called");
            Fire(_event);
        }

        private static IDEEvent.Trigger CompletionTerminationTrigger
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