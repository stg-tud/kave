using System;
using System.Windows;
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

        private TaskbarIcon NotifyIcon { get; set; }

        public virtual void ShowSoftBalloonPopup()
        {
            ShowBalloonPopup(() => new SoftBalloonPopup());
        }

        public virtual void ShowHardBalloonPopup()
        {
            ShowBalloonPopup(() => new HardBalloonPopup());
        }

        private void ShowBalloonPopup(Func<BalloonPopupBase> popupFactory)
        {
            Invoke.OnSTA(
                () => NotifyIcon.ShowCustomBalloon(
                    popupFactory(),
                    PopupAnimation.Slide,
                    null));
        }
    }
}