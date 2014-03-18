using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public abstract class WindowBase : UserControl
    {
        protected void CreateCloseDialog()
        {
            var settingsStore = Shell.Instance.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<UploadSettings>();
            if (settings.DoNotShowNotificationCloseDialogAgain)
            {
                CloseBalloon();
            }
            else
            {
                ShowCloseDialog();
            }
        }

        private void ShowCloseDialog()
        {
            var dialog = new NotificationAbortDialog();
            dialog.Show();

            if (!dialog.Canceled)
            {
                HandleAbortAction(dialog);
            }
            CloseBalloon();
        }

        private void HandleAbortAction(NotificationAbortDialog dialog)
        {
            if (dialog.DontShowAgain)
            {
                HideDialogInFuture();
            }
            CloseBalloon();
        }

        private void HideDialogInFuture()
        {
            var settingsStore = Shell.Instance.GetComponent<ISettingsStore>();
            var settings = settingsStore.GetSettings<UploadSettings>();
            settings.DoNotShowNotificationCloseDialogAgain = true;
            settingsStore.SetSettings(settings);
        }

        private void CloseBalloon()
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
