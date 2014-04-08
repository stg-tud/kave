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
            get { return Checkbox.IsChecked == true;  }
        }

        public bool Confirmed
        {
            get { return DialogResult == true; }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}