using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    [ShellComponent]
    public class NotifyTrayIcon
    {
        public NotifyTrayIcon()
        {
            //TODO Fix me: brings tests to fail
            NotifyIcon = null;
            //InitTaskbarIcon();
            //Invoke.OnSTA(InitTaskbarIcon);
        }

        private void InitTaskbarIcon()
        {
            NotifyIcon = new TaskbarIcon {Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        public TaskbarIcon NotifyIcon { get; private set; }
    }
}
