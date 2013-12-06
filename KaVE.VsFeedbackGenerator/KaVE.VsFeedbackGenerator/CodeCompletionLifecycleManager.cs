using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Forms;
using JetBrains.ReSharper.Feature.Services.Lookup;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.UI.Controls;
using JetBrains.Util;
using KaVE.JetBrains.Annotations;
using KaVE.Utils;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using KaVE.VsFeedbackGenerator.MessageBus;
using KaVE.VsFeedbackGenerator.VsIntegration;

namespace KaVE.VsFeedbackGenerator
{
    internal static class LookupWindowManagerExtensions
    {
        private static readonly FieldInfo LookupWindowField =
            typeof(LookupWindowManagerImpl).GetField("myCachedLookupWindow", BindingFlags.NonPublic | BindingFlags.Instance);

        // there's also a protected property ListBox
        private static readonly FieldInfo LookupListBox =
            typeof (LookupWindow).GetField("myListBox", BindingFlags.NonPublic | BindingFlags.Instance);

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
    }

    [Language(typeof(CSharpLanguage))]
    public class CodeCompletionLifecycleManager
    {
        private enum LifecyclePhase
        {
            Unused,
            Open,
            Applied,
            Cancelled
        }

        private readonly ILookupWindowManager _lookupWindowManager;
        private readonly ICodeCompletionLifecycleHandler _handler;

        private LifecyclePhase _completionPhase;
        private ILookup _currentLookup;

        public CodeCompletionLifecycleManager(ILookupWindowManager lookupWindowManager, IIDESession session, IMessageBus messageBus)
        {
            _lookupWindowManager = lookupWindowManager;
            _completionPhase = LifecyclePhase.Unused;
            _handler = new CodeCompletionEventHandler(session, messageBus);
            _lookupWindowManager.BeforeLookupWindowShown += OnBeforeLookupShown;
            // Notes:
            // - AfterLookupWindowShown is fired immediately after the window pops up
            // - LookupWindowClosed and CurrentLookup.Closed are fired before CurrentLookup.ItemCompleted
        }

        /// <summary>
        /// Invoked when a completion starts.
        /// </summary>
        private void OnBeforeLookupShown(Object sender, EventArgs e)
        {
            _completionPhase = LifecyclePhase.Open;
            _handler.OnOpened(_lookupWindowManager.CurrentLookup.Prefix);
            RegisterToLookupEvents();
        }

        private void RegisterToLookupEvents()
        {
            _currentLookup = _lookupWindowManager.CurrentLookup;
            _currentLookup.BeforeShownItemsUpdated += OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged += OnCurrentItemChanged;
            _currentLookup.Closed += OnClosed;
            //_currentLookup.ItemCompleted += OnItemCompleted;
            var lookupListBox = _currentLookup.Window.GetLookupListBox();
            lookupListBox.Click += LookupListBox_OnClick;
            _currentLookup.Window.Scrolled += Window_OnScrolled;
            // DO NOT FIRE
            //lookupListBox.Scrolled += LookupListBox_OnScrolled;
            //lookupListBox.Leave += LookupListBox_OnLeave;
            //lookupListBox.LostFocus += LookupListBox_OnLostFocus;
            //lookupListBox.TextChanged += LookupListBox_OnTextChanged;
        }

        private void Window_OnScrolled(object sender, EventArgs eventArgs)
        {
            // TODO check whether this fires...
        }

        private void LookupListBox_OnClick(object sender, EventArgs eventArgs)
        {
            // sender is ILookupItem
            OnItemCompleted();
        }

        private void LookupListBox_OnVisibleChanged(object sender, EventArgs eventArgs)
        {
            
        }

        private void LookupListBox_OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyCode)
            {
                case Keys.Escape:
                    OnCancellation(DateTime.Now);
                    break;
                case Keys.Enter:
                    OnItemCompleted();
                    break;
            }
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
        }

        /// <summary>
        /// Invoked when the typed prefix changes, either because another character is typed or because a character
        /// is deleted.
        /// </summary>
        private void CheckPrefixChange()
        {
            // TODO test wheather this is actually called on delete!
            _handler.OnPrefixChanged(_lookupWindowManager.CurrentLookup.Prefix);
        }

        /// <summary>
        /// Invoked when the completion is closed, regardless of whether an item was applied or the completion was
        /// cancelled. In the former case, invocation occurs before that of <see cref="OnItemCompleted"/>. Therefore,
        /// it defers the notification of the handler(s) for some time, to make sure any <see cref="OnItemCompleted"/>
        /// event arives first.
        /// </summary>
        private void OnClosed(object sender, EventArgs eventArgs)
        {
            var terminatedAt = DateTime.Now;
            Invoke.Later(() => OnCancellation(terminatedAt), 10000);

            _currentLookup.BeforeShownItemsUpdated -= OnBeforeShownItemsUpdated;
            _currentLookup.CurrentItemChanged -= OnCurrentItemChanged;
            _currentLookup.Closed -= OnClosed;
        }

        /// <summary>
        /// Invoked delayed from <see cref="OnClosed"/>. When the <see cref="_completionPhase"/> is still
        /// <see cref="LifecyclePhase.Open"/>, it assumes that the lookup was cancelled.
        /// </summary>
        /// <param name="cancellationTime"></param>
        private void OnCancellation(DateTime cancellationTime)
        {
            lock (_handler)
            {
                if (_completionPhase == LifecyclePhase.Open)
                {
                    _handler.OnCancellation(cancellationTime);
                }
            }
        }

        /// <summary>
        /// Invoked when the completion is terminated by application of an item. Invocation occurs after that of
        /// <see cref="OnClosed"/>.
        /// </summary>
        private void OnItemCompleted()
        {
            lock (_handler)
            {
                Asserts.Not(_completionPhase == LifecyclePhase.Cancelled, "event was cancelled earlier");
                _completionPhase = LifecyclePhase.Applied;
                _handler.OnApplication(DateTime.Now, _currentLookup.Selection.Item);
            }
        }
    }
}
