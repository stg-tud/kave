using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.UI.Controls;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Utils;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Key = System.Windows.Input.Key;

namespace KaVE.VsFeedbackGenerator
{
    internal static class LookupWindowManagerExtensions
    {
        private static readonly FieldInfo LookupWindowField =
            typeof (LookupWindowManagerImpl).GetField(
                "myCachedLookupWindow",
                BindingFlags.NonPublic | BindingFlags.Instance);

        // there's also a protected property ListBox
        private static readonly FieldInfo LookupListBox =
            typeof (LookupWindow).GetField("myListBox", BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly FieldInfo LookupLifetime =
            typeof (Lookup).GetField("myLifetime", BindingFlags.NonPublic | BindingFlags.Instance);

        public static CustomListBoxControl<LookupListItem> GetLookupListBox([NotNull] this ILookupWindowManager manager)
        {
            var lookupWindow = manager.GetLookupWindow();
            if (lookupWindow != null)
            {
                return lookupWindow.GetLookupListBox();
            }
            return null;
        }

        [CanBeNull]
        private static ILookupWindow GetLookupWindow([NotNull] this ILookupWindowManager manager)
        {
            return (ILookupWindow) LookupWindowField.GetValue(manager);
        }

        [NotNull]
        public static CustomListBoxControl<LookupListItem> GetLookupListBox([NotNull] this ILookupWindow lookupWindow)
        {
            return (CustomListBoxControl<LookupListItem>) LookupListBox.GetValue(lookupWindow);
        }

        [NotNull]
        public static Lifetime GetLifetime([NotNull] this ILookup lookup)
        {
            return (Lifetime) LookupLifetime.GetValue(lookup);
        }
    }

    [Language(typeof (CSharpLanguage))]
    public class CodeCompletionLifecycleManager : IDisposable
    {
        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly IActionManager _actionManager;
        private readonly ICodeCompletionLifecycleHandler _handler;
        private readonly DelegateActionHandler _forceCompleteItemHandler;
        private readonly DelegateActionHandler _completeEnterHandler;
        private readonly DelegateActionHandler _completeTabHandler;

        private ILookup _currentLookup;
        private IDEEvent.Trigger? _terminationTrigger;
        private ScheduledAction _finishLifecycleAction = ScheduledAction.NoOp;

        public CodeCompletionLifecycleManager(ILookupWindowManager lookupWindowManager,
            IActionManager actionManager,
            IIDESession session,
            IMessageBus messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _actionManager = actionManager;
            _handler = new CodeCompletionEventHandler(session, messageBus);
            _forceCompleteItemHandler = new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Typing);
            _completeEnterHandler = new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut);
            _completeTabHandler = new DelegateActionHandler(() => _terminationTrigger = IDEEvent.Trigger.Shortcut);

            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
        }

        /// <summary>
        /// Invoked when a completion starts.
        /// </summary>
        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            FinishLifecycle();
            _handler.OnOpened(_lookupWindowManager.CurrentLookup.Prefix);
            _terminationTrigger = null;
            RegisterToLookupEvents();
        }

        private void RegisterToLookupEvents()
        {
            // Event handlers are registered in order of the actual events's occurence

            // _lookupWindowManager.AfterLookupWindowShown is fired immediately after the window pops up,
            // i.e., after this method finished and before any other event is fired.

            _currentLookup = _lookupWindowManager.CurrentLookup;
            _currentLookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged += OnCurrentItemChanged;

            // R# actions that lead to the completion being finished
            var lifetime = _currentLookup.GetLifetime();
            _actionManager.GetExecutableAction("ForceCompleteItem").AddHandler(lifetime, _forceCompleteItemHandler);
            _actionManager.GetExecutableAction("TextControl.Enter").AddHandler(lifetime, _completeEnterHandler);
            _actionManager.GetExecutableAction("TextControl.Tab").AddHandler(lifetime, _completeTabHandler);
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
            _handler.OnSelectionChanged(_lookupWindowManager.CurrentLookup.Selection.Item);
            _terminationTrigger = null;
        }

        /// <summary>
        /// Invoked when the typed prefix changes, either because another character is typed or because a character
        /// is deleted.
        /// </summary>
        private void CheckPrefixChange()
        {
            // TODO test wheather this is actually called on delete!
            _handler.OnPrefixChanged(_lookupWindowManager.CurrentLookup.Prefix);
            _terminationTrigger = null;
        }

        /// <summary>
        /// MouseDown is fired before Closed, as opposed to Click. Therefore, we listen on MouseDown here and assume
        /// that it will be followed by a MouseUp and, thereby, terminate the completion.
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

            _actionManager.GetExecutableAction("ForceCompleteItem").RemoveHandler(_forceCompleteItemHandler);
            _actionManager.GetExecutableAction("TextControl.Enter").RemoveHandler(_completeEnterHandler);
            _actionManager.GetExecutableAction("TextControl.Tab").RemoveHandler(_completeTabHandler);
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

    internal class DelegateActionHandler : IActionHandler
    {
        private readonly Action _action;

        public DelegateActionHandler(Action action)
        {
            _action = action;
        }

        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            _action();
            nextExecute();
        }
    }
}