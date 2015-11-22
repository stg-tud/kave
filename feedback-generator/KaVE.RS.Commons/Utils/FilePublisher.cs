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
using JetBrains.Util;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Properties;

namespace KaVE.RS.Commons.Utils
{
    public class FilePublisher : IPublisher
    {
        private readonly Func<string> _requestFileLocation;
        private readonly IIoUtils _ioUtils;
        private readonly IPublisherUtils _publisherUtils;

        public FilePublisher([NotNull] Func<string> requestFileLocation)
        {
            _requestFileLocation = requestFileLocation;
            _ioUtils = Registry.GetComponent<IIoUtils>();
            _publisherUtils = Registry.GetComponent<IPublisherUtils>();
        }

        public void Publish(UserProfileEvent upe,
            IEnumerable<IDEEvent> events,
            Action progress)
        {
            var filename = _requestFileLocation();
            Asserts.Not(filename.IsNullOrEmpty(), Messages.NoFileGiven);

            try
            {
                using (var file = _ioUtils.OpenFile(filename, FileMode.Create, FileAccess.Write))
                {
                    _publisherUtils.WriteEventsToZipStream(events, file, progress);
                }
            }
            catch (Exception e)
            {
                Asserts.Fail(Messages.PublishingFileFailed, e.Message);
            }
        }
    }
}