using System;
using JetBrains.Application;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class UploadReminder
    {
        private readonly ISettingsStore _settingsStore;
        private readonly NotifyTrayIcon _taskbarIcon;
        private readonly ICallbackManager _callbackManager;

        public UploadReminder(ISettingsStore settingsStore, NotifyTrayIcon notify, ICallbackManager callbackManager)
        {
            _taskbarIcon = notify;
            _settingsStore = settingsStore;
            _callbackManager = callbackManager;

            EnsureUploadSettingsInitialized();
            RegisterCallback();
        }

        private void RegisterCallback()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            var nextNotificationTime = settings.LastNotificationDate.AddDays(1);
            _callbackManager.RegisterCallback(ShowNotificationAndUpdateSettings, nextNotificationTime, RegisterCallback);
        }

        private void EnsureUploadSettingsInitialized()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            if (settings.IsInitialized())
            {
                return;
            }
            settings.Initialize();
            _settingsStore.SetSettings(settings);
        }

        private void ShowNotificationAndUpdateSettings()
        {
            // at this point, lastNotification is at least one day past
            if (OneWeekSinceLastUpload())
            {
                _taskbarIcon.ShowHardBalloonPopup();
                UpdateLastNotificationDate();
            }
            else if (OndDaySinceLastUpload())
            {
                _taskbarIcon.ShowSoftBalloonPopup();
                UpdateLastNotificationDate();
            }
        }

        private bool OndDaySinceLastUpload()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            return settings.LastUploadDate.AddDays(1) < DateTime.Now;
        }

        private void UpdateLastNotificationDate()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            settings.LastNotificationDate = DateTime.Now;
            _settingsStore.SetSettings(settings);
        }

        private bool OneWeekSinceLastUpload()
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            return settings.LastUploadDate.AddDays(7) < DateTime.Now;
        }
    }
}