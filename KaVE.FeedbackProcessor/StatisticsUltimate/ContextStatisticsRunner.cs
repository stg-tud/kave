/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public class ContextStatisticsRunner : StatisticsRunnerBase
    {
        private readonly IPreprocessingIo _io;
        private readonly IContextStatisticsLogger _log;

        private IContextStatistics _results;
        private AssemblyCounter _asmCounter;
        private readonly IContextFilter _cf;

        public ContextStatisticsRunner(IPreprocessingIo io,
            IContextFilter cf,
            IContextStatisticsLogger log,
            int numProcs)
            : base(io, log, numProcs)
        {
            _io = io;
            _cf = cf;
            _log = log;
        }

        public void Run()
        {
            _results = new ContextStatistics();
            _asmCounter = new AssemblyCounter();

            _log.StartUp(_cf);

            FindZips();
            InParallel(CreateStatistics);

            _log.Results(_results, _asmCounter.Counts);
        }

        private void CreateStatistics(int taskId)
        {
            _log.StartingStatCreation(taskId);

            var extractor = new ContextStatisticsExtractor(_cf);

            string zip;
            while (GetNextZip(out zip))
            {
                _log.CreatingStats(taskId, zip);
                var file = _io.GetFullPath_In(zip);
                using (var ra = new ReadingArchive(file))
                {
                    var es = ra.GetAllLazy<Context>();
                    var stats = extractor.Extract(es);
                    StoreResult(zip, stats);
                }
            }

            _log.FinishedStatCreation(taskId);
        }

        private void StoreResult(string zip, IContextStatistics stats)
        {
            lock (Lock)
            {
                _results.Add(stats);
                _asmCounter.Count(stats.UniqueAssemblies);
            }
        }
    }
}