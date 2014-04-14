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
            if (!TimeIsBetween(nextNotificationTime, new TimeSpan(10, 0, 0), new TimeSpan(16, 0, 0)))
            {
                nextNotificationTime = ShiftTimeBetween10And16Hour(nextNotificationTime);
            }

            _callbackManager.RegisterCallback(ShowNotificationAndUpdateSettings, nextNotificationTime, RegisterCallback);
        }

        private static DateTime ShiftTimeBetween10And16Hour(DateTime nextNotificationTime)
        {
            var rand = new Random();
            return new DateTime(nextNotificationTime.Year, nextNotificationTime.Month, nextNotificationTime.Day, rand.Next(10, 16), rand.Next(0, 60), 0);
        }

        //TODO: Maybe create extension method?!
        private static bool TimeIsBetween(DateTime datetime, TimeSpan start, TimeSpan end)
        {
            var now = datetime.TimeOfDay;
            if (start < end)
                return start <= now && now <= end;
            return !(end < now && now < start);
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