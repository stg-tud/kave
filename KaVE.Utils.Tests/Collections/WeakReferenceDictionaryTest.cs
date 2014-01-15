using System;
using KaVE.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Utils.Tests.Collections
{
    [TestFixture]
    public class WeakReferenceDictionaryTest
    {
        private const string Key = "key";

        private WeakReferenceDictionary<string, object> _dictionaryUnderTest;
        private object _strongReference;

        [SetUp]
        public void SetUp()
        {
            _dictionaryUnderTest = new WeakReferenceDictionary<string, object>();
            _strongReference = new object();
        }

        [Test]
        public void ShouldStoreElement()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);

            Assert.IsTrue(_dictionaryUnderTest.ContainsKey(Key));
            Assert.AreEqual(_strongReference, _dictionaryUnderTest[Key]);
        }

        [Test]
        public void ShouldDropElement()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);

            _strongReference = null;
            GC.Collect();

            Assert.IsFalse(_dictionaryUnderTest.ContainsKey(Key));
            Assert.IsFalse(_dictionaryUnderTest.TryGetValue(Key, out _strongReference));
            Assert.IsNull(_strongReference);
        }

        [Test]
        public void ShouldCountExistingElementsInCount()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);
            _dictionaryUnderTest.Add("foo", _strongReference);

            GC.Collect();

            Assert.AreEqual(2, _dictionaryUnderTest.Count);
        }

        [Test]
        public void ShouldExcludeDroppedElementsFromCount()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);
            _dictionaryUnderTest.Add("heyho", new object());
            _dictionaryUnderTest.Add("foo", _strongReference);
            _dictionaryUnderTest.Add("bar", new object());

            GC.Collect();

            Assert.AreEqual(2, _dictionaryUnderTest.Count);
        }

        [Test]
        public void ShouldReAddDroppedElement()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);

            _strongReference = new object();
            GC.Collect();

            _dictionaryUnderTest.Add(Key, _strongReference);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "An item with the same key has already been added.")]
        public void ShouldFailToReAddExistingElement()
        {
            _dictionaryUnderTest.Add(Key, _strongReference);
            _dictionaryUnderTest.Add(Key, new object());
        }
    }
}
