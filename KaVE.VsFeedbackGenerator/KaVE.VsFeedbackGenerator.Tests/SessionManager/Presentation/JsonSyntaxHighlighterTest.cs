using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager.Presentation
{
    [TestFixture]
    internal class JsonSyntaxHighlighterTest
    {
        [Test]
        public void ShouldHighlightNull()
        {
            const string origin = "null";
            const string expected = origin;

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("true"), TestCase("false")]
        public void ShouldHighlightBooleans(string origin)
        {
            var expected = origin;

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("0"), TestCase("17"), TestCase("-42"), TestCase("2.3"), TestCase("-3.14")]
        public void ShouldHighlightNumbers(string origin)
        {
            var expected = origin;

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightStrings()
        {
            const string origin = "\"Hello World!\"";
            const string expected = origin;

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightKeys()
        {
            const string origin =
                "\"That's a key because it's surrounded by double-quotes and is followed by a colon\":";
            const string expected =
                "<Bold>\"That's a key because it's surrounded by double-quotes and is followed by a colon\":</Bold>";

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldHighlightAllTogether()
        {
            const string origin =
                "{\"string-key\": \"string value\", \"null-key\": null, \"bool-key\": true, \"number-key\": 42}";
            const string expected =
                "{<Bold>\"string-key\":</Bold> \"string value\", " +
                "<Bold>\"null-key\":</Bold> null, " +
                "<Bold>\"bool-key\":</Bold> true, " +
                "<Bold>\"number-key\":</Bold> 42}";

            var actual = origin.AddJsonSyntaxHighlightingWithXaml();
            Assert.AreEqual(expected, actual);
        }
    }
}