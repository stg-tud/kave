using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    [ShellComponent]
    public class NotifyTrayIcon
    {
        private readonly TaskbarIcon _notifyIcon;

        public NotifyTrayIcon()
        {
            _notifyIcon = new TaskbarIcon { Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        public TaskbarIcon NotifyIcon
        {
            get { return _notifyIcon;  }
        }
    }
}
