using System.Windows;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public partial class NotificationAbortDialog
    {
        public NotificationAbortDialog()
        {
            InitializeComponent();
        }

        public bool DontShowAgain
        {
            get { return Checkbox.IsChecked == true;  }
        }

        public bool Canceled
        {
            get { return DialogResult == true; }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}