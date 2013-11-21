using System.Windows.Controls;
using Microsoft.VisualStudio.Shell.Interop;

namespace KaVE.EventGenerator.ReSharper8.SessionManager.Presentation
{
    /// <summary>
    /// Interaktionslogik für FeedbackWindowControl.xaml
    /// </summary>
    public partial class FeedbackWindowControl : UserControl
    {
        public FeedbackWindowControl(IVsUIShell shell)
        {
            InitializeComponent();
            DataContext = SessionHolder.Instance;
        }
    }
}
