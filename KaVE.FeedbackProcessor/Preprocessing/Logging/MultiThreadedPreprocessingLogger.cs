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

namespace KaVE.FeedbackProcessor.Preprocessing.Logging
{
    public interface IMultiThreadedPreprocessingLogger
    {
        void Init(int numWorkers, string dirIn, string dirMerged, string dirOut);

        void ReadingIds(int numUnindexedZips);
        void StartWorkerReadIds(int taskId);
        void ReadIds(int taskId, string zip);
        void StopWorkerReadIds(int taskId);

        void GroupZipsByIds();

        void MergeGroups(int numGroups);
        void StartWorkerMergeGroup(int taskId);
        void MergeGroup(int taskId, int count);
        void StopWorkerMergeGroup(int taskId);

        void Cleaning(int numUnclean);
        void StartWorkerCleanZip(int taskId);
        void CleanZip(int taskId, string zip);
        void StopWorkerCleanZip(int taskId);

        void Error(int taskId, Exception exception);
    }

    public class MultiThreadedPreprocessingLogger : IMultiThreadedPreprocessingLogger
    {
        private readonly object _lock = new object();
        private readonly IPrepocessingLogger _log;

        public MultiThreadedPreprocessingLogger(IPrepocessingLogger log)
        {
            _log = log;
        }

        public void Init(int numWorkers, string dirIn, string dirMerged, string dirOut)
        {
            lock (_lock)
            {
                _log.Log(new string('#', 60));
                _log.Log("# MultiThreadedPreprocessing");
                _log.Log(new string('#', 60));
                _log.Log();
                _log.Log("workers: {0}", numWorkers);
                _log.Log("dirIn: {0}", dirIn);
                _log.Log("dirMerged: {0}", dirMerged);
                _log.Log("dirOut: {0}", dirOut);
            }
        }

        private int _numZips;
        private int _curZip;

        public void ReadingIds(int numUnindexedZips)
        {
            lock (_lock)
            {
                _numZips = numUnindexedZips;

                _log.Log();
                _log.Log(new string('-', 60));
                _log.Log("Reading ids from {0} zips", numUnindexedZips);
                _log.Log(new string('-', 60));
            }
        }

        public void StartWorkerReadIds(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Starting worker", taskId);
            }
        }

        public void ReadIds(int taskId, string zip)
        {
            lock (_lock)
            {
                _curZip++;
                var perc = 100*_curZip/(double) _numZips;
                _log.Log("({0}) Reading zip {1}/{2} ({3:0.00}% started): {4}", taskId, _curZip, _numZips, perc, zip);
            }
        }

        public void StopWorkerReadIds(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Stopping worker", taskId);
            }
        }

        public void GroupZipsByIds()
        {
            lock (_lock)
            {
                _log.Log();
                _log.Log(new string('-', 60));
                _log.Log("GroupZipsByIds");
                _log.Log(new string('-', 60));
            }
        }

        private int _numGroups;
        private int _curGroup;

        public void MergeGroups(int numGroups)
        {
            lock (_lock)
            {
                _numGroups = numGroups;

                _log.Log();
                _log.Log(new string('-', 60));
                _log.Log("Merging {0} groups", numGroups);
                _log.Log(new string('-', 60));
            }
        }

        public void StartWorkerMergeGroup(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Starting worker", taskId);
            }
        }

        public void MergeGroup(int taskId, int count)
        {
            lock (_lock)
            {
                _curGroup++;
                var perc = 100*_curGroup/(double) _numGroups;
                _log.Log(
                    "({0}) Merging group {1}/{2} ({3:0.0}% started), contains {4} zips",
                    taskId,
                    _curGroup,
                    _numGroups,
                    perc,
                    count);
            }
        }

        public void StopWorkerMergeGroup(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Stopping worker", taskId);
            }
        }

        private int _numUnclean;
        private int _curUnclean;

        public void Cleaning(int numUnclean)
        {
            lock (_lock)
            {
                _numUnclean = numUnclean;

                _log.Log();
                _log.Log(new string('-', 60));
                _log.Log("Cleaning {0} zips", numUnclean);
                _log.Log(new string('-', 60));
            }
        }

        public void StartWorkerCleanZip(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Starting worker", taskId);
            }
        }

        public void CleanZip(int taskId, string zip)
        {
            lock (_lock)
            {
                _curUnclean++;
                var perc = 100*_curUnclean/(double) _numUnclean;
                _log.Log(
                    "({0}) Cleaning zip {1}/{2} ({3:0.0}% started): {4}",
                    taskId,
                    _curUnclean,
                    _numUnclean,
                    perc,
                    zip);
            }
        }

        public void StopWorkerCleanZip(int taskId)
        {
            lock (_lock)
            {
                _log.Log("({0}) Stopping worker", taskId);
            }
        }

        public void Error(int taskId, Exception e)
        {
            lock (_lock)
            {
                _log.Log(new string('#', 60));
                _log.Log("# exception for worker {0}: {1}", taskId, e.Message);

                var frames = e.StackTrace.Replace("\r\n", "\n").Split('\n');
                foreach (var frame in frames)
                {
                    _log.Log("# {0}", frame);
                }
                _log.Log(new string('#', 60));
            }
        }
    }
}