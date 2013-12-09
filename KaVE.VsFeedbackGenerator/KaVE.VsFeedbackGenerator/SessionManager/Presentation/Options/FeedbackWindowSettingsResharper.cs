using JetBrains.Application.Communication;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation.Options
{
    [SettingsKey(typeof (InternetSettings), "FeedbackWindow Settings")]
    internal class FeedbackWindowSettingsResharper : FeedbackGeneratorSettings
    {
        [SettingsEntry("True", "FeedbackGenerator Names")]
        public bool? FeedbackGeneratorNames
        {
            get { return Names; }
            set { Names = value; }
        }

        [SettingsEntry("True", "FeedbackGenerator Duration")]
        public bool? FeedbackGeneratorDuration
        {
            get { return Duration; }
            set { Duration = value; }
        }

        [SettingsEntry("True", "FeedbackGenerator StartTime")]
        public bool? FeedbackGeneratorStartTime
        {
            get { return StartTime; }
            set { StartTime = value; }
        }

        [SettingsEntry("True", "FeedbackGenerator SessioIDs")]
        public bool? FeedbackGeneratorSessionIDs
        {
            get { return SessionIDs; }
            set { SessionIDs = value; }
        }
    }
}