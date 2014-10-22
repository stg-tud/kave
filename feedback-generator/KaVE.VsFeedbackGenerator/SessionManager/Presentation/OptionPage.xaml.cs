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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using JetBrains.ActionManagement;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Common.Options;
using JetBrains.ReSharper.Features.Finding.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.Utils;
using MessageBox = JetBrains.Util.MessageBox;
using Ressource = KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [OptionsPage(PID, "KaVE Feedback", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = ToolsPage.PID)]
    public partial class OptionPage : IOptionsPage
    {
        private readonly OptionPageViewModel _optionPageViewModel;
        private readonly IActionManager _actionManager;
        private const string PID = "FeedbackGenerator.OptionPage";

        public OptionPage(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            IActionManager actionManager)
        {
            _optionPageViewModel = new OptionPageViewModel();
            _optionPageViewModel.ErrorNotificationRequest.Raised += new NotificationRequestHandler(this).Handle;

            _actionManager = actionManager;
            InitializeComponent();

            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveCodeNames, RemoveCodeNamesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveStartTimes, RemoveStartTimesCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveDurations, RemoveDurationsCheckBox);
            SetToggleButtonBinding(ctx, lifetime, s => (bool?) s.RemoveSessionIDs, RemoveSessionUUIDCheckBox);
            ctx.SetBinding(lifetime, (ExportSettings s) => s.UploadUrl, UploadUrlTextBox, TextBox.TextProperty);
            ctx.SetBinding(lifetime, (ExportSettings s) => s.WebAccessPrefix, WebPraefixTextBox, TextBox.TextProperty);
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
            var message = Ressource.SessionManager.ResourceManager.GetString("Option_SettingsCleaner_Dialog");
            var result = MessageBox.ShowYesNo(message);

            if (result)
            {
                _actionManager.ExecuteActionGuarded(SettingsCleaner.ActionId, "reset-all");
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            var window = Window.GetWindow(this);
            Asserts.NotNull(window, "option page has no option window");
            window.Close();
        }

        public bool OnOk()
        {
            var uploadInformationVerification = _optionPageViewModel.ValidateUploadInformation(UploadUrlTextBox.Text, WebPraefixTextBox.Text);

            if (!uploadInformationVerification.IsUrlValid)
            {
                UploadUrlTextBox.Background = Brushes.Pink;
            }

            if (!uploadInformationVerification.IsPrefixValid)
            {
                WebPraefixTextBox.Background = Brushes.Pink;
            }

            return uploadInformationVerification.IsValidUploadInformation;
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