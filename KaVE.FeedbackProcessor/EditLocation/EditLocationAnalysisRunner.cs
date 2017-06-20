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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.FeedbackProcessor.Preprocessing.Model;
using KaVE.FeedbackProcessor.StatisticsUltimate;

namespace KaVE.FeedbackProcessor.EditLocation
{
    internal class EditLocationAnalysisRunner : StatisticsRunnerBase
    {
        private readonly IPreprocessingIo _io;
        private readonly IEditLocationAnalysisLogger _log;

        private IEditLocationResults _results;

        public EditLocationAnalysisRunner(int numProcs, IPreprocessingIo io, IEditLocationAnalysisLogger log) : base(
            io,
            log,
            numProcs)
        {
            _io = io;
            _log = log;
        }

        public void Run()
        {
            _results = new EditLocationResults {Zip = "all"};

            FindZips();
            InParallel(CreateStatistics);

            _log.FinalResults(_results);
        }

        private void CreateStatistics(int taskId)
        {
            _log.StartingStatCreation(taskId);

            string zip;
            while (GetNextZip(out zip))
            {
                _log.CreatingStats(taskId, zip);
                var res = Analyze(zip);

                lock (Lock)
                {
                    _results.Add(res);
                    _log.IntermediateResults(taskId, res);
                }
            }

            _log.FinishedStatCreation(taskId);
        }

        private EditLocationResults Analyze(string zip)
        {
            var res = new EditLocationResults {Zip = zip};
            var file = _io.GetFullPath_In(zip);
            using (var ra = new ReadingArchive(file))
            {
                var locAnal = new RelativeEditLocationAnalysis();
                while (ra.HasNext())
                {
                    var @event = ra.GetNext<IDEEvent>();
                    res.NumEvents++;

                    var complEvent = @event as CompletionEvent;
                    if (complEvent == null)
                    {
                        continue;
                    }

                    var fileName = complEvent.ActiveDocument.FileName;
                    if (fileName != null && !fileName.EndsWith(".cs"))
                    {
                        continue;
                    }

                    res.NumCompletionEvents++;

                    var loc = locAnal.Analyze(complEvent.Context2.SST);
                    if (!loc.HasEditLocation || loc.Size < 2)
                    {
                        continue;
                    }

                    res.NumLocations++;

                    if (complEvent.TerminatedState == TerminationState.Applied)
                    {
                        res.AppliedEditLocations.Add(loc);
                    }
                    else
                    {
                        res.OtherEditLocations.Add(loc);
                    }
                }
            }
            return res;
        }
    }
}