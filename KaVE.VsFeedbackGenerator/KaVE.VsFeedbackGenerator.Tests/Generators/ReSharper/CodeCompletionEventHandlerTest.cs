using System.Collections.Generic;
using System.Linq;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.Util;
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
    class CodeCompletionEventHandlerTest : EventGeneratorTestBase
    {
        private IIDESession _testSession;
        private CodeCompletionEventHandler _generator;

        [SetUp]
        public void SetupMockEnvironment()
        {
            _testSession = new TestIDESession();
            _generator = new CodeCompletionEventHandler(_testSession, TestMessageBus);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithApplication()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(3);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Click, lookupItems[0]);

            var ce = GetSinglePublishedEventAs<CompletionEvent>();
            Assert.AreEqual(CompletionEvent.TerminationState.Applied, ce.TerminatedAs);
        }

        [Test]
        public void ShouldAddSelectionChangesToCompletionEvent()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(5);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandleSelectionChanged(lookupItems[3]);
            _generator.HandleSelectionChanged(lookupItems[2]);
            _generator.HandleSelectionChanged(lookupItems[1]);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Typing, lookupItems[1]);

            var ce = GetSinglePublishedEventAs<CompletionEvent>();
            Assert.AreEqual(3, ce.Selections.Count);
            Assert.AreEqual(lookupItems[3].ToProposal(), ce.Selections[0].Proposal);
            Assert.AreEqual(lookupItems[2].ToProposal(), ce.Selections[1].Proposal);
            Assert.AreEqual(lookupItems[1].ToProposal(), ce.Selections[2].Proposal);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithCancellation()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Shortcut);

            var ce = GetSinglePublishedEventAs<CompletionEvent>();
            Assert.AreEqual(CompletionEvent.TerminationState.Cancelled, ce.TerminatedAs);
        }

        [Test]
        public void ShouldFireEventOnFilter()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(0);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);

            var ce = GetSinglePublishedEventAs<CompletionEvent>();
            Assert.AreEqual(CompletionEvent.TerminationState.Filtered, ce.TerminatedAs);
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TerminatedBy);
        }

        [Test]
        public void ShouldCreateFollowupEventAfterFiltering()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(0);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublishedEventAs<CompletionEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TriggeredBy);
            Assert.AreEqual("a", ce.Prefix);
        }

        [Test]
        public void ShouldDuplicateLastSelectionToFollowupEventOnFiltering()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublishedEventAs<CompletionEvent>();
            Assert.AreEqual(1, ce.Selections.Count);
            Assert.AreEqual(lookupItems[0].ToProposal(), ce.Selections[0].Proposal);
        }

        [Test]
        public void ShouldNotDuplicateLastSelectionToFollowupEventOnFilteringIfLastSelectionWasFiltered()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandlePrefixChanged("a", new List<ILookupItem>());
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublishedEventAs<CompletionEvent>();
            Assert.IsTrue(ce.Selections.IsEmpty());
        }

        [Test]
        public void ShouldNotAddSelectionToFollowupEventOnFilteringIfThereWasNoSelectionBefore()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("");
            _generator.HandleOpened(lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublishedEventAs<CompletionEvent>();
            Assert.IsTrue(ce.Selections.IsEmpty());
        }
    }
}