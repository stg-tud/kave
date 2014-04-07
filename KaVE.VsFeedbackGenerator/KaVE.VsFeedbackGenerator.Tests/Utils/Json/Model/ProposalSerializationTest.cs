using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class ProposalSerializationTest
    {
        [Test]
        public void ShouldSerializeProposal()
        {
            var uut = new Proposal
            {
                Name = TestNameFactory.GetAnonymousTypeName(),
                Relevance = 42
            };
            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
