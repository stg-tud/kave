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
using KaVE.Commons.Utils.Collections;

namespace KaVE.RS.SolutionAnalysis.SortByUser
{
    public interface ISortByUserLogger
    {
        void StartScanning();
        void FoundUsers(IKaVESet<User> users);
        void StartMerging();
        void UserResult(User user);
        void ReadingArchive(string fileName);
        void Progress();
        void WritingArchive(string fileName);
        void Merging(IKaVESet<string> files);
        void FinalStats(int numFilesBefore, int numFilesAfter);
        void CountInputEvent();
        void StoreOutputEvents(int count);
    }

    public class SortByUserLogger : ISortByUserLogger
    {
        public void FoundUsers(IKaVESet<User> users)
        {
            Log("");
            Log(@"### Found users ##########################");
            foreach (var user in users)
            {
                Log("");
                Log("* '{0}' -- {1} ids, {2} files:", user.GetHashCode(), user.Identifiers.Count, user.Files.Count);
                Log(@"   ids:");
                foreach (var id in user.Identifiers)
                {
                    Log(@"      - {0}", id);
                }
                Log(@"   files:");
                foreach (var id in user.Files)
                {
                    Log(@"      - {0}", id);
                }
            }
        }

        public void UserResult(User user)
        {
            Log("");
            Log("## Processing '{0}'", user.GetHashCode());
        }

        public void StartMerging()
        {
            Log("");
            Log(@"### Merging archives ##########################");
        }

        public void StartScanning()
        {
            Log(@"### Scanning archives ##########################");
        }

        public void ReadingArchive(string fileName)
        {
            Log("");
            Log(@"Reading {0}...", fileName);
            Log("");
        }

        public void Progress()
        {
            Append(".");
        }

        public void WritingArchive(string fileName)
        {
            Log("");
            Log(@"Writing {0}...", fileName);
        }

        public void Merging(IKaVESet<string> files)
        {
            Log("");
            Log(@"Merging {0}...", string.Join(", ", files));
        }

        private long _numEventsBefore;
        private long _numEventsAfter;

        public void CountInputEvent()
        {
            _numEventsBefore++;
        }

        public void StoreOutputEvents(int count)
        {
            _numEventsAfter += count;
        }

        public void FinalStats(int numFilesBefore, int numFilesAfter)
        {
            Log("");
            Log(@"### Results ##########################");
            Log("");
            Log(@"{0} files before, {1} files after...", numFilesBefore, numFilesAfter);
            Log(
                @"{0} evenst before, {1} events after (delta: {2})...",
                _numEventsBefore,
                _numEventsAfter,
                (_numEventsAfter - _numEventsBefore));
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