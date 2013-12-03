using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.Util;
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

        private readonly ILookup _lookup;
        private readonly IVsDTE _dte;
        private readonly IMessageBus _messageBus;

        private readonly CompletionEvent _event;
        private bool _isApplied;
        private bool _beforeShownCalled;

        public CodeCompletionEventHandler(ILookup lookup, IVsDTE dte, IMessageBus messageBus)
            : base(dte, messageBus)
        {
            _lookup = lookup;
            _dte = dte;
            _messageBus = messageBus;
            _event = CreateEvent(lookup);
            _isApplied = false;
            _beforeShownCalled = false;
            RegisterToLookupEvents();
        }

        private CompletionEvent CreateEvent(ILookup lookup)
        {
            var @event = Create<CompletionEvent>();
            @event.ProposalCollection = lookup.Items.ToProposalCollection();
            @event.Prefix = _lookup.Prefix;
            return @event;
        }

        private void RegisterToLookupEvents()
        {
            // registrations are done in the order the events occur in
            _lookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;
            _lookup.CurrentItemChanged += OnSelectionChanged;
            _lookup.Typing += OnFiltering;
            _lookup.Closed += OnLookupClosed;
            _lookup.ItemCompleted += OnCompletionApplied;
        }

        private void OnBeforeShownItemsUpdated(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            // sometimes, the lookup is empty before this call
            // TODO test whether this is sometimes not called
            _beforeShownCalled = true;
            _event.ProposalCollection = items.Select(p => p.First).ToProposalCollection();
        }

        /// <summary>
        /// This is invoked for the initial selection, any manual selection change (using the arrow keys), selection
        /// change caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion).
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            _event.AddSelection(_lookup.Selection.Item.ToProposal());
        }

        private void OnFiltering(object sender, EventArgs<char> eventArgs)
        {
            FireCurrentCompletionEvent(CompletionEvent.TerminationState.Filtered, DateTime.Now);
            new CodeCompletionEventHandler(_lookup, _dte, _messageBus);
            UnregisterFromCurrentLookup();
        }

        /// <summary>
        /// This is invoked when code completion is terminated either by appying a proposal or by cancelation. In the
        /// former case, it is invoked before <see cref="OnCompletionApplied"/>. However, unregistering from the lookup
        /// here does not prevent the call to <see cref="OnCompletionApplied"/>. Therefore, we do all the cleanup here
        /// and schedule <see cref="OnCompletionCancelled"/>, which will fire the event if
        /// <see cref="OnCompletionApplied"/> has not been invoked in the meanwhile.
        /// </summary>
        private void OnLookupClosed(object sender, EventArgs e)
        {
            var terminatedAt = DateTime.Now;
            Invoker.Later(() => OnCompletionCancelled(terminatedAt), 10000);
            UnregisterFromCurrentLookup();
        }

        private void OnCompletionCancelled(DateTime terminatedAt)
        {
            lock (_event)
            {
                if (!_isApplied)
                {
                    _isCancelled = true;
                    FireCurrentCompletionEvent(CompletionEvent.TerminationState.Cancelled, terminatedAt);
                }
            }
        }

        private void UnregisterFromCurrentLookup()
        {
            _lookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            _lookup.Closed -= OnLookupClosed;
            _lookup.CurrentItemChanged -= OnSelectionChanged;
            _lookup.ItemCompleted -= OnCompletionApplied;
        }

        private void OnCompletionApplied(object sender,
            ILookupItem lookupItem,
            Suffix suffix,
            LookupItemInsertType lookupItemInsertType)
        {
            lock (_event)
            {
                Asserts.Null(_event.FinishedAt, "event was terminated earlier");
                _isApplied = true;
                FireCurrentCompletionEvent(CompletionEvent.TerminationState.Applied, DateTime.Now);
            }
        }

        private void FireCurrentCompletionEvent(CompletionEvent.TerminationState state, DateTime finishedAt)
        {
            _event.FinishedAt = finishedAt;
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