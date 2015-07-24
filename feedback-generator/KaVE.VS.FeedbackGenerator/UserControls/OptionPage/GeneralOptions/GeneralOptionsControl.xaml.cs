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
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.ReSharper.Features.Navigation.Resources;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using KaVE.RS.Commons;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.Settings.ExportSettingsSuite;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions
{
    [OptionsPage(PID, "General Settings", typeof (FeaturesFindingThemedIcons.SearchOptionsPage),
        ParentId = RootOptionPage.PID, Sequence = 1.0)]
    public partial class GeneralOptionsControl : IOptionsPage
    {
        private const string PID = "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.GeneralOptions.GeneralOptionsControl";

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _ctx;
        private readonly ActionExecutor _actionExecutor;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly DataContexts _dataContexts;
        private readonly ExportSettings _exportSettings;

        public GeneralOptionsControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            ActionExecutor actionExecutor,
            KaVEISettingsStore settingsStore,
            DataContexts dataContexts)
        {
            _lifetime = lifetime;
            _ctx = ctx;
            _actionExecutor = actionExecutor;
            _settingsStore = settingsStore;
            _dataContexts = dataContexts;

            InitializeComponent();

            _exportSettings = settingsStore.GetSettings<ExportSettings>();

            DataContext = new GeneralOptionsViewModel()
            {
                ExportSettings = _exportSettings,
            };

            if (_ctx != null)
            {
                BindToGeneralChanges();
            }
        }
        
        private void OnResetSettings(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.ShowYesNo(GeneralOptionsMessages.SettingResetDialog);
            if (result)
            {
                _dataContexts.RegisterDataRule(_lifetime, new DataRule<string>(SettingDataConstants.StandardDataRuleName, SettingDataConstants.DataConstant, "GeneralSettings"));

                _actionExecutor.ExecuteActionGuarded<SettingsCleaner>(_dataContexts.CreateWithDataRules(_lifetime));
                
                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        private void OnResetFeedback(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.ShowYesNo(GeneralOptionsMessages.FeedbackResetDialog);
            if (result)
            {
                _dataContexts.RegisterDataRule(_lifetime, new DataRule<string>(SettingDataConstants.StandardDataRuleName, SettingDataConstants.DataConstant, "Feedback"));

                _actionExecutor.ExecuteActionGuarded<SettingsCleaner>(_dataContexts.CreateWithDataRules(_lifetime));

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
        }

        #endregion
    }
}