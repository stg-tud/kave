using System;
using System.Globalization;
using System.Timers;
using System.Windows.Controls.Primitives;
using Hardcodet.Wpf.TaskbarNotification;
using JetBrains.Application;
using JetBrains.Util;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [ShellComponent]
    public class UploadReminder
    {
        private Timer _timer;
        private readonly SettingsStore _settingsStore;
        private readonly SessionManagerWindowRegistrar _sessionWindowRegistrar;
        private readonly TaskbarIcon _taskbarIcon;

        public UploadReminder(SettingsStore settingsStore, NotifyTrayIcon notify, SessionManagerWindowRegistrar sessionWindowRegistrar)
        {
            _taskbarIcon = notify.NotifyIcon;
            _settingsStore = settingsStore;
            _sessionWindowRegistrar = sessionWindowRegistrar;

            InitLastUpdateTime(settingsStore);

            //Start timer every hour + offset
            //_timer = new Timer(1000*60*60 + new Random().Next(0, 60));
            _timer = new Timer(1000 * 60 * 1);
            _timer.Elapsed += ExecuteOnceAnHour;
            _timer.Enabled = true;

            //_taskbarIcon.ShowCustomBalloon(new BalloonNotification(_sessionWindowRegistrar, SessionManagerActionId, false), PopupAnimation.Slide, null); 
        }

        private void InitLastUpdateTime(SettingsStore settingsStore)
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            if (settings.LastUploadTime.IsEmpty())
            {
                settings.LastUploadTime = DateTime.Today.ToString(CultureInfo.InvariantCulture);
                settingsStore.SetSettings(settings);
            }
        }

        private void ExecuteOnceAnHour(Object obj, ElapsedEventArgs elapsedEventArgs)
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            var lastUpdate = DateTime.Parse(settings.LastUploadTime);
            //Hard popup
            if (lastUpdate >= DateTime.Today.AddMonths(1))
            {
                _taskbarIcon.ShowCustomBalloon(new BalloonNotification(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId, false), PopupAnimation.Slide, null); 
            }
            //Soft popup, because date is older than a week
            else if (lastUpdate >= DateTime.Today.AddDays(7))
            {
                _taskbarIcon.ShowCustomBalloon(new BalloonNotification(_sessionWindowRegistrar, SessionManagerWindowActionHandler.ActionId, true), PopupAnimation.Slide, null); 
            }  
        }
    }
}
