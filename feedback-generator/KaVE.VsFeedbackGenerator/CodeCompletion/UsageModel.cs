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
 *    - Uli Fahrer
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.ObjectUsage;
using KaVE.Utils;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    public interface IUsageModel
    {
        CoReProposal[] Query([NotNull] Query query);
    }

    public class UsageModel : IUsageModel
    {
        private readonly Network _network;

        private int _classContextHandle;
        private int _methodContextHandle;
        private int _definitionHandle;

        private readonly Dictionary<CallSite, int> _callNodes = new Dictionary<CallSite, int>();
        private readonly Dictionary<CallSite, int> _parameterNodes = new Dictionary<CallSite, int>();

        private readonly ISet<CallSite> _queriedMethods = new HashSet<CallSite>();

        public UsageModel(Network network)
        {
            _network = network;
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            _network.GetAllNodes().ForEach(AssignToClassMember);
        }

        private void AssignToClassMember(int nodeHandle)
        {
            var nodeId = _network.GetNodeId(nodeHandle);

            if (nodeId.Equals(ModelConstants.ClassContextTitle))
            {
                _classContextHandle = nodeHandle;
            }
            else if (nodeId.Equals(ModelConstants.MethodContextTitle))
            {
                _methodContextHandle = nodeHandle;
            }
            else if (nodeId.Equals(ModelConstants.DefinitionTitle))
            {
                _definitionHandle = nodeHandle;
            }
            else if (nodeId.StartsWith(ModelConstants.CallPrefix))
            {
                var nodeName = _network.GetNodeName(nodeHandle);
                var site = new CallSite
                {
                    kind = CallSiteKind.RECEIVER,
                    method = new CoReMethodName(nodeName)
                };

                _callNodes.Add(site, nodeHandle);
            }
            else if (nodeId.StartsWith(ModelConstants.ParameterPrefix))
            {
                var nodeName = _network.GetNodeName(nodeHandle);
                var site = new CallSite
                {
                    kind = CallSiteKind.PARAMETER,
                    method = new CoReMethodName(nodeName)
                };

                _parameterNodes.Add(site, nodeHandle);
            }
        }

        public CoReProposal[] Query(Query query)
        {
            _network.ClearAllEvidence();

            AddEvidenceIfAvailable(_classContextHandle, ModelConstants.NewClassContext(query.classCtx));
            AddEvidenceIfAvailable(_methodContextHandle, ModelConstants.NewMethodContext(query.methodCtx));
            AddEvidenceIfAvailable(_definitionHandle, ModelConstants.NewDefinition(query.definition));

            query.sites.ForEach(AddCallSiteEvidenceIfAvailable);

            _network.UpdateBeliefs();

            return CollectProposals();
        }

        private void AddEvidenceIfAvailable(int nodeHandle, String outcome)
        {
            var outcomeIds = _network.GetOutcomeIds(nodeHandle);
            var legalSmileState = ConvertToLegalSmileName(outcome);

            if (outcomeIds.Contains(legalSmileState))
            {
                _network.SetEvidence(nodeHandle, legalSmileState);
            }
        }

        private void AddCallSiteEvidenceIfAvailable(CallSite site)
        {
            switch (site.kind)
            {
                case CallSiteKind.PARAMETER:
                    AddCallSiteEvidenceIfAvailable(site, _parameterNodes);
                    break;
                case CallSiteKind.RECEIVER:
                    AddCallSiteEvidenceIfAvailable(site, _callNodes);
                    _queriedMethods.Add(site);
                    break;
            }
        }

        private void AddCallSiteEvidenceIfAvailable(CallSite callSite, Dictionary<CallSite, int> sitesToHandle)
        {
            if (sitesToHandle.ContainsKey(callSite))
            {
                _network.SetEvidence(sitesToHandle[callSite], ModelConstants.StateTrue);
            }
        }

        private CoReProposal[] CollectProposals()
        {
            var unqueriedCallSites = _callNodes.Keys.Except(_queriedMethods);
            var proposals =
                unqueriedCallSites.Select(cs => new CoReProposal(cs.method, GetProbability(cs)));

            var sortedProposals = new SortedProbabilitySet();
            sortedProposals.AddRange(proposals);

            return sortedProposals.ToArray();
        }

        private double GetProbability(CallSite site)
        {
            var nodeName = ModelConstants.NewReceiverCallSite(site);
            var nodeId = ConvertToLegalSmileName(nodeName);

            return _network.GetNodeValue(nodeId)[0];
        }

        public static string ConvertToLegalSmileName(string name)
        {
            if (Regex.IsMatch(name, "^[0-9]"))
            {
                name = "x" + name;
            }

            var pattern = new Regex("[^A-Za-z0-9]");
            return pattern.Replace(name, "_");
        }

        internal class SortedProbabilitySet : SortedSet<CoReProposal>
        {
            private const double Epsilon = 0.01;

            public SortedProbabilitySet() : base(PropabilityComparer()) {}

            private static global::JetBrains.Comparer<CoReProposal> PropabilityComparer()
            {
                return new global::JetBrains.Comparer<CoReProposal>(
                    (p1, p2) =>
                        (Math.Abs(p1.Probability - p2.Probability) < Epsilon)
                            ? 0
                            : ((p1.Probability > p2.Probability) ? -1 : 1));
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(UsageModel oth)
        {
            return Equals(_network, oth._network);
        }

        public override int GetHashCode()
        {
            return (_network == null) ? 0 : _network.GetHashCode();
        }
    }
}