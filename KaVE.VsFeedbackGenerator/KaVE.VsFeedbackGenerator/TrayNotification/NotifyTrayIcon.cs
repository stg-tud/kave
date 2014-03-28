using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Properties;

namespace KaVE.VsFeedbackGenerator.TrayNotification
{
    public interface INotifyTrayIcon
    {
        void ShowCustomNotification(UserControl control, PopupAnimation animation, int? timeout);
    }

    [ShellComponent]
    public class NotifyTrayIcon : INotifyTrayIcon
    {
        public NotifyTrayIcon()
        {
            Invoke.OnSTA(InitTaskbarIcon);
        }

        private void InitTaskbarIcon()
        {
            NotifyIcon = new TaskbarIcon {Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        private TaskbarIcon NotifyIcon { get; set; }

        public void ShowCustomNotification(UserControl control, PopupAnimation animation, int? timeout)
        {
            NotifyIcon.ShowCustomBalloon(control, animation, timeout);
        }
    }
}