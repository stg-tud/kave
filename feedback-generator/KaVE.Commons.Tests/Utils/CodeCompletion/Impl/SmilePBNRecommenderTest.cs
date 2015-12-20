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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.CodeCompletion.Impl;
using NUnit.Framework;
using Smile;
using Constants = KaVE.Commons.Utils.CodeCompletion.Impl.SmilePBNRecommenderConstants;
using Fix = KaVE.Commons.Tests.Utils.CodeCompletion.Impl.SmilePBNRecommenderFixture;

namespace KaVE.Commons.Tests.Utils.CodeCompletion.Impl
{
    [TestFixture]
    internal class SmilePBNRecommenderTest
    {
        private SmilePBNRecommender _uut;
        private Network _network;

        [SetUp]
        public void SetUp()
        {
            _network = Fix.CreateNetwork();
            _uut = new SmilePBNRecommender(Fix.SomeType(), _network);
        }

        [Test, Ignore]
        public void Save()
        {
            _network.WriteFile("c:/.../Network.xdsl");
        }

        [Test, Ignore]
        public void SimpleIntegrationTest()
        {
            var n = new Network();
            n.ReadFile(@"C:\Users\seb\Dropbox\Shared (Mac)\smile-export.xdsl");

            var tStringBuilder = new CoReTypeName("LSystem/Text/StringBuilder");
            var rec = new SmilePBNRecommender(tStringBuilder, n);
            var query = new Query
            {
                type = tStringBuilder,
                classCtx = new CoReTypeName("LCC"),
                methodCtx = new CoReMethodName("LMC.m()LV;"),
                definition =
                    DefinitionSites.CreateDefinitionByConstructor("LSystem/Text/StringBuilder.<init>()LSystem/Void;")
            };

            var call = CallSites.CreateReceiverCallSite("LSystem/Text/StringBuilder.ToString()LSystem/String;");

            PrintProposals("before", rec.Query(query));
            query.sites.Add(call);
            PrintProposals("added ToString", rec.Query(query));
            query.sites.Remove(call);
            PrintProposals("removed ToString", rec.Query(query));
        }

        private static void PrintProposals(string title, CoReProposal[] proposals)
        {
            Console.WriteLine("#### {0}", title);
            foreach (var p in proposals)
            {
                var m = p.Name.ToString().Replace("LSystem/Text/StringBuilder.", "");
                Console.Write("* {0} ({1})\n", m, p.FormattedProbability);
            }
            Console.WriteLine();
        }

        [Test, Ignore]
        public void MoreExtensiveTestingOfCornerCases()
        {
            // e.g., different thresholds are not tested yet!
            Assert.Fail();
        }

