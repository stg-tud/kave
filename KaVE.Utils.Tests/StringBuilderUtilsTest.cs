using System.Text;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    class StringBuilderUtilsTest
    {
        [Test]
        public void ShouldAppendStringIfConditionIsTrue()
        {
            var builder = new StringBuilder();
            builder.Append("Hello ");
            builder.AppendIf(true, "World!");

            Assert.AreEqual("Hello World!", builder.ToString());
        }

        [Test]
        public void ShouldNotAppendStringIfConditionIsFalse()
        {
            var builder = new StringBuilder();
            builder.Append("Hello ");
            builder.AppendIf(false, "World!");

            Assert.AreEqual("Hello ", builder.ToString());
        }
    }
}
