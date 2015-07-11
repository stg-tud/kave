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
using System.Linq;
using Smile;

namespace KaVE.Commons.Utils.CodeCompletion.Impl
{
    /// <summary>
    ///     implements a simple heuristic to detect equal networks
    ///     * equal nodes array
    ///     * equal outcomes
    ///     * equal probabilities per node
    /// </summary>
    public static class SmileNetworkUtils
    {
        public static bool EqualsExtension(this Network a, object obj)
        {
            if (a == null)
            {
                return obj == null;
            }
            if (obj == null)
            {
                return false;
            }
            var b = obj as Network;
            if (b == null)
            {
                return false;
            }
            return Equals(a, b);
        }

        public static bool Equals(Network a, Network b)
        {
            var aNodeIds = a.GetAllNodeIds();
            var bNodeIds = b.GetAllNodeIds();
            if (!aNodeIds.SequenceEqual(bNodeIds))
            {
                return false;
            }

            foreach (var nodeId in aNodeIds)
            {
                var aOutcomes = a.GetOutcomeIds(nodeId);
                var bOutcomes = b.GetOutcomeIds(nodeId);
                if (!aOutcomes.SequenceEqual(bOutcomes))
                {
                    return false;
                }

                var aProbs = a.GetNodeDefinition(nodeId);
                var aProbsInt = aProbs.Select(FixPrecision);
                var bProbs = b.GetNodeDefinition(nodeId);
                var bProbsInt = bProbs.Select(FixPrecision);
                if (!aProbsInt.SequenceEqual(bProbsInt))
                {
                    return false;
                }
            }

            return true;
        }

        public static int GetHashCodeExtension(this Network n)
        {
            unchecked
            {
                const int seed = 13;
                var hash = seed;
                foreach (var nodeId in n.GetAllNodeIds())
                {
                    hash = hash*seed + nodeId.GetHashCode();
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var outcome in n.GetOutcomeIds(nodeId))
                    {
                        hash = hash*seed + outcome.GetHashCode();
                    }
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var prob in n.GetNodeDefinition(nodeId))
                    {
                        var fixedPrecision = FixPrecision(prob);
                        hash = hash*seed + fixedPrecision.GetHashCode();
                    }
                }
                return hash;
            }
        }

        private static int FixPrecision(double p)
        {
            return (int) Math.Round(p*100000);
        }
    }
}