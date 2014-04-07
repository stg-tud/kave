using System;
using KaVE.Model.Events.CompletionEvent;
using NUnit.Framework;
using KaVE.Model.Tests.Events.CompletionEvent;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class ProposalSelectionSerializationTest
    {
        [Test]
        public void ShouldSerializeProposalSelection()
        {
            var uut = new ProposalSelection(CompletionEventTestFactory.CreateAnonymousProposal())
            {
                SelectedAt = DateTime.Now
            };

            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
