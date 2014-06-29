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
 */

using System;
using System.Globalization;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Analysis;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [Language(typeof (CSharpLanguage))]
    public class ProposalProvider : CSharpItemsProviderBasic
    {
        private readonly IProposalNetworkProvider _networkProvider;

        public ProposalProvider(IProposalNetworkProvider networkProvider)
        {
            _networkProvider = networkProvider;
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            try
            {
                var ctx = ContextAnalysis.Analyze(context);
                var typeName = ctx.TriggerTarget as ITypeName;
                return typeName != null && _networkProvider.CanProvide(typeName);
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            Context ctx;
            try
            {
                ctx = ContextAnalysis.Analyze(context);
            }
            catch (Exception)
            {
                return base.AddLookupItems(context, collector);
            }
            var type = ctx.TriggerTarget as ITypeName;
            if (type == null)
            {
                return base.AddLookupItems(context, collector);
            }
            var network = _networkProvider.Provide(type);
            var calledMethods = ctx.EntryPointToCalledMethods[ctx.EnclosingMethod];
            var proposals = network.GenerateProposals(calledMethods).ToList();
            proposals.Sort((p1, p2) => (int) (p2.Probability - p1.Probability)*100000);
            var length = proposals.Count.ToString(CultureInfo.InvariantCulture).Length;
            for (var i = 0; i < proposals.Count; i ++)
            {
                var proposal = proposals[i];
                collector.AddToTop(
                    new ProposalLookupItem(
                        context.LookupItemsFactory.CreateTextLookupItem(proposal.Name + "()"),
                        PrependToLength(i.ToString(CultureInfo.InvariantCulture), length, '0'),
                        Math.Round(proposal.Probability * 100, 2) + " %"));
            }
            return base.AddLookupItems(context, collector);
        }

        private static string PrependToLength(string text, int length, char prepend)
        {
            var acc = text;
            while (acc.Length < length)
            {
                acc = prepend + acc;
            }
            return acc;
        }
    }
}