using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator.Generators.ReSharper
{
    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionEventGenerator
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly IIDESession _session;
        private readonly IMessageBus _messageBus;

        private CodeCompletionEventHandler _handler;

        public CodeCompletionEventGenerator(ILookupWindowManager lookupWindowManager, IIDESession session, IMessageBus messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _session = session;
            _messageBus = messageBus;
            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
            // Notes:
            // - AfterLookupWindowShown is fired immediately after the window pops up
            // - LookupWindowClosed and CurrentLookup.Closed are fired before CurrentLookup.ItemCompleted
        }

        /// <summary>
        /// Invoked when a completion starts.
        /// </summary>
        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            _handler = CreateHandler();
            var lookup = _lookupWindowManager.CurrentLookup;
            lookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;
            lookup.CurrentItemChanged += OnCurrentItemChanged;
            lookup.Typing += OnTyping;
            lookup.Closed += OnClosed;
            lookup.ItemCompleted += OnItemCompleted;
        }

        /// <summary>
        /// Invoked when the lookup items are computed, before they are shown.
        /// </summary>
        private void OnBeforeShownItemsUpdated(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            _handler.OnBeforeShownItemsUpdated(items.Select(pair => pair.First));
        }

        /// <summary>
        /// Invoked when the selection changes, either by pressing the arrow keys, or as a result of filtering, or
        /// because the user clicked an item (in which case the completion is closed and completed immediately
        /// after the selection change).
        /// </summary>
        private void OnCurrentItemChanged(object sender, EventArgs eventArgs)
        {
            _handler.OnSelectionChanged(_lookupWindowManager.CurrentLookup.Selection.Item);
        }

        /// <summary>
        /// Invoked when the typed prefix changes, either because another character is typed or because a character
        /// is deleted.
        /// </summary>
        private void OnTyping(object sender, EventArgs<char> eventArgs)
        {
            // TODO test wheather this is actually called on delete!
            _handler.OnFiltering();
            _handler = CreateHandler();
        }

        private CodeCompletionEventHandler CreateHandler()
        {
            return new CodeCompletionEventHandler(_lookupWindowManager.CurrentLookup.Prefix, _session, _messageBus);
        }

        /// <summary>
        /// Invoked when the completion is closed, regardless of whether an item was applied or the completion was
        /// cancelled. In the former case, invocation occurs before that of <see cref="OnItemCompleted"/>.
        /// </summary>
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            _handler.OnLookupClosed();
            var lookup = _lookupWindowManager.CurrentLookup;
            lookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            lookup.CurrentItemChanged -= OnCurrentItemChanged;
            lookup.Typing -= OnTyping;
            lookup.Closed -= OnClosed;
            lookup.ItemCompleted -= OnItemCompleted;
        }

        /// <summary>
        /// Invoked when the completion is terminated by application of an item. Invocation occurs after that of
        /// <see cref="OnClosed"/>.
        /// </summary>
        private void OnItemCompleted(object sender, ILookupItem lookupItem, Suffix suffix, LookupItemInsertType lookupItemInsertType)
        {
            _handler.OnCompletionApplied();
        }
    }
}
