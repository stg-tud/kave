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

using KaVE.Commons.Utils.CodeCompletion.Impl;
using NUnit.Framework;
using Smile;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    internal class SmileNetworkUtilsTest
    {
        [Test]
        public void DefaultValues()
        {
            var n = new Network();
            Assert.AreNotEqual(0, n.GetHashCodeExtension());
            Assert.AreNotEqual(1, n.GetHashCodeExtension());
        }

        [Test]
        public void Equality_Default()
        {
            var a = new Network();
            var b = new Network();
            Assert.True(a.EqualsExtension(b));
            Assert.AreEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        [Test]
        public void Equality_SettingValues()
        {
            var a = new Network();
            AddNode(a, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});
            var b = new Network();
            AddNode(b, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});

            Assert.True(a.EqualsExtension(b));
            Assert.AreEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        [Test]
        public void Equality_ProbabilitiesWithTinyDifferences()
        {
            var a = new Network();
            AddNode(a, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.300001, 0.499999});
            var b = new Network();
            AddNode(b, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});

            Assert.True(a.EqualsExtension(b));
            Assert.AreEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        [Test]
        public void Equality_DifferentNodes()
        {
            var a = new Network();
            AddNode(a, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});
            var b = new Network();

            Assert.False(a.EqualsExtension(b));
            Assert.AreNotEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        [Test]
        public void Equality_DifferentOutcomes()
        {
            var a = new Network();
            AddNode(a, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});
            var b = new Network();
            AddNode(b, "n1", new[] {"a", "b", "x"}, new[] {0.2, 0.3, 0.5});

            Assert.False(a.EqualsExtension(b));
            Assert.AreNotEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        [Test]
        public void Equality_DifferentProbabilities()
        {
            var a = new Network();
            AddNode(a, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.3, 0.5});
            var b = new Network();
            AddNode(b, "n1", new[] {"a", "b", "c"}, new[] {0.2, 0.2, 0.6});

            Assert.False(a.EqualsExtension(b));
            Assert.AreNotEqual(a.GetHashCodeExtension(), b.GetHashCodeExtension());
        }

        public static void AddNode(Network n, string nodeTitle, string[] outcomes, double[] probabilities)
        {
            var handle = n.AddNode(Network.NodeType.Cpt);
            n.SetNodeId(handle, "a");

            foreach (var outcome in outcomes)
            {
                n.AddOutcome(handle, outcome);
            }
            n.DeleteOutcome(handle, 0);
            n.DeleteOutcome(handle, 0);

            n.SetNodeDefinition(handle, probabilities);
        }
    }
}