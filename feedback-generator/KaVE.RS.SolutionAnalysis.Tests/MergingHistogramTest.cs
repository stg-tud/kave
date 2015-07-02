using KaVE.Commons.Utils.Assertion;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class MergingHistogramTest
    {
        private MergingHistogram _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new MergingHistogram(3);
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void Error_DenominatorSmallerThanNumSlots()
        {
            _sut.AddRatio(1, 2);
        }

        [TestCase(1, 5, 1), TestCase(2, 5, 2), TestCase(3, 5, 2), TestCase(4, 5, 3), TestCase(5, 5, 3),
         TestCase(1, 6, 1), TestCase(2, 6, 1), TestCase(3, 6, 2), TestCase(4, 6, 2), TestCase(5, 6, 3),
         TestCase(6, 6, 3)]
        public void Example(int enumerator, int denominator, int expectedSlot)
        {
            _sut.AddRatio(enumerator, denominator);
            Assert.AreEqual(1, _sut.Values[expectedSlot]);
        }
    }
}