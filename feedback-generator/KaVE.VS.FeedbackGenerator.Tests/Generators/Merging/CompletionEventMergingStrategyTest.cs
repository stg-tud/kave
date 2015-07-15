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
 */

using System;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events.CompletionEvent;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators.Merging;
using KaVE.VS.FeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Merging
{
    internal class CompletionEventMergingStrategyTest
    {
        private CompletionEventMergingStrategy _strategy;
        private ProposalCollection _proposalCollection;
        private CompletionEvent _event;
        private CompletionEvent _subsequentEvent;

        /// <summary>
        ///     Creates two <see cref="CompletionEvent" />s and sets all properties that are not considered by the merging
        ///     strategy.
        /// </summary>
        [SetUp]
        public void SetUpSubsequentCompletionEvents()
        {
            _proposalCollection = LookupItemsMockUtils.MockLookupItemList(5).ToProposalCollection();
            var now = DateTime.Now;
            var activeWindow = WindowName.Get("testWindow");
            var activeDocument = DocumentName.Get("testDocument");

            _event = new CompletionEvent
            {
                IDESessionUUID = "TestUUID",
                ActiveWindow = activeWindow,
                ActiveDocument = activeDocument,
                TriggeredAt = now,
                Prefix = "",
                ProposalCollection = _proposalCollection,
                TerminatedAt = now.AddMilliseconds(100)
            };

            _subsequentEvent = new CompletionEvent
            {
                IDESessionUUID = "TestUUID",
                ActiveWindow = activeWindow,
                ActiveDocument = activeDocument,
                TriggeredAt = now.AddMilliseconds(200),
                Prefix = "a",
                ProposalCollection = new ProposalCollection(_proposalCollection.Proposals.Take(3).ToList()),
                TerminatedAt = now.AddMilliseconds(300),
                TerminatedState = TerminationState.Applied,
                TerminatedBy = IDEEvent.Trigger.Shortcut
            };
        }

        [SetUp]
        public void SetUpStrategyUnderTest()
        {
            _strategy = new CompletionEventMergingStrategy();
        }

        [Test]
        public void ShouldMergeIfEarlierEventIsTriggeredAutomaticallyAndTerminatedByFiltering()
        {
            GivenEventsMeetMergeConditions();

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        /// <summary>
        ///     The first event from a completion process should always be kept, in order to keep the invocation state.
        /// </summary>
        [TestCase(IDEEvent.Trigger.Shortcut), TestCase(IDEEvent.Trigger.Typing), TestCase(IDEEvent.Trigger.Unknown),
         TestCase(IDEEvent.Trigger.Click)]
        public void ShouldNotMergeIfEarlierEventIsTriggeredByShortcut(IDEEvent.Trigger earlierEventTrigger)
        {
            GivenEventsMeetMergeConditions();
            _event.TriggeredBy = earlierEventTrigger;

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldMergeIfEarlierEventContainsOnlyInitialSelection()
        {
            GivenEventsMeetMergeConditions();
            _event.AddSelection(_proposalCollection.Proposals[4]);

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldNotMergeIfEarlierEventContainsMultipleSelections()
        {
            GivenEventsMeetMergeConditions();
            _event.AddSelection(_proposalCollection.Proposals[4]);
            _event.AddSelection(_proposalCollection.Proposals[3]);

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldMergeDespiteSubsequentEventContainingMultipleInteractions()
        {
            GivenEventsMeetMergeConditions();
            _subsequentEvent.AddSelection(_proposalCollection.Proposals[0]);
            _subsequentEvent.AddSelection(_proposalCollection.Proposals[1]);

            Assert.IsTrue(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [TestCase(TerminationState.Applied), TestCase(TerminationState.Cancelled)]
        public void ShouldNotMergeIfEarlierEventWasntTerminatedByFiltering(TerminationState earlierEventTerminatedAs)
        {
            GivenEventsMeetMergeConditions();
            _event.TerminatedState = earlierEventTerminatedAs;

            Assert.IsFalse(_strategy.AreMergable(_event, _subsequentEvent));
        }

        [Test]
        public void ShouldTakeInitialPropertiesFromEarlierAndTerminationPropertiesFromSubsequentEventOnMerge()
        {
            GivenEventsMeetMergeConditions();

            _subsequentEvent.AddSelection(_proposalCollection.Proposals[1]);

            var mergedEvent = _strategy.Merge(_event, _subsequentEvent);

            Assert.IsInstanceOf(typeof (CompletionEvent), mergedEvent);
            var mergedCompletionEvent = (CompletionEvent) mergedEvent;
            // could be taken from either event, as both are equal
            Assert.AreEqual(_event.IDESessionUUID, mergedCompletionEvent.IDESessionUUID);
            Assert.AreEqual(_event.Context2, mergedCompletionEvent.Context2);
            Assert.AreEqual(_event.ActiveWindow, mergedCompletionEvent.ActiveWindow);
            Assert.AreEqual(_event.ActiveDocument, mergedCompletionEvent.ActiveDocument);
            // has to be taken from earlier event
            Assert.AreEqual(_event.TriggeredAt, mergedEvent.TriggeredAt);
            Assert.AreEqual(_event.TriggeredBy, mergedEvent.TriggeredBy);
            // has to be taken from subsequent event
            Assert.AreEqual(_subsequentEvent.Prefix, mergedCompletionEvent.Prefix);
            Assert.AreEqual(_subsequentEvent.ProposalCollection, mergedCompletionEvent.ProposalCollection);
            Assert.AreEqual(_subsequentEvent.TerminatedState, mergedCompletionEvent.TerminatedState);
            Assert.AreEqual(_subsequentEvent.TerminatedAt, mergedCompletionEvent.TerminatedAt);
            Assert.AreEqual(_subsequentEvent.TerminatedBy, mergedCompletionEvent.TerminatedBy);
            // Selections is recomputed, see ShouldRebaseSelectionOffsetsOnMerge
        }

        [Test]
        public void ShouldRebaseSelectionOffsetsOnMerge()
        {
            GivenEventsMeetMergeConditions();
            _event.TriggeredAt = DateTime.Now;

            _subsequentEvent.TriggeredAt = _event.TriggeredAt.Value.AddSeconds(4);
            _subsequentEvent.Selections.Add(
                new ProposalSelection(CompletionEventTestFactory.CreateAnonymousProposal())
                {
                    SelectedAfter = TimeSpan.FromSeconds(3)
                });
            _subsequentEvent.Selections.Add(
                new ProposalSelection(CompletionEventTestFactory.CreateAnonymousProposal())
                {
                    SelectedAfter = TimeSpan.FromSeconds(6)
                });

            var mergedEvent = (CompletionEvent) _strategy.Merge(_event, _subsequentEvent);

            Assert.AreEqual(TimeSpan.FromSeconds(7), mergedEvent.Selections[0].SelectedAfter);
            Assert.AreEqual(TimeSpan.FromSeconds(10), mergedEvent.Selections[1].SelectedAfter);
        }

        private void GivenEventsMeetMergeConditions()
        {
            _event.TriggeredBy = IDEEvent.Trigger.Automatic;
            _event.TerminatedState = TerminationState.Filtered;
        }
    }
}