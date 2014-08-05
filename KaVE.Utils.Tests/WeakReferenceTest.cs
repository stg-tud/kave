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
