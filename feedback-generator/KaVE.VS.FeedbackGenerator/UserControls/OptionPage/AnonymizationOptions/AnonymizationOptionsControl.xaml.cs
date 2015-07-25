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
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.DataFlow;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Options;
using JetBrains.UI.Resources;
using KaVE.RS.Commons;
using KaVE.VS.FeedbackGenerator.Settings;
using KaVE.VS.FeedbackGenerator.UserControls.Anonymization;
using KaVEISettingsStore = KaVE.RS.Commons.Settings.ISettingsStore;
using MessageBox = JetBrains.Util.MessageBox;

namespace KaVE.VS.FeedbackGenerator.UserControls.OptionPage.AnonymizationOptions
{
    [OptionsPage(PID, "Anonymization Settings", typeof (OptionsThemedIcons.ExportLayer),
        ParentId = RootOptionPage.PID, Sequence = 2.0)]
    public partial class AnonymizationOptionsControl : IOptionsPage
    {
        private const string PID = "KaVE.VS.FeedbackGenerator.UserControls.OptionPage.AnonymizationOptions.AnonymizationOptionsControl";

        private readonly Lifetime _lifetime;
        private readonly OptionsSettingsSmartContext _ctx;
        private readonly KaVEISettingsStore _settingsStore;
        private readonly ActionExecutor _actionExecutor;
        private readonly DataContexts _dataContexts;
        private readonly AnonymizationSettings _anonymizationSettings;

        public bool OnOk()
        {
            _settingsStore.SetSettings(_anonymizationSettings);
            return true;
        }

        public AnonymizationOptionsControl(Lifetime lifetime,
            OptionsSettingsSmartContext ctx,
            KaVEISettingsStore settingsStore,
            ActionExecutor actionExecutor,
            DataContexts dataContexts)
        {
            _lifetime = lifetime;
            _ctx = ctx;
            _settingsStore = settingsStore;
            _actionExecutor = actionExecutor;
            _dataContexts = dataContexts;

            InitializeComponent();

            _anonymizationSettings = settingsStore.GetSettings<AnonymizationSettings>();

            var anonymizationContext = new AnonymizationContext(_anonymizationSettings);

            DataContext = anonymizationContext;

            if (_ctx != null)
            {
                BindChangesToAnonymization();
            }
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

        private void OnResetSettings(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.ShowYesNo(AnonymizationOptionsMessages.ResetSettingDialog);
            if (result)
            {
                var settingResetType = new SettingResetType {ResetType = ResetTypes.AnonymizationSettings};
                _actionExecutor.ExecuteActionGuarded<SettingsCleaner>(settingResetType.GetDataContextForSettingResultType(_dataContexts,_lifetime));

                var window = Window.GetWindow(this);
                if (window != null)
                {
                    window.Close();
                }
            }
        }

        #region jetbrains smart-context bindings

        private void BindChangesToAnonymization()
        {
            _ctx.SetBinding(
                _lifetime,
                (AnonymizationSettings s) => (bool?)s.RemoveCodeNames,
                Anonymization.RemoveCodeNamesCheckBox,
                ToggleButton.IsCheckedProperty);

            _ctx.SetBinding(
                _lifetime,
                (AnonymizationSettings s) => (bool?)s.RemoveDurations,
                Anonymization.RemoveDurationsCheckBox,
                ToggleButton.IsCheckedProperty);

            _ctx.SetBinding(
                _lifetime,
                (AnonymizationSettings s) => (bool?)s.RemoveSessionIDs,
                Anonymization.RemoveSessionIDsCheckBox,
                ToggleButton.IsCheckedProperty);

            _ctx.SetBinding(
                _lifetime,
                (AnonymizationSettings s) => (bool?)s.RemoveStartTimes,
                Anonymization.RemoveStartTimesCheckBox,
                ToggleButton.IsCheckedProperty);
        }

        #endregion
    }
}