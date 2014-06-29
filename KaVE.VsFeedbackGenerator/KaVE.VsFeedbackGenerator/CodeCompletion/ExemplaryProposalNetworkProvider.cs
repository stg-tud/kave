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

using JetBrains.Application;
using KaVE.Model.Names;
using Smile;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [ShellComponent]
    internal class ExemplaryProposalNetworkProvider : IProposalNetworkProvider
    {
        private Network _exemplaryNetwork;

        private Network ExemplaryNetwork
        {
            get { return _exemplaryNetwork ?? (_exemplaryNetwork = InitializeNetwork()); }
        }

        private static Network InitializeNetwork()
        {
            var methodNames = new[] {"Init", "Execute", "Finish"};
            var network = new Network();
            network.AddNode(Network.NodeType.Cpt, "Proposal");
            network.SetOutcomeId("Proposal", 0, methodNames[0]);
            network.SetOutcomeId("Proposal", 1, methodNames[1]);
            network.AddOutcome("Proposal", methodNames[2]);

            for (var i = 0; i < 3; i++)
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

        public bool CanProvide(ITypeName typeName)
        {
            return typeName.Name.Equals("MyButton");
        }

        public ProposalNetwork Provide(ITypeName typeName)
        {
            return CanProvide(typeName) ? new ProposalNetwork(ExemplaryNetwork, typeName) : null;
        }
    }

    internal class MyButton
    {
        public void Init() {}
        public void Execute() {}
        public void Finish() {}
    }
}