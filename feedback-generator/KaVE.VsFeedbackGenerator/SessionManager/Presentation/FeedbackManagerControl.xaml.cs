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

using System.Windows;
using System.Windows.Controls;
using JetBrains.ActionManagement;
using JetBrains.DataFlow;
using JetBrains.UI.Options;
using KaVE.ReSharper.Commons.Utils;
using KaVE.VsFeedbackGenerator.Export;
using KaVE.VsFeedbackGenerator.Interactivity;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    public partial class SessionManagerControl
    {
        private readonly FeedbackViewModel _feedbackViewModel;
        private readonly IActionManager _actionManager;
        private readonly ISettingsStore _settingsStore;

        public SessionManagerControl(FeedbackViewModel feedbackViewModel,
            IActionManager actionManager,
            ISettingsStore settingsStore)
        {
            DataContext = feedbackViewModel;
            _feedbackViewModel = feedbackViewModel;
            _feedbackViewModel.ConfirmationRequest.Raised += new ConfirmationRequestHandler(this).Handle;

            _actionManager = actionManager;
            _settingsStore = settingsStore;

            InitializeComponent();
        }

        public void OnVisibilityChanged(PropertyChangedEventArgs<bool> e)
        {
            var wasVisible = e.HasOld && e.Old;
            var isVisible = e.HasNew && e.New;
            if (!wasVisible && isVisible)
            {
                RefreshControl();
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            RefreshControl();
        }

        private void RefreshControl()
        {
            _feedbackViewModel.Refresh();
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

        public void Export_OnClick(object sender, RoutedEventArgs e)
        {
            _actionManager.ExecuteActionGuarded<UploadWizardAction>(SessionManagerWindowRegistrar._lifetime);
        }

        private void VisitUploadPageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var export = settingsStore.GetSettings<ExportSettings>();

            System.Diagnostics.Process.Start(export.UploadUrl);
        }

        private void VisitHomepageButton_OnClick(object sender, RoutedEventArgs e)
        {
            var prefix = _settingsStore.GetSettings<ExportSettings>().WebAccessPrefix;
            System.Diagnostics.Process.Start(prefix + "http://kave.cc");
        }

        private void OpenOptionPage_OnClick(object sender, RoutedEventArgs e)
        {
            _actionManager.ExecuteActionGuarded<ShowOptionsAction>(SessionManagerWindowRegistrar._lifetime);
        }
    }
}