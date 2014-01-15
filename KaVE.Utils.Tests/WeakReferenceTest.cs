using System;
using KaVE.Utils;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    class WeakReferenceTest
    {
        private WeakReference<object> _refUnderTest;
        private object _strongReference;

        [SetUp]
        public void SetUpReference()
        {
            _strongReference = new object();
            _refUnderTest = new WeakReference<object>(_strongReference);
        }

        [Test]
        public void ShouldHoldReferenceWhenStrongReferenceExists()
        {
            GC.Collect();
            
            Assert.IsTrue(_refUnderTest.IsAlive());
            Assert.AreEqual(_strongReference, _refUnderTest.Target);
        }

        [Test]
        public void ShouldDropReferenceWhenNoStrongReferenceExists()
        {
            _strongReference = null;

            GC.Collect();

            Assert.IsFalse(_refUnderTest.IsAlive());
        }
    }
}
