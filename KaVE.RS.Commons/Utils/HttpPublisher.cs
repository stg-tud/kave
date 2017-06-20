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
using JetBrains.Annotations;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.IO;

namespace KaVE.RS.Commons.Utils
{
    public class HttpPublisher : IPublisher
    {
        private readonly int _eventCountPerUpload;
        private readonly Uri _hostAddress;
        private readonly IIoUtils _ioUtils;
        private readonly IPublisherUtils _publisherUtils;

        public HttpPublisher([NotNull] Uri hostAddress, int eventCountPerUpload = 1000)
        {
            _hostAddress = hostAddress;
            _ioUtils = Registry.GetComponent<IIoUtils>();
            _publisherUtils = Registry.GetComponent<IPublisherUtils>();
            _eventCountPerUpload = eventCountPerUpload;
        }

        public void Publish(UserProfileEvent upe,
            IEnumerable<IDEEvent> events,
            Action progress)
        {
            var en = events.GetEnumerator();

            bool eventsWereWritten;
            do
            {
                var eventsToExport = Next(_eventCountPerUpload, en).PrependWith(upe);

                using (var stream = new MemoryStream())
                {
                    eventsWereWritten = _publisherUtils.WriteEventsToZipStream(eventsToExport, stream, progress);
                    if (eventsWereWritten)
                    {
                        _publisherUtils.UploadEventsByHttp(_ioUtils, _hostAddress, stream);
                    }
                }
            } while (eventsWereWritten);
        }

        private static IEnumerable<IDEEvent> Next(int max, IEnumerator<IDEEvent> en)
        {
            for (var i = 0; i < max && en.MoveNext(); i++)
            {
                yield return en.Current;
            }
        }
    }
}