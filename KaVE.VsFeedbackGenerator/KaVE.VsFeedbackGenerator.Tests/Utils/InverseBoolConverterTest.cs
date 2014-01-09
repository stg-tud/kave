using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    internal class InverseBoolConverterTest
    {
        [TestCase(true),
         TestCase(false)]
        public void ShouldInvert(bool value)
        {
            var converter = new InverseBoolConverter();
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(!value, result);
        }

        [TestCase(true),
         TestCase(false)]
        public void ShouldInvertBack(bool value)
        {
            var converter = new InverseBoolConverter();
            var result = converter.ConvertBack(value, null, null, null);
            Assert.AreEqual(!value, result);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldFailToConvertNonBool()
        {
            var converter = new InverseBoolConverter();
            converter.Convert(new object(), null, null, null);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldFailToConvertBackNonBool()
        {
            var converter = new InverseBoolConverter();
            converter.ConvertBack(new object(), null, null, null);
        }
    }
}