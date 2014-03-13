using System;
using KaVE.Utils.DateTime;
using KaVE.VsFeedbackGenerator.SessionManager;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class UploadSettingsTest
    {
        [Test]
        public void ShouldBeInvalidWithoutData()
        {
            var uut = new UploadSettings();

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeInvalidWithoutLastNotificationTime()
        {
            var uut = new UploadSettings {LastUploadDate = DateTime.Now};

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeInvalidWithoutLastUploadTime()
        {
            var uut = new UploadSettings {LastNotificationDate = DateTime.Now};

            Assert.IsFalse(uut.IsInitialized());
        }

        [Test]
        public void ShouldBeValidWithAllData()
        {
            var uut = new UploadSettings {LastNotificationDate = DateTime.Now, LastUploadDate = DateTime.Now};

            Assert.IsTrue(uut.IsInitialized());
        }

        [Test]
        public void ShouldInitializeWithCurrentDate()
        {
            var uut = new UploadSettings();

            uut.Initialize();
            var dateTime = DateTime.Now;

            Assert.IsTrue(uut.IsInitialized());
            var comparer = new SimilarDateTimeComparer(50);
            Assert.IsTrue(comparer.Equal(dateTime, uut.LastNotificationDate));
            Assert.IsTrue(comparer.Equal(dateTime, uut.LastUploadDate));
        }
    }
}