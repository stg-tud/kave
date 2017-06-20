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

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface IGroupMergerLogger
    {
        void NextGroup(int count, string relZipOut);
        void Reading(string relZip);
        void Result(int numEvents);
        void WorkingIn(string getFullPathRaw, string getFullPathMerged);
    }

    public class GroupMergerLogger : IGroupMergerLogger
    {
        private readonly IPrepocessingLogger _log;

        public GroupMergerLogger(IPrepocessingLogger log)
        {
            _log = log;

            _log.Log();
            _log.Log(new string('#', 60));
            _log.Log("# merging multiple zips into one");
            _log.Log(new string('#', 60));
        }

        public void NextGroup(int count, string relZipOut)
        {
            _log.Log();
            _log.Log("#### merging {0} zips into {1}", count, relZipOut);
        }

        public void Reading(string relZip)
        {
            _log.Log("- {0} ...", relZip);
        }

        public void Result(int numEvents)
        {
            _log.Log("==> merged {0} events", numEvents);
        }

        public void WorkingIn(string dirIn, string dirOut)
        {
            _log.Log();
            _log.Log("working directories:");
            _log.Log("- in: {0}", dirIn);
            _log.Log("- out: {0}", dirOut);
        }
    }
}