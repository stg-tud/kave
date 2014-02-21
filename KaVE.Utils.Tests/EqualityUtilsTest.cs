using System;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    class EqualityUtilsTest
    {
        [Test]
        public void ShouldDeclareSameInstanceEqual()
        {
            var obj = new TestClass(23);

            // ReSharper disable once EqualExpressionComparison
            Assert.IsTrue(Equals(obj, obj));
        }

        [Test]
        public void ShouldDeclareEqualIstancesEquals()
        {
            var obj1 = new TestClass(42);
            var obj2 = new TestClass(42);

            Assert.IsTrue(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        [Test]
        public void ShouldDeclareNullDifferentFromInstance()
        {
            var obj = new TestClass(0);

            Assert.IsFalse(obj.Equals(null, other => other.Value == obj.Value));
        }

        [Test]
        public void ShouldDeclareDifferentInstancesDifferent()
        {
            var obj1 = new TestClass(1);
            var obj2 = new TestClass(2);

            Assert.IsFalse(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        [Test]
        public void ShouldDeclareObjectOfDifferentTypeDifferent()
        {
            var obj1 = new TestClass(23);
            var obj2 = new Object();

            Assert.IsFalse(obj1.Equals(obj2, other => other.Value == obj1.Value));
        }

        private class TestClass
        {
            public readonly int Value;

            public TestClass(int value)
            {
                Value = value;
            }
        }
    }
}
