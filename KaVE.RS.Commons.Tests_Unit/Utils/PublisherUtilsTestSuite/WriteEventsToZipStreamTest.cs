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

using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.RS.Commons.Utils;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils.PublisherUtilsTestSuite
{
    internal class WriteEventsToZipStreamTest
    {
        private IPublisherUtils _uut;

        [SetUp]
        public void SetUp()
        {
            _uut = new PublisherUtils();
        }

        [Test]
        public void CreatesFileNamesFromEventIndexAndType()
        {
            var events = new List<IDEEvent>
            {
                TestEventFactory.Some<UserProfileEvent>(),
                TestEventFactory.Some<EditEvent>(),
                TestEventFactory.Some<ErrorEvent>(),
                TestEventFactory.Some<WindowEvent>()
            };

            var stream = new MemoryStream();
            _uut.WriteEventsToZipStream(events, stream, () => { });

            var actual = GetFileEntriesFromZipStream(stream);

            Assert.AreEqual(new[] {"UserProfileEvent.json", "0-EditEvent.json", "1-ErrorEvent.json", "2-WindowEvent.json"}, actual);
        }

        [Test]
        public void ReportsProgress()
        {
            var events = new List<IDEEvent>
            {
                TestEventFactory.Some<EditEvent>(),
                TestEventFactory.Some<ErrorEvent>(),
                TestEventFactory.Some<WindowEvent>()
            };

            var stream = new MemoryStream();
            var actual = 0;
            var wroteEvents = _uut.WriteEventsToZipStream(events, stream, () => actual++);

            Assert.AreEqual(3, actual);
            Assert.IsTrue(wroteEvents);
        }

        [Test]
        public void DoesntReportProgressForUserProfileEvents()
        {
            var events = new List<IDEEvent>
            {
                TestEventFactory.Some<UserProfileEvent>(),
                TestEventFactory.Some<ErrorEvent>(),
                TestEventFactory.Some<WindowEvent>()
            };

            var stream = new MemoryStream();
            var actual = 0;
            var wroteEvents = _uut.WriteEventsToZipStream(events, stream, () => actual++);

            Assert.AreEqual(2, actual);
            Assert.IsTrue(wroteEvents);
        }

        [Test]
        public void ReturnsFalseIfStreamContainsOnlyUserProfileEvent()
        {
            var events = new List<IDEEvent>
            {
                TestEventFactory.Some<UserProfileEvent>()
            };

            var stream = new MemoryStream();
            var actual = _uut.WriteEventsToZipStream(events, stream, () => { });

            Assert.IsFalse(actual);
        }

        private static List<string> GetFileEntriesFromZipStream(MemoryStream stream)
        {
            // Have to reset the position of the stream in order to read from it again.
            stream.Position = 0;

            var zipStream = new ZipInputStream(stream);
            var actual = new List<string>();
            ZipEntry e;
            while ((e = zipStream.GetNextEntry()) != null)
            {
                actual.Add(e.FileName);
            }
            return actual;
        }
    }
}