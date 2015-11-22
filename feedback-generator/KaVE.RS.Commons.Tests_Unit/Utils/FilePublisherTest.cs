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
using System.IO;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    [TestFixture]
    internal class FilePublisherTest : PublisherTestBase
    {
        private const string SomeTargetLocation = "existing target file";

        [Test]
        public void ShouldOpenFileForWriting()
        {
            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(_userProfileEvent, TestEventSource(1), () => { });

            _ioUtilsMock.Verify(m => m.OpenFile(SomeTargetLocation, FileMode.Create, FileAccess.Write));
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowExceptionOnNullArgument()
        {
            var uut = new FilePublisher(() => null);
            uut.Publish(_userProfileEvent, TestEventSource(1), () => { });
        }

        [Test, ExpectedException(typeof (AssertException))]
        public void ShouldThrowExceptionOnEmptyArgument()
        {
            var uut = new FilePublisher(() => "");
            uut.Publish(_userProfileEvent, TestEventSource(1), () => { });
        }

        private const string CopyFailureMessage = "Datei-Export fehlgeschlagen: XYZ";

        [Test, ExpectedException(typeof (AssertException), ExpectedMessage = CopyFailureMessage)]
        public void ShouldThrowExceptionIfSomething()
        {
            _ioUtilsMock.Setup(io => io.OpenFile(SomeTargetLocation, FileMode.Create, FileAccess.Write))
                        .Throws(new Exception("XYZ"));
            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(_userProfileEvent, TestEventSource(1), () => { });
        }

        [Test]
        public void AllSourceEventsArePublished()
        {
            var testEvents = TestEventSource(10).ToList();

            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(null, testEvents, () => { });

            Assert.AreEqual(1, _exportedPackages.Count);
            var exported = _exportedPackages.SelectMany(e => e).ToList();
            CollectionAssert.AreEqual(testEvents, exported);
        }

        [Test]
        public void ProgressCallsArePassedThrough()
        {
            const int expected = 8;
            var count = 0;
            var uut = new FilePublisher(() => SomeTargetLocation);
            uut.Publish(null, TestEventSource(expected), () => count++);

            Assert.AreEqual(expected, count);
        }
    }
}