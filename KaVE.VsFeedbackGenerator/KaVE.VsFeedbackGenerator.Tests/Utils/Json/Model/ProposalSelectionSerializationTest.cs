using System;
using KaVE.Model.Events.CompletionEvent;
using KaVE.TestUtils.Model.Events.CompletionEvent;
using NUnit.Framework;

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
                SelectedAfter = TimeSpan.FromMinutes(23)
            };

            JsonAssert.SerializationPreservesData(uut);
        }
    }
}
