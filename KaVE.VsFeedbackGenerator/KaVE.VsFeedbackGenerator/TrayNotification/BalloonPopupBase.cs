using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public abstract class BalloonPopupBase : UserControl
    {
        protected void OpenUploadWizard()
        {
            ClosePopup();
            var wizard = new UploadWizard();
            wizard.ShowDialog();
            //TODO: Handle result and call upload logic
        }

        protected void ClosePopup()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
