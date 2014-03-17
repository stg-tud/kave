using System;
using JetBrains.Application.Settings;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    // ReSharper disable CSharpWarnings::CS0612
    [SettingsKey(typeof(System.Reflection.Missing), "KaVE Upload Settings")]
    public class UploadSettings
    {
        // TODO can R# store DateTimes to save the manual convertion
        [SettingsEntry(0, "Notification Time"), Obsolete]
        public long LastNotificationTicks { get; set; }

        [SettingsEntry(0, "Upload Time"), Obsolete]
        public long LastUploadTicks { get; set; }

        [SettingsEntry(false, "Do Not Show Again Flag")]
        public bool DoNotShowNotificationCloseDialogAgain { get; set; }

        public DateTime LastNotificationDate
        {
            get { return Parse(LastNotificationTicks); }

            set { LastNotificationTicks = Serialize(value); }
        }

        public DateTime LastUploadDate
        {
            get { return Parse(LastUploadTicks); }

            set { LastUploadTicks = Serialize(value); }
        }

        private static DateTime Parse(long dateTimeSerialization)
        {
            return new DateTime(dateTimeSerialization);
        }

        private static long Serialize(DateTime dateTime)
        {
            return dateTime.Ticks;
        }

        public bool IsInitialized()
        {
            var hasUninitializedField = LastNotificationTicks == 0 ||
                                        LastUploadTicks == 0;
            return !hasUninitializedField;
        }

        public void Initialize()
        {
            var now = DateTime.Now;
            LastUploadDate = now;
            LastNotificationDate = now;
        }
    }
    // ReSharper restore CSharpWarnings::CS0612
}