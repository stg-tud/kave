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
using System.IO;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.IO;
using KaVE.Commons.Utils.IO.Archives;
using KaVE.Commons.Utils.ObjectUsageExport;

namespace KaVE.RS.SolutionAnalysis
{
    internal class CompletionEventToUsageHistoryRunner
    {
        private readonly string _dirEvents;
        private readonly string _dirHistories;

        private int _numTotalEvents;
        private int _numTotalTuples;

        public CompletionEventToUsageHistoryRunner(string dirEvents, string dirHistories)
        {
            _dirEvents = dirEvents;
            _dirHistories = dirHistories;
        }

        public void Run()
        {
            var i = 0;
            var exports = FindExports();

            Log("found {0} exports:", exports.Count);
            foreach (var export in exports)
            {
                Log("\t- {0}", export);
            }
            Log("");

            foreach (var exportFile in exports)
            {
                Log("Processing {0}", exportFile);
                var targetZip = Path.Combine(_dirHistories, (i++) + ".zip");
                using (var wa = new WritingArchive(targetZip))
                {
                    try
                    {
                        Run(exportFile, wa);
                    }
                    catch (Exception e)
                    {
                        Log("EE error during processing...\n{0}", e);
                    }
                }
            }

            Log("");
            Log("found {0} events in total, extracted {1} tuples in total", _numTotalEvents, _numTotalTuples);
        }

        private static void Log(string message, params object[] args)
        {
            Console.Write(@"{0} | ", DateTime.Now);
            Console.WriteLine(message, args);
        }

        private IList<string> FindExports()
        {
            return Directory.EnumerateFiles(_dirEvents, "*.zip", SearchOption.AllDirectories).ToList();
        }

        private void Run(string exportFile, WritingArchive wa)
        {
            var numEvents = 0;
            var numTuples = 0;
            Dictionary<string, List<CompletionEvent>> @events = new Dictionary<string, List<CompletionEvent>>();
            foreach (var @event in ReadCompletionEvents(exportFile))
            {
                if (@event.TriggeredAt.HasValue)
                {
                    var idx = GetIndex(@event);
                    if (!@events.ContainsKey(idx))
                    {
                        @events[idx] = new List<CompletionEvent>();
                    }
                    @events[idx].Add(@event);
                    numEvents++;
                }
            }
            _numTotalEvents += numEvents;
            Log("\tfound {0} completion events over {1} days", numEvents, @events.Keys.Count);

            foreach (var idx in @events.Keys)
            {
                var eventsPerFileOnThatDay = @events[idx];

                if (eventsPerFileOnThatDay.Count < 2)
                {
                    continue;
                }

                var tuple = FindFirstAndLast(eventsPerFileOnThatDay);

                numTuples += GenerateAndStoreTuples(tuple.Item1, tuple.Item2, wa);
            }
            _numTotalTuples += numTuples;
            Log("\textracted {0} tuples", numTuples);
        }

        private static IList<CompletionEvent> ReadCompletionEvents(string exportFile)
        {
            var events = new List<CompletionEvent>();
            var ra = new ReadingArchive(exportFile);
            while (ra.HasNext())
            {
                var e = ra.GetNext<IDEEvent>();
                var cce = e as CompletionEvent;
                if (cce == null)
                {
                    continue;
                }
                if (!cce.TriggeredAt.HasValue)
                {
                    continue;
                }
                if (!IsCSharpFile(cce))
                {
                    continue;
                }
                events.Add(cce);
            }
            return events;
        }

        private static bool IsCSharpFile(IDEEvent cce)
        {
            var extension = Path.GetExtension(cce.ActiveDocument.FileName);
            var isCSharpFile = extension != null && extension.EndsWith("cs");
            return isCSharpFile;
        }

        private Tuple<Context, Context> FindFirstAndLast(List<CompletionEvent> es)
        {
            var earliest = DateTime.MaxValue;
            var latest = DateTime.MinValue;
            Context earliestCtx = null;
            Context latestCtx = null;

            foreach (var e in es)
            {
                if (e.TriggeredAt.HasValue)
                {
                    if (e.TriggeredAt.Value < earliest)
                    {
                        earliest = e.TriggeredAt.Value;
                        earliestCtx = e.Context2;
                    }
                    if (e.TriggeredAt.Value > latest)
                    {
                        latest = e.TriggeredAt.Value;
                        latestCtx = e.Context2;
                    }
                }
            }

            return Tuple.Create(earliestCtx, latestCtx);
        }

        private int GenerateAndStoreTuples(Context first, Context last, WritingArchive wa)
        {
            var numTuples = 0;
            var usageExtractor = new UsageExtractor();

            var usagesFirst = usageExtractor.Export(first);
            var usagesLast = usageExtractor.Export(last);

            var keys = GetKeys(usagesFirst, usagesLast);

            foreach (var key in keys)
            {
                var a = Find(key, usagesFirst);
                var b = Find(key, usagesLast);

                if (a == null && b == null)
                {
                    continue;
                }
                a = a ?? Strip(b);
                b = b ?? Strip(a);
                if (a.Equals(b))
                {
                    continue;
                }

                numTuples++;
                wa.Add(Tuple.Create(a, b));
            }

            return numTuples;
        }

        private ISet<Tuple<CoReMethodName, CoReTypeName>> GetKeys(IKaVEList<Query> aqs, IKaVEList<Query> bqs)
        {
            var keys = new HashSet<Tuple<CoReMethodName, CoReTypeName>>();

            foreach (var qs in new[] {aqs, bqs})
            {
                foreach (var q in qs)
                {
                    keys.Add(Tuple.Create(q.methodCtx, q.type));
                }
            }

            return keys;
        }

        private Query Find(Tuple<CoReMethodName, CoReTypeName> key, IKaVEList<Query> usages)
        {
            foreach (var usage in usages)
            {
                var isMethod = usage.methodCtx.Equals(key.Item1);
                var isType = usage.type.Equals(key.Item2);
                if (isMethod && isType)
                {
                    return usage;
                }
            }

            return null;
        }

        private Query Strip(Query orig)
        {
            return new Query
            {
                type = orig.type,
                classCtx = orig.classCtx,
                methodCtx = orig.methodCtx,
                definition = orig.definition
            };
        }

        private static string GetIndex(CompletionEvent @event)
        {
            var date = @event.TriggeredAt ?? DateTime.MinValue;
            var dateStr = string.Format("{0}{1}{2}", date.Year, date.Month, date.Day);
            var encTypeStr = @event.Context2.SST.EnclosingType.GetHashCode();
            return string.Format("{0}-{1}", dateStr, encTypeStr);
        }
    }
}