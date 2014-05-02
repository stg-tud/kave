using System;
using KaVE.Model.Events;
using KaVE.Model.Events.ReSharper;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    class IDEEventDetailsToJsonConverterTest
    {
        [Test]
        public void ShouldConvertActionEventDetailsToXaml()
        {
            var actionEvent = new ActionEvent {ActionId = "MyActionId", ActionText = "My &Action"};
            const string expected = "    \"ActionId\": \"MyActionId\"\r\n" +
                                    "    \"ActionText\": \"My &Action\"";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotIncludeIDEEventPropertiesInDetails()
        {
            var actionEvent = new ActionEvent
            {
                ActiveDocument = DocumentName.Get("Doc"),
                ActiveWindow = WindowName.Get("Window"),
                IDESessionUUID = "UUID",
                TerminatedAt = DateTime.Now,
                TriggeredAt = DateTime.Now,
                TriggeredBy = IDEEvent.Trigger.Click
            };
            const string expected = "";
            var actual = actionEvent.GetDetailsAsJson();
            Assert.AreEqual(expected, actual);
        }
    }
}
