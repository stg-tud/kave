using System;
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
        private System.Threading.Timer _timer;
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

            _timer = CreateTimer(TimeSpan.FromMinutes(0.5), TimeSpan.FromSeconds(new Random().Next(0, 60)));
        }

        private System.Threading.Timer CreateTimer(TimeSpan intervall, TimeSpan minuteOffset)
        {
            return new System.Threading.Timer(ExecuteOnceAnHour, null, intervall.Add(minuteOffset), TimeSpan.Zero);
        }

        private void ExecuteOnceAnHour(Object obj)
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

            _timer = CreateTimer(TimeSpan.FromMinutes(0.5), TimeSpan.FromSeconds(new Random().Next(0, 60)));
        }

        private void OpenSessionManager()
        {
            Invoke.OnDispatcherAsync(
                    () => ReentrancyGuard.Current.Execute(SessionManagerActionId,
                        () => _sessionWindow.ToolWindow.Show()));
        }

    }
}
