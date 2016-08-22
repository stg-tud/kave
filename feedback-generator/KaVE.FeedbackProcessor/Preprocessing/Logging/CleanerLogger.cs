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

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface ICleanerLogger : IDisposable
    {
        void WorkingIn(string dirIn, string dirOut);
        void RegisteredFilters(IEnumerable<string> filters);
        void ReadingZip(string zip);
        void WritingEvents();
        void FinishedWriting(IDictionary<string, int> counts);
    }

    public class CleanerLogger : ICleanerLogger
    {
        private readonly IPrepocessingLogger _log;
        private readonly IDictionary<string, int> _aggregatedCounts;

        public CleanerLogger(IPrepocessingLogger log)
        {
            _log = log;
            _aggregatedCounts = new Dictionary<string, int>();
        }

        public void WorkingIn(string dirIn, string dirOut)
        {
            _log.Log();
            _log.Log(new string('#', 60));
            _log.Log("# started cleaning...");
            _log.Log(new string('#', 60));
            _log.Log();
            _log.Log("folders:");
            _log.Log("- in: {0}", dirIn);
            _log.Log("- out: {0}", dirOut);
        }

        public void RegisteredFilters(IEnumerable<string> filters)
        {
            _log.Log();
            _log.Log("registered filters:");
            foreach (var filter in filters)
            {
                _log.Log("- {0}", filter);
            }
        }

        public void ReadingZip(string zip)
        {
            _log.Log();
            _log.Log("#### zip: {0}", zip);
            _log.Log("reading... ", zip);
        }

        public void WritingEvents()
        {
            _log.Append("done");
            _log.Log("writing... ");
        }

        public void FinishedWriting(IDictionary<string, int> counts)
        {
            _log.Append("done");
            foreach (var k in counts.Keys)
            {
                _log.Log("- {0}: {1}", k, counts[k]);
                Count(k, counts[k]);
            }
        }

        private void Count(string k, int count)
        {
            var newCount = count;
            if (_aggregatedCounts.ContainsKey(k))
            {
                newCount += _aggregatedCounts[k];
            }
            _aggregatedCounts[k] = newCount;
        }

        public void Dispose()
        {
            _log.Log();
            _log.Log("#### cleaning stats over all files ####");
            foreach (var k in _aggregatedCounts.Keys)
            {
                _log.Log("- {0}: {1}", k, _aggregatedCounts[k]);
            }
        }
    }
}