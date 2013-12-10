using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation.Options
{
    [SettingsKey(typeof(EnvironmentSettings), "FeedbackWindow Settings")]
    internal class FeedbackGeneratorResharperSettings : FeedbackGeneratorSettings
    {
        [SettingsEntry("True", "KaVE FeedbackGenerator Names")]
        public bool? FeedbackGeneratorNames
        {
            get { return Names; }
            set { Names = value; }
        }

        [SettingsEntry("True", "KaVE FeedbackGenerator Duration")]
        public bool? FeedbackGeneratorDuration
        {
            get { return Duration; }
            set { Duration = value; }
        }

        [SettingsEntry("True", "KaVE FeedbackGenerator StartTime")]
        public bool? FeedbackGeneratorStartTime
        {
            get { return StartTime; }
            set { StartTime = value; }
        }

        [SettingsEntry("True", "KaVE FeedbackGenerator SessionIDs")]
        public bool? FeedbackGeneratorSessionIDs
        {
            get { return SessionIDs; }
            set { SessionIDs = value; }
        }
    }
}