using System;
using NUnit.Framework;

namespace KaVE.Utils.Tests
{
    [TestFixture]
    class WeakReferenceTest
    {
        private WeakReference<object> _uut;
        private object _strongReference;

        [SetUp]
        public void SetUpReference()
        {
            _strongReference = new object();
            _uut = new WeakReference<object>(_strongReference);
        }

        [Test]
        public void ShouldHoldReferenceWhenStrongReferenceExists()
        {
            GC.Collect();
            
            Assert.IsTrue(_uut.IsAlive());
            Assert.AreEqual(_strongReference, _uut.Target);
        }

        [Test]
        public void ShouldDropReferenceWhenNoStrongReferenceExists()
        {
            _strongReference = null;

            GC.Collect();

            Assert.IsFalse(_uut.IsAlive());
        }

        [Test]
        public void ShouldUpdateReferenceWhenSet()
        {
            var newStrongReference = new object();

            _uut.Target = newStrongReference;

            Assert.AreEqual(newStrongReference, _uut.Target);
        }
    }
}
