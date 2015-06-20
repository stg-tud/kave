/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * Contributors:
 *    - Uli Fahrer
 *    - Sven Amann
 */

using System;
using KaVE.Commons.TestUtils.Utils;
using KaVE.ReSharper.Commons.Utils;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.TrayNotification;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [RequiresSTA]
    internal class UploadReminderTest
    {
        private Mock<ISettingsStore> _mockSettingsStore;
        private UploadSettings _uploadSettings;
        private UploadSettings _updatedUploadSettings;

        private Mock<NotifyTrayIcon> _mockTrayIcon;

        private Mock<ICallbackManager> _mockCallbackManager;
        private DateTime _registeredInvocationDate;
        private Action _registeredRescheduleAction;

        private TestDateUtils _dateUtils;
        private Mock<ILogManager> _mockLogManager;

        [SetUp]
        public void SetUpSettings()
        {
            _uploadSettings = new UploadSettings();
            _uploadSettings.Initialize();

            _mockSettingsStore = new Mock<ISettingsStore>();
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(_uploadSettings);
            _mockSettingsStore.Setup(store => store.SetSettings(It.IsAny<UploadSettings>()))
                              .Callback<UploadSettings>(settings => _updatedUploadSettings = settings);
        }

        [SetUp]
        public void SetUpCallbackManager()
        {
            _mockCallbackManager = new Mock<ICallbackManager>();
            _mockCallbackManager.Setup(
                mgr => mgr.RegisterCallback(It.IsAny<Action>(), It.IsAny<DateTime>(), It.IsAny<Action>()))
                                .Callback<Action, DateTime, Action>(
                                    (callback, date, finish) =>
                                    {
                                        _registeredInvocationDate = date;
                                        _registeredRescheduleAction = finish;
                                        callback();
                                    });
        }

        [SetUp]
        public void SetUp()
        {
            _mockLogManager = new Mock<ILogManager>();
            _mockTrayIcon = new Mock<NotifyTrayIcon>(_mockLogManager.Object);
            _dateUtils = new TestDateUtils();
        }

        private void GivenLogSizeInBytesIsBigEnoughToShowReminder()
        {
            GivenLogsSizeInBytesIs(1024*1024);
        }

        private void GivenLogsSizeInBytesIs(int logsSizeInBytes)
        {
            _mockLogManager.Setup(lm => lm.LogsSizeInBytes).Returns(logsSizeInBytes);
        }

        private static DateTime CreateDateWithDayAndHour(int day, int hour)
        {
            return new DateTime(1970, 1, day, hour, 0, 0);
        }

        private void WhenUploadReminderIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new UploadReminder(
                _mockSettingsStore.Object,
                _mockTrayIcon.Object,
                _mockCallbackManager.Object,
                _dateUtils,
                _mockLogManager.Object);
        }

        private void ThenNoPopupIsOpened()
        {
            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup(), Times.Never);
            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup(), Times.Never);
        }

        [Test]
        public void ShouldInitializeSettingsIfNotInitialized()
        {
            _mockSettingsStore.Setup(store => store.GetSettings<UploadSettings>()).Returns(new UploadSettings());

            WhenUploadReminderIsInitialized();

            Assert.IsTrue(_updatedUploadSettings.IsInitialized());
        }

        [Test]
        public void ShouldNotInitializeSettingsIfAlreadyInitialized()
        {
            _dateUtils.Now = CreateDateWithDayAndHour(1, 0);
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(11, 0);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(11, 0);

            WhenUploadReminderIsInitialized();

            _mockSettingsStore.Verify(ss => ss.SetSettings(_updatedUploadSettings), Times.Never);
        }

        [Test]
        public void ShouldOpenSoftNotificationAfterOneDayWithoutNotificationOrUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 14);
            GivenLogSizeInBytesIsBigEnoughToShowReminder();

            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowSoftBalloonPopup());
        }

        [Test]
        public void ShouldOpenHardNotificationAfterOneWeekWithoutUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(7, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(8, 14);
            GivenLogSizeInBytesIsBigEnoughToShowReminder();

            WhenUploadReminderIsInitialized();

            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup());
        }

        [Test]
        public void ShouldOpenHardNotificationInWorkingHoursOnTheNextDayIfWorkingHoursAreAlreadyOver()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(9, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(10, 18);
            GivenLogSizeInBytesIsBigEnoughToShowReminder();

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(11, 10);
            var expectedMax = CreateDateWithDayAndHour(11, 16);

            _mockTrayIcon.Verify(ti => ti.ShowHardBalloonPopup());

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        public void ShouldNotOpenNotificationIfUploadedToday()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(2, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 14);

            WhenUploadReminderIsInitialized();

            ThenNoPopupIsOpened();
        }

        [Test]
        public void ShouldNotOpenNotificationIfLogsAreTooSmall()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 14);
            GivenLogsSizeInBytesIs(100*1024);

            WhenUploadReminderIsInitialized();

            ThenNoPopupIsOpened();
        }

        [Test]
        public void ShouldNotUpdateLastNotificationDateIfNoNotificationDueToRecentUpload()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _uploadSettings.LastUploadDate = CreateDateWithDayAndHour(2, 12);
            var expected = _uploadSettings.LastNotificationDate;
            _dateUtils.Now = CreateDateWithDayAndHour(2, 14);

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
        public void ShouldRegisterCorrectTime_LastNotificationOnThePreviousDayAndCurrentTimeIsBeforeWorkingHours()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 8);

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 10);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        public void ShouldRegisterCorrectTime_LastNotificationOnThePreviousDayAndCurrentTimeIsInWorkingHours()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 14);

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 14);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        public void
            ShouldRegisterCorrectTime_LastNotificationOnThePreviousDayOutSideWorkingHourAndCurrentTimeIsInWorkingHours()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 18);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 13);

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(2, 10);
            var expectedMax = CreateDateWithDayAndHour(2, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        public void ShouldRegisterCorrectTime_LastNotificationOnThePreviousDayAndCurrentTimeIsAfterWorkingHours()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 17);

            WhenUploadReminderIsInitialized();

            var actual = _registeredInvocationDate;
            var expectedMin = CreateDateWithDayAndHour(3, 10);
            var expectedMax = CreateDateWithDayAndHour(3, 16);

            AssertDateBetween(expectedMin, expectedMax, actual);
        }

        [Test]
        public void FinishActionShouldReschedule()
        {
            _uploadSettings.LastNotificationDate = CreateDateWithDayAndHour(1, 12);
            _dateUtils.Now = CreateDateWithDayAndHour(2, 8);

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
    }
}