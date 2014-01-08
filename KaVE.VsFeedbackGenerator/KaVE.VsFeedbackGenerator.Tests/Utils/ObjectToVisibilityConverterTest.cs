using System.Windows;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    class ObjectToVisibilityConverterTest
    {
        [Test]
        public void ShouldConvertNullToHidden()
        {
            AssertConverts(null, Visibility.Collapsed);
        }
        [Test]
        public void ShouldConvertObjectToVisible()
        {
            AssertConverts(new object(), Visibility.Visible);
        }
        [Test]
        public void ShouldConvertNullableNullToHidden()
        {
            // ReSharper disable once RedundantCast
            AssertConverts((int?) null, Visibility.Collapsed);
        }
        [Test]
        public void ShouldConvertNullableValueToVisible()
        {
            AssertConverts((int?) 1, Visibility.Visible);
        }

        private static void AssertConverts(object value, Visibility exptectedResult)
        {
            var converter = new ObjectToVisibilityConverter();
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(exptectedResult, result);
        }
    }
}
