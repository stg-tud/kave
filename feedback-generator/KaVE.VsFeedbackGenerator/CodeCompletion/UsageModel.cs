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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.ObjectUsage;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    public class UsageModel
    {
        private readonly Network _network;
        private readonly string[] _contexts = {"pattern", "classContext", "methodContext", "definitionSite"};
        private readonly string[] _methods;

        public UsageModel(Network network)
        {
            _network = network;
            _methods = network.GetAllNodeIds().Where(id => !_contexts.Contains(id)).ToArray();
        }

        public KeyValuePair<CoReMethodName, double>[] Query([NotNull] Query query)
        {
            _network.ClearAllEvidence();

            SetClassContext(query.classCtx);

            SetMethodContext(query.methodCtx);

            SetDefinitionSite(query.definition);

            var evidences =
                query.sites.Select(site => Escape(CallSiteEvidence(site)))
                     .Where(method => _methods.Contains(method))
                     .Distinct()
                     .ToList();
            evidences.ForEach(m => _network.SetEvidence(m, "true"));

            _network.UpdateBeliefs();

            return CollectProposals(evidences);
        }

        private void SetClassContext(CoReTypeName typeName)
        {
            if (typeName != null)
            {
                var classCtx = ClassContextEvidence(typeName);
                SetEvidence(_contexts[1], classCtx);
            }
        }

        private void SetMethodContext(CoReMethodName methodName)
        {
            if (methodName != null)
            {
                var methodCtx = MethodContextEvidence(methodName);
                SetEvidence(_contexts[2], methodCtx);
            }
        }

        private void SetDefinitionSite(DefinitionSite definitionSite)
        {
            if (definitionSite != null)
            {
                var defintionSite = DefinitionSiteEvidence(definitionSite);
                SetEvidence(_contexts[3], defintionSite);
            }
        }

        private void SetEvidence(string nodeId, string stateId)
        {
            var escaped = Escape(stateId);
            if (_network.GetOutcomeIds(nodeId).Contains(escaped))
            {
                _network.SetEvidence(nodeId, escaped);
            }
        }

        private KeyValuePair<CoReMethodName, double>[] CollectProposals(IEnumerable<string> evidences)
        {
            var proposals =
                _methods.Except(evidences)
                        .ToDictionary(
                            method => new CoReMethodName(_network.GetNodeName(method)),
                            method => _network.GetNodeValue(method)[0]);

            var sortedProposals =
                new SortedSet<KeyValuePair<CoReMethodName, double>>(
                    new global::JetBrains.Comparer<KeyValuePair<CoReMethodName, double>>(
                        (p1, p2) => (Math.Abs(p1.Value - p2.Value) < 0.0001) ? 0 : ((p1.Value > p2.Value) ? -1 : 1)));
            sortedProposals.AddRange(proposals);

            return sortedProposals.ToArray();
        }

        public static string Escape(string origin)
        {
            var regex = new Regex("[^a-zA-z0-9]");
            return regex.Replace(origin, "_");
        }

        private static string DefinitionSiteEvidence([NotNull] DefinitionSite site)
        {
            switch (site.kind)
            {
                case DefinitionKind.RETURN:
                    return string.Format("{0}:{1}", site.kind, site.method.Name);
                case DefinitionKind.NEW:
                    return string.Format("INIT:{0}", site.method.Name);
                case DefinitionKind.PARAM:
                    return string.Format("{0}({1}):{2}", site.kind, site.arg, site.method.Name);
                case DefinitionKind.FIELD:
                    return string.Format("{0}:{1}", site.kind, site.field.Name);
                case DefinitionKind.THIS:
                case DefinitionKind.CONSTANT:
                case DefinitionKind.UNKNOWN:
                    return site.kind.ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string CallSiteEvidence([NotNull] CallSite site)
        {
            return site.call.Name;
        }

        private static string MethodContextEvidence([NotNull] CoReMethodName name)
        {
            return name.Name;
        }

        private static string ClassContextEvidence([NotNull] CoReTypeName name)
        {
            return name.Name;
        }
    }
}