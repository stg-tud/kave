using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class StringUtilsTest
    {
        [Test]
        public void SimpleRoundTrip()
        {
            const string expected = "asd";
            var actual = expected.AsBytes().AsString();
            Assert.AreEqual(expected, actual);
        }
    }
}