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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Filters;
using KaVE.FeedbackProcessor.Preprocessing.Logging;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.Preprocessing
{
    public interface ICleaner : IDisposable
    {
        void Clean(string relZip);
    }

    public class Cleaner : ICleaner
    {
        private readonly IPreprocessingIo _io;
        private readonly ICleanerLogger _log;
        private Dictionary<string, int> _counts;

        public ISet<IFilter> Filters { get; private set; }
        private bool _hasReportedFilters;

        public Cleaner(IPreprocessingIo io, ICleanerLogger log)
        {
            _io = io;
            _log = log;

            _log.WorkingIn(io.GetFullPath_Merged(""), io.GetFullPath_Out(""));

            Filters = new HashSet<IFilter>();
        }

        public void Clean(string relZip)
        {
            ReportFilters();

            _counts = new Dictionary<string, int>();

            var events = ReadEvents(relZip);

            events = ApplyFilters(events);
            events = RemoveDuplicates(events);
            events = OrderEvents(events);

            WriteResults(events, relZip);

            _log.FinishedWriting(_counts);
        }

        private void ReportFilters()
        {
            if (!_hasReportedFilters)
            {
                _log.RegisteredFilters(Filters.Select(f => f.Name));
            }
            _hasReportedFilters = true;
        }

        private IEnumerable<IDEEvent> ReadEvents(string relZip)
        {
            _log.ReadingZip(relZip);

            var zip = _io.GetFullPath_Merged(relZip);
            using (var ra = new ReadingArchive(zip))
            {
                while (ra.HasNext())
                {
                    yield return ra.GetNext<IDEEvent>();
                }
            }
        }

        private IEnumerable<IDEEvent> ApplyFilters(IEnumerable<IDEEvent> events)
        {
            events = AddCounter(events, "before applying any filter");
            foreach (var filter in Filters)
            {
                events = events.Where(filter.Func2);
                events = AddCounter(events, string.Format("after applying '{0}'", filter.Name));
            }
            return events;
        }

        private IEnumerable<IDEEvent> RemoveDuplicates(IEnumerable<IDEEvent> events)
        {
            events = events.Distinct();
            events = AddCounter(events, "after removing duplicates");
            return events;
        }

        private IEnumerable<IDEEvent> OrderEvents(IEnumerable<IDEEvent> events)
        {
            events = events.OrderBy(e => e.TriggeredAt);
            events = AddCounter(events, "after ordering");
            return events;
        }

        private void WriteResults(IEnumerable<IDEEvent> events, string relZip)
        {
            _log.WritingEvents();
            var zip = _io.GetFullPath_Out(relZip);
            _io.EnsureParentExists(zip);
            using (var wa = new WritingArchive(zip))
            {
                wa.AddAll(events);
            }
        }

        private IEnumerable<IDEEvent> AddCounter(IEnumerable<IDEEvent> events, string name)
        {
            return events.Where(
                _ =>
                {
                    Count(name);
                    return true;
                });
        }

        private void Count(string filterName)
        {
            if (_counts.ContainsKey(filterName))
            {
                _counts[filterName]++;
            }
            else
            {
                _counts[filterName] = 1;
            }
        }

        public void Dispose()
        {
            _log.Dispose();
        }
    }
}