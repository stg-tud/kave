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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO.Archives;

namespace KaVE.RS.SolutionAnalysis.CompletionEventStatistics
{
    public interface ICompletionEventStatisticsIo
    {
        IKaVEList<string> FindZips();
        IKaVEList<ICompletionEvent> FindAppliedCompletionEvents(string zip);
    }

    public class CompletionEventStatisticsIo : ICompletionEventStatisticsIo
    {
        private readonly string _dirIn;

        public CompletionEventStatisticsIo(string dirIn)
        {
            if (!dirIn.EndsWith(@"\"))
            {
                dirIn += @"\";
            }
            _dirIn = dirIn;
        }

        public IKaVEList<string> FindZips()
        {
            var findCcZips = Directory.EnumerateFiles(_dirIn, "*.zip", SearchOption.AllDirectories);
            var shortened = findCcZips.Select(z => z.Substring(_dirIn.Length));
            return Lists.NewListFrom(shortened);
        }

        public IKaVEList<ICompletionEvent> FindAppliedCompletionEvents(string zip)
        {
            return Lists.NewListFrom(ReadCce(zip));
        }

        private IEnumerable<ICompletionEvent> ReadCce(string zipName)
        {
            var fullPath = Path.Combine(_dirIn, zipName);
            using (var ra = new ReadingArchive(fullPath))
            {
                while (ra.HasNext())
                {
                    var e = ra.GetNext<IDEEvent>() as CompletionEvent;
                    if (e != null)
                    {
                        if (e.TerminatedState == TerminationState.Applied)
                        {
                            var sel = e.LastSelectedProposal;
                            if (sel != null && sel.Name is IMethodName)
                            {
                                yield return e;
                            }
                        }
                    }
                }
            }
        }
    }
}