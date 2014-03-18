using System;
using KaVE.Utils.DateTime;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture, RequiresSTA]
    internal class UploadReminderTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private UploadSettings _uploadSettings;
        private Mock<NotifyTrayIcon> _mockTrayIcon;
        private Mock<ICallbackManager> _mockCallbackManager;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _uploadSettings = new UploadSettings();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockTrayIcon = new Mock<NotifyTrayIcon>();
            _mockCallbackManager = new Mock<ICallbackManager>();
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

        [Test]
        public void ShouldRegisterCallbackWith7DaysDelay()
        {
            var now = DateTime.Now;
            _uploadSettings.LastNotificationDate = now;

            WhenUploadReminderIsInitialized();

            var inSevenDays = now.AddDays(7);

            _mockCallbackManager.Verify(
                manager =>
                    manager.RegisterCallback(
                        It.IsAny<Action>(),
                        It.IsInRange(inSevenDays, inSevenDays.AddSeconds(5), Range.Inclusive)));
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object, null);
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