using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    [Language(typeof (CSharpLanguage))]
    internal class CodeCompletionEventGeneratorRegistration
    {
        public CodeCompletionEventGeneratorRegistration(CodeCompletionLifecycleManager manager,
            CodeCompletionEventHandler handler)
        {
            manager.OnTriggered += handler.HandleTriggered;
            manager.OnOpened += handler.HandleOpened;
            manager.OnSelectionChanged += handler.HandleSelectionChanged;
            manager.OnPrefixChanged += handler.HandlePrefixChanged;
            manager.OnClosed += handler.HandleClosed;
            manager.OnApplied += handler.HandleApplied;
            manager.OnCancelled += handler.HandleCancelled;
        }
    }

    [Language(typeof (CSharpLanguage))]
    internal class CodeCompletionEventHandler : AbstractEventGenerator
    {
        private CompletionEvent _event;
        private bool _lookupItemsSet;

        public CodeCompletionEventHandler(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus) {}

        public void HandleTriggered(string prefix)
        {
            _event = Create<CompletionEvent>();
            _event.Prefix = prefix;
            _lookupItemsSet = false;
        }

        public void HandleOpened(IEnumerable<ILookupItem> items)
        {
            // TODO test whether this is sometimes not called
            _lookupItemsSet = true;
            _event.ProposalCollection = items.ToProposalCollection();
        }

        public void HandleSelectionChanged(ILookupItem selectedItem)
        {
            _event.AddSelection(selectedItem.ToProposal());
        }

        public void HandlePrefixChanged(string newPrefix, IEnumerable<ILookupItem> displayedLookupItems)
        {
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedAt = DateTime.Now;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;
            var lastSelection = _event.Selections.LastOrDefault();
            Fire(_event);

            _event = Create<CompletionEvent>();
            _event.Prefix = newPrefix;
            _event.ProposalCollection = displayedLookupItems.ToProposalCollection();
            if (lastSelection != null && _event.ProposalCollection.Proposals.Contains(lastSelection.Proposal))
            {
                _event.Selections.Add(lastSelection);
            }
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
        }

        public void HandleClosed()
        {
            _event.TerminatedAt = DateTime.Now;
        }

        public void HandleApplied(IDEEvent.Trigger trigger, ILookupItem appliedItem)
        {
            Asserts.That(_lookupItemsSet, "beforeShown not called");
            _event.TerminatedAs = CompletionEvent.TerminationState.Applied;
            _event.TerminatedBy = trigger;
            Fire(_event);
        }

        public void HandleCancelled(IDEEvent.Trigger trigger)
        {
            Asserts.That(_lookupItemsSet, "beforeShown not called");
            _event.TerminatedAs = CompletionEvent.TerminationState.Cancelled;
            _event.TerminatedBy = trigger;
            Fire(_event);
        }
    }
}