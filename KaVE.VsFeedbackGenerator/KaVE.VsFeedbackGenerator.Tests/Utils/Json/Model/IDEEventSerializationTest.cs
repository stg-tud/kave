using System;
using KaVE.Model.Events;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    class IDEEventSerializationTest
    {
        [Test]
        public void ShouldSerializeAllFieldsOfEvent()
        {
            var ideEvent = new InfoEvent()
            {
                IDESessionUUID = "0xDEADBEEF",
                ActiveDocument = DocumentName.Get("Random"),
                ActiveWindow = WindowName.Get("Random"),
                TriggeredBy = IDEEvent.Trigger.Click,
                TriggeredAt = DateTime.Now,
                Duration = TimeSpan.FromSeconds(5)
            };
            
            JsonAssert.SerializationPreservesData(ideEvent);
        }

        [Test]
        public void ShouldDeserializeFromWithDurationSet()
        {
            var triggeredAt = new DateTime(2014, 5, 5, 12, 0, 2, 69, DateTimeKind.Local);
            var terminatedAt = new DateTime(2014, 5, 5, 12, 0, 7, 69, DateTimeKind.Local);
            var duration = TimeSpan.FromSeconds(5);
            var eventJson = string.Format("{{\"TriggeredAt\":\"2014-05-05T12:00:02.069000+02:00\",\"Duration\":\"00:00:05\"}}");

            var ideEvent = eventJson.ParseJsonTo<InfoEvent>();

            Assert.AreEqual(duration, ideEvent.Duration);
            Assert.AreEqual(triggeredAt, ideEvent.TriggeredAt);
            Assert.AreEqual(terminatedAt, ideEvent.TerminatedAt);
        }

        [Test, Ignore("see todo comment in IDEEvent constructor")]
        public void ShouldNotHaveStartTimeWhenNotSerialized()
        {
            var infoEvent = "{}".ParseJsonTo<InfoEvent>();

            Assert.IsNull(infoEvent.TriggeredAt);
        }
    }
}
