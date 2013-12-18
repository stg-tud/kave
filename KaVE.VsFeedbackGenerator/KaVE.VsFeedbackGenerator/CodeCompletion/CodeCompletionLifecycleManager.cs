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
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

namespace KaVE.VsFeedbackGenerator.CodeCompletion
{
    [Language(typeof (CSharpLanguage))]
    public class CodeCompletionLifecycleManager : IDisposable
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly IActionManager _actionManager;

        private ILookup _currentLookup;
        private IDEEvent.Trigger? _terminationTrigger;
        private ScheduledAction _delayedCancelAction = ScheduledAction.NoOp;
        private string _initialPrefix;
        private string _currentPrefix;
        private ILookupItem _currentSelection;

        public CodeCompletionLifecycleManager(ILookupWindowManager lookupWindowManager,
            IActionManager actionManager,
            IIDESession session)
        {
            _lookupWindowManager = lookupWindowManager;
            _actionManager = actionManager;

            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
        }

        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            FinishLifecycle();
            _initialPrefix = _lookupWindowManager.CurrentLookup.Prefix;
            _currentPrefix = _initialPrefix;
            OnTriggered(_initialPrefix);
            _terminationTrigger = null;
            RegisterToLookupEvents();
        }

        /// <param name="inialPrefix">The prefix present when completion is triggered.</param>
        public delegate void TriggeredHandler(string inialPrefix);

        /// <summary>
        /// Fired when the code completion is about to be opened.
        /// </summary>
        public event TriggeredHandler OnTriggered = delegate { };

        private void RegisterToLookupEvents()
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            var lifetime = _currentLookup.GetLifetime();

            // Event handlers are registered in order of the actual events's occurence

            // _lookupWindowManager.AfterLookupWindowShown is fired immediately after the window pops up,
            // i.e., after this method finished and before any other event is fired.
            _currentLookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;

            _currentLookup.CurrentItemChanged += HandleCurrentItemChanged;

            // R# actions that lead to the completion being finished
            _actionManager.GetExecutableAction("ForceCompleteItem")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Typing));
            _actionManager.GetExecutableAction("TextControl.Enter")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut));
            _actionManager.GetExecutableAction("TextControl.Tab")
                .AddHandler(lifetime, new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut));
            var lookupListBox = _currentLookup.Window.GetLookupListBox();
            lookupListBox.MouseDown += HandleMouseDown;

            _currentLookup.Closed += HandleClosed;
            // _lookupWindowManager.LookupWindowClosed is fired here
            _currentLookup.ItemCompleted += HandleItemCompleted;
        }

        /// <summary>
        /// Invoked when the lookup items are computed, before they are shown.
        /// </summary>
        private void OnBeforeShownItemsUpdated(object sender, IList<Pair<ILookupItem, MatchingResult>> items)
        {
            OnOpened(items.Select(pair => pair.First));
        }

        /// <param name="displayedItems">The items initially displayed.</param>
        public delegate void OpenedHandler(IEnumerable<ILookupItem> displayedItems);

        /// <summary>
        /// Fired when the completion opened. This may sometimes not occur, when the completion is closed more quickly
        /// than the lookup items can be calculated.
        /// </summary>
        public event OpenedHandler OnOpened = delegate {};

        /// <summary>
        /// Invoked when the selection changes, either by pressing the arrow keys, or as a result of filtering, or
        /// because the user clicked an item (in which case the completion is closed and completed immediately
        /// after the selection change).
        /// </summary>
        private void HandleCurrentItemChanged(object sender, EventArgs eventArgs)
        {
            MaybeHandlePrefixChange();
            MaybeHandleSelectionChange();
        }

        private void MaybeHandlePrefixChange()
        {
            var newPrefix = _currentLookup.Prefix;
            if (_currentPrefix != newPrefix)
            {
                var lookupItems = _lookupWindowManager.GetDisplayedLookupItems();
                _currentPrefix = newPrefix;
                _currentSelection = null;
                _terminationTrigger = null;

                OnPrefixChanged(newPrefix, lookupItems);
            }
        }

        /// <param name="newPrefix">The prefix after it was changed.</param>
        /// <param name="displayedLookupItems">The lookup items displayed after the change.</param>
        public delegate void PrefixChangedHandler(string newPrefix, IEnumerable<ILookupItem> displayedLookupItems);

        /// <summary>
        /// Fired when the prefix changes (typing or deletion of a character).
        /// </summary>
        public event PrefixChangedHandler OnPrefixChanged = delegate { }; 

        private void MaybeHandleSelectionChange()
        {
            var selectedItem = _lookupWindowManager.CurrentLookup.Selection.Item;
            if (_currentSelection != selectedItem)
            {
                _currentSelection = selectedItem;

                OnSelectionChanged(selectedItem);
            }
        }

        /// <param name="selectedItem">The item that is now selected.</param>
        public delegate void SelectionChangedHandler(ILookupItem selectedItem);

        /// <summary>
        /// Fired for the initial selection, any manual selection change (using the arrow keys), selection changes
        /// caused by filtering, and when an unselected item is clicked (which immediately applies the selected
        /// completion). Fired exactly once per selection, i.e., for two subsequent calls the lookup item passed to
        /// the handler are always different.
        /// </summary>
        public event SelectionChangedHandler OnSelectionChanged = delegate { }; 

        /// <summary>
        /// MouseDown is fired before Closed, as opposed to Click. Therefore, we listen on MouseDown here and assume
        /// that it will be followed by a MouseUp and, thereby, terminate the completion. This assumption leads to the
        /// following scenario where termination-trigger detection fails: When the mouse is pressed over the lookup,
        /// moved to somewhere else, and released there, and before any other interaction with the lookup the same is
        /// cancelled by something else than a click or the escape button (focus change or ?), the trigger says "Click".
        /// </summary>
        private void HandleMouseDown(object sender, EventArgs eventArgs)
        {
            _terminationTrigger = IDEEvent.Trigger.Click;
        }

        /// <summary>
        /// Invoked when the completion is closed, regardless of whether an item was applied or the completion was
        /// cancelled. In the former case, invocation occurs before that of <see cref="HandleItemCompleted"/>. Therefore,
        /// it defers the notification of the handler(s) for some time, to make sure any <see cref="HandleItemCompleted"/>
        /// event arives first.
        /// </summary>
        private void HandleClosed(object sender, EventArgs eventArgs)
        {
            var isEscapePressed = Key.Escape.IsPressed();
            OnClosed();
            if (isEscapePressed)
            {
                _terminationTrigger = IDEEvent.Trigger.Shortcut;
                OnCancel();
            }
            else
            {
                _delayedCancelAction = Invoke.Later(OnCancel, 10000);
            }
            UnregisterFromLookupEvents();
        }

        public delegate void ClosedHandler();

        /// <summary>
        /// Invoked when the completion is closed. This happens for every completion, regardless of whether is is
        /// applied or cancelled. One of <see cref="OnApplied"/> or <see cref="OnCancelled"/> is fired afterwards,
        /// depending on how the completion was actually closed.
        /// </summary>
        public event ClosedHandler OnClosed = delegate { };

        private void OnCancel()
        {
            OnCancelled(_terminationTrigger.GetValueOrDefault(IDEEvent.Trigger.Unknown));
        }

        /// <param name="trigger">What triggered the termination.</param>
        public delegate void CancelledHandler(IDEEvent.Trigger trigger);

        /// <summary>
        /// Fired when the code completion is cancelled.
        /// 
        /// Attention: This may be far later than the call to <see cref="OnClosed"/>.
        /// </summary>
        public event CancelledHandler OnCancelled = delegate { };

        private void UnregisterFromLookupEvents()
        {
            _currentLookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged -= HandleCurrentItemChanged;
            _currentLookup.Closed -= HandleClosed;
            _currentLookup.ItemCompleted -= HandleItemCompleted;

            var lookupListBox = _currentLookup.Window.GetLookupListBox();
            lookupListBox.MouseDown -= HandleMouseDown;
        }

        private void HandleItemCompleted(object sender,
            ILookupItem lookupitem,
            Suffix suffix,
            LookupItemInsertType lookupiteminserttype)
        {
            _delayedCancelAction.Cancel();
            OnApplied(_terminationTrigger.GetValueOrDefault(IDEEvent.Trigger.Typing), lookupitem);
        }

        /// <param name="trigger">What triggered the termination.</param>
        /// <param name="appliedItem">The item that is applied.</param>
        public delegate void AppliedHandler(IDEEvent.Trigger trigger, ILookupItem appliedItem);

        /// <summary>
        /// Fired when the code completion is closed due to the application of an item.
        /// </summary>
        public event AppliedHandler OnApplied = delegate { };

        public void Dispose()
        {
            FinishLifecycle();
        }

        private void FinishLifecycle()
        {
            _delayedCancelAction.RunNow();
        }
    }
}