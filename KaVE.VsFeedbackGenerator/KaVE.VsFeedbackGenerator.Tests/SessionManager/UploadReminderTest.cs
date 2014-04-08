using System;
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
            GivenDaysPassedSinceLastNotification(10);
            GivenDaysPassedSinceLastUpload(10);

            UploadSettings newSettings = null;
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                              .Callback<UploadSettings>(settings => newSettings = settings);

            WhenUploadReminderIsInitialized();

            _mockSettingsStore.Verify(ss => ss.SetSettings(newSettings), Times.Never);
        }

        [Test]
        public void ShouldRegisterCallbackWithOneDayDelay()
        {
            var inOneDay = DateTime.Now.AddDays(1);
            GivenDaysPassedSinceLastNotification(0);

            WhenUploadReminderIsInitialized();

            _mockCallbackManager.Verify(
                manager =>
                    manager.RegisterCallback(
                        It.IsAny<Action>(),
                        It.IsInRange(inOneDay, inOneDay.AddSeconds(5), Range.Inclusive),
                        It.IsAny<Action>()));
        }

        [Test]
        public void ShouldOpenSoftNotificationAfterOneDayWithoutNotificationOrUpload()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(1);
            GivenDaysPassedSinceLastUpload(1);

            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup());
        }

        [Test]
        public void ShouldOpenHardNotificationAfterOneWeekWithoutUpload()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(1);
            GivenDaysPassedSinceLastUpload(7);

            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup());
        }

        [Test]
        public void ShouldNotOpenNotificationIfUploadedToday()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(1);
            GivenDaysPassedSinceLastUpload(0);
            
            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup(), Times.Never);
            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup(), Times.Never);
        }

        [Test]
        public void ShouldNotUpdateLastNotificationDateIfNoNotificationDueToRecentUpload()
        {
            GivenCallbackManagerInvokesCallbackImmediately();
            GivenDaysPassedSinceLastNotification(1);
            GivenDaysPassedSinceLastUpload(0);
            var expected = _uploadSettings.LastNotificationDate;

            WhenUploadReminderIsInitialized();

            var actual = _uploadSettings.LastNotificationDate;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRegisterFinishedActionToReScheduleNotification()
        {
            Action rescheduleAction = null;
            var actual = new DateTime();
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>(
                                    (callback, nextDateTimeToInvoke, finish) =>
                                    {
                                        actual = nextDateTimeToInvoke;
                                        rescheduleAction = finish;
                                    });

            WhenUploadReminderIsInitialized();
            rescheduleAction();

            var expected = _uploadSettings.LastNotificationDate.AddDays(1);
            Assert.AreEqual(expected, actual);
        }


        private void GivenCallbackManagerInvokesCallbackImmediately()
        {
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>((callback, date, finish) => callback());
        }

        private void GivenDaysPassedSinceLastNotification(int days)
        {
            _uploadSettings.LastNotificationDate = DateTime.Now.AddDays(-days);
        }

        private void GivenDaysPassedSinceLastUpload(int days)
        {
            _uploadSettings.LastUploadDate = DateTime.Now.AddDays(-days);
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object);
        }
    }
}