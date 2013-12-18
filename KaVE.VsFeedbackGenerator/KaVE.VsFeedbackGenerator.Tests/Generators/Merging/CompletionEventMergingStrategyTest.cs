using System;
using System.Linq;
using System.Security.Cryptography;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Generators.Merging;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.Merging
{
    [TestFixture]
    class CompletionEventMergingStrategyTest
    {
        private CompletionEventMergingStrategy _strategy;
        private ProposalCollection _proposalCollection;
        private CompletionEvent _event;
        private CompletionEvent _subsequentEvent;

        /// <summary>
        /// Creates two <see cref="CompletionEvent"/>s and sets all properties that are not considered by the merging
        /// strategy.
        /// </summary>
        [SetUp]
        public void SetUpSubsequentCompletionEvents()
        {
            _proposalCollection = ReSharperMockUtils.MockLookupItemList(5).ToProposalCollection();
            var now = DateTime.Now;
            var activeWindow = WindowName.Get("testWindow");
            var activeDocument = DocumentName.Get("testDocument");
            var context = new Context();

            _event = new CompletionEvent
            {
                IDESessionUUID = "TestUUID",
                ActiveWindow = activeWindow,
                ActiveDocument = activeDocument,
                TriggeredAt = now,
                Prefix = "",
                Context = context,
                ProposalCollection = _proposalCollection,
                TerminatedAt = now.AddMilliseconds(100),
            };

            _subsequentEvent = new CompletionEvent
            {
                IDESessionUUID = "TestUUID",
                ActiveWindow = activeWindow,
                ActiveDocument = activeDocument,
                TriggeredAt = now.AddMilliseconds(200),
                Prefix = "a",
                Context = context,
                ProposalCollection = new ProposalCollection(_proposalCollection.Proposals.Take(3)),
                TerminatedAt = now.AddMilliseconds(300),
                TerminatedAs = CompletionEvent.TerminationState.Applied,
                TerminatedBy = IDEEvent.Trigger.Shortcut,
            };
        }

        [SetUp]
        public void SetUpStrategyUnderTest()
        {
            _strategy = new CompletionEventMergingStrategy();
        }

        [Test]
        public void ShouldMergeTwoSubsequentAutomaticFilterEventsWithoutInteractions()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        /// <summary>
        /// The first event from a completion process should always be kept, in order to keep the invocation state.
        /// </summary>
        [TestCase(IDEEvent.Trigger.Shortcut)]
        [TestCase(IDEEvent.Trigger.Typing)]
        [TestCase(IDEEvent.Trigger.Unknown)]
        [TestCase(IDEEvent.Trigger.Click)]
        public void ShouldNotMergeIfEarlierEventIsTriggeredByShortcut(IDEEvent.Trigger earlierEventTrigger)
        {
            _event.TriggeredBy = earlierEventTrigger;
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldMergeIfEarlierEventContainsOnlyInitialSelection()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.AddSelection(_proposalCollection.Proposals[4]);
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldNotMergeIfEarlierEventContainsMultipleSelections()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.AddSelection(_proposalCollection.Proposals[4]);
            _event.AddSelection(_proposalCollection.Proposals[3]);
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldMergeDespiteSubsequentEventContainingMultipleInteractions()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.TriggeredBy = IDEEvent.Trigger.Automatic;
            _subsequentEvent.AddSelection(_proposalCollection.Proposals[0]);
            _subsequentEvent.AddSelection(_proposalCollection.Proposals[1]);

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [TestCase(CompletionEvent.TerminationState.Applied)]
        [TestCase(CompletionEvent.TerminationState.Cancelled)]
        public void ShouldNotMergeIfEarlierEventWasntTerminatedByFiltering(CompletionEvent.TerminationState earlierEventTerminatedAs)
        {
            _event.TerminatedAs = earlierEventTerminatedAs;

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldTakeInitialPropertiesFromEarlierAndTerminationPropertiesFromSubsequentEventOnMerge()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.TerminatedAs = CompletionEvent.TerminationState.Filtered;
            _event.TerminatedBy = IDEEvent.Trigger.Automatic;

            _subsequentEvent.AddSelection(_proposalCollection.Proposals[1]);

            var mergedEvent = _strategy.Merge(_event, _subsequentEvent);

            Assert.IsInstanceOf(typeof(CompletionEvent), mergedEvent);
            var mergedCompletionEvent = (CompletionEvent)mergedEvent;
            // could be taken from either event, as both are equal
            Assert.AreEqual(_event.IDESessionUUID, mergedCompletionEvent.IDESessionUUID);
            Assert.AreEqual(_event.Context, mergedCompletionEvent.Context);
            Assert.AreEqual(_event.ActiveWindow, mergedCompletionEvent.ActiveWindow);
            Assert.AreEqual(_event.ActiveDocument, mergedCompletionEvent.ActiveDocument);
            // has to be taken from earlier event
            Assert.AreEqual(_event.TriggeredAt, mergedEvent.TriggeredAt);
            Assert.AreEqual(_event.TriggeredBy, mergedEvent.TriggeredBy);
            // has to be taken from subsequent event
            Assert.AreEqual(_subsequentEvent.Prefix, mergedCompletionEvent.Prefix);
            Assert.AreEqual(_subsequentEvent.ProposalCollection, mergedCompletionEvent.ProposalCollection);
            Assert.AreEqual(_subsequentEvent.Selections, mergedCompletionEvent.Selections);
            Assert.AreEqual(_subsequentEvent.TerminatedAs, mergedCompletionEvent.TerminatedAs);
            Assert.AreEqual(_subsequentEvent.TerminatedAt, mergedCompletionEvent.TerminatedAt);
            Assert.AreEqual(_subsequentEvent.TerminatedBy, mergedCompletionEvent.TerminatedBy);
        }
    }
}
