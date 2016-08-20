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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.RS.SolutionAnalysis.CleanUp.Filters;

namespace KaVE.RS.SolutionAnalysis.CleanUp
{
    public class CleanUpRunner
    {
        private readonly ICleanUpIo _io;
        private readonly ICleanUpLogger _log;
        private Dictionary<string, int> _counts;

        public ISet<IFilter> Filters { get; private set; }

        public CleanUpRunner(ICleanUpIo io, ICleanUpLogger log)
        {
            _io = io;
            _log = log;
            Filters = new HashSet<IFilter>();
        }

        public void Run()
        {
            var zips = _io.GetZips();

            _log.FoundZips(zips);

            foreach (var zip in zips)
            {
                _counts = new Dictionary<string, int>();

                var events = ReadEvents(zip);

                events = ApplyFilters(events);
                events = RemoveDuplicates(events);
                events = OrderEvents(events);

                WriteResults(events, zip);
            }

            _log.Finish();
        }

        private IEnumerable<IDEEvent> ReadEvents(string zip)
        {
            _log.ReadingZip(zip);
            return _io.ReadZip(zip);
        }

        private IEnumerable<IDEEvent> ApplyFilters(IEnumerable<IDEEvent> events)
        {
            _log.ApplyingFilters();
            events = AddCounter(events, "before applying any filter");
            foreach (var filter in Filters)
            {
                _log.ApplyingFilter(filter.Name);
                events = events.Where(filter.Func2);
                events = AddCounter(events, string.Format("after applying '{0}'", filter.Name));
            }
            return events;
        }

        private IEnumerable<IDEEvent> RemoveDuplicates(IEnumerable<IDEEvent> events)
        {
            _log.RemovingDuplicates();
            events = events.Distinct();
            events = AddCounter(events, "after removing duplicates");
            return events;
        }

        private IEnumerable<IDEEvent> OrderEvents(IEnumerable<IDEEvent> events)
        {
            _log.OrderingEvents();
            events = events.OrderBy(e => e.TriggeredAt);
            events = AddCounter(events, "after ordering");
            return events;
        }

        private void WriteResults(IEnumerable<IDEEvent> events, string zip)
        {
            _log.WritingEvents();
            _io.WriteZip(events, zip);
            _log.IntermediateResult(zip, _counts);
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
    }
}