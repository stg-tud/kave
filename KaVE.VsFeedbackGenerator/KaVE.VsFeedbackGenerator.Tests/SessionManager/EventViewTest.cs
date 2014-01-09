using System;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class EventViewTest
    {
        [Test]
        public void ShouldIncludeOnlySpecializedEventFieldsInDetails()
        {
            var @event = new CommandEvent
            {
                IDESessionUUID = "TestSessionUUID",
                TriggeredAt = new DateTime(2014, 1, 9, 10, 8, 0),
                TriggeredBy = IDEEvent.Trigger.Unknown,
                ActiveWindow = WindowName.Get("TestWindow"),
                ActiveDocument = DocumentName.Get("C:\\test.doc"),
                Command = CommandName.Get("test.command"),
                TerminatedAt = new DateTime(2014, 1, 9, 10, 22, 0),
            };

            var eventView = new EventView(@event);
            var actualDetails = eventView.Details;

            Assert.AreEqual(
                "  \"Command\": \"test.command\"",
                actualDetails);
        }


        [Test]
        public void ShouldReturnCorrectDetailsIfSomeGeneralEventInformationAreNotSet()
        {
            var @event = new WindowEvent
            {
                IDESessionUUID = "TestSessionUUID",
                TriggeredAt = new DateTime(2014, 1, 9, 10, 8, 0),
                TriggeredBy = IDEEvent.Trigger.Unknown,
                ActiveWindow = WindowName.Get("TestWindow"),
                Window = WindowName.Get("TestWindow"),
                Action = WindowEvent.WindowAction.Close,
            };

            var eventView = new EventView(@event);
            var actualDetails = eventView.Details;

            Assert.AreEqual(
                "  \"Window\": \"TestWindow\",\r\n" +
                "  \"Action\": \"Close\"",
                actualDetails);
        }
    }
}