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
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Import;

namespace KaVE.FeedbackProcessor
{
    internal class EventStreamFilter
    {
        private readonly Predicate<IDEEvent> _eventFilter;

        public EventStreamFilter(Predicate<IDEEvent> eventFilter)
        {
            _eventFilter = eventFilter;
        }

        public IEnumerable<IDEEvent> Filter(IEnumerable<IDEEvent> events)
        {
            return events.Where(e => _eventFilter(e));
        }

        public IEnumerable<IDEEvent> Filter(string filename)
        {
            return Filter(GetAllEventsFromFile(filename));
        }

        private IEnumerable<IDEEvent> GetAllEventsFromFile(string file)
        {
            var zip = ZipFile.Read(file);
            var fileLoader = new FeedbackArchiveReader();
            return fileLoader.ReadAllEvents(zip);
        }

        public static Predicate<IDEEvent> TimeBoxFilter(DateTime from, DateTime to)
        {
            return e => from <= e.TriggeredAt && e.TriggeredAt <= to;
        }

        public static Predicate<IDEEvent> TimeBoxFilter(string from, string to)
        {
            return e => DateTime.Parse(from) <= e.TriggeredAt && e.TriggeredAt <= DateTime.Parse(to);
        }

        public static Predicate<IDEEvent> DocumentEventFilter(string documentFileName)
        {
            return e =>
            {
                var documentEvent = e as DocumentEvent;
                return documentEvent != null && documentEvent.Document != null &&
                       documentEvent.Document.FileName == documentFileName;
            };
        }

        public static Predicate<IDEEvent> ActiveDocumentFilter(string documentFileName)
        {
            return e => e.ActiveDocument != null &&
                        e.ActiveDocument.FileName == documentFileName;
        }
    }
}