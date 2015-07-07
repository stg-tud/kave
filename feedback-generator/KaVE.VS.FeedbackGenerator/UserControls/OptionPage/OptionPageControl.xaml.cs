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

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Navigation.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Options.OptionPages.ToolsPages;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using KaVE.VS.FeedbackGenerator.UserControls.UserProfile;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage
{
    [OptionsPage(PID, "KaVE Project", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = ToolsPage.PID)]
    public partial class OptionPageControl : IOptionsPage
    {
        private const string PID = "KaVE.OptionPageControl";

        private OptionPageViewModel MyDataContext
        {
            get { return (OptionPageViewModel) DataContext; }
        }

        private readonly Lifetime _lifetime;
        private readonly IActionManager _actionManager;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly ExportSettings _exportSettings;
        private readonly UserProfileSettings _userProfileSettings;

        public OptionPageControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            IActionManager actionManager,
            KaVEISettingsStore settingsStore)
        {
            _lifetime = lifetime;
            _actionManager = actionManager;
            _settingsStore = settingsStore;

            InitializeComponent();

            _exportSettings = settingsStore.GetSettings<ExportSettings>();
            _userProfileSettings = settingsStore.GetSettings<UserProfileSettings>();

            DataContext = new OptionPageViewModel
            {
                AnonymizationContext = new AnonymizationContext(_exportSettings),
                UserProfileContext = new UserProfileContext(_exportSettings, _userProfileSettings)
            };

            BindChangesToAnonymization(lifetime, ctx);
            BindToUserProfileChanges(lifetime, ctx);
            BindToGeneralChanges(lifetime, ctx);
        }

        #region jetbrains smart-context bindings

        private void BindToGeneralChanges(Lifetime lifetime, OptionsSettingsSmartContext ctx)
        {
            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => s.UploadUrl,
                UploadUrlTextBox,
                System.Windows.Controls.TextBox.TextProperty);
            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => s.WebAccessPrefix,
                WebPraefixTextBox,
                System.Windows.Controls.TextBox.TextProperty);

            ctx.SetBinding(
                lifetime,
                (ModelStoreSettings s) => s.ModelStorePath,
                ModelStorePathTextBox,
                System.Windows.Controls.TextBox.TextProperty);
        }

        private void BindChangesToAnonymization(Lifetime lifetime, IContextBoundSettingsStore ctx)
        {
            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => (bool?) s.RemoveCodeNames,
                Anonymization.RemoveCodeNamesCheckBox,
                ToggleButton.IsCheckedProperty);

            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => (bool?) s.RemoveDurations,
                Anonymization.RemoveDurationsCheckBox,
                ToggleButton.IsCheckedProperty);

            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => (bool?) s.RemoveSessionIDs,
                Anonymization.RemoveSessionIDsCheckBox,
                ToggleButton.IsCheckedProperty);

            ctx.SetBinding(
                lifetime,
                (ExportSettings s) => (bool?) s.RemoveStartTimes,
                Anonymization.RemoveStartTimesCheckBox,
                ToggleButton.IsCheckedProperty);
        }

        private void BindToUserProfileChanges(Lifetime lifetime, IContextBoundSettingsStore ctx)
        {
            /*ctx.SetBinding(
                lifetime,
                (UserProfileSettings s) => (bool?) s.IsProvidingProfile,
                UserProfile.ProvideUserInformationCheckBox,
                ToggleButton.IsCheckedProperty);
            ctx.SetBinding(
                lifetime,
                (UserProfileSettings s) => s.Name,
                UserProfile.UsernameTextBox,
                System.Windows.Controls.TextBox.TextProperty);
            ctx.SetBinding(
                lifetime,
                (UserProfileSettings s) => s.Email,
                UserProfile.EmailTextBox,
                System.Windows.Controls.TextBox.TextProperty);*/
        }

        #endregion

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                ModelStorePathTextBox.Text = dialog.SelectedPath;
            }
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.ShowYesNo(Properties.SessionManager.Option_SettingsCleaner_Dialog);
            if (result)
            {
                _actionManager.ExecuteActionGuarded<SettingsCleaner>(_lifetime);

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        public bool OnOk()
        {
            // TODO: validation

            _settingsStore.SetSettings(_exportSettings);
            _settingsStore.SetSettings(_userProfileSettings);


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