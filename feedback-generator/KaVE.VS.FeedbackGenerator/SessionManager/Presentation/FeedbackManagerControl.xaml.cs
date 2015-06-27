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
using JetBrains.DataFlow;
using JetBrains.UI.Options;
using KaVE.RS.Commons;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Interactivity;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.Export;
using KaVE.VS.FeedbackGenerator.Utils;

namespace KaVE.VS.FeedbackGenerator.SessionManager.Presentation
{
    public partial class SessionManagerControl
    {
        private readonly FeedbackViewModel _feedbackViewModel;
        private readonly ActionExecutor _actionExec;
        private readonly ISettingsStore _settingsStore;

        public SessionManagerControl(FeedbackViewModel feedbackViewModel,
            ActionExecutor actionExec,
            ISettingsStore settingsStore)
        {
            DataContext = feedbackViewModel;
            _feedbackViewModel = feedbackViewModel;
            _actionExec = actionExec;
            _feedbackViewModel.ConfirmationRequest.Raised += new ConfirmationRequestHandler(this).Handle;

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
            _actionExec.ExecuteActionGuarded<UploadWizardAction>();
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
            _actionExec.ExecuteActionGuarded<ShowOptionsAction>();
        }
    }
}