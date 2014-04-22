using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public abstract class BalloonPopupBase : UserControl
    {
        protected void OpenUploadWizard()
        {
            ClosePopup();
            var wizard = new UploadWizard(Registry.GetComponent<FeedbackViewModel>());
            wizard.ShowDialog();
        }

        protected void ClosePopup()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
