using System;
using KaVE.Utils.DateTime;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    class UploadReminderTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private UploadSettings _uploadSettings;
        private Mock<NotifyTrayIcon> _mockTrayIcon;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _uploadSettings = new UploadSettings();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockTrayIcon = new Mock<NotifyTrayIcon>();

        }

        [Test]
        public void ShouldInitializeSettings()
        {
            UploadSettings newSettings = null;
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                             .Callback<UploadSettings>(settings => newSettings = settings);

            WhenUploadReminderIsInitialized();

            AssertUploadSettings(newSettings);
        }

        // TODO bitte fixen, Uli ;P
        [Test, Ignore]
        public void ShouldRegisterCallbackWith7DaysDelay()
        {
            _uploadSettings.LastNotificationDate = DateTime.Now;
            var mockCallbackManager = new Mock<CallbackManager>();

            WhenUploadReminderIsInitialized();

            mockCallbackManager.Verify(manager => manager.RegisterCallback(It.IsAny<Action>(), -1));
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, null);
        }


        private static void AssertUploadSettings(UploadSettings newSettings)
        {
            Assert.IsNotNull(newSettings);
            var now = DateTime.Now;
            var comparer = new SimilarDateTimeComparer(1000);
            Assert.IsTrue(comparer.Equal(now, newSettings.LastNotificationDate));
            Assert.IsTrue(comparer.Equal(now, newSettings.LastUploadDate));
        }
    }
}
