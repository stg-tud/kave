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

using System.Collections.Generic;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Generators.ReSharper;
using KaVE.VS.FeedbackGenerator.Tests.TestFactories;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.ReSharper
{
    internal class CodeCompletionEventHandlerTest : EventGeneratorTestBase
    {
        private CodeCompletionEventHandler _generator;

        [SetUp]
        public void SetupMockEnvironment()
        {
            _generator = new CodeCompletionEventHandler(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithApplication()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(3);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Click, lookupItems[0]);

            var ce = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(TerminationState.Applied, ce.TerminatedState);
        }

        [Test]
        public void ShouldAddSelectionChangesToCompletionEvent()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(5);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleSelectionChanged(lookupItems[3]);
            _generator.HandleSelectionChanged(lookupItems[2]);
            _generator.HandleSelectionChanged(lookupItems[1]);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Typing, lookupItems[1]);

            var ce = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(3, ce.Selections.Count);
            Assert.AreEqual(lookupItems[3].ToProposal(), ce.Selections[0].Proposal);
            Assert.AreEqual(lookupItems[2].ToProposal(), ce.Selections[1].Proposal);
            Assert.AreEqual(lookupItems[1].ToProposal(), ce.Selections[2].Proposal);
        }

        [Test]
        public void ShouldHandleCodeCompletionWithCancellation()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Shortcut);

            var ce = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(TerminationState.Cancelled, ce.TerminatedState);
        }

        [Test]
        public void ShouldFireEventOnFilter()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(0);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);

            var ce = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(TerminationState.Filtered, ce.TerminatedState);
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TerminatedBy);
        }

        [Test]
        public void ShouldCreateFollowupEventAfterFiltering()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(0);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublished<CompletionEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Automatic, ce.TriggeredBy);
        }

        [Test]
        public void ShouldDuplicateLastSelectionToFollowupEventOnFiltering()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublished<CompletionEvent>();
            Assert.AreEqual(1, ce.Selections.Count);
            Assert.AreEqual(lookupItems[0].ToProposal(), ce.Selections[0].Proposal);
        }

        [Test]
        public void ShouldNotDuplicateLastSelectionToFollowupEventOnFilteringIfLastSelectionWasFiltered()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleSelectionChanged(lookupItems[0]);
            _generator.HandlePrefixChanged("a", new List<ILookupItem>());
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublished<CompletionEvent>();
            Assert.IsTrue(ce.Selections.IsEmpty());
        }

        [Test]
        public void ShouldNotAddSelectionToFollowupEventOnFilteringIfThereWasNoSelectionBefore()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(1);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandlePrefixChanged("a", lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublished<CompletionEvent>();
            Assert.IsTrue(ce.Selections.IsEmpty());
        }

        [Test]
        public void ShouldOverrideProposalCollectionIfDisplayedItemsAreUpdated()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            var expected = lookupItems.ToProposalCollection();

            _generator.HandleTriggered("", LookupItemsMockUtils.MockLookupItemList(1));
            _generator.HandleDisplayedItemsChanged(lookupItems);
            _generator.HandleClosed();
            _generator.HandleCancelled(IDEEvent.Trigger.Click);

            var ce = GetLastPublished<CompletionEvent>();
            Assert.AreEqual(expected, ce.ProposalCollection);
        }

        [TestCase(13, TestName = "Less proposals than transformation limit"), TestCase(1000, TestName = "More proposals than transformation limit")]
        public void ShouldSetProposalCount(int testProposalCount)
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(testProposalCount);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Typing, lookupItems[1]);

            var completionEvent = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(testProposalCount, completionEvent.ProposalCount);
        }

        [Test]
        public void ShouldSetIndexOfSelections()
        {
            const int testIndex = 4;
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(5);

            _generator.HandleTriggered("", lookupItems);
            _generator.HandleSelectionChanged(lookupItems[testIndex]);
            _generator.HandleClosed();
            _generator.HandleApplied(IDEEvent.Trigger.Typing, lookupItems[1]);

            var actualEvent = GetSinglePublished<CompletionEvent>();
            Assert.AreEqual(testIndex, actualEvent.Selections[0].Index);
        }
    }
}