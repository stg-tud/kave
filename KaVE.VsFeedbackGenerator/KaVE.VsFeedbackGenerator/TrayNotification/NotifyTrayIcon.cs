using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    [ShellComponent]
    public class NotifyTrayIcon
    {
        private TaskbarIcon _notifyIcon;

        public NotifyTrayIcon()
        {
            _notifyIcon = null;// InitTaskbarIcon();
            //Invoke.OnSTA(InitTaskbarIcon);
        }

        private void InitTaskbarIcon()
        {
            _notifyIcon = new TaskbarIcon {Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        public TaskbarIcon NotifyIcon
        {
            get { return _notifyIcon;  }
        }
    }
}
