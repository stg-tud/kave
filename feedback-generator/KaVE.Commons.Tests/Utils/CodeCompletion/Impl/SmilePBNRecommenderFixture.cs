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

using System.Collections.Generic;
using System.IO;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using Smile;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    internal static class SmilePBNRecommenderFixture
    {
        public static CoReTypeName SomeType()
        {
            return new CoReTypeName("LType");
        }

        public static Network CreateNetwork()
        {
            var network = new Network();

            var patternHandle = network.AddNode(Network.NodeType.Cpt, SmilePBNRecommenderConstants.PatternTitle);
            network.SetNodeName(patternHandle, SmilePBNRecommenderConstants.PatternTitle);
            network.SetNodeProperties(patternHandle, new[] {"p1", "p2", "p3"}, new[] {0.3, 0.5, 0.2});

            AddNode(
                network,
                patternHandle,
                SmilePBNRecommenderConstants.ClassContextTitle,
                SmilePBNRecommenderConstants.ClassContextTitle,
                new[] {"LType", "LType2", "LType3"},
                new[] {0.6, 0.2, 0.2, 0.3, 0.4, 0.3, 0.2, 0.3, 0.5});
            AddNode(
                network,
                patternHandle,
                SmilePBNRecommenderConstants.MethodContextTitle,
                SmilePBNRecommenderConstants.MethodContextTitle,
                new[] {"LType.M()LVoid;", "LType.Ret()LRet;", "LType.Param(LArg;)LVoid;", "LType.Id(LArg;)LRet;"},
                new[] {0.6, 0.2, 0.1, 0.1, 0.3, 0.4, 0.2, 0.1, 0.2, 0.3, 0.4, 0.1});
            AddNode(
                network,
                patternHandle,
                SmilePBNRecommenderConstants.DefinitionTitle,
                SmilePBNRecommenderConstants.DefinitionTitle,
                new[] {"RETURN:LType.Create()LType;", "THIS", "CONSTANT", "FIELD:LType.object;LType"},
                new[] {0.6, 0.2, 0.1, 0.1, 0.1, 0.3, 0.3, 0.3, 0.1, 0.4, 0.1, 0.4});

            AddMethod(network, patternHandle, "LType.Init()LVoid;", new[] {0.95, 0.15, 0.05});
            AddMethod(network, patternHandle, "LType.Execute()LVoid;", new[] {0.6, 0.99, 0.7});
            AddMethod(network, patternHandle, "LType.Finish()LVoid;", new[] {0.05, 0.5, 0.9});

            return network;
        }

        public static string CreateNetworkAsString()
        {
            var net = CreateNetwork();
            var tmpFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".xdsl");
            net.WriteFile(tmpFile);
            var str = File.ReadAllText(tmpFile);
            File.Delete(tmpFile);
            return str;
        }

        private static void AddMethod(Network net, int patternNodeHandle, string methodName, double[] probabilities)
        {
            var nodeId = "C_" + SmilePBNRecommender.ConvertToLegalSmileName(methodName);
            AddNode(
                net,
                patternNodeHandle,
                nodeId,
                methodName,
                new[] {SmilePBNRecommenderConstants.StateTrue, SmilePBNRecommenderConstants.StateFalse},
                Expand(probabilities));
        }

        private static double[] Expand(double[] trues)
        {
            var probs = new double[trues.Length*2];
            for (int i = 0; i < trues.Length; i++)
            {
                probs[2*i] = trues[i];
                probs[2*i + 1] = 1 - trues[i];
            }
            return probs;
        }

        private static void AddNode(
            Network net,
            int patternNodeHandle,
            string nodeId,
            string nodeName,
            IEnumerable<string> states,
            double[] probs)
        {
            var handle = net.AddNode(Network.NodeType.Cpt, nodeId);
            net.SetNodeName(handle, nodeName);
            net.AddArc(patternNodeHandle, handle);
            net.SetNodeProperties(handle, states, probs);
        }

        private static void SetNodeProperties(this Network net,
            int nodeHandle,
            IEnumerable<string> states,
            double[] probs)
        {
            foreach (var state in states)
            {
                var convertedName = SmilePBNRecommender.ConvertToLegalSmileName(state);
                net.AddOutcome(nodeHandle, convertedName);
            }
            //Remove default states
            net.DeleteOutcome(nodeHandle, "State0");
            net.DeleteOutcome(nodeHandle, "State1");

            net.SetNodeDefinition(nodeHandle, probs);
        }

        public static Query CreateDefaultQuery()
        {
            var query = new Query
            {
                type = new CoReTypeName("LTypeUnknown"),
                classCtx = new CoReTypeName("LTypeUnknown"),
                methodCtx = new CoReMethodName("LTypeUnknown.M()LVoid;"),
                definition = new DefinitionSite {kind = DefinitionSiteKind.UNKNOWN}
            };

            return query;
        }
    }
}