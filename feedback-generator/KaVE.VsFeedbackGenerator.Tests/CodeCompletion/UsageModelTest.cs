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
using KaVE.Model.ObjectUsage;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using NUnit.Framework;
using Smile;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class UsageModelTest
    {
        private UsageModel _uut;
        private Network _network;

        [SetUp]
        public void SetUp()
        {
            _network = UsageModelFixture.Network();
            _uut = new UsageModel(_network);
        }

        [Test, Ignore]
        public void SaveFixtureToDisk()
        {
            _network.WriteFile("c:/.../Network.xdsl");
        }

        [Test]
        public void ShouldNotSetEvidencesOnEmptyQuery()
        {
            var query = new Query();

            _uut.Query(query);

            Assert.IsTrue(_network.GetAllNodes().All(id => !_network.IsEvidence(id)));
        }

        [Test]
        public void ShouldNotSetEvidencesOnNotMatchingQuery()
        {
            var query = new Query
            {
                definition = new DefinitionSite
                {
                    kind = DefinitionKind.RETURN,
                    method = new CoReMethodName("LStrangeType.M()LType;")
                },
                classCtx = new CoReTypeName("LStrangeType"),
                methodCtx = new CoReMethodName("LStrangeType.M()LType;"),
                type = new CoReTypeName("LType")
            };
            query.sites.Add(new CallSite {method = new CoReMethodName("LStrangeType.M()LType;")});

            _uut.Query(query);

            Assert.IsTrue(_network.GetAllNodes().All(id => !_network.IsEvidence(id)));
        }

        [Test]
        public void ShouldSetEvidenceOnClassContext()
        {
            var query = new Query {classCtx = new CoReTypeName("LType")};

            _uut.Query(query);

            Assert.AreEqual(UsageModel.Escape("LType"), _network.GetEvidenceId("classContext"));
        }

        [Test]
        public void ShouldSetEvidenceOnMethodContext()
        {
            var query = new Query {methodCtx = new CoReMethodName("LType.M()LVoid;")};

            _uut.Query(query);

            Assert.AreEqual(UsageModel.Escape("LType.M()LVoid;"), _network.GetEvidenceId("methodContext"));
        }

        [Test]
        public void ShouldSetEvidenceOnDefinitionSite()
        {
            var query = new Query
            {
                definition =
                    new DefinitionSite
                    {
                        kind = DefinitionKind.RETURN,
                        method = new CoReMethodName("LType.Create()LType;")
                    }
            };

            _uut.Query(query);

            Assert.AreEqual(UsageModel.Escape("RETURN:LType.Create()LType;"), _network.GetEvidenceId("definitionSite"));
        }

        [Test]
        public void ShouldSetEvidenceOnCallSite()
        {
            var query = new Query();
            query.sites.Add(
                new CallSite {kind = CallSiteKind.RECEIVER, method = new CoReMethodName("LType.Init()LVoid;")});

            _uut.Query(query);

            Assert.AreEqual("true", _network.GetEvidenceId(UsageModel.Escape("LType.Init()LVoid;")));
        }

        [Test]
        public void ShouldNotProduceAnyProposalsIfAllMethodsAreAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(new CallSite {method = new CoReMethodName("LType.Init()LVoid;")});
            query.sites.Add(new CallSite {method = new CoReMethodName("LType.Execute()LVoid;")});
            query.sites.Add(new CallSite {method = new CoReMethodName("LType.Finish()LVoid;")});
            var expected = new KeyValuePair<CoReMethodName, double>[] {};

            var actual = model.Query(query);

            UsageModelFixture.AssertEqualityIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldProduceAllProposalsIfNoMethodIsAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            var expected = new[]
            {
                new CoReMethodName("LType.Init()LVoid;"),
                new CoReMethodName("LType.Execute()LVoid;"),
                new CoReMethodName("LType.Finish()LVoid;")
            };

            var proposals = model.Query(query);
            var actual = proposals.Select(p => p.Key);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceSomeProposalsIfSomeMethodIsAlreadyCalled()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(new CallSite {method = new CoReMethodName("LType.Init()LVoid;")});
            var expected = new[]
            {
                new CoReMethodName("LType.Execute()LVoid;"),
                new CoReMethodName("LType.Finish()LVoid;")
            };

            var proposals = model.Query(query);
            var actual = proposals.Select(p => p.Key);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceOrderedProposals()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            var expected = new[]
            {
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Execute()LVoid;"), 0.815),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Finish()LVoid;"), 0.445),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Init()LVoid;"), 0.37)
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEqualityIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldProduceAdaptedPropertiesIfContextsAreSet()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query
            {
                classCtx = new CoReTypeName("LType"),
                methodCtx = new CoReMethodName("LType.M()LVoid;"),
                definition =
                    new DefinitionSite {kind = DefinitionKind.FIELD, field = new CoReFieldName("LType.object;LType")}
            };
            var expected = new[]
            {
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Execute()LVoid;"), 0.803),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Init()LVoid;"), 0.453),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Finish()LVoid;"), 0.37)
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEqualityIgnoringRoundingErrors(expected, actual);
        }

        [Test]
        public void ShouldProduceAdaptedPropertiesIfCallSiteIsSet()
        {
            var net = UsageModelFixture.Network();
            var model = new UsageModel(net);
            var query = new Query();
            query.sites.Add(
                new CallSite {kind = CallSiteKind.RECEIVER, method = new CoReMethodName("LType.Init()LVoid;")});
            var expected = new[]
            {
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Execute()LVoid;"), 0.682),
                new KeyValuePair<CoReMethodName, double>(new CoReMethodName("LType.Finish()LVoid;"), 0.164)
            };

            var actual = model.Query(query);

            UsageModelFixture.AssertEqualityIgnoringRoundingErrors(expected, actual);
        }
    }
}