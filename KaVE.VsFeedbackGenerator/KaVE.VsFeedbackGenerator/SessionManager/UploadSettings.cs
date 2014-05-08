using System;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    [SettingsKey(typeof(FeedbackSettings), "KaVE Feeback-Upload Settings")]
    // WARNING: Do not change classname, as it is used to identify settings
    public class UploadSettings
    {
        private const string DateTimeMinValue = "01/01/0001 00:00:00";

        [SettingsEntry(false, "Wheather or not to show a confirmation dialog when closing the upload-reminder popup.")]
        public bool DoNotShowNotificationCloseDialogAgain { get; set; }

        [SettingsEntry(DateTimeMinValue, "Timestamp of the last time the upload-reminder popup was shown to the user.")]
        public DateTime LastNotificationDate { get; set; }

        [SettingsEntry(DateTimeMinValue, "Timestamp of the last time an export was done.")]
        public DateTime LastUploadDate { get; set; }

        public bool IsInitialized()
        {
            var hasUninitializedField = LastNotificationDate == DateTime.MinValue ||
                                        LastUploadDate == DateTime.MinValue;
            return !hasUninitializedField;
        }

        public void Initialize()
        {
            var now = DateTime.Now;
            LastUploadDate = now;
            LastNotificationDate = now;
        }
    }
}