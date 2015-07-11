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
using KaVE.Commons.Model.ObjectUsage;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    public class ExamplePBNRecommender : IPBNRecommender
    {
        private readonly Network _net;
        private int _patternNodeHandle;

        private readonly string[] _known = {"Init", "Execute", "Finish"};
        private readonly List<string> _selected = new List<string>();

        public ExamplePBNRecommender()
        {
            _net = new Network();
            AddNode("patterns", new[] {"p0", "p1"}, new[] {0.5, 0.5});
            AddMethod("Init", new[] {0.95, 0.15});
            AddMethod("Execute", new[] {0.7, 0.25});
            AddMethod("Finish", new[] {0.05, 0.8});
        }

        private void AddNode(string name, string[] states, double[] probs)
        {
            _patternNodeHandle = _net.AddNode(Network.NodeType.Cpt);
            _net.SetNodeId(_patternNodeHandle, name);
            _net.SetNodeName(_patternNodeHandle, name);
            _net.SetNodeDefinition(_patternNodeHandle, probs);

            for (int i = 0; i < states.Length; i++)
            {
                _net.SetOutcomeId(_patternNodeHandle, i, states[i]);
            }
        }

        private void AddMethod(string methodName, double[] trueProbs)
        {
            var probs = ExpandProbs(trueProbs);

            var handle = _net.AddNode(Network.NodeType.Cpt);
            _net.AddArc(_patternNodeHandle, handle);
            _net.SetNodeId(handle, methodName);
            _net.SetNodeName(handle, methodName);
            _net.SetNodeDefinition(handle, probs);

            _net.SetOutcomeId(handle, 0, "true");
            _net.SetOutcomeId(handle, 1, "false");
        }

        private static double[] ExpandProbs(double[] @in)
        {
            var @out = new double[@in.Length*2];
            for (var i = 0; i < @in.Length; i++)
            {
                @out[2*i] = @in[i];
                @out[2*i + 1] = 1 - @in[i];
            }
            return @out;
        }

        public void Reset()
        {
            _selected.RemoveAll(s => true);
            _net.ClearAllEvidence();
        }

        public void Set(string m)
        {
            var nodeExists = Array.Exists(_net.GetAllNodeIds(), s => s.Equals(m));
            if (nodeExists)
            {
                var handle = _net.GetNode(m);
                _net.SetEvidence(handle, 0);
                _selected.Add(m);
            }
        }

        public Network GetNetwork()
        {
            return _net;
        }

        public Tuple<string, double>[] GetProbabilities()
        {
            _net.UpdateBeliefs();

            var res = new List<Tuple<string, double>>();
            foreach (var m in _known)
            {
                if (!_selected.Contains(m))
                {
                    var t = Tuple.Create(m, GetProbability(m));
                    res.Add(t);
                }
            }
            return res.OrderByDescending(t => t.Item2).ToArray();
        }

        private double GetProbability(string name)
        {
            var handle = _net.GetNode(name);
            var raw = _net.GetNodeValue(handle)[0];
            return Math.Round(raw, 2);
        }

        public CoReTypeName GetTypeOfRecommender()
        {
            throw new NotImplementedException();
        }

        public CoReProposal[] Query(Query query)
        {
            // TODO @seb: adapt this class to IPBNNetwork interface
            throw new NotImplementedException();
        }
    }
}