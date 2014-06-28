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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.CodeCompletion.Infrastructure;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.TextControl;
using JetBrains.UI.Icons;
using JetBrains.UI.RichText;
using JetBrains.Util;
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

        private static readonly IEqualityComparer<double> EqComp =
            new global::JetBrains.EqualityComparer<double>((d1, d2) => Math.Abs(d1 - d2) < 0.01, d => (int) d);

        private readonly Dictionary<CSharpCodeCompletionContext, Context> _contexts =
            new Dictionary<CSharpCodeCompletionContext, Context>();

        private Network _proposalNetwork;
        private static readonly string[] Ranks = {"1st", "2nd", "3rd"};

        internal static Network CreateProposalNetwork(string[] methodNames)
        {
            var network = new Network();
            network.AddNode(Network.NodeType.Cpt, "Proposal");
            network.SetOutcomeId("Proposal", 0, methodNames[0]);
            network.SetOutcomeId("Proposal", 1, methodNames[1]);
            network.AddOutcome("Proposal", methodNames[2]);

            for (var i = 0; i < 3; i ++)
            {
                network.AddNode(Network.NodeType.Cpt, methodNames[i]);
                network.SetOutcomeId(methodNames[i], 0, "False");
                network.SetOutcomeId(methodNames[i], 1, "True");
                network.AddArc("Proposal", methodNames[i]);
            }

            network.SetNodeDefinition("Proposal", new[] {0.333, 0.334, 0.333});
            network.SetNodeDefinition(methodNames[0], new[] {0.7, 0.3, 0.1, 0.9, 0.2, 0.8});
            network.SetNodeDefinition(methodNames[1], new[] {0.8, 0.2, 0.5, 0.5, 0.1, 0.9});
            network.SetNodeDefinition(methodNames[2], new[] {0.8, 0.2, 0.9, 0.1, 1.0, 0.0});
            return network;
        }

        protected Network ProposalNetwork
        {
            get { return _proposalNetwork ?? (_proposalNetwork = CreateProposalNetwork(ExpectedMethods)); }
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
                return base.IsAvailable(context);
            }
        }

        protected override bool AddLookupItems(CSharpCodeCompletionContext context, GroupedItemsCollector collector)
        {
            var ctx = _contexts[context]; // cached to not call Analysis too often
            _contexts.Remove(context);
            var filtered =
                ctx.EntryPointToCalledMethods[ctx.EnclosingMethod].Where(m => m.DeclaringType.Name == ExpectedType)
                                                                  .ToList();
            var network = ProposalNetwork;
            for (var i = 0; i < 3; i ++)
            {
                var method = ExpectedMethods[i];
                var present = filtered.Any(m => m.Name.Equals(method));
                network.SetEvidence(method, present ? "True" : "False");
            }
            network.UpdateBeliefs();
            var values = network.GetNodeValue("Proposal");
            values.Sort((d1, d2) => (int) ((d2 - d1)*10000));
            for (var i = 0; i < 3; i ++)
            {
                var method = ExpectedMethods[i];
                var value = network.GetNodeValue("Proposal")[i];
                var rank = Ranks[values.IndexOf(value, EqComp)];
                collector.AddToTop(
                    new LookupItemWrapper(
                        context.LookupItemsFactory.CreateTextLookupItem(method + "()"),
                        rank,
                        " " + ((int) (value*10000))/100.0 + "%"));
            }
            network.ClearAllEvidence();
            return base.AddLookupItems(context, collector);
        }
    }

    internal class LookupItemWrapper : IWrappedLookupItem
    {
        private readonly ILookupItem _wrappedItem;
        private readonly string _prefix;
        private readonly RichText _postfix;

        public LookupItemWrapper(ILookupItem wrappedItem, string prefix, RichText postfix)
        {
            _wrappedItem = wrappedItem;
            _prefix = prefix;
            _postfix = postfix;
        }

        public bool AcceptIfOnlyMatched(LookupItemAcceptanceContext itemAcceptanceContext)
        {
            return _wrappedItem.AcceptIfOnlyMatched(itemAcceptanceContext);
        }

        public MatchingResult Match(string prefix, ITextControl textControl)
        {
            return _wrappedItem.Match(prefix, textControl);
        }

        public void Accept(ITextControl textControl,
            TextRange nameRange,
            LookupItemInsertType lookupItemInsertType,
            Suffix suffix,
            ISolution solution,
            bool keepCaretStill)
        {
            _wrappedItem.Accept(textControl, nameRange, lookupItemInsertType, suffix, solution, keepCaretStill);
        }

        public TextRange GetVisualReplaceRange(ITextControl textControl, TextRange nameRange)
        {
            return _wrappedItem.GetVisualReplaceRange(textControl, nameRange);
        }

        public bool Shrink()
        {
            return _wrappedItem.Shrink();
        }

        public void Unshrink()
        {
            _wrappedItem.Unshrink();
        }

        public IconId Image
        {
            get { return _wrappedItem.Image; }
        }

        public RichText DisplayName
        {
            get { return _wrappedItem.DisplayName + _postfix; }
        }

        public RichText DisplayTypeName
        {
            get { return _wrappedItem.DisplayTypeName; }
        }

        public bool CanShrink
        {
            get { return _wrappedItem.CanShrink; }
        }

        public string OrderingString
        {
            get { return _prefix + (_wrappedItem.OrderingString ?? DisplayName.ToString()); }
        }

        public int Multiplier
        {
            get { return _wrappedItem.Multiplier; }
            set { _wrappedItem.Multiplier = value; }
        }

        public bool IsDynamic
        {
            get { return _wrappedItem.IsDynamic; }
        }

        public bool IgnoreSoftOnSpace
        {
            get { return _wrappedItem.IgnoreSoftOnSpace; }
            set { _wrappedItem.IgnoreSoftOnSpace = value; }
        }

        public string Identity
        {
            get { return _wrappedItem.Identity; }
        }

        public ILookupItem Item
        {
            get { return _wrappedItem; }
        }
    }

    internal class MyButton
    {
        public void Init() {}
        public void Execute() {}
        public void Finish() {}
    }
}