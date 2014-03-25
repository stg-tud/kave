using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            Invoke.OnSTA(InitTaskbarIcon);
        }

        private void InitTaskbarIcon()
        {
            NotifyIcon = new TaskbarIcon {Icon = Resources.Bulb, Visibility = Visibility.Hidden};
        }

        public TaskbarIcon NotifyIcon { get; private set; }
        
        public virtual void ShowCustomNotification(UserControl control, PopupAnimation animation, int? timeout)
        {
            NotifyIcon.ShowCustomBalloon(control, animation, timeout);
        }
    }
}
