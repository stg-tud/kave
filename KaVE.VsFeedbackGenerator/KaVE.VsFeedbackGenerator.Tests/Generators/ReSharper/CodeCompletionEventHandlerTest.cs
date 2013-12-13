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
        private IList<IDEEvent> _publishedEvents;

        [SetUp]
        public void SetupMockEnvironment()
        {
            _publishedEvents = new List<IDEEvent>();
            _testSession = new TestIDESession();
            _mockMessageBus = new Mock<IMessageBus>();
            _mockMessageBus.Setup(bus => bus.Publish<IDEEvent>(It.IsAny<IDEEvent>())).Callback(
                (IDEEvent ideEvent) => _publishedEvents.Add(ideEvent));
        }

        [Test]
        public void ShouldHandleCodeCompletionWithApplication()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItems = MockLookupItemList(3);

            handler.OnOpened("");
            handler.SetLookupItems(lookupItems);
            handler.OnSelectionChanged(lookupItems[0]);
            handler.OnClosed();
            handler.OnApplication(lookupItems[0]);
            handler.OnFinished();

            var ce = GetSinglePublishedCompletionEvent();
            Assert.AreEqual(CompletionEvent.TerminationState.Applied, ce.TerminatedAs);
        }

        [Test]
        public void ShouldAddSelectionChangesToCompletionEvent()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItems = MockLookupItemList(5);

            handler.OnOpened("");
            handler.SetLookupItems(lookupItems);
            handler.OnSelectionChanged(lookupItems[3]);
            handler.OnSelectionChanged(lookupItems[2]);
            handler.OnSelectionChanged(lookupItems[1]);
            handler.OnClosed();
            handler.OnApplication(lookupItems[1]);
            handler.OnFinished();

            var ce = GetSinglePublishedCompletionEvent();
            Assert.AreEqual(3, ce.Selections.Count);
            Assert.AreEqual(lookupItems[3].ToProposal(), ce.Selections[0].Proposal);
            Assert.AreEqual(lookupItems[2].ToProposal(), ce.Selections[1].Proposal);
            Assert.AreEqual(lookupItems[1].ToProposal(), ce.Selections[2].Proposal);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithCancellation()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItems = MockLookupItemList(1);

            handler.OnOpened("");
            handler.SetLookupItems(lookupItems);
            handler.OnClosed();
            handler.OnFinished();

            var ce = GetSinglePublishedCompletionEvent();
            Assert.AreEqual(CompletionEvent.TerminationState.Cancelled, ce.TerminatedAs);
        }

        [Test]
        public void ShouldFireEventOnFilter()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItems = MockLookupItemList(0);

            handler.OnOpened("");
            handler.SetLookupItems(lookupItems);
            handler.OnPrefixChanged("a");

            var ce = GetSinglePublishedCompletionEvent();
            Assert.AreEqual(CompletionEvent.TerminationState.Filtered, ce.TerminatedAs);
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TerminatedBy);
        }

        [Test]
        public void ShouldCreateFollowupEventAfterFiltering()
        {
            var handler = new CodeCompletionEventHandler(_testSession, _mockMessageBus.Object);
            var lookupItems = MockLookupItemList(0);

            handler.OnOpened("");
            handler.SetLookupItems(lookupItems);
            handler.OnPrefixChanged("a");
            handler.OnClosed();
            handler.OnFinished();

            var ce = GetLastPublishedCompletionEvent();
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TriggeredBy);
            Assert.AreEqual("a", ce.Prefix);
        }

        private IList<ILookupItem> MockLookupItemList(int numberOfItems)
        {
            IList<ILookupItem> result = new List<ILookupItem>();
            for (var i = 0; i < numberOfItems; i++)
            {
                result.Add(ReSharperMockUtils.MockLookupItem());
            }
            return result;
        }

        private CompletionEvent GetSinglePublishedCompletionEvent()
        {
            Assert.AreEqual(1, _publishedEvents.Count);
            return GetLastPublishedCompletionEvent();
        }

        private CompletionEvent GetLastPublishedCompletionEvent()
        {
            var @event = _publishedEvents.Last();
            Assert.IsInstanceOf(typeof (CompletionEvent), @event);
            return @event as CompletionEvent;
        }
    }
}