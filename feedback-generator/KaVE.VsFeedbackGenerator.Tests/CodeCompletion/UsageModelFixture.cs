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
using NUnit.Framework;
using Smile;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    internal static class UsageModelFixture
    {
        public static Network Network()
        {
            var net = new Network();
            var handle = net.AddNode(Smile.Network.NodeType.Cpt, "pattern");
            net.SetNodeName(handle, "pattern");
            SetNodeProperties(net, handle, new[] {"p1", "p2"}, new[] {0.5, 0.5});
            AddMethod(net, handle, "Init", new[] {0.95, 0.15});
            AddMethod(net, handle, "Execute", new[] {0.7, 0.25});
            AddMethod(net, handle, "Finish", new[] {0.05, 0.8});
            return net;
        }

        private static void AddMethod(Network net, int patternNodeHandle, string methodName, double[] trueProbs)
        {
            var handle = net.AddNode(Smile.Network.NodeType.Cpt, methodName);
            net.SetNodeName(handle, string.Format("LType.{0}()LReturn;", methodName));
            net.AddArc(patternNodeHandle, handle);

            var fullProbs = new double[trueProbs.Length*2];
            for (var i = 0; i < trueProbs.Length; i++)
            {
                fullProbs[2*i] = trueProbs[i];
                fullProbs[2*i + 1] = 1 - trueProbs[i];
            }
            SetNodeProperties(net, handle, new[] {"true", "false"}, fullProbs);
        }

        private static void SetNodeProperties(Network net, int handle, string[] ids, double[] probs)
        {
            for (var i = 0; i < ids.Length; i++)
            {
                net.SetOutcomeId(handle, i, ids[i]);
            }
            net.SetNodeDefinition(handle, probs);
        }

        public static void AssertEquivalenceIgnoringRoundingErrors<TKey>(IDictionary<TKey, double> expected,
            IDictionary<TKey, double> actual)
        {
            Func<double, string> policy = d => string.Format("{0:0.###}", d);

            var convertedExpected = expected.ValuesToString(policy);
            var convertedActual = actual.ValuesToString(policy);

            CollectionAssert.AreEquivalent(convertedExpected, convertedActual);
        }

        private static IDictionary<TKey, string> ValuesToString<TKey, TValue>(this IDictionary<TKey, TValue> src,
            Func<TValue, string> policy = null)
        {
            return src.ToDictionary(
                pair => pair.Key,
                pair => (policy == null) ? pair.Value.ToString() : policy(pair.Value));
        }
    }
}