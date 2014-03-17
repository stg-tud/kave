using System;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
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
        private readonly ICallbackManager _callbackManager;

        public UploadReminder(ISettingsStore settingsStore, NotifyTrayIcon notify, ICallbackManager callbackManager, SessionManagerWindowRegistrar sessionWindowRegistrar)
        {
            _taskbarIcon = notify.NotifyIcon;
            _settingsStore = settingsStore;
            _callbackManager = callbackManager;
            _sessionWindowRegistrar = sessionWindowRegistrar;
            
            InitLastUploadTime();
            RegisterCallback();

           //_taskbarIcon.ShowCustomBalloon(new HardBalloonPopup(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId), PopupAnimation.Slide, null);
        }

        private void RegisterCallback()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            var nextNotificationTime = settings.LastNotificationDate.AddDays(7);
            _callbackManager.RegisterCallback(ExecuteOnceAWeek, nextNotificationTime);
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

            if (FourWeeksReachedWithoutAnUpload(settings))
            {
                _taskbarIcon.ShowCustomBalloon(
                    new HardBalloonPopup(
                        _sessionWindowRegistrar,
                        SessionManagerWindowActionHandler.ActionId),
                        PopupAnimation.Slide,
                        null);
            }
            else
            {
                _taskbarIcon.ShowCustomBalloon(
                    new SoftBalloonPopup(
                        _sessionWindowRegistrar,
                        SessionManagerWindowActionHandler.ActionId),
                        PopupAnimation.Slide,
                        null);
            }

            settings.LastNotificationDate = DateTime.Now;
            _settingsStore.SetSettings(settings);
            RegisterCallback();
        }

        private static bool FourWeeksReachedWithoutAnUpload(UploadSettings settings)
        {
            return settings.LastNotificationDate.AddDays(28) >= DateTime.Now;
        }
    }
}
