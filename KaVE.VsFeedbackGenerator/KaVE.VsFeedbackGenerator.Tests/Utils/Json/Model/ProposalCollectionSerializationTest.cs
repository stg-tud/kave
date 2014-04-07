using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Tests.Events.CompletionEvent;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class ProposalCollectionSerializationTest
    {
        [Test]
        public void ShouldSerializationProposalCollection()
        {
            var uut = new ProposalCollection(CompletionEventTestFactory.CreateAnonymousProposals(3));

            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
