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

namespace KaVE.FeedbackProcessor.SortByUser
{
    public interface ISortByUserLogger
    {
        void StartScanning();
        void FoundUsers(IKaVESet<User> users);
        void StartMerging();
        void StartProcessingUser(User user);
        void ReadingArchive(string fileName);
        void CachedArchive(string fileName);
        void Progress();
        void WritingArchive(string fileName);
        void Merging(IKaVESet<string> files);
        void FinalStats(int numFilesBefore, int numFilesAfter);
        void CountInputEvent();
        void StoreOutputEvents(int count);
        void StartUserIdentification();
        void FoundNumArchives(int count);
        void WorkingIn(string dirIn, string dirOut);
    }

    public class SortByUserLogger : ISortByUserLogger
    {
        private int _numUsers;
        private int _currentUser;

        public void StartUserIdentification()
        {
            Log("");
            Log(@"### User identification ##########################");
        }

        public void FoundNumArchives(int count)
        {
            _numFiles = count;
            Log("");
            Log("found {0} archives to process", count);
        }

        public void WorkingIn(string dirIn, string dirOut)
        {
            Log("dirIn: {0}", dirIn);
            Log("dirOut: {0}", dirOut);
        }

        public void FoundUsers(IKaVESet<User> users)
        {
            _numUsers = users.Count;

            var numUser = 0;

            Log("");
            Log(@"found {0} users:", users.Count);
            foreach (var user in users)
            {
                Log("");
                Log(
                    "user {3}: '{0}' -- {1} ids, {2} files:",
                    user.GetHashCode(),
                    user.Identifiers.Count,
                    user.Files.Count,
                    ++numUser);
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

        public void StartProcessingUser(User user)
        {
            Log("");
            Log("## Processing '{0}' ({1}/{2})", user.GetHashCode(), ++_currentUser, _numUsers);
        }

        public void StartMerging()
        {
            Log("");
            Log(@"### Merging archives ##########################");
        }

        public void StartScanning()
        {
            Log("");
            Log(@"### Scanning archives ##########################");
        }

        private int _numFiles;
        private int _currentFileNum;

        public void ReadingArchive(string fileName)
        {
            Log("");
            Log(@"Reading {0}... ({1}/{2})", fileName, ++_currentFileNum, _numFiles);
            Log("");
        }

        public void Progress()
        {
            Append(".");
        }

        public void CachedArchive(string fileName)
        {
            Log("");
            Log(@"Reading {0}... ({1}/{2}) -- not read, index exists", fileName, ++_currentFileNum, _numFiles);
            Log("");
        }

        public void WritingArchive(string fileName)
        {
            Log("");
            Log(@"Writing {0}...", fileName);
        }

        public void Merging(IKaVESet<string> files)
        {
            _numFiles = files.Count;
            _currentFileNum = 0;

            Log("");
            Log(@"Merging {0} files: {1}", files.Count, string.Join(", ", files));
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
                @"{0} events before, {1} events after (delta: {2})...",
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