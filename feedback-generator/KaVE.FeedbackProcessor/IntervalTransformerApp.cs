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
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Import;
using KaVE.FeedbackProcessor.Intervals;

namespace KaVE.FeedbackProcessor
{
    internal class IntervalTransformerApp
    {
        private readonly ILogger _logger;

        public IntervalTransformerApp(ILogger logger)
        {
            _logger = logger;
        }

        public void Run(string eventArchiveFile)
        {
            var transformer = new IntervalTransformer();
            var events = GetAllEventsFromFile(eventArchiveFile);

            _logger.Info("Transforming intervals from {0}.", eventArchiveFile);
            foreach (var interval in transformer.Transform(events))
            {
                _logger.Info(interval.ToString());
            }
        }

        private IEnumerable<IDEEvent> GetAllEventsFromFile(string file)
        {
            var zip = ZipFile.Read(file);
            var fileLoader = new FeedbackArchiveReader();
            return fileLoader.ReadAllEvents(zip);
        }
    }
}