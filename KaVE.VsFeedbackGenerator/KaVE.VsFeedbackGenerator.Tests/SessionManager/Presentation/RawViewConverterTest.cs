using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class RawViewConverterTest
    {
        [Test]
        public void ShouldDoubleTheIndention()
        {
            const string origin =
                "first line\nsecond line\n  line to be indented\n    line to be even more indented\nline  with  holes  that  should  stay  the  same";
            const string expected =
                "first line\nsecond line\n    line to be indented\n        line to be even more indented\nline  with  holes  that  should  stay  the  same";

            var actual = RawViewConverter.AdjustIndent(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightNull()
        {
            const string origin = "null";
            const string expected = "<Bold>null</Bold>";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("true"), TestCase("false")]
        public void ShouldHighlightBooleans(string origin)
        {
            var expected = "<Bold Foreground=\"Darkred\">" + origin + "</Bold>";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [TestCase("0"), TestCase("17"), TestCase("-42"), TestCase("2.3"), TestCase("-3.14")]
        public void ShouldHighlightNumbers(string origin)
        {
            var expected = "<Span Foreground=\"Darkgreen\">" + origin + "</Span>";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightStrings()
        {
            const string origin = "\"Hello World!\"";
            const string expected = "<Span Foreground=\"Blue\">\"Hello World!\"</Span>";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightKeys()
        {
            const string origin =
                "\"That's a key because it's surrounded by double-quotes and is followed by a colon\":";
            const string expected =
                "<Bold Foreground=\"Blue\">\"That's a key because it's surrounded by double-quotes and is followed by a colon\":</Bold>";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightAllTogether()
        {
            const string origin =
                "{\"string-key\": \"string value\", \"null-key\": null, \"bool-key\": true, \"number-key\": 42}";
            const string expected =
                "{<Bold Foreground=\"Blue\">\"string-key\":</Bold> <Span Foreground=\"Blue\">\"string value\"</Span>, <Bold Foreground=\"Blue\">\"null-key\":</Bold> <Bold>null</Bold>, <Bold Foreground=\"Blue\">\"bool-key\":</Bold> <Bold Foreground=\"Darkred\">true</Bold>, <Bold Foreground=\"Blue\">\"number-key\":</Bold> <Span Foreground=\"Darkgreen\">42</Span>}";

            var actual = RawViewConverter.Highlight(origin);
            Assert.AreEqual(expected, actual);
        }
    }
}