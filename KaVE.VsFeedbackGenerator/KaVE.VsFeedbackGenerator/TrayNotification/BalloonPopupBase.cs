using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public abstract class BalloonPopupBase : UserControl
    {
        protected void OpenUploadWizard()
        {
            var wizard = new UploadWizard();
            wizard.ShowDialog();
        }

        protected void ClosePopup()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
