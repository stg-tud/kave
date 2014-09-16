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
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.CodeCompletion;
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
            net.SetNodeProperties(handle, new[] {"Create", "Use", "Destroy"}, new[] {0.3, 0.5, 0.2});
            net.AddNode(
                handle,
                "classContext",
                new[] {"LType", "LType2", "LType3"},
                new[] {0.6, 0.2, 0.2, 0.3, 0.4, 0.3, 0.2, 0.3, 0.5});
            net.AddNode(
                handle,
                "methodContext",
                new[] {"LType.M()LVoid;", "LType.Ret()LRet;", "LType.Param(LArg;)LVoid;", "LType.Id(LArg;)LRet;"},
                new[] {0.6, 0.2, 0.1, 0.1, 0.3, 0.4, 0.2, 0.1, 0.2, 0.3, 0.4, 0.1});
            net.AddNode(
                handle,
                "definitionSite",
                new[] {"RETURN:LType.Create()LType;", "THIS", "CONSTANT", "FIELD:LType.object;LType"},
                new[] {0.6, 0.2, 0.1, 0.1, 0.1, 0.3, 0.3, 0.3, 0.1, 0.4, 0.1, 0.4});

            net.AddMethod(handle, "LType.Init()LVoid;", new[] {0.95, 0.15, 0.05});
            net.AddMethod(handle, "LType.Execute()LVoid;", new[] {0.6, 0.99, 0.7});
            net.AddMethod(handle, "LType.Finish()LVoid;", new[] {0.05, 0.5, 0.9});
            return net;
        }

        private static void AddMethod(this Network net, int patternNodeHandle, string methodName, double[] trues)
        {
            net.AddNode(patternNodeHandle, methodName, new[] {"true", "false"}, Expand(trues));
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

        private static void AddNode(this Network net,
            int patternNodeHandle,
            string name,
            string[] states,
            double[] probs)
        {
            var id = net.AddNode(Smile.Network.NodeType.Cpt, UsageModel.Escape(name));
            net.SetNodeName(id, name);
            net.AddArc(patternNodeHandle, id);
            net.SetNodeProperties(id, states, probs);
        }

        private static void SetNodeProperties(this Network net, int id, string[] states, double[] probs)
        {
            Asserts.That(states.Length > 0);
            for (var i = 0; i < states.Length; i++)
            {
                if (i < 2)
                {
                    net.SetOutcomeId(id, i, UsageModel.Escape(states[i]));
                }
                else
                {
                    net.AddOutcome(id, UsageModel.Escape(states[i]));
                }
            }
            if (states.Length == 1)
            {
                net.DeleteOutcome(id, 1);
            }
            net.SetNodeDefinition(id, probs);
        }

        public static void AssertEqualityIgnoringRoundingErrors<TKey>(KeyValuePair<TKey, double>[] expected,
            KeyValuePair<TKey, double>[] actual)
        {
            Func<double, string> policy = d => string.Format("{0:0.###}", d);

            var convertedExpected = expected.ValuesToString(policy);
            var convertedActual = actual.ValuesToString(policy);

            CollectionAssert.AreEquivalent(convertedExpected, convertedActual);
        }

        private static IEnumerable<KeyValuePair<TKey, string>> ValuesToString<TKey, TValue>(
            this IEnumerable<KeyValuePair<TKey, TValue>> src,
            Func<TValue, string> policy = null)
        {
            return
                src.Select(
                    p => new KeyValuePair<TKey, string>(p.Key, (policy == null) ? p.Value.ToString() : policy(p.Value)));
        }
    }
}