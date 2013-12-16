using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.Util;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [Language(typeof (CSharpLanguage))]
    public class CodeCompletionLifecycleManager : IDisposable
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly IActionManager _actionManager;
        private readonly ICodeCompletionLifecycleHandler _handler;

        private ILookup _currentLookup;
        private IDEEvent.Trigger? _terminationTrigger;
        private ScheduledAction _finishLifecycleAction = ScheduledAction.NoOp;
        private string _initialPrefix;
        private string _currentPrefix;
        private ILookupItem _currentSelection;

        public CodeCompletionLifecycleManager(ILookupWindowManager lookupWindowManager,
            IActionManager actionManager,
            IIDESession session,
            IMessageBus messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _actionManager = actionManager;
            _handler = new CodeCompletionEventHandler(session, messageBus);

            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
        }

        /// <summary>
        /// Invoked when a completion starts.
        /// </summary>
        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            FinishLifecycle();
            _initialPrefix = _lookupWindowManager.CurrentLookup.Prefix;
            _currentPrefix = _initialPrefix;
            _handler.OnOpened(_initialPrefix);
            _terminationTrigger = null;
            RegisterToLookupEvents();
        }

        private void RegisterToLookupEvents()
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            var lifetime = _currentLookup.GetLifetime();

            // Event handlers are registered in order of the actual events's occurence

            // _lookupWindowManager.AfterLookupWindowShown is fired immediately after the window pops up,
            // i.e., after this method finished and before any other event is fired.
            _currentLookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;

            _currentLookup.CurrentItemChanged += OnCurrentItemChanged;

            // R# actions that lead to the completion being finished
            _actionManager.GetExecutableAction("ForceCompleteItem")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Typing));
            _actionManager.GetExecutableAction("TextControl.Enter")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut));
            _actionManager.GetExecutableAction("TextControl.Tab")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut));
            var lookupListBox = _currentLookup.Window.GetLookupListBox();
            lookupListBox.MouseDown += LookupListBox_OnMouseDown;

            _currentLookup.Closed += OnClosed;
            // _lookupWindowManager.LookupWindowClosed is fired here
            _currentLookup.ItemCompleted += OnItemCompleted;
        }

        /// <summary>
        /// Invoked when the lookup items are computed, before they are shown.
        /// </summary>
        private void OnBeforeShownItemsUpdated(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            _handler.SetLookupItems(items.Select(pair => pair.First));
        }

        /// <summary>
        /// Invoked when the selection changes, either by pressing the arrow keys, or as a result of filtering, or
        /// because the user clicked an item (in which case the completion is closed and completed immediately
        /// after the selection change).
        /// </summary>
        private void OnCurrentItemChanged(object sender, EventArgs eventArgs)
        {
            MaybeHandlePrefixChange();
            MaybeHandleSelectionChange();
            _terminationTrigger = null;
        }

        private void MaybeHandlePrefixChange()
        {
            var newPrefix = _currentLookup.Prefix;
            if (_currentPrefix != newPrefix)
            {
                var lookupItems = _lookupWindowManager.GetDisplayedLookupItems();
                _handler.OnPrefixChanged(newPrefix, lookupItems);
                _currentPrefix = newPrefix;
            }
            _terminationTrigger = null;
        }

        private void MaybeHandleSelectionChange()
        {
            var selectedItem = _lookupWindowManager.CurrentLookup.Selection.Item;
            if (_currentSelection != selectedItem)
            {
                _handler.OnSelectionChanged(selectedItem);
                _currentSelection = selectedItem;
            }
        }

        /// <summary>
        /// MouseDown is fired before Closed, as opposed to Click. Therefore, we listen on MouseDown here and assume
        /// that it will be followed by a MouseUp and, thereby, terminate the completion. This assumption leads to the
        /// following scenario where termination-trigger detection fails: When the mouse is pressed over the lookup,
        /// moved to somewhere else, and released there, and before any other interaction with the lookup the same is
        /// cancelled by something else than a click or the escape button (focus change or ?), the trigger says "Click".
        /// </summary>
        private void LookupListBox_OnMouseDown(object sender, EventArgs eventArgs)
        {
            _terminationTrigger = IDEEvent.Trigger.Click;
        }

        /// <summary>
        /// Invoked when the completion is closed, regardless of whether an item was applied or the completion was
        /// cancelled. In the former case, invocation occurs before that of <see cref="OnItemCompleted"/>. Therefore,
        /// it defers the notification of the handler(s) for some time, to make sure any <see cref="OnItemCompleted"/>
        /// event arives first.
        /// </summary>
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            CaptureTerminationTrigger();

            _handler.OnClosed();

            _finishLifecycleAction = Invoke.Later(
                () =>
                {
                    if (_terminationTrigger.HasValue)
                    {
                        _handler.SetTerminatedBy(_terminationTrigger.Value);
                    }
                    _handler.OnFinished();
                },
                10000);

            UnregisterFromLookupEvents();
        }

        private void CaptureTerminationTrigger()
        {
            if (Key.Escape.IsPressed())
            {
                _terminationTrigger = IDEEvent.Trigger.Shortcut;
            }
        }

        private void UnregisterFromLookupEvents()
        {
            _currentLookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged -= OnCurrentItemChanged;
            _currentLookup.Closed -= OnClosed;
            _currentLookup.ItemCompleted -= OnItemCompleted;

            var lookupListBox = _currentLookup.Window.GetLookupListBox();
            lookupListBox.MouseDown -= LookupListBox_OnMouseDown;
        }

        private void OnItemCompleted(object sender,
            ILookupItem lookupitem,
            Suffix suffix,
            LookupItemInsertType lookupiteminserttype)
        {
            _handler.OnApplication(lookupitem);
        }

        public void Dispose()
        {
            FinishLifecycle();
        }

        private void FinishLifecycle()
        {
            _finishLifecycleAction.RunNow();
        }
    }
}