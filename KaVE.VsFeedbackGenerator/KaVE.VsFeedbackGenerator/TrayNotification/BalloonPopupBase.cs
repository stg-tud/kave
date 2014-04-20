using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public abstract class BalloonPopupBase : UserControl
    {
        protected void OpenUploadWizard()
        {
            ClosePopup();
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
