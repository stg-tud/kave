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
 */

using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using JetBrains.ActionManagement;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Common.Options;
using JetBrains.ReSharper.Features.Finding.Resources;
using JetBrains.Threading;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [OptionsPage(PID, "KaVE Feedback", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = ToolsPage.PID)]
    public partial class FeedbackGeneratorOptionPage : IOptionsPage
    {
        private readonly IActionManager _actionManager;
        private const string PID = "FeedbackGenerator.OptionPage";

        public FeedbackGeneratorOptionPage(Lifetime lifetime, OptionsSettingsSmartContext ctx, IActionManager actionManager)
        {
            _actionManager = actionManager;
            InitializeComponent();
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveCodeNames, RemoveCodeNamesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveStartTimes, RemoveStartTimesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveDurations, RemoveDurationsCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveSessionIDs, RemoveSessionUUIDCheckBox);
        }

        private static void SetToggleButtonBinding(IContextBoundSettingsStore ctx,
            Lifetime lifetime,
            Expression<Func<ExportSettings, bool?>> settingProperty,
            ToggleButton toggleButton)
        {
            ctx.SetBinding(lifetime, settingProperty, toggleButton, ToggleButton.IsCheckedProperty);
        }

        private void RestoreSettings_OnClick(object sender, RoutedEventArgs e)
        {
            _actionManager.ExecuteActionGuarded(SettingsCleaner.ActionId, "reset-all");
            var window = Window.GetWindow(this);
            Asserts.NotNull(window, "option page has no option window");
            window.Close();
        }

        public bool OnOk()
        {
            return true;
        }

        public bool ValidatePage()
        {
            return true;
        }

        public EitherControl Control
        {
            get { return this; }
        }

        public string Id
        {
            get { return PID; }
        }
    }
}