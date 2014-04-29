using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class IDEEventToXamlFormattedJsonConverterTest
    {
        // TODO @Dennis: Mir fehlt ein Test der mal ein IDEEvent umwandelt. Der letzte Test wäre ein guter Kandidat hierfür.

        [Test]
        public void ShouldHighlightNull()
        {
            const string origin = "null";
            const string expected = "<Bold Foreground=\"Black\">null</Bold>";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("true"), TestCase("false")]
        public void ShouldHighlightBooleans(string origin)
        {
            var expected = "<Bold Foreground=\"Darkred\">" + origin + "</Bold>";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("0"), TestCase("17"), TestCase("-42"), TestCase("2.3"), TestCase("-3.14")]
        public void ShouldHighlightNumbers(string origin)
        {
            var expected = "<Span Foreground=\"Darkgreen\">" + origin + "</Span>";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightStrings()
        {
            const string origin = "\"Hello World!\"";
            const string expected = "<Span Foreground=\"Blue\">\"Hello World!\"</Span>";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightKeys()
        {
            const string origin =
                "\"That's a key because it's surrounded by double-quotes and is followed by a colon\":";
            const string expected =
                "<Bold Foreground=\"Blue\">\"That's a key because it's surrounded by double-quotes and is followed by a colon\":</Bold>";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightAllTogether()
        {
            const string origin =
                "{\"string-key\": \"string value\", \"null-key\": null, \"bool-key\": true, \"number-key\": 42}";
            const string expected =
                "{<Bold Foreground=\"Blue\">\"string-key\":</Bold> <Span Foreground=\"Blue\">\"string value\"</Span>, " +
                "<Bold Foreground=\"Blue\">\"null-key\":</Bold> <Bold Foreground=\"Black\">null</Bold>, " +
                "<Bold Foreground=\"Blue\">\"bool-key\":</Bold> <Bold Foreground=\"Darkred\">true</Bold>, " +
                "<Bold Foreground=\"Blue\">\"number-key\":</Bold> <Span Foreground=\"Darkgreen\">42</Span>}";

            var actual = IDEEventToXamlFormattedJsonConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }
    }
}