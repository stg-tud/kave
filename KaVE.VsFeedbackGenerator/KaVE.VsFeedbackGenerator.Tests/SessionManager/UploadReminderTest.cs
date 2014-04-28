using System;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;
using TestDateUtils = KaVE.VsFeedbackGenerator.Tests.Utils.TestDateUtils;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture, RequiresSTA]
    internal class UploadReminderTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private UploadSettings _uploadSettings;
        private Mock<NotifyTrayIcon> _mockTrayIcon;
        private Mock<ICallbackManager> _mockCallbackManager;

        private DateTime _registeredInvocationDate;
        private Action _registeredRescheduleAction;

        private TestDateUtils _dateUtils;
        private UploadSettings _newUploadSettings;

        [SetUp]
        public void SetUp()
        {
            _uploadSettings = new UploadSettings();
            _uploadSettings.Initialize();

            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                  .Callback<UploadSettings>(settings => _newUploadSettings = settings);

            _mockTrayIcon = new Mock<NotifyTrayIcon>();

            _dateUtils = new TestDateUtils();

            _mockCallbackManager = new Mock<ICallbackManager>();
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>(
                                    (callback, nextDateTimeToInvoke, finish) =>
                                    {
                                        _registeredInvocationDate = nextDateTimeToInvoke;
                                        _registeredRescheduleAction = finish;
                                    });
        }

        private void MockStoreToReturnUninitializedSettings()
        {
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(new UploadSettings());
        }

        private static DateTime CreateDateWithDayAndHour(int day, int hour)
        {
            var relativeDate = new DateTime(1970, 1, day, hour, 1, 0);
            return relativeDate;
        }

        public void SetNow(DateTime date)
        {
            _dateUtils.Now = date;
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(_mockSettingsStore.Object, _mockTrayIcon.Object, _mockCallbackManager.Object, _dateUtils);
        }

        [Test]
        public void ShouldInitializeSettingsIfNotInitialized()
        {
            MockStoreToReturnUninitializedSettings();

            WhenUploadReminderIsInitialized();

            Assert.IsTrue(_newUploadSettings.IsInitialized());
            _mockSettingsStore.Verify(ss => ss.SetSettings(_newUploadSettings));
        }

        [Test]
        public void ShouldNotInitializeSettingsIfAlreadyInitialized()
        {
            _dateUtils.Now = CreateDateWithDayAndHour(1, 0);
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(11, 0);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(11, 0);

            WhenUploadReminderIsInitialized();

            _mockSettingsStore.Verify(ss => ss.SetSettings(_newUploadSettings), Times.Never);
        }

        [Test]
        public void ShouldOpenSoftNotificationAfterOneDayWithoutNotificationOrUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(2, 14));

            GivenCallbackManagerInvokesCallbackImmediately();
            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup());
        }

        [Test]
        public void ShouldOpenHardNotificationAfterOneWeekWithoutUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(7, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(8, 14));

            GivenCallbackManagerInvokesCallbackImmediately();
            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup());
        }

        [Test]
        public void ShouldNotOpenNotificationIfUploadedToday()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(2, 12);
            SetNow(CreateDateWithDayAndHour(2, 14));

            GivenCallbackManagerInvokesCallbackImmediately();
            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup(), Times.Never);
            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup(), Times.Never);
        }

        [Test]
        public void ShouldNotUpdateLastNotificationDateIfNoNotificationDueToRecentUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(2, 12);
            SetNow(CreateDateWithDayAndHour(2, 14));

            var expected = _uploadSettings.LastNotificationDate;

            GivenCallbackManagerInvokesCallbackImmediately();
            WhenUploadReminderIsInitialized();

            var actual = _uploadSettings.LastNotificationDate;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRegisterFinishedActionToReScheduleNotification()
        {
            var yesterdayOutsideWorkingHours = CreateDateWithDayAndHour(1, 0);
            _uploadSettings.LastNotificationDate = yesterdayOutsideWorkingHours;

            WhenUploadReminderIsInitialized();
            _registeredRescheduleAction();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 10);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        // TODO @Uli: think about nice name
        public void ShouldRegisterCorrectTime_asdaldkja()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(2, 8));

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 10);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        // TODO @Uli: think about nice name
        public void ShouldRegisterCorrectTime_asdaldkjaasd()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(2, 14));

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 14);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        // TODO @Uli: think about nice name
        public void ShouldRegisterCorrectTime_asdaldkjafrgasd()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(2, 17));

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(3, 10);
            var expectedMax = CreateDateWithDayAndHour(3, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        // TODO @Uli: think about additional tests for notification date / upload date combinations

        [Test]
        public void FinishActionShouldReschedule()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            SetNow(CreateDateWithDayAndHour(2, 8));

            WhenUploadReminderIsInitialized();
            _registeredRescheduleAction();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 10);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        private static void AssertDateBetween(DateTime expectedMin, DateTime expectedMax, DateTime actual)
        {
            var isInRange = actual >= expectedMin && actual <= expectedMax;
            Assert.True(isInRange);
        }

        private void GivenCallbackManagerInvokesCallbackImmediately()
        {
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>((callback, date, finish) => callback());
        }
    }
}