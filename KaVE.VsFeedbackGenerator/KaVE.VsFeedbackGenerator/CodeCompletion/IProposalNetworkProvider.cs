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
using KaVE.Model.Names;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    public interface IProposalNetworkProvider
    {
        bool CanProvide(ITypeName typeName);
        ProposalNetwork Provide(ITypeName typeName);
    }

    public interface IProposalNetwork
    {
        ISet<Proposal> GenerateProposals(ICollection<IMethodName> methodNames);
    }

    public class ProposalNetwork : IProposalNetwork
    {
        private readonly Network _network;
        private readonly ITypeName _typeName;

        public ProposalNetwork(Network network, ITypeName typeName)
        {
            _network = network;
            _typeName = typeName;
        }

        public ISet<Proposal> GenerateProposals(ICollection<IMethodName> methodNames)
        {
            var methods = _network.GetOutcomeIds("Proposal");
            foreach (var method in methods)
            {
                var isMethodPresent =
                    methodNames.Any(name => name.DeclaringType.Equals(_typeName) && name.Name.Equals(method));
                _network.SetEvidence(method, isMethodPresent ? "True" : "False");
            }
            _network.UpdateBeliefs();
            var probabilities = _network.GetNodeValue("Proposal");
            _network.ClearAllEvidence();
            var proposals = new HashSet<Proposal>();
            for (var i = 0; i < methods.Length; i ++)
            {
                proposals.Add(new Proposal(methods[i], probabilities[i]));
            }
            return proposals;
        }
    }

    public class Proposal
    {
        public Proposal(string name, double probability)
        {
            Name = name;
            Probability = probability;
        }

        public string Name { get; private set; }
        public double Probability { get; private set; }
    }
}