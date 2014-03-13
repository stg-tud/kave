using System;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class UploadReminder
    {
        private readonly ISettingsStore _settingsStore;
        private readonly SessionManagerWindowRegistrar _sessionWindowRegistrar;
        private readonly TaskbarIcon _taskbarIcon;

        public UploadReminder(ISettingsStore settingsStore, NotifyTrayIcon notify, SessionManagerWindowRegistrar sessionWindowRegistrar)
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
            var nextNotificationTime = settings.LastNotificationDate.AddDays(7);
            Invoke.Later(ExecuteOnceAWeek, nextNotificationTime);
        }

        private void InitLastUploadTime()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            if (settings.IsInitialized())
            {
                return;
            }
            settings.Initialize();
            _settingsStore.SetSettings(settings);
        }

        private void ExecuteOnceAWeek()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            _taskbarIcon.ShowCustomBalloon(
                settings.LastNotificationDate.AddDays(28) < DateTime.Now
                    ? new BalloonNotification(
                        _sessionWindowRegistrar,
                        SessionManagerWindowActionHandler.ActionId,
                        false)
                    : new BalloonNotification(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId, true),
                PopupAnimation.Slide,
                null);

            settings.LastNotificationDate = DateTime.Now;
            _settingsStore.SetSettings(settings);
            RegisterCallback();
        }
    }
}
