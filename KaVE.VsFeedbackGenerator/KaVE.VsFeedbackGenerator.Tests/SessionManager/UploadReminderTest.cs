using System;
using JetBrains.Extension;
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
            var expected = CreateDateTimeRangeFromDateTime(inOneDay);
            
            GivenDaysPassedSinceLastNotification(0);
            WhenUploadReminderIsInitialized();

            _mockCallbackManager.Verify(
                manager =>
                    manager.RegisterCallback(
                        It.IsAny<Action>(),
                        It.IsInRange(expected.Item1, expected.Item2, Range.Inclusive),
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

            var dayAfterLastNotification = _uploadSettings.LastNotificationDate.AddDays(1);
            var expected = CreateDateTimeRangeFromDateTime(dayAfterLastNotification);

            Assert.True(DateIsBetween(actual, expected.Item1, expected.Item2));
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

        //TODO: Rename >.>
        private static Tuple<DateTime, DateTime> CreateDateTimeRangeFromDateTime(DateTime datetime)
        {
            var minDatetime = new DateTime(datetime.Year, datetime.Month, datetime.Day, 10, 0, 0);
            var maxDatetime = new DateTime(datetime.Year, datetime.Month, datetime.Day, 16, 0, 0);
            return new Tuple<DateTime, DateTime>(minDatetime, maxDatetime);
        }

        private static bool DateIsBetween(DateTime datetime, DateTime start, DateTime end)
        {
            return datetime >= start && datetime < end;
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object);
        }
    }
}