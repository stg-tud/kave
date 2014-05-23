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
using System.Windows;
using System.Windows.Controls.Primitives;
using JetBrains.ActionManagement;
using KaVE.VsFeedbackGenerator.Interactivity;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public partial class UploadWizard
    {
        private readonly IActionManager _actionManager;
        private readonly ISettingsStore _settingsStore;

        public class UploadOptions : Notification
        {
            public enum ExportType
            {
                ZipFile, HttpUpload
            }

            public ExportType? Type;
        }

        public UploadOptions.ExportType? ResultType { get; private set; }

        public UploadWizard(IActionManager actionManager, ISettingsStore settingsStore)
        {
            _actionManager = actionManager;
            _settingsStore = settingsStore;
            InitializeComponent();

            LoadCheckboxState();
        }

        private void LoadCheckboxState()
        {
            var settings = _settingsStore.GetSettings<ExportSettings>();

            BindCheckbox(NamesCheckBox, settings.RemoveCodeNames, (s, v) => s.RemoveCodeNames = v);
            BindCheckbox(DurationCheckBox, settings.RemoveDurations, (s, v) => s.RemoveDurations = v);
            BindCheckbox(SessionUUIDCheckBox, settings.RemoveSessionIDs, (s, v) => s.RemoveSessionIDs = v);
            BindCheckbox(StartTimeCheckBox, settings.RemoveStartTimes, (s, v) => s.RemoveStartTimes = v);
        }

        private static void BindCheckbox(ToggleButton button, bool value, Action<ExportSettings, bool> setter)
        {
            button.IsChecked = value;
            var binding = new Binding(setter);
            button.Checked += binding.OnCheckedChanged;
            button.Unchecked += binding.OnCheckedChanged;
        }

        private class Binding
        {
            private readonly Action<ExportSettings, bool> _setter;

            public Binding(Action<ExportSettings, bool> setter)
            {
                _setter = setter;
            }

            public void OnCheckedChanged(object sender, RoutedEventArgs routedEventArgs)
            {
                var toogleButton = (ToggleButton) sender;
                var settingsStore = Registry.GetComponent<ISettingsStore>();
                var settings = settingsStore.GetSettings<ExportSettings>();
                _setter(settings, toogleButton.IsChecked.GetValueOrDefault(false));
                settingsStore.SetSettings(settings);
            }
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            _actionManager.ExecuteActionGuarded("KaVE.VsFeedbackGenerator.SessionManager",  "AgentAction");
        }

        private void UploadButtonClicked(object sender, RoutedEventArgs e)
        {
            ResultType = UploadOptions.ExportType.HttpUpload;
            Close();
        }

        private void ZipButtonClicked(object sender, RoutedEventArgs e)
        {
            ResultType = UploadOptions.ExportType.ZipFile;
            Close();
        }
    }
}