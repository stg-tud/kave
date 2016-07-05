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
using System.Linq;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.CodeElements;

namespace KaVE.RS.SolutionAnalysis.CompletionEventStatistics
{
    public class CompletionEventStatisticsRunner
    {
        private readonly ICompletionEventStatisticsIo _io;
        private readonly ICompletionEventStatisticsLogger _log;

        public CompletionEventStatisticsRunner(ICompletionEventStatisticsIo io, ICompletionEventStatisticsLogger log)
        {
            _io = io;
            _log = log;
        }

        public void Run()
        {
            foreach (var zip in FindZips())
            {
                _log.StartingZip(zip);

                foreach (var ce in FindAppliedcompletionEvents(zip))
                {
                    var m = GetSelectedMethodName(ce);
                    if (m != null)
                    {
                        _log.Store(m);
                    }
                    else
                    {
                        _log.FoundOtherProposal();
                    }
                }
                _log.DoneWithZip();
            }

            _log.Done();
        }

        private IList<string> FindZips()
        {
            var zips = _io.FindZips();
            _log.FoundZips(zips);
            return zips;
        }

        private IList<ICompletionEvent> FindAppliedcompletionEvents(string zip)
        {
            var ces = _io.FindAppliedCompletionEvents(zip);
            _log.FoundAppliedCompletionEvents(ces);
            return ces;
        }

        private static IMethodName GetSelectedMethodName(ICompletionEvent ce)
        {
            if (ce.LastSelectedProposal != null) {
                var proposalName = ce.LastSelectedProposal.Name;
                var m = proposalName as IMethodName;
                return m;
            }
            return null;
        }
    }
}