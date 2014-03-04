using System.Windows;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public partial class CustomWpfDialog
    {
        public CustomWpfDialog(string question, string title)
        {
            InitializeComponent();
            txtQuestion.Text = question;
            Title = title;
        }

        /*public static bool Show(string question, string title)
        {
            var inst = new CustomWpfDialog(question, title);
            inst.ShowDialog();
            return inst.DialogResult == true;
        }*/

        public bool DontShowAgain
        {
            get { return checkbox.IsChecked == true;  }
        }

        public bool Canceled
        {
            get { return DialogResult == true; }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}