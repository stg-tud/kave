using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
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
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator : AbstractEventGenerator
    {
        private static readonly Key EnterKey = KeyUtils.ResolveKey("Enter");

        private readonly ILookupWindowManager _lookupWindowManager;
        private ILookup _currentLookup;
        private CompletionEvent _currentEvent;

        public CodeCompletionEventGenerator(ILookupWindowManager lookupWindowManager, IVsDTE dte, IMessageBus messageBus) : base(dte, messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
            // Notes:
            // - AfterLookupWindowShown is fired immediately after the lookup pops up
            // - LookupWindowClosed is fired before CurrentLookup.ItemCompleted, like CurrentLookup.Closed
        }

        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            RegisterToCurrentLookup();
            InitCurrentCompletionEvent();
        }

        private void RegisterToCurrentLookup()
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            // registrations are done in the order the events occur in
            _currentLookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged += OnSelectionChanged;
            _currentLookup.Closed += OnLookupClosed;
            _currentLookup.ItemCompleted += OnCompletionApplied;
        }

        private void InitCurrentCompletionEvent()
        {
            Asserts.Null(_currentEvent, "multiple events at a time");
            _currentEvent = Create<CompletionEvent>();
            _currentEvent.ProposalCollection = _currentLookup.Items.ToProposalCollection();
            _currentEvent.Prefix = _currentLookup.Prefix;
        }

        private void OnBeforeShownItemsUpdated(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            // TODO this tries to capture the case where this assignment makes a difference. If it doesn't, it should be removed...
            var oldCollection = _currentEvent.ProposalCollection;
            _currentEvent.ProposalCollection = items.Select(p => p.First).ToProposalCollection();
            Asserts.That(oldCollection.Equals(_currentEvent.ProposalCollection), "information changed");
        }

        /// <summary>
        /// This is invoked for the initial selection, any manual selection change (using the arrow keys), selection
        /// change caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion).
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs eventArgs)
        {
            _currentEvent.AddSelection(_lookupWindowManager.CurrentLookup.Selection.Item.ToProposal());
        }

        private void OnLookupClosed(object sender, EventArgs e)
        {
            // TODO this is always called, even if OnCompletionApplied is called afterwards!
            FireCurrentCompletionEvent(CompletionEvent.TerminationAction.Cancel);
            UnregisterFromCurrentLookup();
        }

        private void OnCompletionApplied(object sender, ILookupItem lookupItem, Suffix suffix,
            LookupItemInsertType lookupItemInsertType)
        {
            FireCurrentCompletionEvent(CompletionEvent.TerminationAction.Apply);
            UnregisterFromCurrentLookup();
        }

        private void FireCurrentCompletionEvent(CompletionEvent.TerminationAction action)
        {
            _currentEvent.TerminatedBy = action;
            _currentEvent.TriggeredBy = CompletionTerminationTrigger;
            Fire(_currentEvent);
            _currentEvent = null;
        }

        private static IDEEvent.Trigger CompletionTerminationTrigger
        {
            get
            {
                return EnterKey.IsPressed()
                    ? IDEEvent.Trigger.Typing
                    : IDEEvent.Trigger.Click;
            }
        }

        private void UnregisterFromCurrentLookup()
        {
            _currentLookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            _currentLookup.Closed -= OnLookupClosed;
            _currentLookup.CurrentItemChanged -= OnSelectionChanged;
            _currentLookup.ItemCompleted -= OnCompletionApplied;
            _currentLookup = null;
        }
    }
}
