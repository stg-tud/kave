using System;
using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
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
            // TODO test whether this is sometimes not called
            _beforeShownCalled = true;
            _event.ProposalCollection = items.ToProposalCollection();
        }

        public void OnSelectionChanged(ILookupItem selectedItem)
        {
            _event.AddSelection(selectedItem.ToProposal());
        }

        public void OnPrefixChanged(string newPrefix, IEnumerable<ILookupItem> displayedLookupItems)
        {
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedAt = DateTime.Now;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;
            Fire(_event);

            _event = Create<CompletionEvent>();
            _event.Prefix = newPrefix;
            _event.ProposalCollection = displayedLookupItems.ToProposalCollection();
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
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