        [Test]
        public static void CharactersAndNumbersAreNotConverted()
        {
            const string expected = "aA1_";
            var actual = SmilePBNRecommender.ConvertToLegalSmileName(expected);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void PreceedingNumbersAreHandled()
        {
            var actual = SmilePBNRecommender.ConvertToLegalSmileName("1");
            const string expected = "x1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void NonAlphanumericCharactersAreConvertedToUnderscore()
        {
            const string input = "aA1!\"§$%&/()=?{[]}-+*#;,:.><|öäüÖÄÜ^€@";
            var actual = SmilePBNRecommender.ConvertToLegalSmileName(input);
            const string expected = "aA1___________________________________";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotSetEvidencesOnEmptyQuery()
        {
            var query = Fix.CreateDefaultQuery();

            _uut.Query(query);

            var actual = _network.GetAllNodes().All(id => !_network.IsEvidence(id));
            Assert.IsTrue(actual);
        }

        [Test]
        public void ShouldNotSetEvidencesOnNotMatchingQuery()
        {
            var query = Fix.CreateDefaultQuery();
            query.definition = DefinitionSites.CreateDefinitionByReturn("LStrangeType.M()LType;");
            query.sites.Add(CallSites.CreateReceiverCallSite("LStrangeType.M()LType;"));

            _uut.Query(query);

            var actual = _network.GetAllNodes().All(id => !_network.IsEvidence(id));
            Assert.IsTrue(actual);
        }

        [Test]
        public void ShouldSetEvidenceOnClassContext()
        {
            var query = Fix.CreateDefaultQuery();
            query.classCtx = new CoReTypeName("LType");

            _uut.Query(query);

            var expected = SmilePBNRecommender.ConvertToLegalSmileName("LType");
            var actual = _network.GetEvidenceId(Constants.ClassContextTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnMethodContext()
        {
            var query = Fix.CreateDefaultQuery();
            query.methodCtx = new CoReMethodName("LType.M()LVoid;");

            _uut.Query(query);

            var expected = SmilePBNRecommender.ConvertToLegalSmileName("LType.M()LVoid;");
            var actual = _network.GetEvidenceId(Constants.MethodContextTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnDefinitionSite()
        {
            var query = Fix.CreateDefaultQuery();
            query.definition = DefinitionSites.CreateDefinitionByReturn("LType.Create()LType;");

            _uut.Query(query);

            var expected = SmilePBNRecommender.ConvertToLegalSmileName("RETURN:LType.Create()LType;");
            var actual = _network.GetEvidenceId(Constants.DefinitionTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnCallSite()
        {
            var query = Fix.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));

            _uut.Query(query);

            var actual = _network.GetEvidenceId(SmilePBNRecommender.ConvertToLegalSmileName("C_LType.Init()LVoid;"));
            Assert.AreEqual(Constants.StateTrue, actual);
        }

        [Test]
        public void ShouldNotProduceAnyProposalsIfAllMethodsAreAlreadyCalled()
        {
            var query = Fix.CreateDefaultQuery();

            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Execute()LVoid;"));
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Finish()LVoid;"));

            var expected = new CoReProposal[] {};
            var actual = _uut.Query(query);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceAllProposalsIfNoMethodIsAlreadyCalled()
        {
            var query = Fix.CreateDefaultQuery();

            var expected = new[]
            {
                new CoReMethodName("LType.Init()LVoid;"),
                new CoReMethodName("LType.Execute()LVoid;"),
                new CoReMethodName("LType.Finish()LVoid;")
            };

            var proposals = _uut.Query(query);
            var actual = proposals.Select(p => p.Name);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceSomeProposalsIfSomeMethodIsAlreadyCalled()
        {
            var query = Fix.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));

            var expected = new[]
            {
                new CoReMethodName("LType.Execute()LVoid;"),
                new CoReMethodName("LType.Finish()LVoid;")
            };

            var proposals = _uut.Query(query);
            var actual = proposals.Select(p => p.Name);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceOrderedProposals()
        {
            var query = Fix.CreateDefaultQuery();

            var expected = new[]
            {
                new CoReProposal(new CoReMethodName("LType.Execute()LVoid;"), 0.815),
                new CoReProposal(new CoReMethodName("LType.Finish()LVoid;"), 0.445),
                new CoReProposal(new CoReMethodName("LType.Init()LVoid;"), 0.37)
            };

            var actual = _uut.Query(query);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldProduceAdaptedPropertiesIfContextsAreSet()
        {
            var query = Fix.CreateDefaultQuery();
            query.classCtx = new CoReTypeName("LType");
            query.methodCtx = new CoReMethodName("LType.M()LVoid;");
            query.definition = DefinitionSites.CreateDefinitionByField("LType.object;LType");

            var expected = new[]
            {
                new CoReProposal(new CoReMethodName("LType.Execute()LVoid;"), 0.803),
                new CoReProposal(new CoReMethodName("LType.Init()LVoid;"), 0.453),
                new CoReProposal(new CoReMethodName("LType.Finish()LVoid;"), 0.37)
            };

            var actual = _uut.Query(query);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void ShouldRemoveStoredCallsFromPreviousQueries()
        {
            // place a query that includes a call...
            var query = Fix.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Execute()LVoid;"));
            _uut.Query(query);

            // ...then fall back to a default case
            ShouldProduceOrderedProposals();
        }

        [Test]
        public void ShouldProduceAdaptedPropertiesIfCallSiteIsSet()
        {
            var query = Fix.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));

            var expected = new[]
            {
                new CoReProposal(new CoReMethodName("LType.Execute()LVoid;"), 0.682),
                new CoReProposal(new CoReMethodName("LType.Finish()LVoid;"), 0.164)
            };

            var actual = _uut.Query(query);

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void Equality_Default()
        {
            var a = new SmilePBNRecommender(new CoReTypeName("LT"), new Network());
            var b = new SmilePBNRecommender(new CoReTypeName("LT"), new Network());
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentType()
        {
            var a = new SmilePBNRecommender(new CoReTypeName("LT"), new Network());
            var b = new SmilePBNRecommender(new CoReTypeName("LT2"), new Network());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentNetwork()
        {
            var n = new Network();
            n.AddNode(Network.NodeType.Cpt, "some_node");
            var a = new SmilePBNRecommender(new CoReTypeName("LT"), n);
            var b = new SmilePBNRecommender(new CoReTypeName("LT"), new Network());
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }
    }
}