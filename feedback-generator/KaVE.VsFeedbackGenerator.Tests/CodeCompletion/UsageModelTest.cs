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
 *    - Uli Fahrer
 */

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
            _network = UsageModelFixture.CreateNetwork();
            _uut = new UsageModel(_network);
        }

        [Test, Ignore]
        public void Save()
        {
            _network.WriteFile("c:/.../Network.xdsl");
        }

        [Test]
	    public static void CharactersAndNumbersAreNotConverted() {
		    const string expected = "aA1_";
            var actual = UsageModel.ConvertToLegalSmileName(expected);
		
		    Assert.AreEqual(expected, actual);
	    }

        [Test]
        public static void PreceedingNumbersAreHandled()
        {
            var actual = UsageModel.ConvertToLegalSmileName("1");
            const string expected = "x1";

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public static void NonAlphanumericCharactersAreConvertedToUnderscore()
        {
            const string input = "aA1!\"§$%&/()=?{[]}-+*#;,:.><|öäüÖÄÜ^€@";
            var actual = UsageModel.ConvertToLegalSmileName(input);
		    const string expected = "aA1___________________________________";
		
		    Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotSetEvidencesOnEmptyQuery()
        {
            var query = UsageModelFixture.CreateDefaultQuery();

            _uut.Query(query);

            var actual = _network.GetAllNodes().All(id => !_network.IsEvidence(id));
            Assert.IsTrue(actual);
        }

        [Test]
        public void ShouldNotSetEvidencesOnNotMatchingQuery()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.definition = DefinitionSites.CreateDefinitionByReturn("LStrangeType.M()LType;");
            query.sites.Add(CallSites.CreateReceiverCallSite("LStrangeType.M()LType;"));

            _uut.Query(query);

            var actual = _network.GetAllNodes().All(id => !_network.IsEvidence(id));
            Assert.IsTrue(actual);
        }

        [Test]
        public void ShouldSetEvidenceOnClassContext()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.classCtx = new CoReTypeName("LType");

            _uut.Query(query);

            var expected = UsageModel.ConvertToLegalSmileName("LType");
            var actual = _network.GetEvidenceId(ModelConstants.ClassContextTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnMethodContext()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.methodCtx = new CoReMethodName("LType.M()LVoid;");

            _uut.Query(query);

            var expected = UsageModel.ConvertToLegalSmileName("LType.M()LVoid;");
            var actual = _network.GetEvidenceId(ModelConstants.MethodContextTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnDefinitionSite()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.definition = DefinitionSites.CreateDefinitionByReturn("LType.Create()LType;");

            _uut.Query(query);

            var expected = UsageModel.ConvertToLegalSmileName("RETURN:LType.Create()LType;");
            var actual = _network.GetEvidenceId(ModelConstants.DefinitionTitle);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSetEvidenceOnCallSite()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));

            _uut.Query(query);

            var actual = _network.GetEvidenceId(UsageModel.ConvertToLegalSmileName("C_LType.Init()LVoid;"));
            Assert.AreEqual(ModelConstants.StateTrue, actual);
        }

        [Test]
        public void ShouldNotProduceAnyProposalsIfAllMethodsAreAlreadyCalled()
        {
            var query = UsageModelFixture.CreateDefaultQuery();

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
            var query = UsageModelFixture.CreateDefaultQuery();

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
            var query = UsageModelFixture.CreateDefaultQuery();
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
            var query = UsageModelFixture.CreateDefaultQuery();

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
            var query = UsageModelFixture.CreateDefaultQuery();
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
        public void ShouldProduceAdaptedPropertiesIfCallSiteIsSet()
        {
            var query = UsageModelFixture.CreateDefaultQuery();
            query.sites.Add(CallSites.CreateReceiverCallSite("LType.Init()LVoid;"));

            var expected = new[]
            {
                new CoReProposal(new CoReMethodName("LType.Execute()LVoid;"), 0.682),
                new CoReProposal(new CoReMethodName("LType.Finish()LVoid;"), 0.164)
            };

            var actual = _uut.Query(query);

            CollectionAssert.AreEquivalent(expected, actual);
        }
    }
}