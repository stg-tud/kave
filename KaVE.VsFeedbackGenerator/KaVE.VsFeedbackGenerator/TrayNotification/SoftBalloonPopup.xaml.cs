using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Hardcodet.Wpf.TaskbarNotification;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for BalloonNotification.xaml
    /// </summary>
    public partial class BalloonNotification
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

        private void Wizard_Button_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO: Implement me
            CloseBalloon();
            /*Invoke.OnDispatcherAsync(
                    () => ReentrancyGuard.Current.Execute(_actionId,
                        () => _window.ToolWindow.Show()));*/
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO: Check if HideDialogFlag is set
            var dialog = new CustomWpfDialog("Wenn Sie abbrechen, werden auf ihrer Festplatte weiter Daten geschrieben",
                "Wizard abbrechen?");
            dialog.Show();
            if (!dialog.Canceled)
            {
                if (dialog.DontShowAgain)
                {
                    HideDialogInFuture();
                }
                CloseBalloon();
            }
        }

        private void HideDialogInFuture()
        {
            //TODO: Implement me
        }

        private void CloseBalloon()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}




   
