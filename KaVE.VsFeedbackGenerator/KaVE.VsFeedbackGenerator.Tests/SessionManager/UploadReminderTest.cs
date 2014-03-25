using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private Mock<CallbackManager> _mockCallbackManager;

        [SetUp]
        public void SetUp()
        {
            _mockSettingsStore = new Mock<ISettingsStore>();
            _uploadSettings = new UploadSettings();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockTrayIcon = new Mock<NotifyTrayIcon>();
            _mockCallbackManager = new Mock<CallbackManager>();

            _mockCallbackManager.Setup(mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                              .Callback<Action, DateTime, Action>((callback, date, finish) => callback.Invoke());
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
                        It.IsInRange(inOneDay, inOneDay.AddSeconds(5), Range.Inclusive), It.IsAny<Action>()));
        }
        
        [Test]
        public void ShouldOpenSoftNotificationWhenNotAWeekElapsed()
        {
            //var actual = ReturnGivenNotificationControl();
            UserControl actual = null;
            _mockTrayIcon.Setup(
                notification =>
                    notification.ShowCustomNotification(It.IsAny<UserControl>(), PopupAnimation.Slide, null))
                         .Callback<UserControl, PopupAnimation, int?>((control, animation, time) => actual = control);


            var expected = typeof (SoftBalloonPopup);
            var yesterday = DateTime.Now.AddDays(-1);
            _uploadSettings.LastUploadDate = yesterday;
            _uploadSettings.LastNotificationDate = yesterday;

            WhenUploadReminderIsInitialized();
            AssertNotificationType(actual, expected);
        }

        [Test]
        public void ShouldOpenHardNotificationAfterAWeek()
        {
            //TODO: Outsourcing this will return null value ?!
            //var actual = ReturnGivenNotificationControl();
            UserControl actual = null;
            _mockTrayIcon.Setup(
                notification =>
                    notification.ShowCustomNotification(It.IsAny<UserControl>(), PopupAnimation.Slide, null))
                         .Callback<UserControl, PopupAnimation, int?>((control, animation, time) => actual = control);

            var expected = typeof(HardBalloonPopup);
            var weekAgo = DateTime.Now.AddDays(-7);
            _uploadSettings.LastUploadDate = weekAgo;
            _uploadSettings.LastNotificationDate = weekAgo;

            WhenUploadReminderIsInitialized();
            
            AssertNotificationType(actual, expected);
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object, null);
        }

        /*
        private UserControl ReturnGivenNotificationControl()
        {
            UserControl givenControl = null;

            _mockTrayIcon.Setup(
                notification =>
                    notification.ShowCustomNotification(It.IsAny<UserControl>(), PopupAnimation.Slide, null))
                         .Callback<UserControl, PopupAnimation, int?>((control, animation, time) => givenControl = control);
            
            return givenControl;
        }*/

        private static void AssertNotificationType(UserControl actual, Type expected)
        {
            Assert.NotNull(actual);
            Assert.IsInstanceOf(expected, actual);
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