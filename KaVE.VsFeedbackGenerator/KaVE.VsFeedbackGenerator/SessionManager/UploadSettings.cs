using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [SettingsKey(typeof(System.Reflection.Missing), "KaVE Upload Settings")]
    public class UploadSettings
    {
        [SettingsEntry("2012-02-13", "Upload Time")]
        public string LastUploadTime { get; set; }
    }
}
