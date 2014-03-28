using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Mock<INotifyTrayIcon> _mockTrayIcon;
        private Mock<ICallbackManager> _mockCallbackManager;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _uploadSettings = new UploadSettings();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockTrayIcon = new Mock<INotifyTrayIcon>();
            _mockCallbackManager = new Mock<ICallbackManager>();
        }

        [Test]
        public void ShouldInitializeSettingsIfNotInitialized()
        {
            UploadSettings newSettings = null;
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                              .Callback<UploadSettings>(settings => newSettings = settings);

            WhenUploadReminderIsInitialized();

            Assert.IsTrue(newSettings.IsInitialized());
            _mockSettingsStore.Verify(ss => ss.SetSettings(newSettings));
        }

        [Test]
        public void ShouldNotInitializeSettingsIfAlreadyInitialized()
        {
            var originalDate = DateTime.Now.AddSeconds(-10);
            _uploadSettings.LastNotificationDate = originalDate;
            _uploadSettings.LastUploadDate = originalDate;

            UploadSettings newSettings = null;
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                              .Callback<UploadSettings>(settings => newSettings = settings);

            WhenUploadReminderIsInitialized();

            _mockSettingsStore.Verify(ss => ss.SetSettings(newSettings), Times.Never);
        }

        [Test]
        public void ShouldRegisterCallbackWithOneDayDelay()
        {
            var now = DateTime.Now;
            _uploadSettings.LastNotificationDate = now;

            WhenUploadReminderIsInitialized();

            var inOneDay = now.AddDays(1);

            _mockCallbackManager.Verify(
                manager =>
                    manager.RegisterCallback(
                        It.IsAny<Action>(),
                        It.IsInRange(inOneDay, inOneDay.AddSeconds(5), Range.Inclusive),
                        It.IsAny<Action>()));
        }

        [Test]
        public void ShouldOpenSoftNotificationWhenNotAWeekElapsed()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(1);
            var futureNotificationControl = GivenFutureNotificationControl();

            WhenUploadReminderIsInitialized();

            var expected = typeof(SoftBalloonPopup);
            AssertNotificationType(futureNotificationControl(), expected);
        }

        [Test]
        public void ShouldOpenHardNotificationAfterAWeek()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(7);
            var futureNotificationControl = GivenFutureNotificationControl();

            WhenUploadReminderIsInitialized();

            var expected = typeof(HardBalloonPopup);
            AssertNotificationType(futureNotificationControl(), expected);
        }

        // TODO add testcase that ensures re-scheduling of notification (to check that finishedAction is set correctly)

        private void GivenCallbackManagerInvokesCallbackImmediately()
        {
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>((callback, date, finish) => callback.Invoke());
        }

        private void GivenDaysPassedSinceLastNotification(int value)
        {
            var yesterday = DateTime.Now.AddDays(-value);
            _uploadSettings.LastUploadDate = yesterday;
            _uploadSettings.LastNotificationDate = yesterday;
        }

        private Func<UserControl> GivenFutureNotificationControl()
        {
            UserControl actual = null;
            _mockTrayIcon.Setup(
                notification =>
                    notification.ShowCustomNotification(It.IsAny<UserControl>(), PopupAnimation.Slide, null))
                         .Callback<UserControl, PopupAnimation, int?>((control, animation, time) => actual = control);
            return () => actual;
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object, null);
        }

        private static void AssertNotificationType(UserControl actual, Type expected)
        {
            Assert.NotNull(actual);
            Assert.IsInstanceOf(expected, actual);
        }
    }
}