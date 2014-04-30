using System;
using System.Windows;
using System.Windows.Controls.Primitives;
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
            var settings = settingsStore.GetSettings<FeedbackGeneratorResharperSettings>();

            BindCheckbox(NamesCheckBox, settings.FeedbackGeneratorNames, (s, v) => s.FeedbackGeneratorNames = v);
            BindCheckbox(DurationCheckBox, settings.FeedbackGeneratorNames, (s, v) => s.FeedbackGeneratorDuration = v);
            BindCheckbox(SessionUUIDCheckBox, settings.FeedbackGeneratorNames, (s, v) => s.FeedbackGeneratorSessionIDs = v);
            BindCheckbox(StartTimeCheckBox, settings.FeedbackGeneratorNames, (s, v) => s.FeedbackGeneratorStartTime = v);
        }

        private static void BindCheckbox(ToggleButton button, bool? value, Action<FeedbackGeneratorResharperSettings, bool?> setter)
        {
            button.IsChecked = value;
            var binding = new Binding(setter);
            button.Checked += binding.OnCheckedChanged;
            button.Unchecked += binding.OnCheckedChanged;
        }

        private class Binding
        {
            private readonly Action<FeedbackGeneratorResharperSettings, bool?> _setter;

            public Binding(Action<FeedbackGeneratorResharperSettings, bool?> setter)
            {
                _setter = setter;
            }

            public void OnCheckedChanged(object sender, RoutedEventArgs routedEventArgs)
            {
                var toogleButton = (ToggleButton) sender;
                var settingsStore = Registry.GetComponent<ISettingsStore>();
                var settings = settingsStore.GetSettings<FeedbackGeneratorResharperSettings>();
                _setter(settings, toogleButton.IsChecked);
                settingsStore.SetSettings(settings);
            }
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            var sessionManagerRegistrar = Registry.GetComponent<SessionManagerWindowRegistrar>();
            // TODO @Uli open SessionManager here
            //ReentrancyGuard.Current.Execute("", () => sessionManagerRegistrar.ToolWindow.Show());
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