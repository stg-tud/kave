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
 * 
 * Contributors:
 *    - Uli Fahrer
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JetBrains.ActionManagement;
using JetBrains.Application;
using JetBrains.DataFlow;
using KaVE.Utils.Reflection;
using KaVE.VsFeedbackGenerator.Export;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ShellComponent]
    public class SessionManagerInitializer
    {
        public SessionManagerInitializer(ISettingsStore settingsStore)
        {
            EnsureLastReviewDateReset(settingsStore);
        }

        /// <summary>
        ///     If the IDE crashed, the date might not be reset and we end in an inconsistent state. We prevent this by resetting
        ///     on startup.
        /// </summary>
        private static void EnsureLastReviewDateReset(ISettingsStore settingsStore)
        {
            settingsStore.UpdateSettings<ExportSettings>(settings => settings.LastReviewDate = null);
        }
    }

    public partial class SessionManagerControl
    {
        private readonly FeedbackViewModel _feedbackViewModel;
        private readonly IActionManager _actionManager;
        private readonly IDateUtils _dateUtils;
        private readonly ISettingsStore _settingsStore;

        public SessionManagerControl(FeedbackViewModel feedbackViewModel,
            IActionManager actionManager,
            IDateUtils dateUtils,
            ISettingsStore settingsStore)
        {
            DataContext = feedbackViewModel;
            _feedbackViewModel = feedbackViewModel;
            _feedbackViewModel.ConfirmationRequest.Raised += new ConfirmationRequestHandler(this).Handle;
            _feedbackViewModel.OnPropertyChanged(
                self => self.IsBusy,
                isBusy =>
                {
                    if (isBusy)
                    {
                        SetLastReviewDate(_dateUtils.Now);
                    }
                });
            _feedbackViewModel.SessionSelection += (o, sessions) => SetSelection(SessionListView, sessions);
            _feedbackViewModel.EventSelection += (o, events) => SetSelection(EventListView, events);

            _actionManager = actionManager;
            _dateUtils = dateUtils;
            _settingsStore = settingsStore;

            InitializeComponent();
        }

        private static void SetSelection(ListBox view, IEnumerable<object> elements)
        {
            var selection = view.SelectedItems;
            foreach (var element in elements)
            {
                selection.Add(element);
            }
        }

        public void OnVisibilityChanged(PropertyChangedEventArgs<bool> e)
        {
            var wasVisible = e.HasOld && e.Old;
            var isVisible = e.HasNew && e.New;
            if (!wasVisible && isVisible)
            {
                RefreshControl();
            }
            else if (wasVisible && !isVisible)
            {
                SetLastReviewDate(null);
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshControl();
        }

        private void RefreshControl()
        {
            if (!_feedbackViewModel.IsBusy)
            {
                _feedbackViewModel.Refresh();
            }
        }

        private void SetLastReviewDate(DateTime? lastReviewDate)
        {
            _settingsStore.UpdateSettings<ExportSettings>(settings => settings.LastReviewDate = lastReviewDate);
        }

        /// <summary>
        ///     Makes the overflow dropdown button invisible.
        /// </summary>
        private void ToolBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            var toolBar = sender as ToolBar;
            if (toolBar == null)
            {
                return;
            }
            var overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        ///     Unfortunately, SelectedItems is not a bidirectional control property, hence, we cannot bind it to a
        ///     property of out view model to access the set of selected items. Therefore, we react on selection changes
        ///     here and update our model property manually.
        /// </summary>
        private void SessionListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _feedbackViewModel.SelectedSessions = SessionListView.SelectedItems.Cast<SessionViewModel>();
        }

        /// <summary>
        ///     Unfortunately, SelectedItems is not a bidirectional control property, hence, we cannot bind it to a
        ///     property of out view model to access the set of selected items. Therefore, we react on selection changes
        ///     here and update our model property manually.
        /// </summary>
        private void EventListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_feedbackViewModel.SingleSelectedSession != null)
            {
                _feedbackViewModel.SingleSelectedSession.SelectedEvents =
                    EventListView.SelectedItems.Cast<EventViewModel>();
            }
        }

        public void Export_OnClick(object sender, RoutedEventArgs e)
        {
            UploadWizardActionHandler.Execute(_actionManager);
        }

        private void VisitUploadPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var export = settingsStore.GetSettings<ExportSettings>();

            var idx = export.UploadUrl.LastIndexOf('/');
            var url = export.UploadUrl.Substring(0, idx);

            System.Diagnostics.Process.Start(url);
        }

        private void VisitHomepageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var prefix = _settingsStore.GetSettings<ExportSettings>().WebAccessPrefix;
            System.Diagnostics.Process.Start(prefix + "http://kave.cc");
        }

        private void OpenOptionPage_OnClick(object sender, RoutedEventArgs e)
        {
            _actionManager.ExecuteActionGuarded("ShowOptions", "AgentAction");
        }
    }
}