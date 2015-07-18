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
using System.Windows.Controls;
using Avalon.Windows.Dialogs;
using JetBrains.ActionManagement;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Navigation.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using KaVE.Commons.Utils;
using KaVE.RS.Commons.Settings.KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage
{
    [OptionsPage(PID, "General Settings", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = RootOptionPage.PID, Sequence = 1.0)]
    public partial class GeneralOptionsControl : IOptionsPage
    {
        private const string PID = "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptionsControl";

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _ctx;
        private readonly IActionManager _actionManager;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly ExportSettings _exportSettings;
        private readonly ModelStoreSettings _modelStoreSettings;

        public GeneralOptionsControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            IActionManager actionManager,
            KaVEISettingsStore settingsStore,
            IRandomizationUtils rnd,
            OptionPageViewModel optionPageViewModel)
        {
            _lifetime = lifetime;
            _ctx = ctx;
            _actionManager = actionManager;
            _settingsStore = settingsStore;

            InitializeComponent();

            _exportSettings = settingsStore.GetSettings<ExportSettings>();
            _modelStoreSettings = settingsStore.GetSettings<ModelStoreSettings>();

            optionPageViewModel.ModelStoreSettings = _modelStoreSettings;
            optionPageViewModel.ExportSettings = _exportSettings;

            DataContext = optionPageViewModel;

            if (_ctx != null)
            {
                BindToGeneralChanges();
            }
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
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
            _settingsStore.SetSettings(_modelStoreSettings);
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

        #region jetbrains smart-context bindings

        private void BindToGeneralChanges()
        {
            _ctx.SetBinding(
                _lifetime,
                (ExportSettings s) => s.UploadUrl,
                UploadUrlTextBox,
                TextBox.TextProperty);
            _ctx.SetBinding(
                _lifetime,
                (ExportSettings s) => s.WebAccessPrefix,
                WebPraefixTextBox,
                TextBox.TextProperty);

            _ctx.SetBinding(
                _lifetime,
                (ModelStoreSettings s) => s.ModelStorePath,
                ModelStorePathTextBox,
                TextBox.TextProperty);
        }

        #endregion
    }
}