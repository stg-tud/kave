using System.Windows.Controls;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    /// <summary>
    /// Interaktionslogik für FeedbackWindowControl.xaml
    /// </summary>
    public partial class FeedbackWindowControl : UserControl
    {
        public FeedbackWindowControl(SessionHolder holder)
        {
            DataContext = holder;
            InitializeComponent();
        }

        private void Initialize(object sender, System.EventArgs e)
        {
            // TODO somehow ensure that data is refreshed when window is opened
            // ((SessionHolder) DataContext).RefreshSessions();
        }
    }
}