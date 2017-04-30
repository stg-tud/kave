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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class InteractionStatisticsRunner : StatisticsRunnerBase
    {
        private readonly IPreprocessingIo _io;
        private readonly InteractionStatisticsLogger _log;


        private IDictionary<string, InteractionStatistics> _results;

        public InteractionStatisticsRunner(IPreprocessingIo io, InteractionStatisticsLogger log, int numProcs)
            : base(io, log, numProcs)
        {
            _io = io;
            _log = log;
        }

        public void Run()
        {
            _results = new Dictionary<string, InteractionStatistics>();

            _log.ReportTimeout();

            FindZips();
            InParallel(CreateStatistics);

            _log.Result(_results);
        }

        private void CreateStatistics(int taskId)
        {
            _log.StartingStatCreation(taskId);

            var extractor = new InteractionStatisticsExtractor();

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

        private void StoreResult(string zip, InteractionStatistics stats)
        {
            lock (Lock)
            {
                _results[zip] = stats;
            }
        }
    }
}