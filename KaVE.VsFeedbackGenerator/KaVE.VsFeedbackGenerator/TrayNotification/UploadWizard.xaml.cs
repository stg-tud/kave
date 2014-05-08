using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using JetBrains.ActionManagement;
using JetBrains.DataFlow;
using JetBrains.Threading;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    ///     Interaction logic for UploadWizard.xaml
    /// </summary>
    public partial class UploadWizard
    {
        public bool IsZipExport { get; private set; }
        public bool IsUploadExport { get; private set; }


        public UploadWizard()
        {
            InitializeComponent();

            IsZipExport = false;
            IsUploadExport = false;

            //TODO: Find someway to solve this issue with the procedure made in FeedbackGeneratorOptionPage.xaml.cs
            LoadCheckboxState();
        }

        private void LoadCheckboxState()
        {
            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<ExportSettings>();

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
            var actionManager = Registry.GetComponent<IActionManager>();
            var threading = Registry.GetComponent<IThreading>();
            actionManager.ExecuteActionGuarded("KaVE.VsFeedbackGenerator.SessionManager", threading, "AgentAction");
        }

        private void UploadButtonClicked(object sender, RoutedEventArgs e)
        {
            IsUploadExport = true;
            Close();
        }

        private void ZipButtonClicked(object sender, RoutedEventArgs e)
        {
            IsZipExport = true;
            Close();
        }
    }
}