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
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.ObjectUsageExport;

namespace KaVE.RS.SolutionAnalysis.CompletionEventToMicroCommits
{
    public interface IMicroCommitGenerator
    {
        string GetTemporalIndex(CompletionEvent @event);
        Tuple<Context, Context> FindFirstAndLast(IList<CompletionEvent> es);
        List<Tuple<Query, Query>> GenerateTuples(Context first, Context last);
    }

    public class MicroCommitGenerator : IMicroCommitGenerator
    {
        private readonly IUsageExtractor _usageExtractor;

        public MicroCommitGenerator(IUsageExtractor usageExtractor)
        {
            _usageExtractor = usageExtractor;
        }

        private static ISet<Tuple<CoReMethodName, CoReTypeName>> GetLocationIndices(IKaVEList<Query> aqs,
            IKaVEList<Query> bqs)
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

        public List<Tuple<Query, Query>> GenerateTuples(Context first, Context last)
        {
            var tuples = new List<Tuple<Query, Query>>();

            var usageExtractor = _usageExtractor;

            var usagesFirst = usageExtractor.Export(first);
            var usagesLast = usageExtractor.Export(last);

            var keys = GetLocationIndices(usagesFirst, usagesLast);

            var unknownType = TypeName.UnknownName.ToCoReName();

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
                if (HaveEqualCallSites(a, b))
                {
                    continue;
                }

                var type = a.type;
                if (unknownType.Equals(type))
                {
                    continue;
                }

                tuples.Add(Tuple.Create(a, b));
            }

            return tuples;
        }

        private static bool HaveEqualCallSites(Query a, Query b)
        {
            return a.sites.Equals(b.sites);
        }

        private static Query Find(Tuple<CoReMethodName, CoReTypeName> key, IKaVEList<Query> usages)
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

        private static Query Strip(Query orig)
        {
            return new Query
            {
                type = orig.type,
                classCtx = orig.classCtx,
                methodCtx = orig.methodCtx,
                definition = orig.definition
            };
        }

        public string GetTemporalIndex(CompletionEvent @event)
        {
            var date = @event.TriggeredAt ?? DateTime.MinValue;
            var dateStr = string.Format("{0:0000}{1:00}{2:00}", date.Year, date.Month, date.Day);
            var encTypeStr = @event.Context2.SST.EnclosingType.GetHashCode();
            return string.Format("{0}_{1}_{2}", @event.IDESessionUUID, dateStr, encTypeStr);
        }

        public Tuple<Context, Context> FindFirstAndLast(IList<CompletionEvent> es)
        {
            var earliestDate = DateTime.MaxValue;
            var latestDate = DateTime.MinValue;
            Context earliest = null;
            Context latest = null;

            Asserts.That(es.Count > 1);

            foreach (var e in es)
            {
                if (e.TriggeredAt.HasValue)
                {
                    if (e.TriggeredAt.Value < earliestDate)
                    {
                        earliestDate = e.TriggeredAt.Value;
                        earliest = e.Context2;
                    }
                    if (e.TriggeredAt.Value >= latestDate)
                    {
                        latestDate = e.TriggeredAt.Value;
                        latest = e.Context2;
                    }
                }
            }
            Asserts.NotSame(earliest, latest);
            Asserts.NotNull(earliest);
            Asserts.NotNull(latest);

            // TODO: in case of an "applied" proposal, enrich context with selection

            return Tuple.Create(earliest, latest);
        }
    }
}