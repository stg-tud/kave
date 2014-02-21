using System.Collections.Generic;
using KaVE.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Collections
{
    [TestFixture]
    internal class SetEqualityComparerTest
    {
        [Test]
        public void ShouldDeclareTheSameSetEqual()
        {
            var uut = CreateComparer<int>();
            var intSet = CreateSet(1, 3, 5, 23, 42, 66, 1337);

            Assert.IsTrue(uut.Equals(intSet, intSet));
            Assert.AreEqual(uut.GetHashCode(intSet), uut.GetHashCode(intSet));
        }

        [Test]
        public void ShouldDeclareEmptySetsEqual()
        {
            var uut = CreateComparer<int>();
            var set1 = CreateSet<int>();
            var set2 = CreateSet<int>();

            Assert.IsTrue(uut.Equals(set1, set2));
            Assert.AreEqual(uut.GetHashCode(set1), uut.GetHashCode(set2));
        }

        [Test]
        public void ShouldDeclareSetsWithSameContentEqual()
        {
            var uut = CreateComparer<int>();
            var set1 = CreateSet(1, 23, 15);
            var set2 = CreateSet(15, 1, 23);

            Assert.IsTrue(uut.Equals(set1, set2));
            Assert.AreEqual(uut.GetHashCode(set1), uut.GetHashCode(set2));
        }

        [Test]
        public void ShouldDeclareSetsWithEqualContentEqual()
        {
            var uut = CreateComparer<Element>();
            var element1 = new Element(1);
            var element2 = new Element(2);
            var set1 = CreateSet(element1, element2);
            var set2 = CreateSet(element2,element1);

            Assert.IsTrue(uut.Equals(set1, set2));
            Assert.AreEqual(uut.GetHashCode(set1), uut.GetHashCode(set2));
        }

        [Test]
        public void ShouldDeclareDifferentSetsDifferent()
        {
            var uut = CreateComparer<int>();
            var set1 = CreateSet(1, 2, 3);
            var set2 = CreateSet(2, 3, 4);

            Assert.IsFalse(uut.Equals(set1, set2));
            Assert.AreNotEqual(uut.GetHashCode(set1), uut.GetHashCode(set2));
        }

        [Test]
        public void ShouldDeclareSetsWithUnequalContentDifferent()
        {
            var uut = CreateComparer<Element>();
            var element1 = new Element(1);
            var element2 = new Element(2);
            var set1 = CreateSet(element1);
            var set2 = CreateSet(element2, element1);

            Assert.IsFalse(uut.Equals(set1, set2));
            Assert.AreNotEqual(uut.GetHashCode(set1), uut.GetHashCode(set2));
        }

        private class Element
        {
            private readonly int _hashCode;

            public Element(int hashCode)
            {
                _hashCode = hashCode;
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == _hashCode;
            }

            public override int GetHashCode()
            {
                return _hashCode;
            }
        }

        private static ISet<TElement> CreateSet<TElement>(params TElement[] elements)
        {
            return new HashSet<TElement>(elements);
        }

        private static SetEqualityComparer<TElement> CreateComparer<TElement>()
        {
            return new SetEqualityComparer<TElement>();
        }
    }
}