using System.Windows;
using System.Windows.Controls.Primitives;
using KaVE.JetBrains.Annotations;
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

        private void CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = ((ToggleButton) sender);
            StoreCheckboxState(checkbox);
        }

        private static void StoreCheckboxState([CanBeNull] ToggleButton checkbox)
        {
            Asserts.NotNull(checkbox);

            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<FeedbackGeneratorResharperSettings>();

            //TODO: String switching sucks because it depends on the xaml name
            switch (checkbox.Name)
            {
                case "NamesCheckBox":
                    settings.FeedbackGeneratorNames = checkbox.IsChecked;
                    break;
                case "DurationCheckBox":
                    settings.FeedbackGeneratorDuration = checkbox.IsChecked;
                    break;
                case "SessionUUIDCheckBox":
                    settings.FeedbackGeneratorSessionIDs = checkbox.IsChecked;
                    break;
                case "StartTimeCheckBox":
                    settings.FeedbackGeneratorStartTime = checkbox.IsChecked;
                    break;
            }
            settingsStore.SetSettings(settings);
        }
    }
}