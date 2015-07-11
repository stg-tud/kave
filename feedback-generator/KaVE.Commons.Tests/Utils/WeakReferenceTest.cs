/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
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

        [Test]
        public void ShouldBeEqual()
        {
            var secondReference = new WeakReference<object>(_strongReference);

            Assert.AreEqual(_uut, secondReference);
            Assert.AreEqual(_uut.GetHashCode(), secondReference.GetHashCode());
        }

        [Test]
        public void ShouldBeDifferent()
        {
            var strongReference = new int[0];
            var secondReference = new WeakReference<object>(strongReference);

            Assert.AreNotEqual(_uut, secondReference);
            Assert.AreNotEqual(_uut.GetHashCode(), secondReference.GetHashCode());
        }
    }
}
