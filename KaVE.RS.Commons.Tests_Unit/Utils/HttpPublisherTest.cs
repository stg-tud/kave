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
using System.Linq;
using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    [TestFixture]
    internal class HttpPublisherTest : PublisherTestBase
    {
        private static readonly Uri ValidUri = new Uri("http://server");

        private HttpPublisher _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new HttpPublisher(ValidUri, TestEventCountPerUpload);
        }

        [Test]
        public void UploadsEventsInPackages()
        {
            _uut.Publish(_userProfileEvent, TestEventSource(2*TestEventCountPerUpload + 1), () => { });

            Assert.AreEqual(3, _exportedPackages.Count);
            Assert.AreEqual(TestEventCountPerUpload + 1, _exportedPackages[0].Count);
            Assert.AreEqual(TestEventCountPerUpload + 1, _exportedPackages[1].Count);
            Assert.AreEqual(2, _exportedPackages[2].Count);
        }

        [Test]
        public void UploadsEventsInPackages_NoUserProfileEvent()
        {
            _uut.Publish(null, TestEventSource(2*TestEventCountPerUpload + 1), () => { });

            Assert.AreEqual(3, _exportedPackages.Count);
            Assert.AreEqual(TestEventCountPerUpload, _exportedPackages[0].Count);
            Assert.AreEqual(TestEventCountPerUpload, _exportedPackages[1].Count);
            Assert.AreEqual(1, _exportedPackages[2].Count);
        }

        [Test]
        public void AllSourceEventsArePublished()
        {
            var testEvents = TestEventSource(2*TestEventCountPerUpload + 1).ToList();
            _uut.Publish(null, testEvents, () => { });

            var exported = _exportedPackages.SelectMany(e => e).ToList();
            CollectionAssert.AreEqual(testEvents, exported);
        }

        [Test]
        public void ProgressCallsArePassedThrough()
        {
            const int expected = 8;
            var count = 0;
            _uut.Publish(null, TestEventSource(expected), () => count++);

            Assert.AreEqual(expected, count);
        }
    }
}