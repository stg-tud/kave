using System;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using KaVE.EventGenerator.ReSharper8.Utils;
using KaVE.EventGenerator.ReSharper8.VsIntegration;
using KaVE.EventGenerator.VisualStudio10.Generators;
using KaVE.MessageBus.MessageBus;
using KaVE.Model.Events.CompletionEvent;
using System.Collections.Generic;
using KaVE.Utils.Assertion;

namespace KaVE.EventGenerator.ReSharper8.Generators
{
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator : AbstractEventGenerator
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private ILookup _currentLookup;
        private CompletionEvent _currentEvent;

        public CodeCompletionEventGenerator(ILookupWindowManager lookupWindowManager, IVsDTE dte, SMessageBus messageBus) : base(dte.DTE, messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
        }

        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            RegisterToCurrentLookup();
            InitCurrentCompletionEvent();
        }

        private void RegisterToCurrentLookup()
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            _currentLookup.BeforeShownItemsUpdated += OnBeforeLookupItemsShown;
            _currentLookup.Closed += OnLookupClosed;
            _currentLookup.CurrentItemChanged += OnCurrentLookupSelectionChanged;
            _currentLookup.ItemCompleted += OnCompletionApplied;
            _currentLookup.Typing += HandleTypingEvent;
        }

        private void InitCurrentCompletionEvent()
        {
            Asserts.Null(_currentEvent, "multiple events at a time");
            _currentEvent = Create<CompletionEvent>();
            _currentEvent.ProposalCollection = _currentLookup.Items.ToProposalCollection();
            AddCurrentSelectionToEvent();
        }

        private void OnBeforeLookupItemsShown(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            // TODO is this required? the method is not always invoked...
            _currentEvent.ProposalCollection = items.Select(p => p.First).ToProposalCollection();
        }

        private void HandleTypingEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("Typing");
        }

        private void OnCurrentLookupSelectionChanged(object sender, EventArgs eventArgs)
        {
            AddCurrentSelectionToEvent();
        }

        private void AddCurrentSelectionToEvent()
        {
            var selection = _lookupWindowManager.CurrentLookup.Selection.Item.ToProposal();
            var typedToken = _currentLookup.Window.Lookup.Prefix;
            _currentEvent.AddSelection(new ProposalSelection(selection, typedToken));
        }

        private void OnLookupClosed(object sender, EventArgs e)
        {
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
            Fire(_currentEvent);
            _currentEvent = null;
        }

        private void UnregisterFromCurrentLookup()
        {
            _currentLookup.BeforeShownItemsUpdated -= OnBeforeLookupItemsShown;
            _currentLookup.Closed -= OnLookupClosed;
            _currentLookup.CurrentItemChanged -= OnCurrentLookupSelectionChanged;
            _currentLookup.ItemCompleted -= OnCompletionApplied;
            _currentLookup.Typing -= HandleTypingEvent;
            _currentLookup = null;
        }
    }
}
