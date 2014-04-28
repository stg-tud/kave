using System.Windows;
using System.Windows.Controls.Primitives;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    ///     Interaction logic for UploadWizard.xaml
    /// </summary>
    public partial class UploadWizard
    {
        public UploadWizard(FeedbackViewModel feedbackViewModel)
        {
            FeedbackViewModel = feedbackViewModel;
            InitializeComponent();
            //TODO: Find someway to solve this issue with the procedure made in FeedbackGeneratorOptionPage.xaml.cs
            LoadCheckboxState();
        }

        public FeedbackViewModel FeedbackViewModel { get; private set; }

        private void LoadCheckboxState()
        {
            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<FeedbackGeneratorResharperSettings>();

            NamesCheckBox.IsChecked = settings.FeedbackGeneratorNames;
            DurationCheckBox.IsChecked = settings.FeedbackGeneratorDuration;
            SessionUUIDCheckBox.IsChecked = settings.FeedbackGeneratorSessionIDs;
            StartTimeCheckBox.IsChecked = settings.FeedbackGeneratorStartTime;
        }

        private void On_Review_Click(object sender, RoutedEventArgs e)
        {
            var sessionManagerRegistrar = Registry.GetComponent<SessionManagerWindowRegistrar>();
            sessionManagerRegistrar.ToolWindow.Show();
        }

        private void RemoveNamesOptionChanged(object sender, RoutedEventArgs args)
        {
            StoreCheckboxState((ToggleButton) sender, (settings, value) => settings.FeedbackGeneratorNames = value);
        }

        private void RemoveDateTimesOptionChanged(object sender, RoutedEventArgs args)
        {
            StoreCheckboxState((ToggleButton)sender, (settings, value) => settings.FeedbackGeneratorStartTime = value);
        }

        private void RemoveDurationsOptionChanged(object sender, RoutedEventArgs args)
        {
            StoreCheckboxState((ToggleButton)sender, (settings, value) => settings.FeedbackGeneratorDuration = value);
        }

        private void RemoveSessionUUIDOptionChanged(object sender, RoutedEventArgs args)
        {
            StoreCheckboxState((ToggleButton)sender, (settings, value) => settings.FeedbackGeneratorSessionIDs = value);
        }

        private delegate void PropertySetter(FeedbackGeneratorResharperSettings settings, bool? value);

        private static void StoreCheckboxState(ToggleButton checkbox, PropertySetter setter)
        {
            Asserts.NotNull(checkbox);

            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<FeedbackGeneratorResharperSettings>();
            setter(settings, checkbox.IsChecked);
            settingsStore.SetSettings(settings);
        }
    }
}