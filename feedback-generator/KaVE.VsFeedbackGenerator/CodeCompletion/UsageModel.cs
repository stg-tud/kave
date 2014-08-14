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

using System.Collections.Generic;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Model.ObjectUsage;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    public class UsageModel
    {
        private readonly Network _network;
        private readonly string[] _methods;

        public UsageModel(Network network)
        {
            _network = network;
            _methods = network.GetAllNodeIds().Where(id => id != "pattern").ToArray();
        }

        public IDictionary<CoReMethodName, double> Query([NotNull] Query query)
        {
            var evidences = new List<string>();

            foreach (
                var method in
                    query.sites.Select(site => site.call.Method).Where(method => _methods.Contains(method)).Distinct())
            {
                evidences.Add(method);
                _network.SetEvidence(method, "true");
            }

            _network.UpdateBeliefs();

            var proposals =
                _methods.Except(evidences)
                        .ToDictionary(
                            method => new CoReMethodName(_network.GetNodeName(method)),
                            method => _network.GetNodeValue(method)[0]);

            _network.ClearAllEvidence();
            return proposals;
        }
    }
}