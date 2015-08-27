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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Features.Intellisense.CodeCompletion.CSharp.Rules;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.RS.Commons.Analysis;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using KaVE.VS.FeedbackGenerator.MessageBus;

namespace KaVE.VS.FeedbackGenerator.Generators.ReSharper
{
    [SolutionComponent, Language(typeof(CSharpLanguage))]
    internal class CodeCompletionEventGeneratorRegistration
    {
        public CodeCompletionEventGeneratorRegistration(CodeCompletionLifecycleManager manager,
            CodeCompletionEventHandler handler)
        {
            manager.OnTriggered += handler.HandleTriggered;
            manager.DisplayedItemsUpdated += handler.HandleDisplayedItemsChanged;
            manager.OnSelectionChanged += handler.HandleSelectionChanged;
            manager.OnPrefixChanged += handler.HandlePrefixChanged;
            manager.OnClosed += handler.HandleClosed;
            manager.OnApplied += handler.HandleApplied;
            manager.OnCancelled += handler.HandleCancelled;
        }
    }

    [Language(typeof (CSharpLanguage))]
    public class CodeCompletionContextAnalysisTrigger : CSharpItemsProviderBase<CSharpCodeCompletionContext>
    {
        private readonly CodeCompletionEventHandler _handler;
        private readonly ILogger _logger;

        public CodeCompletionContextAnalysisTrigger(CodeCompletionEventHandler handler, ILogger logger)
        {
            _handler = handler;
            _logger = logger;
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            return true;
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            ContextAnalysis.AnalyseAsync(
                context.NodeInFile,
                _logger,
                OnSuccess,
                delegate { },
                delegate { });
            return false;
        }

        private void OnSuccess(Context context)
        {
            _handler.SetContext(context);
        }
    }

    [ShellComponent]
    public class CodeCompletionEventHandler : EventGeneratorBase
    {
        /// <summary>
        /// Transforming a large amount of lookup items to proposals takes a considerable amount of time.
        /// In some cases, autocompletion will propose the entire class library, thus freezing up the
        /// UI for several seconds.
        /// </summary>
        private static readonly int ProposalTransformationLimit = 250;

        private CompletionEvent _event;
        private Context _context;
        private ILookupItem[] _lastDisplayedItems; 

        public CodeCompletionEventHandler(IRSEnv env, IMessageBus messageBus, IDateUtils dateUtils)
            : base(env, messageBus, dateUtils)
        {
            _context = new Context();
        }

        public void SetContext(Context context)
        {
            _context = context;
        }

        public void HandleTriggered(string prefix, IEnumerable<ILookupItem> displayedItems)
        {
            _event = Create<CompletionEvent>();
            _event.Context2 = _context;
            HandleDisplayedItemsChanged(displayedItems);
        }

        public void HandleDisplayedItemsChanged(IEnumerable<ILookupItem> displayedItems)
        {
            _lastDisplayedItems = displayedItems.Take(ProposalTransformationLimit).ToArray();
        }

        public void HandleSelectionChanged(ILookupItem selectedItem)
        {
            _event.AddSelection(selectedItem.ToProposal());
        }

        public void HandlePrefixChanged(string newPrefix, IEnumerable<ILookupItem> displayedLookupItems)
        {
            _event.TerminatedState = TerminationState.Filtered;
            _event.TerminatedAt = DateTime.Now;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;
            var lastSelection = _event.Selections.LastOrDefault();
            Fire(_event);

            _event = Create<CompletionEvent>();
            _event.Context2 = _context;
            HandleDisplayedItemsChanged(displayedLookupItems);
            if (lastSelection != null && _lastDisplayedItems.Any(l => l != null && l.ToProposal().Equals(lastSelection.Proposal)))
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
            _event.TerminatedState = TerminationState.Applied;
            _event.TerminatedBy = trigger;
            Fire(_event);
        }

        public void HandleCancelled(IDEEvent.Trigger trigger)
        {
            _event.TerminatedState = TerminationState.Cancelled;
            _event.TerminatedBy = trigger;
            Fire(_event);
        }

        protected void Fire(CompletionEvent @event)
        {
            @event.ProposalCollection = _lastDisplayedItems.ToProposalCollection();
            base.Fire(@event);
        }

        public void FireInfo(string info)
        {
            var infoEvent = Create<InfoEvent>();
            infoEvent.Info = info;

            base.Fire(infoEvent);
        }
    }
}