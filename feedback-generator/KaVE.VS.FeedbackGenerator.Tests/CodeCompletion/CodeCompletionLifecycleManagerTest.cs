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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.LookupItems;
using JetBrains.ReSharper.Feature.Services.CodeCompletion.Infrastructure.Match;
using JetBrains.UI.ActionsRevised;
using JetBrains.UI.ActionsRevised.Handlers;
using JetBrains.UI.ActionsRevised.Loader;
using JetBrains.UI.ActionSystem.Text;
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.CodeCompletion;
using KaVE.VS.FeedbackGenerator.Tests.TestFactories;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.CodeCompletion
{
    [TestFixture]
    internal class CodeCompletionLifecycleManagerTest
    {
        private const string EnterActionId = TextControlActions.ENTER_ACTION_ID;
        private const string TabActionId = TextControlActions.TAB_ACTION_ID;
        private const string ForceCompleteActionId = "ForceCompleteItem";

        private Mock<IExtendedLookup> _mockLookup;
        private Mock<IExtendedLookupWindowManager> _mockLookupWindowManager;
        private CodeCompletionLifecycleManager _manager;
        private Mock<IActionManager> _mockActionManager;
        private IActionDefs _actionDefs;
        private IActionHandlers _actionHandlers;

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
            _mockLookup.Setup(l => l.DisplayedItems).Returns(LookupItemsMockUtils.MockLookupItemList(3));
            _mockLookupWindowManager = new Mock<IExtendedLookupWindowManager>();
            _mockLookupWindowManager.Setup(m => m.CurrentLookup).Returns(_mockLookup.Object);
        }

        private Mock<IActionManager> SetUpActionManager()
        {
            var am = Mock.Of<IActionManager>();
            var mockActionManager = Mock.Get(am);
            _actionDefs = Mock.Of<IActionDefs>();
            _actionHandlers = Mock.Of<IActionHandlers>();

            mockActionManager.Setup(m => m.Defs).Returns(_actionDefs);
            mockActionManager.Setup(m => m.Handlers).Returns(_actionHandlers);

            Dictionary<IActionDefWithId, IAction> registeredHandlers = new Dictionary<IActionDefWithId, IAction>();

            // store new handlers in handlers dictionary together with their actions
            Mock.Get(_actionHandlers)
                .Setup(ah => ah.AddHandler(It.IsAny<IActionDefWithId>(), It.IsAny<IAction>()))
                .Callback<IActionDefWithId, IAction>(
                    (id, action) => { registeredHandlers.Add(id, action); });
            
            // mock the execution of registered actions
            Mock.Get(_actionHandlers)
                .Setup(ah => ah.Evaluate(It.IsAny<IActionDefWithId>(), It.IsAny<IDataContext>()))
                .Callback<IActionDefWithId, IDataContext>(
                    (id, context) =>
                    {
                        var action = registeredHandlers[id];
                        var delAction = action as KaVEDelegateActionHandler;
                        if (delAction != null)
                        {
                            // TODO RS9: use "Execute" and make Action private again... no idea how to mock the "chaining" to the next delegate though
                            delAction.Action();
                        }
                    });

            SetupExecutableAction(am, _actionDefs, ForceCompleteActionId);
            SetupExecutableAction(am, _actionDefs, EnterActionId);
            SetupExecutableAction(am, _actionDefs, TabActionId);

            return mockActionManager;
        }

        public void SetupExecutableAction(IActionManager manager, IActionDefs defs, string actionId)
        {
            var actionDef = Mock.Of<IActionDefWithId>();
            Mock.Get(defs).Setup(d => d.GetActionDefById(actionId)).Returns(actionDef);
        }

        [Test]
        public void ShouldFireTriggeredWhenLookupWindowIsAboutToOpen()
        {
            const string expectedPrefix = "testPrefix";
            _mockLookup.Setup(l => l.Prefix).Returns(() => expectedPrefix);
            string actualPrefix = null;
            IEnumerable<ILookupItem> actualItems = null;
            _manager.OnTriggered += (prefix, items) =>
            {
                actualPrefix = prefix;
                actualItems = items;
            };

            WhenBeforeLookupWindowShownIsRaised();

            Assert.AreEqual(expectedPrefix, actualPrefix);
            CollectionAssert.AreEqual(_mockLookup.Object.DisplayedItems, actualItems);
        }

        [Test]
        public void ShouldFireDisplayedItemsUpdatedWhenLookupItemsBecomeAvailable()
        {
            var expectedLookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            IEnumerable<ILookupItem> actualLookupItems = null;
            _manager.DisplayedItemsUpdated += items => actualLookupItems = items;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(expectedLookupItems);

            CollectionAssert.AreEqual(expectedLookupItems, actualLookupItems);
        }

        [Test]
        public void ShouldFireSelectionChangedWhenCurrentItemChanges()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            IList<ILookupItem> actualSelectedItems = new List<ILookupItem>();
            _manager.OnSelectionChanged += actualSelectedItems.Add;

            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, actualSelectedItems.Count);
        }

        [Test]
        public void ShouldFirePrefixChangedIfPrefixChangedAndCurrentItemChangeIsRaised()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            string actualPrefix = null;
            IEnumerable<ILookupItem> actualItems = null;
            _manager.OnPrefixChanged += (prefix, items) =>
            {
                actualPrefix = prefix;
                actualItems = items;
            };

            _mockLookup.Setup(l => l.Prefix).Returns(() => "set");
            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);
            const string expectedPrefix = "setM";
            _mockLookup.Setup(l => l.Prefix).Returns(() => expectedPrefix);
            var expectedItems = lookupItems.Take(2).ToList();
            _mockLookup.Setup(l => l.DisplayedItems).Returns(expectedItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);

            Assert.AreEqual(expectedPrefix, actualPrefix);
            CollectionAssert.AreEqual(expectedItems, actualItems);
        }

        [Test]
        public void ShouldNotFirePrefixChangedIfPrefixDidNotChangedAndCurrentItemChangeIsRaised()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            var invokations = 0;
            _manager.OnPrefixChanged += (prefix, items) => invokations++;

            _mockLookup.Setup(l => l.Prefix).Returns(() => "set");
            WhenBeforeLookupWindowShownIsRaised();
            WhenBeforeShownItemsUpdatedIsRaised(lookupItems);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);
            WhenCurrentItemChangedIsRaised(lookupItems[0]);

            Assert.AreEqual(0, invokations);
        }

        [Test]
        public void ShouldFireClosedWhenLookupIsClosed()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
            var invocations = 0;
            var detectedTrigger = default(IDEEvent.Trigger);
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
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            WhenMouseDownIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Click, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerClickWithSelectionChange()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            WhenMouseDownIsRaised();
            WhenCurrentItemChangedIsRaised(lookupItems[1]);
            WhenItemCompletedIsRaised(lookupItems[1]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Click, detectedTrigger);
            Assert.AreEqual(lookupItems[1], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerEnter()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            WhenActionIsExecuted(EnterActionId);
            WhenClosedIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Shortcut, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        [Test]
        public void ShouldFireAppliedWhenItemIsCompletedAfterClose_TriggerTab()
        {
            var lookupItems = LookupItemsMockUtils.MockLookupItemList(4);
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
            WhenActionIsExecuted(TabActionId);
            WhenClosedIsRaised();
            WhenItemCompletedIsRaised(lookupItems[0]);

            Assert.AreEqual(1, invocations);
            Assert.AreEqual(IDEEvent.Trigger.Shortcut, detectedTrigger);
            Assert.AreEqual(lookupItems[0], actualAppliedItem);
        }

        private void WhenActionIsExecuted(string actionId)
        {
            var def = _mockActionManager.Object.Defs.GetActionDefById(actionId);
            _mockActionManager.Object.Handlers.Evaluate(def, Mock.Of<IDataContext>());
        }

        private void WhenItemCompletedIsRaised(ILookupItem appliedItem)
        {
            _mockLookup.Raise(l => l.ItemCompleted += null, this, appliedItem, null, null);
        }

        private void WhenClosedIsRaised()
        {
            _mockLookup.Raise(l => l.Closed += null, EventArgs.Empty);
        }

        private void WhenMouseDownIsRaised()
        {
            _mockLookup.Raise(l => l.MouseDown += null, this, null);
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
}