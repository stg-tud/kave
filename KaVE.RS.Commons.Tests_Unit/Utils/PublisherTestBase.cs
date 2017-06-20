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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.RS.Commons.Tests_Unit.Utils
{
    internal class PublisherTestBase
    {
        protected const int TestEventCountPerUpload = 5;

        protected Mock<IIoUtils> _ioUtilsMock;
        protected Mock<IPublisherUtils> _publisherUtilsMock;

        protected static UserProfileEvent _userProfileEvent;

        protected List<List<IDEEvent>> _exportedPackages;

        protected static IEnumerable<IDEEvent> TestEventSource(int count)
        {
            var baseDate = new DateTime(2014, 1, 1);
            for (int i = 0; i < count; i++)
            {
                yield return new WindowEvent {TriggeredAt = baseDate.AddDays(i)};
            }
        }

        [SetUp]
        public void BaseSetUp()
        {
            _exportedPackages = new List<List<IDEEvent>>();

            _ioUtilsMock = new Mock<IIoUtils>();
            Registry.RegisterComponent(_ioUtilsMock.Object);
            _publisherUtilsMock = new Mock<IPublisherUtils>();
            _publisherUtilsMock
                .Setup(
                    p =>
                        p.WriteEventsToZipStream(
                            It.IsAny<IEnumerable<IDEEvent>>(),
                            It.IsAny<Stream>(),
                            It.IsAny<Action>()))
                .Returns<IEnumerable<IDEEvent>, Stream, Action>(MockWriteEventsToZipStream);

            _publisherUtilsMock.Setup(
                p => p.UploadEventsByHttp(It.IsAny<IIoUtils>(), It.IsAny<Uri>(), It.IsAny<MemoryStream>()));

            Registry.RegisterComponent(_publisherUtilsMock.Object);

            _userProfileEvent = new UserProfileEvent {ProfileId = "p"};
        }

        private bool MockWriteEventsToZipStream(IEnumerable<IDEEvent> events, Stream stream, Action progress)
        {
            var package = events.ToList();

            foreach (var e in package)
            {
                if (!(e is UserProfileEvent))
                {
                    progress();
                }
            }

            if (!package.Any() || (package.Count == 1 && package.Single() is UserProfileEvent))
            {
                return false;
            }

            _exportedPackages.Add(package);
            return true;
        }

        [TearDown]
        public void CleanUpRegistry()
        {
            Registry.Clear();
        }
    }
}