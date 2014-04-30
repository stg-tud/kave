using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class XamlFormattingUtilTest
    {
        private const string Content = "This is GOOD content";
        private const string Color = "Awesome";

        [Test]
        public void ShouldMakeTextBold()
        {
            const string expected = "<Bold>" + Content + "</Bold>";

            var actual = XamlFormattingUtil.Bold(Content);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextBoldAndColored()
        {
            const string expected = "<Bold Foreground=\"" + Color + "\">" + Content + "</Bold>";

            var actual = XamlFormattingUtil.Bold(Content, Color);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextColored()
        {
            const string expected = "<Span Foreground=\"" + Color + "\">" + Content + "</Span>";

            var actual = XamlFormattingUtil.Colored(Content, Color);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextItalic()
        {
            const string expected = "<Italic>" + Content + "</Italic>";

            var actual = XamlFormattingUtil.Italic(Content);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldMakeTextItalicAndColored()
        {
            const string expected = "<Italic Foreground=\"" + Color + "\">" + Content + "</Italic>";

            var actual = XamlFormattingUtil.Italic(Content, Color);
            Assert.AreEqual(expected, actual);
        }
    }
}