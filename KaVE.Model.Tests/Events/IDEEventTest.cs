using System;
using KaVE.Model.Events;
using NUnit.Framework;

namespace KaVE.Model.Tests.Events
{
    [TestFixture]
    class IDEEventTest
    {
        [Test]
        public void ShouldDeriveDurationFromStartAndEndTime()
        {
            var expected = TimeSpan.FromSeconds(1);
            var now = DateTime.Now;
            var infoEvent = new InfoEvent
            {
                TriggeredAt = now,
                TerminatedAt = now.AddSeconds(1)
            };

            Assert.AreEqual(expected, infoEvent.Duration);
        }

        [Test]
        public void ShouldDeriveEndTimeFromStartTimeAndDuration()
        {
            var now = DateTime.Now;
            var expected = now.AddSeconds(3);
            var infoEvent = new InfoEvent
            {
                TriggeredAt = now,
                Duration = TimeSpan.FromSeconds(3)
            };

            Assert.AreEqual(expected, infoEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutDuration()
        {
            var infoEvent = new InfoEvent
            {
                TriggeredAt = DateTime.Now,
                Duration = null
            };

            Assert.IsNull(infoEvent.TerminatedAt);
        }

        [Test]
        public void ShouldHaveNoEndTimeWithoutStartTime()
        {
            var infoEvent = new InfoEvent
            {
                TriggeredAt = null,
                Duration = TimeSpan.FromMinutes(9)
            };

            Assert.IsNull(infoEvent.TerminatedAt);
        }
    }
}
