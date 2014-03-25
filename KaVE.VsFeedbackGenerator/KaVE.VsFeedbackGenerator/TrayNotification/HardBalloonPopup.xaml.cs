using System.Windows;
using System.Windows.Input;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for SoftBalloonPopup.xaml
    /// </summary>
    public partial class HardBalloonPopup
    { 
        private readonly SessionManagerWindowRegistrar _window;
        private readonly string _actionId;

        public HardBalloonPopup(SessionManagerWindowRegistrar window, string actionId)
        {
            InitializeComponent();
            _window = window;
            _actionId = actionId;
        }

        private void Wizard_Button_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO: Implement me
            /*Invoke.OnDispatcherAsync(
                    () => ReentrancyGuard.Current.Execute(_actionId,
                        () => _window.ToolWindow.Show()));*/
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CreateCloseDialog();
        }
    }
}




   
