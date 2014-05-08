using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [SettingsKey(typeof (FeedbackSettings), "Kave Feedback-Export Settings")]
    // WARNING: Do not change classname, as it is used to identify settings
    internal class ExportSettings
    {
        [SettingsEntry(false, "KaVE FeedbackGeneration RemoveCodeNames")]
        public bool RemoveCodeNames { get; set; }

        [SettingsEntry(false, "KaVE FeedbackGenerator RemoveDurations")]
        public bool RemoveDurations { get; set; }

        [SettingsEntry(false, "KaVE FeedbackGenerator RemoveStartTimes")]
        public bool RemoveStartTimes { get; set; }

        [SettingsEntry(false, "KaVE FeedbackGenerator RemoveSessionIDs")]
        public bool RemoveSessionIDs { get; set; }
    }
}