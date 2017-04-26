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
using System.Threading.Tasks;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class StatisticsRunner
    {
        private readonly object _lock = new object();

        private readonly IPreprocessingIo _io;
        private readonly StatisticsLogger _log;
        private readonly int _numProcs;


        private ISet<string> _ids;
        private IDictionary<string, UserStatistics> _results;

        public StatisticsRunner(IPreprocessingIo io, StatisticsLogger log, int numProcs)
        {
            _io = io;
            _log = log;
            _numProcs = numProcs;
        }

        public void Run()
        {
            _ids = new HashSet<string>();
            _results = new Dictionary<string, UserStatistics>();

            _log.ReportTimeout();

            FindZips();
            InParallel(CreateStatistics);

            _log.Result(_results);
        }

        private void FindZips()
        {
            _log.SearchingZips(_io.GetFullPath_In(""));
            foreach (var zip in _io.FindRelativeZipPaths())
            {
                _ids.Add(zip);
            }
            _log.FoundZips(_ids.Count);
        }

        private void CreateStatistics(int taskId)
        {
            _log.StartingStatCreation(taskId);

            var extractor = new StatisticsExtractor();

            string zip;
            while (GetNextZip(out zip))
            {
                _log.CreatingStats(taskId, zip);
                var file = _io.GetFullPath_In(zip);
                using (var ra = new ReadingArchive(file))
                {
                    var es = ra.GetAllLazy<IDEEvent>();
                    var stats = extractor.CreateStatistics(es);
                    StoreResult(zip, stats);
                }
            }

            _log.FinishedStatCreation(taskId);
        }

        #region multi-threading utils

        private void InParallel(Action<int> task)
        {
            var tasks = new Task[_numProcs];
            for (var i = 0; i < _numProcs; i++)
            {
                var taskId = i;
                tasks[i] = Task.Factory.StartNew(() => { task(taskId); });
            }
            Task.WaitAll(tasks);
        }

        private bool GetNextZip(out string zip)
        {
            lock (_lock)
            {
                if (_ids.Count > 0)
                {
                    zip = _ids.First();
                    _ids.Remove(zip);
                    return true;
                }
                zip = null;
                return false;
            }
        }

        private void StoreResult(string zip, UserStatistics stats)
        {
            lock (_lock)
            {
                _results[zip] = stats;
            }
        }

        #endregion
    }
}