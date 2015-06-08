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
 * 
 * Contributors:
 *    - Dennis Albrecht
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.CodeCompletion;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.ObjectUsageExport;
using KaVE.VsFeedbackGenerator.Analysis;
using KaVE.VsFeedbackGenerator.Utils;

//using KaVELogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [Language(typeof (CSharpLanguage))]
    public class PBNProposalItemsProvider : CSharpItemsProviderBasic
    {
        private readonly ILogger _logger;
        private readonly IPBNRecommenderStore _store;
        private readonly QueryExtractor _queryGen;
        private readonly Dictionary<CoReTypeName, IPBNRecommender> _models;

        private Query _currentQuery;

        public PBNProposalItemsProvider(IPBNRecommenderStore store, ILogger logger)
        {
            _models = new Dictionary<CoReTypeName, IPBNRecommender>();
            _store = store;
            _queryGen = new QueryExtractor();
            _logger = logger;
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext csContext)
        {
            return base.IsAvailable(csContext);
        }

        private bool IsAvailable()
        {
            // written this way to prevent unnecessary file read
            return _models.ContainsKey(_currentQuery.type) || _store.IsAvailable(_currentQuery.type);
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            Context kaveContext = ContextAnalysis.Analyze(context, _logger).Context;
            _currentQuery = _queryGen.Extract(kaveContext);
            if (_currentQuery != null && IsAvailable())
            {
                var rec = LoadIfAvailable();
                if (rec != null)
                {
                    try
                    {
                        var proposals = rec.Query(_currentQuery);

                        WrapExistingItems(collector, proposals);
                        WrapNewItems(collector, proposals);
                    }
                    catch (AssertException e)
                    {
                        _logger.Error(e);
                    }
                }
                else
                {
                    _logger.Info("no recommender model found for {0}", _currentQuery.type);
                }
            }

            return base.AddLookupItems(context, collector);
        }

        private IPBNRecommender LoadIfAvailable()
        {
            if (_currentQuery == null)
            {
                return null;
            }

            var type = _currentQuery.type;
            if (_models.ContainsKey(type))
            {
                return _models[type];
            }
            return _store.Load(type);
        }

        private static void WrapExistingItems(GroupedItemsCollector collector, CoReProposal[] proposals)
        {
            // ToList is necessary to avoid ConcurrentModificationExceptions
            foreach (var candidate in collector.Items.ToList())
            {
                ConditionallyAddWrappedLookupItem(collector, proposals, candidate);
            }
        }

        private static void WrapNewItems(GroupedItemsCollector collector, IEnumerable<CoReProposal> proposals)
        {
            collector.ItemAdded +=
                (candidate => ConditionallyAddWrappedLookupItem(collector, proposals, candidate));
        }

        private static void ConditionallyAddWrappedLookupItem(GroupedItemsCollector collector,
            IEnumerable<CoReProposal> proposals,
            ILookupItem candidate)
        {
            if (candidate is PBNProposalWrappedLookupItem)
            {
                return;
            }
            var representation = candidate.ToProposal().ToCoReName();
            if (representation != null)
            {
                var matchingProposal = proposals.FirstOrDefault(p => p.Name.Equals(representation));
                if (matchingProposal != null)
                {
                    collector.AddToTop(new PBNProposalWrappedLookupItem(candidate, matchingProposal.Probability));
                }
            }
        }
    }
}