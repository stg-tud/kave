using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for HardBalloonPopup.xaml
    /// </summary>
    public partial class SoftBalloonPopup
    { 
        private readonly SessionManagerWindowRegistrar _window;
        private readonly string _actionId;
        
        public SoftBalloonPopup(SessionManagerWindowRegistrar window, string actionId)
        {
            InitializeComponent();
            _window = window;
            _actionId = actionId;

            StartFadeInAndOutAnimation();
        }

        //TODO Move to xaml 
        private void StartFadeInAndOutAnimation()
        {
            var storyboard = TryFindResource("FadeInAndOut") as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
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




   
