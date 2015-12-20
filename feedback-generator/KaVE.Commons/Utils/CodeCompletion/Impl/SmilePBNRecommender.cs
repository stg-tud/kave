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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class SmilePBNRecommender : IPBNRecommender
    {
        private const double MinimalProbability = 0.1;

        [NotNull]
        private readonly CoReTypeName _type;

        [NotNull]
        private readonly Network _network;

        private int _classContextHandle;
        private int _methodContextHandle;
        private int _definitionHandle;

        private readonly Dictionary<CallSite, int> _callNodes = new Dictionary<CallSite, int>();
        private readonly Dictionary<CallSite, int> _parameterNodes = new Dictionary<CallSite, int>();

        private readonly ISet<CallSite> _queriedMethods = new HashSet<CallSite>();

        public SmilePBNRecommender([NotNull] CoReTypeName type, [NotNull] Network network)
        {
            _type = type;
            _network = network;
            InitializeNodes();
        }

        public CoReTypeName GetTypeOfRecommender()
        {
            return _type;
        }

        private void InitializeNodes()
        {
            foreach (var nodeHandle in _network.GetAllNodes())
            {
                AssignToClassMember(nodeHandle);
            }
        }

        private void AssignToClassMember(int nodeHandle)
        {
            var nodeId = _network.GetNodeId(nodeHandle);

            if (nodeId.Equals(SmilePBNRecommenderConstants.ClassContextTitle))
            {
                _classContextHandle = nodeHandle;
            }
            else if (nodeId.Equals(SmilePBNRecommenderConstants.MethodContextTitle))
            {
                _methodContextHandle = nodeHandle;
            }
            else if (nodeId.Equals(SmilePBNRecommenderConstants.DefinitionTitle))
            {
                _definitionHandle = nodeHandle;
            }
            else if (nodeId.StartsWith(SmilePBNRecommenderConstants.CallPrefix))
            {
                var nodeName = _network.GetNodeName(nodeHandle);
                var site = new CallSite
                {
                    kind = CallSiteKind.RECEIVER,
                    method = new CoReMethodName(nodeName)
                };

                _callNodes.Add(site, nodeHandle);
            }
            else if (nodeId.StartsWith(SmilePBNRecommenderConstants.ParameterPrefix))
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
            _queriedMethods.Clear();

            AddEvidenceIfAvailable(_classContextHandle, SmilePBNRecommenderConstants.NewClassContext(query.classCtx));
            AddEvidenceIfAvailable(_methodContextHandle, SmilePBNRecommenderConstants.NewMethodContext(query.methodCtx));
            AddEvidenceIfAvailable(_definitionHandle, SmilePBNRecommenderConstants.NewDefinition(query.definition));

            foreach (var site in query.sites)
            {
                AddCallSiteEvidenceIfAvailable(site);
            }

            _network.UpdateBeliefs();

            return CollectProposals();
        }

        private void AddEvidenceIfAvailable(int nodeHandle, string outcome)
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
                _network.SetEvidence(sitesToHandle[callSite], SmilePBNRecommenderConstants.StateTrue);
            }
        }

        private CoReProposal[] CollectProposals()
        {
            var unqueriedCallSites = _callNodes.Keys.Except(_queriedMethods);
            var proposals =
                unqueriedCallSites.Select(cs => new CoReProposal(cs.method, GetProbability(cs)))
                                  .Where(cp => cp.Probability > MinimalProbability);

            var sortedProposals = Sets.NewSortedSet(CreateProposalComparer());

            foreach (var proposal in proposals)
            {
                sortedProposals.Add(proposal);
            }

            return sortedProposals.ToArray();
        }

        public static Func<CoReProposal, CoReProposal, ComparisonResult> CreateProposalComparer()
        {
            return (p1, p2) =>
                (Math.Abs(p1.Probability - p2.Probability) < 0.00001)
                    ? ComparisonResult.Equal
                    : ((p1.Probability > p2.Probability) ? ComparisonResult.Greater : ComparisonResult.Smaller);
        }

        private double GetProbability(CallSite site)
        {
            var nodeName = SmilePBNRecommenderConstants.NewReceiverCallSite(site);
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

        private bool Equals(SmilePBNRecommender other)
        {
            return _type.Equals(other._type) && _network.EqualsExtension(other._network);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_type.GetHashCode()*397) ^ _network.GetHashCodeExtension();
            }
        }
    }
}