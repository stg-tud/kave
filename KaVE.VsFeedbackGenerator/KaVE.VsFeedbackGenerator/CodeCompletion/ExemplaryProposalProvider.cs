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
 *    - 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.VsFeedbackGenerator.Analysis;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [Language(typeof (CSharpLanguage))]
    public class ExemplaryProposalProvider : CSharpItemsProviderBasic
    {
        private const string ExpectedType = "MyButton";
        private static readonly string[] ExpectedMethods = {"Init", "Execute", "Finish"};

        private readonly Dictionary<CSharpCodeCompletionContext, Context> _contexts =
            new Dictionary<CSharpCodeCompletionContext, Context>();

        private Network proposalNetwork;

        protected Network ProposalNetwork
        {
            get
            {
                if (proposalNetwork == null)
                {
                    proposalNetwork = new Network();
                    proposalNetwork.AddNode(Network.NodeType.Cpt, "Proposal");
                    proposalNetwork.SetOutcomeId("Proposal", 0, ExpectedMethods[0]);
                    proposalNetwork.SetOutcomeId("Proposal", 1, ExpectedMethods[1]);
                    proposalNetwork.AddOutcome("Proposal", ExpectedMethods[2]);

                    proposalNetwork.AddNode(Network.NodeType.Cpt, ExpectedMethods[0]);
                    proposalNetwork.SetOutcomeId(ExpectedMethods[0], 0, "False");
                    proposalNetwork.SetOutcomeId(ExpectedMethods[0], 1, "True");

                    proposalNetwork.AddNode(Network.NodeType.Cpt, ExpectedMethods[1]);
                    proposalNetwork.SetOutcomeId(ExpectedMethods[1], 0, "False");
                    proposalNetwork.SetOutcomeId(ExpectedMethods[1], 1, "True");

                    proposalNetwork.AddNode(Network.NodeType.Cpt, ExpectedMethods[2]);
                    proposalNetwork.SetOutcomeId(ExpectedMethods[2], 0, "False");
                    proposalNetwork.SetOutcomeId(ExpectedMethods[2], 1, "True");

                    proposalNetwork.AddArc("Proposal", ExpectedMethods[0]);
                    proposalNetwork.AddArc("Proposal", ExpectedMethods[1]);
                    proposalNetwork.AddArc("Proposal", ExpectedMethods[2]);

                    proposalNetwork.SetNodeDefinition("Proposal", new[] {0.333, 0.334, 0.333});
                    proposalNetwork.SetNodeDefinition(ExpectedMethods[0], new[] {0.7, 0.3, 0.1, 0.9, 0.2, 0.8});
                    proposalNetwork.SetNodeDefinition(ExpectedMethods[1], new[] {0.8, 0.2, 0.5, 0.5, 0.1, 0.9});
                    proposalNetwork.SetNodeDefinition(ExpectedMethods[2], new[] {0.8, 0.2, 0.9, 0.1, 1.0, 0.0});
                }
                return proposalNetwork;
            }
        }

        protected override bool IsAvailable(CSharpCodeCompletionContext context)
        {
            try
            {
                var ctx = ContextAnalysis.Analyze(context);
                _contexts.Add(context, ctx);
                var typeName = ctx.TriggerTarget as ITypeName;
                return typeName != null && typeName.Name == ExpectedType;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            var ctx = _contexts[context]; // cached to not call Analysis too often
            _contexts.Remove(context);
            var filtered =
                ctx.EntryPointToCalledMethods[ctx.EnclosingMethod].Where(m => m.DeclaringType.Name == ExpectedType).ToList();
            var network = ProposalNetwork;
            for (var i = 0; i < 3; i ++)
            {
                var method = ExpectedMethods[i];
                var present = filtered.Any(m => m.Name.Equals(method));
                network.SetEvidence(method, present ? "True" : "False");
            }
            network.UpdateBeliefs();
            for (var i = 0; i < 3; i ++)
            {
                var value = network.GetNodeValue("Proposal")[i];
                collector.AddToTop(context.LookupItemsFactory.CreateTextLookupItem(value + " " + ExpectedMethods[i]));
            }
            network.ClearAllEvidence();
            return base.AddLookupItems(context, collector);
        }
    }
}