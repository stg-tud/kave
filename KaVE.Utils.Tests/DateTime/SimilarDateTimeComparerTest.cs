using KaVE.Utils.DateTime;
using NUnit.Framework;

namespace KaVE.Utils.Tests.DateTime
{
    [TestFixture]
    class SimilarDateTimeComparerTest
    {
        [TestCase(5, 5)]
        [TestCase(3, 5)]
        [TestCase(0, 0)]
        [TestCase(-1, 5)]
        [TestCase(-5, 5)]
        public void ShouldBeEqualIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer((uint) equalityThreshold);
            Assert.AreEqual(0, comparer.Compare(firstDate, secondDate));
            Assert.IsTrue(comparer.Equal(firstDate, secondDate));
        }

        [TestCase(42, 5)]
        [TestCase(6, 5)]
        [TestCase(1, 0)]
        public void ShouldBeEarlierIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer((uint)equalityThreshold);
            Assert.AreEqual(-1, comparer.Compare(firstDate, secondDate));
            Assert.IsFalse(comparer.Equal(firstDate, secondDate));
        }

        [TestCase(-42, 5)]
        [TestCase(-6, 5)]
        [TestCase(-1, 0)]
        public void ShouldBeLaterIf(int millisDifferenceToSecondDateTime, int equalityThreshold)
        {
            var firstDate = System.DateTime.Now;
            var secondDate = firstDate.AddMilliseconds(millisDifferenceToSecondDateTime);

            var comparer = new SimilarDateTimeComparer((uint)equalityThreshold);
            Assert.AreEqual(1, comparer.Compare(firstDate, secondDate));
            Assert.IsFalse(comparer.Equal(firstDate, secondDate));
        }
    }
}
