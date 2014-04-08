using System.Windows;
using System.Windows.Input;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for HardBalloonPopup.xaml
    /// </summary>
    public partial class HardBalloonPopup
    {
        public HardBalloonPopup()
        {
            InitializeComponent();
        }

        private void Wizard_Button_OnClick(object sender, RoutedEventArgs e)
        {
            OpenUploadWizard();
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
        }

        private new void ClosePopup()
        {
            var settingsStore = Registry.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<UploadSettings>();
            if (settings.DoNotShowNotificationCloseDialogAgain)
            {
                base.ClosePopup();
            }
            else
            {
                ClosePopupWithConfirmation();
            }
        }

        private void ClosePopupWithConfirmation()
        {
            var dialog = new PopupAbortConfirmationDialog();
            dialog.ShowDialog();

            if (dialog.Confirmed)
            {
                SaveDontShowAgainChoice(dialog.DontShowAgain);
                base.ClosePopup();
            }
        }

        private static void SaveDontShowAgainChoice(bool dontShowAgain)
        {
            if (dontShowAgain)
            {
                var settingsStore = Registry.GetComponent<ISettingsStore>();
                var settings = settingsStore.GetSettings<UploadSettings>();
                settings.DoNotShowNotificationCloseDialogAgain = true;
                settingsStore.SetSettings(settings);
            }
        }
    }
}




   
