using KaVE.Commons.TestUtils;
using NUnit.Framework;

namespace KaVE.RS.SolutionAnalysis.Tests
{
    internal class RelativeEditLocationTest
    {
        [Test]
        public void DefaultValues()
        {
            var actual = new RelativeEditLocation();
            Assert.AreEqual(0, actual.Size);
            Assert.AreEqual(0, actual.Location);
            Assert.AreNotEqual(0, actual.GetHashCode());
            Assert.AreNotEqual(1, actual.GetHashCode());
        }

        [Test]
        public void SettingValues()
        {
            var actual = new RelativeEditLocation
            {
                Location = 1,
                Size = 2
            };
            Assert.AreEqual(1, actual.Location);
            Assert.AreEqual(2, actual.Size);
        }

        [Test]
        public void HasLocation()
        {
            var actual = new RelativeEditLocation();
            Assert.False(actual.HasEditLocation);
            actual.Location = 1;
            Assert.True(actual.HasEditLocation);
        }

        #region equality

        [Test]
        public void Equality_Default()
        {
            var a = new RelativeEditLocation();
            var b = new RelativeEditLocation();
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_ReallyTheSame()
        {
            var a = new RelativeEditLocation
            {
                Location = 1,
                Size = 2
            };
            var b = new RelativeEditLocation
            {
                Location = 1,
                Size = 2
            };
            Assert.AreEqual(a, b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentLocation()
        {
            var a = new RelativeEditLocation
            {
                Location = 1
            };
            var b = new RelativeEditLocation();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Equality_DifferentSize()
        {
            var a = new RelativeEditLocation
            {
                Size = 2
            };
            var b = new RelativeEditLocation();
            Assert.AreNotEqual(a, b);
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
        }

        #endregion

        [Test]
        public void ToStringReflection()
        {
            ToStringAssert.Reflection(new RelativeEditLocation());
        }
    }
}