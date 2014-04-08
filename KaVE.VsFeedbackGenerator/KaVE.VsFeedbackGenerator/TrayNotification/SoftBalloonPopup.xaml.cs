using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    /// <summary>
    /// Interaction logic for SoftBalloonPopup.xaml
    /// </summary>
    public partial class SoftBalloonPopup
    {
        public SoftBalloonPopup()
        {
            InitializeComponent();

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
            OpenUploadWizard();
        }

        private void ImgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClosePopup();
        }
    }
}




   
