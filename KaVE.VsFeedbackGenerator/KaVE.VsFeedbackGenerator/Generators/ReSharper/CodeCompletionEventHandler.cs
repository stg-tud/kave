/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Application;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Analysis;
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

    [ShellComponent, Language(typeof (CSharpLanguage))]
    internal class CodeCompletionContextAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        private readonly CodeCompletionEventHandler _handler;
        private readonly ILogger _logger;

        public CodeCompletionContextAnalysisTrigger(CodeCompletionEventHandler handler, ILogger logger)
        {
            _handler = handler;
            _logger = logger;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            try
            {
                var ctx = ContextAnalysis.Analyze(context);
                _handler.SetContext(ctx);
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
            return false;
        }
    }

    [ShellComponent, Language(typeof (CSharpLanguage))]
    internal class CodeCompletionEventHandler : AbstractEventGenerator
    {
        private CompletionEvent _event;
        private bool _lookupItemsSet;
        private Context _context;

        public CodeCompletionEventHandler(IIDESession session, IMessageBus messageBus)
            : base(session, messageBus) {}

        public void SetContext(Context context)
        {
            _context = context;
        }

        public void HandleTriggered(string prefix)
        {
            _event = Create<CompletionEvent>();
            _event.Context = _context;
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
            _event.Context = _context;
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