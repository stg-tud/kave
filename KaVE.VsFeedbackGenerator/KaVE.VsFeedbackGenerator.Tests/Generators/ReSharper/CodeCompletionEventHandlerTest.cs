using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Lookup;
using KaVE.Model.Events;
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
        private IIDESession _testSession;
        private Mock<IMessageBus> _mockMessageBus;
        private IList<CompletionEvent> _publishedEvents;

        [SetUp]
        public void SetupMockEnvironment()
        {
            _publishedEvents = new List<CompletionEvent>();
            _testSession = new TestIDESession();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(bus => bus.Publish<IDEEvent>(It.IsAny<CompletionEvent>())).Callback(
                (IDEEvent ideEvent) => _publishedEvents.Add(ideEvent as CompletionEvent));
        }

        [Test]
        public void ShouldHandleCodeCompletionWithApplication()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItem = ReSharperMockUtils.MockLookupItem();

            handler.OnOpened("");
            handler.SetLookupItems(new List<ILookupItem> {lookupItem});
            handler.OnSelectionChanged(lookupItem);
            handler.OnApplication(DateTime.Now, lookupItem);

            Assert.AreEqual(1, _publishedEvents.Count);
            var ce = _publishedEvents.First();
            CollectionAssert.Contains(ce.ProposalCollection.Proposals, lookupItem.ToProposal());
            Assert.AreEqual(CompletionEvent.TerminationState.Applied, ce.TerminatedAs);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithCancellation()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItem = ReSharperMockUtils.MockLookupItem();

            handler.SetLookupItems(new List<ILookupItem> { lookupItem });
            handler.OnSelectionChanged(lookupItem);
            handler.OnCancellation(DateTime.Now);

            Assert.AreEqual(1, _publishedEvents.Count);
            var ce = _publishedEvents.First();
            CollectionAssert.Contains(ce.ProposalCollection.Proposals, lookupItem.ToProposal());
            Assert.AreEqual(CompletionEvent.TerminationState.Cancelled, ce.TerminatedAs);
        }
    }
}