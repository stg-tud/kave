using System;
using System.Timers;
using System.Globalization;
using JetBrains.Application;
using JetBrains.Threading;
using JetBrains.Util;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.VsFeedbackGenerator.VsIntegration
{
    [ShellComponent]
    public class UploadReminder
    {
        private Timer _timer;
        private readonly SettingsStore _settingsStore;
        private readonly SessionManagerWindowRegistrar _sessionWindow;
        private const string SessionManagerActionId = "KaVE.VsFeedbackGenerator.SessionManager";

        public UploadReminder(SettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            _sessionWindow = Shell.Instance.GetComponent<SessionManagerWindowRegistrar>();

            //Init today date for settings 
            var settings = _settingsStore.GetSettings<UploadSettings>();
            if (settings.LastUploadTime.IsEmpty())
            {
                settings.LastUploadTime = DateTime.Today.ToString(CultureInfo.InvariantCulture);
                settingsStore.SetSettings(settings);
            }

            //Start timer every hour + offset
            _timer = new Timer(1000*60*60 + new Random().Next(0, 60));
            _timer.Elapsed += new ElapsedEventHandler(ExecuteOnceAnHour);
            _timer.Enabled = true;
        }

        private void ExecuteOnceAnHour(Object obj, ElapsedEventArgs elapsedEventArgs)
        {
            var settings = _settingsStore.GetSettings<UploadSettings>();
            var lastUpdate = DateTime.Parse(settings.LastUploadTime);
            //Soft popup, because date is older than a week
            if (lastUpdate > DateTime.Today.AddDays(7))
            {
                var result = MessageBox.ShowYesNoCancel("Upload it or die");
                if (result == true)
                {
                    OpenSessionManager();
                }
            }
            //Hard popup
            else if (lastUpdate > DateTime.Today.AddMonths(1))
            {
                MessageBox.ShowExclamation("UPLOAD!!!!!!!!!!!!!!");
                OpenSessionManager();
            }
        }

        private void OpenSessionManager()
        {
            Invoke.OnDispatcherAsync(
                    () => ReentrancyGuard.Current.Execute(SessionManagerActionId,
                        () => _sessionWindow.ToolWindow.Show()));
        }

    }
}
