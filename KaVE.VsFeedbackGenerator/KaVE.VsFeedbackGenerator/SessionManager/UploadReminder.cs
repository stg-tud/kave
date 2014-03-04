using System;
using System.Globalization;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class UploadReminder
    {
        private readonly SettingsStore _settingsStore;
        private readonly SessionManagerWindowRegistrar _sessionWindowRegistrar;
        private readonly TaskbarIcon _taskbarIcon;

        public UploadReminder(SettingsStore settingsStore, NotifyTrayIcon notify, SessionManagerWindowRegistrar sessionWindowRegistrar)
        {
            _taskbarIcon = notify.NotifyIcon;
            _settingsStore = settingsStore;
            _sessionWindowRegistrar = sessionWindowRegistrar;
            
            InitLastUploadTime();
            RegisterCallback();

           //_taskbarIcon.ShowCustomBalloon(new BalloonNotification(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId, false), PopupAnimation.Slide, null);
        }

        private void RegisterCallback()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            var nextNotificationTime = settings.LastNotificationTimeAsDate.AddDays(7);
            Invoke.Later(ExecuteOnceAWeek, nextNotificationTime);
        }

        private void InitLastUploadTime()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            if (settings.LastNotificationTime.IsEmpty() && settings.LastNotificationTime.IsEmpty())
            {
                var now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                settings.LastUploadTime = now;
                settings.LastNotificationTime = now;
                _settingsStore.SetSettings(settings);
            }
        }

        private void ExecuteOnceAWeek()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            _taskbarIcon.ShowCustomBalloon(
                settings.LastNotificationTimeAsDate.AddDays(28) < DateTime.Now
                    ? new BalloonNotification(
                        _sessionWindowRegistrar,
                        SessionManagerWindowActionHandler.ActionId,
                        false)
                    : new BalloonNotification(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId, true),
                PopupAnimation.Slide,
                null);

            settings.LastNotificationTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            _settingsStore.SetSettings(settings);
            RegisterCallback();
        }
    }
}
