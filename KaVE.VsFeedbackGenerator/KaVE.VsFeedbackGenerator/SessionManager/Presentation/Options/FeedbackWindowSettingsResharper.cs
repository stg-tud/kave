using JetBrains.Application.Communication;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation.Options
{
    [SettingsKey(typeof(InternetSettings), "FeedbackWindow Settings")]
    class FeedbackWindowSettingsResharper : FeedbackGeneratorSettings
    {
        [SettingsEntry("True", "FeedbackGenerator Names")]
        public new bool FeedbackGeneratorNames
        {
            get { return base.Names; }
            set { base.Names = value; }
        }

        [SettingsEntry("True", "FeedbackGenerator Duration")]
        public new bool FeedbackGeneratorDuration
        {
            get { return base.Duration;  }
            set { base.Duration = value;  }
        }

        [SettingsEntry("True", "FeedbackGenerator StartTime")]
        public new bool FeedbackGeneratorStartTime
        {
            get { return base.StartTime; }
            set { base.StartTime = value; }
        }

        [SettingsEntry("True", "FeedbackGenerator SessioIDs")]
        public new bool FeedbackGeneratorSessionIDs
        {
            get { return base.SessionIDs; }
            set { base.SessionIDs = value; }
        }
    }
}
