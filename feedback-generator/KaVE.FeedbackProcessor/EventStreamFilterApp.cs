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
using System.Linq;
using Ionic.Zip;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Import;

namespace KaVE.FeedbackProcessor
{
    internal class EventStreamFilterApp
    {
        private readonly Predicate<IDEEvent> _eventFilter;

        public EventStreamFilterApp(Predicate<IDEEvent> eventFilter)
        {
            _eventFilter = eventFilter;
        }

        public void Run(IEnumerable<IDEEvent> events)
        {
            var filteredEvents = events.Where(e => _eventFilter(e));

            foreach (var e in filteredEvents)
            {
                Console.WriteLine(@"{0} - {1} - {2}", e.TriggeredAt.GetValueOrDefault(), e.GetType().Name, e.IDESessionUUID);
            }
        }

        public void Run(string filename)
        {
            Run(GetAllEventsFromFile(filename));
        }

        private IEnumerable<IDEEvent> GetAllEventsFromFile(string file)
        {
            var zip = ZipFile.Read(file);
            var fileLoader = new FeedbackArchiveReader();
            return fileLoader.ReadAllEvents(zip);
        }

        public static EventStreamFilterApp CreateIntervalFilter(DateTime from, DateTime to)
        {
            return new EventStreamFilterApp(e => from <= e.TriggeredAt && e.TriggeredAt <= to);
        }
    }
}