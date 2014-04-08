using System.Windows;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public partial class PopupAbortConfirmationDialog
    {
        public PopupAbortConfirmationDialog()
        {
            InitializeComponent();
        }

        public bool DontShowAgain
        {
            get { return DontShowAgainCheckbox.IsChecked == true; }
        }

        public bool Confirmed { get; private set; }

        private void OnClickConfirm(object sender, RoutedEventArgs e)
        {
            Confirmed = true;
            Close();
        }

        private void OnClickCancel(object sender, RoutedEventArgs e)
        {
            Confirmed = false;
            Close();
        }
    }
}