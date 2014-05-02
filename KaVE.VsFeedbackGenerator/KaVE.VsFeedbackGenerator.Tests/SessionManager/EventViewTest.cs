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
        public void ShouldDisplayFormattedEventDetails()
        {
            var @event = new CommandEvent {Command = CommandName.Get("test.command")};
            const string expected =
                "    <Bold Foreground=\"Blue\">\"Command\":</Bold> <Span Foreground=\"Blue\">\"test.command\"</Span>";

            var view = new EventView(@event);
            var actual = view.Details;

            Assert.AreEqual(expected, actual);
        }
    }
}