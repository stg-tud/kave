using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [SettingsKey(typeof (KaVESettings), "Root node of all settings of the KaVE feedback extension")]
    // WARNING: Do not change classname, as it is used to identify settings
    internal class FeedbackSettings {}
}