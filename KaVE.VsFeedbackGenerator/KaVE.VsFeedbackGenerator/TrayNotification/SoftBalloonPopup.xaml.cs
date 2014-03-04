using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using JetBrains.Threading;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for BalloonNotification.xaml
    /// </summary>
    public partial class BalloonNotification : UserControl
    {
        private readonly SessionManagerWindowRegistrar _window;
        private readonly string _actionId;

        public BalloonNotification(SessionManagerWindowRegistrar window, string actionId, bool fade)
        {
            InitializeComponent();
            _window = window;
            _actionId = actionId;
            
            if (fade)
            {
                ActivateFadeOutAnimation();
            }
        }

        private void ActivateFadeOutAnimation()
        {
            var storyboard = TryFindResource("FadeInAndOut") as Storyboard;
            if (storyboard != null)
            {
                storyboard.Begin();
            }
        }

        private void Upload_Button_OnClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void View_Feedback_Button_OnClick(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            Invoke.OnDispatcherAsync(
                    () => ReentrancyGuard.Current.Execute(_actionId,
                        () => _window.ToolWindow.Show()));
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}




   
