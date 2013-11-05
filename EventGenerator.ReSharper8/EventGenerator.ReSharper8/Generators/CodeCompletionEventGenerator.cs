using System;
using System.Diagnostics;
using System.Linq;
using EnvDTE;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using KaVE.EventGenerator.ReSharper8.Utils;
using KaVE.EventGenerator.VisualStudio10.Generators;
using KaVE.MessageBus.MessageBus;
using KaVE.Model.Events.CompletionEvent;
using System.Collections.Generic;

namespace KaVE.EventGenerator.ReSharper8.Generators
{
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator : AbstractEventGenerator
    {
        private readonly ILookupWindowManager _lookupWindowManager;

        private ILookup _currentLookup;

        private CompletionEvent _currentEvent;

        /// <summary>
        /// Represents the token on which the completion is triggered
        /// </summary>
        private string _currentToken;

        public CodeCompletionEventGenerator(ILookupWindowManager lookupWindowManager, DTE dte, SMessageBus messageBus) : base(dte, messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _lookupWindowManager.BeforeLookupWindowShown += HandleLookupWindowShownEvent;
        }

        private void HandleLookupWindowShownEvent(Object sender, EventArgs e)
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            _currentLookup.BeforeShownItemsUpdated += HandleBeforeItemUpdateEvent;
            _currentLookup.Closed += HandleCompletionCloseEvent;
            _currentLookup.CurrentItemChanged += HandleCompletionChangedEvent;
            _currentLookup.ItemCompleted += HandleCompletionFireEvent;
            _currentLookup.Typing += HandleCompletionTypingEvent;

            _currentToken = _currentLookup.Window.Lookup.Prefix;

            CreateCurrentCompletionEvent();
        }

        private void CreateCurrentCompletionEvent()
        {
            _currentEvent = Create<CompletionEvent>();
            _currentEvent.ProposalCollection = _currentLookup.Items.ToProposalCollection();
            AddCurrentSelectionToEvent();
        }

        private void HandleCompletionCloseEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("Close");

            _currentEvent.TerminatedBy = CompletionEvent.TerminationAction.Cancel;
            //SimpleLog.LogCompletionEvent(currentEvent);

            // Cleanup
            _currentLookup.ItemCompleted -= HandleCompletionFireEvent;
            _currentLookup.CurrentItemChanged -= HandleCompletionChangedEvent;
            _currentLookup.Typing -= HandleCompletionTypingEvent;
            _currentLookup.BeforeShownItemsUpdated -= HandleBeforeItemUpdateEvent;
        }

        private void HandleBeforeItemUpdateEvent(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            Debug.WriteLine("Completion Updated");
            _currentEvent.ProposalCollection = items.Select(pair => pair.First).ToProposalCollection();
            _currentToken = _currentLookup.Window.Lookup.Prefix;
        }

        private void HandleCompletionTypingEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("Typing");
        }

        private void HandleCompletionChangedEvent(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("Completion Changed");
            AddCurrentSelectionToEvent();
        }

        private void HandleCompletionFireEvent(object sender, ILookupItem lookupItem, Suffix suffix,
            LookupItemInsertType lookupItemInsertType)
        {
            Debug.WriteLine("Fire");

            _currentEvent.TerminatedBy = CompletionEvent.TerminationAction.Apply;
            Fire(_currentEvent);
            _currentEvent = null;
        }

        private void AddCurrentSelectionToEvent()
        {
            var selection = _lookupWindowManager.CurrentLookup.Selection.Item.ToProposal();
            _currentEvent.AddSelection(new ProposalSelection(selection, _currentToken));
        }
    }
}
