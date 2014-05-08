using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [SettingsKey(typeof (FeedbackSettings), "FeedbackWindow Settings")]
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