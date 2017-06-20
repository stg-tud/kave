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

using KaVE.FeedbackProcessor.EditLocation;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.EditLocation
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
        public void ToStringImplementation()
        {
            var sut = new RelativeEditLocation {Location = 2, Size = 3};
            Assert.AreEqual("RelativeEditLocation(2/3)", sut.ToString());
        }
    }
}