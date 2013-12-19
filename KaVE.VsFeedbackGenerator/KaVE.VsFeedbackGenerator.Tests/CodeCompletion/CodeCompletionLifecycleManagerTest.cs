using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.VsFeedbackGenerator.CodeCompletion;
using KaVE.VsFeedbackGenerator.Tests.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class CodeCompletionLifecycleManagerTest
    {
        private const string EnterActionId = "TextControl.Enter";
        private const string TabActionId = "TextControl.Tab";
        private const string ForceCompleteActionId = "ForceCompleteItem";

        private Mock<IExtendedLookup> _mockLookup;
        private Mock<IExtendedLookupWindowManager> _mockLookupWindowManager;
        private CodeCompletionLifecycleManager _manager;
        private Mock<IActionManager> _mockActionManager;

        [SetUp]
        public void SetUpLookupEnvironment()
        {
            SetUpLookup();
            _mockActionManager = SetUpActionManager();
            _manager = new CodeCompletionLifecycleManager(_mockLookupWindowManager.Object, _mockActionManager.Object);
        }

        private void SetUpLookup()
        {
            _mockLookup = new Mock<IExtendedLookup>();
            _mockLookupWindowManager = new Mock<IExtendedLookupWindowManager>();
            _mockLookupWindowManager.Setup(m => m.CurrentLookup).Returns(_mockLookup.Object);
        }

        private static Mock<IActionManager> SetUpActionManager()
        {
            var mockActionManager = new Mock<IActionManager>();
            mockActionManager.SetupExecutableAction(ForceCompleteActionId);
            mockActionManager.SetupExecutableAction(EnterActionId);
            mockActionManager.SetupExecutableAction(TabActionId);
            return mockActionManager;
        }

        [Test]
        public void ShouldFireTriggeredWhenLookupWindowIsAboutToOpen()
        {
            const string expectedPrefix = "testPrefix";
            _mockLookup.Setup(l => l.Prefix).Returns(() => expectedPrefix);
            string actualPrefix = null;
            _manager.OnTriggered += prefix => actualPrefix = prefix;

            WhenBeforeLookupWindowShownIsRaised();

            Assert.AreEqual(expectedPrefix, actualPrefix);
        }

        [Test]
        public void ShouldFireOpenedWhenLookupItemsBecomeAvailable()
        {
            var expectedLookupItems = ReSharperMockUtils.MockLookupItemList(4);
            IEnumerable<ILookupItem> actualLookupItems = null;
            _manager.OnOpened += items => actualLookupItems = items;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(expectedLookupItems);

            CollectionAssert.AreEqual(expectedLookupItems, actualLookupItems);
        }

        [Test]
        public void ShouldFireSelectionChangedWhenCurrentItemChanges()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            IList<ILookupItem> actualSelectedItems = new List<ILookupItem>();
            _manager.OnSelectionChanged += actualSelectedItems.Add;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, actualSelectedItems.Count);
            Assert.AreEqual(lookupItems[0], actualSelectedItems[0]);
        }

        [Test]
        public void ShouldFireSelectionChangedMultipleTimesWhenCurrentItemChangesMultipleTimes()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            IList<ILookupItem> actualSelectedItems = new List<ILookupItem>();
            _manager.OnSelectionChanged += actualSelectedItems.Add;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);
            WhenCurrentItemChangedIsRaised(lookupItems[1]);

            Assert.AreEqual(2, actualSelectedItems.Count);
            Assert.AreEqual(lookupItems[0], actualSelectedItems[0]);
            Assert.AreEqual(lookupItems[1], actualSelectedItems[1]);
        }

        [Test]
        public void ShouldNotFireSelectionChangedMultipleTimesWhenCurrentItemStaysTheSame()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            IList<ILookupItem> actualSelectedItems = new List<ILookupItem>();
            _manager.OnSelectionChanged += actualSelectedItems.Add;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, actualSelectedItems.Count);
        }

        [Test]
        public void ShouldFireClosedWhenLookupIsClosed()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            _manager.OnClosed += () => invocations++;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenClosedIsRaised();

            Assert.AreEqual(1, invocations);
        }

        [Test]
        public void ShouldFireCancelledByUnknownTriggerAfterLookupIsClosedWhenItIsNotAppliedWithin10Seconds()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default (IDEEvent.Trigger);
            _manager.OnCancelled += trigger =>
            {
                invocations++;
                detectedTrigger = trigger;
            };

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenClosedIsRaised();

            Thread.Sleep(11000);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Unknown, detectedTrigger);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerDefault()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default(IDEEvent.Trigger);
            ILookupItem actualAppliedItem = null;
            _manager.OnApplied += (trigger, item) =>
            {
                invocations++;
                detectedTrigger = trigger;
                actualAppliedItem = item;
            };

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenClosedIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Typing, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerClick()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default(IDEEvent.Trigger);
            ILookupItem actualAppliedItem = null;
            _manager.OnApplied += (trigger, item) =>
            {
                invocations++;
                detectedTrigger = trigger;
                actualAppliedItem = item;
            };

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenClosedIsRaised();
            _mockLookup.Raise(l => l.MouseDown += null, this, null);
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Click, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerEnter()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default(IDEEvent.Trigger);
            ILookupItem actualAppliedItem = null;
            _manager.OnApplied += (trigger, item) =>
            {
                invocations++;
                detectedTrigger = trigger;
                actualAppliedItem = item;
            };

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            // TODO trigger enter action
            WhenClosedIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Shortcut, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerTab()
        {
            var lookupItems = ReSharperMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default(IDEEvent.Trigger);
            ILookupItem actualAppliedItem = null;
            _manager.OnApplied += (trigger, item) =>
            {
                invocations++;
                detectedTrigger = trigger;
                actualAppliedItem = item;
            };

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            // TODO trigger tab action
            WhenClosedIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Shortcut, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        private void WhenItemCompletedIsRaised(ILookupItem appliedItem)
        {
            _mockLookup.Raise(l => l.ItemCompleted += null, this, appliedItem, null, null);
        }

        private void WhenClosedIsRaised()
        {
            _mockLookup.Raise(l => l.Closed += null, EventArgs.Empty);
        }

        private void WhenCurrentItemChangedIsRaised(ILookupItem selectedItem)
        {
            _mockLookup.Setup(l => l.SelectedItem).Returns(selectedItem);
            _mockLookup.Raise(l => l.CurrentItemChanged += null, EventArgs.Empty);
        }

        private void WhenBeforeShownItemsUpdatedIsRaised(IEnumerable<ILookupItem> expectedlookupItems)
        {
            var eventArg =
                expectedlookupItems.Select(item => new Pair<ILookupItem, MatchingResult>(item, new MatchingResult()))
                    .AsIList();
            _mockLookup.Raise(l => l.BeforeShownItemsUpdated += null, this, eventArg);
        }

        private void WhenBeforeLookupWindowShownIsRaised()
        {
            _mockLookupWindowManager.Raise(m => m.BeforeLookupWindowShown += null, EventArgs.Empty);
        }
    }

    internal static class ActionManagerMockExtensions
    {
        public static void SetupExecutableAction(this Mock<IActionManager> manager, string actionId)
        {
            // TODO configure triggerable action mocks...
            manager.Setup(m => m.GetExecutableAction(actionId)).Returns(new Mock<IExecutableAction>().Object);
        }
    }
}