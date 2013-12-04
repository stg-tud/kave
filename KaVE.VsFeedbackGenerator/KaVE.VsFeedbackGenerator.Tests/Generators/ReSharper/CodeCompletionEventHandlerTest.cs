using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.ReSharper
{
    [TestFixture]
    public class CodeCompletionEventHandlerTest
    {
        private Mock<IIDESession> _mockSession;
        private Mock<IMessageBus> _mockMessageBus;

        [SetUp]
        public void SetupMockEnvironment()
        {
            _mockSession = new Mock<IIDESession>();
            _mockSession.Setup(session => session.UUID).Returns("TestUUID");
            _mockMessageBus = new Mock<IMessageBus>();
        }

        [Test]
        public void ShouldHandleCodeCompletionWithApplication()
        {
            var builder = new CodeCompletionEventHandler("", _mockSession.Object, _mockMessageBus.Object);
            var lookupItem = ReSharperMockUtils.MockLookupItem();

            _mockMessageBus.Setup(bus => bus.Publish(It.IsAny<CompletionEvent>())).Callback(
                (CompletionEvent ce) =>
                {
                    Assert.AreEqual(1, ce.ProposalCollection.Proposals.Count);
                    CollectionAssert.Contains(ce.ProposalCollection.Proposals, lookupItem.ToProposal());
                });

            builder.OnBeforeShownItemsUpdated(new List<ILookupItem> { lookupItem });
            builder.OnSelectionChanged(lookupItem);
            builder.OnLookupClosed();
            builder.OnCompletionApplied();

            _mockMessageBus.Verify(bus => bus.Publish(It.IsAny<CompletionEvent>()), Times.Once);
        }
    }
}
