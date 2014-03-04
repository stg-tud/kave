using System;
using System.Globalization;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [SettingsKey(typeof(System.Reflection.Missing), "KaVE Upload Settings")]
    public class UploadSettings
    {
        [SettingsEntry("", "Notification Time")]
        public string LastNotificationTime { get; set; }

        [SettingsEntry("", "Upload Time")]
        public string LastUploadTime { get; set; }

        public DateTime LastNotificationTimeAsDate
        {
            get
            {
                return DateTime.Parse(LastNotificationTime, CultureInfo.InvariantCulture);
            }
        }
    }
}
