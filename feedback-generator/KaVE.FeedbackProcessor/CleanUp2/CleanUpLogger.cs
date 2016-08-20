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

namespace KaVE.FeedbackProcessor.CleanUp2
{
    public interface ICleanUpLogger
    {
        void FoundZips(IList<string> zips);
        void ReadingZip(string zip);
        void ApplyingFilters();
        void ApplyingFilter(string name);
        void RemovingDuplicates();
        void OrderingEvents();
        void WritingEvents();
        void IntermediateResult(string zip, IDictionary<string, int> counts);
        void Finish();
    }

    public class CleanUpLogger : ICleanUpLogger
    {
        private int _totalZips;
        private int _currentZip;

        public void FoundZips(IList<string> zips)
        {
            Log("found {0} zips:", zips.Count);
            var i = 1;
            foreach (var zip in zips)
            {
                Log("    {0}: {1}", i++, zip);
            }
            _totalZips = zips.Count;
        }

        public void ReadingZip(string zip)
        {
            Log();
            Log("#### reading zip: {0} ({1}/{2})", zip, ++_currentZip, _totalZips);
        }

        public void ApplyingFilters()
        {
            Log();
            Log("applying filters:");
        }

        public void ApplyingFilter(string name)
        {
            Log("\t- {0}", name);
        }

        public void RemovingDuplicates()
        {
            Log();
            Log("removing duplicates... ");
        }

        public void OrderingEvents()
        {
            Append("done");
            Log("ordering events... ");
        }

        public void WritingEvents()
        {
            Append("done");
            Log("writing events... ");
        }

        public void IntermediateResult(string zip, IDictionary<string, int> counts)
        {
            Append("done");
            Log();
            Log("intermediate result for {0}:", zip);
            foreach (var k in counts.Keys)
            {
                Log("\t- {0}: {1}", k, counts[k]);
                Count(k, counts[k]);
            }
        }

        private readonly IDictionary<string, int> _counts = new Dictionary<string, int>();

        private void Count(string k, int count)
        {
            var newCount = count;
            if (_counts.ContainsKey(k))
            {
                newCount += _counts[k];
            }
            _counts[k] = newCount;
        }

        public void Finish()
        {
            Log();
            Log("#### stats over all files");
            foreach (var k in _counts.Keys)
            {
                Log("\t- {0}: {1}", k, _counts[k]);
            }
        }

        private static void Log()
        {
            Log("");
        }

        private static void Log(string text, params object[] args)
        {
            var content = string.Format(text, args);
            var date = DateTime.Now;
            Console.WriteLine();
            Console.Write(@"{0} {1}", date, content);
        }

        private static void Append(string text, params object[] args)
        {
            Console.Write(text, args);
        }
    }
}