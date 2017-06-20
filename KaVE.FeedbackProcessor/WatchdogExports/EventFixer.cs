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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor.WatchdogExports
{
    public interface IEventFixer
    {
        IEnumerable<IDEEvent> FixAndFilter(IEnumerable<IDEEvent> events);
    }

    public class EventFixer : IEventFixer
    {
        public IEnumerable<IDEEvent> FixAndFilter(IEnumerable<IDEEvent> events)
        {
            IDEEvent cur;
            IDEEvent next;

            using (var en = events.GetEnumerator())
            {
                if (!en.MoveNext())
                {
                    yield break;
                }

                cur = en.Current;
                while (en.MoveNext())
                {
                    next = en.Current;

                    if (IsRepeatedEvent(cur, next, out cur))
                    {
                        continue;
                    }

                    if (!cur.TriggeredAt.HasValue)
                    {
                        cur = next;
                        continue;
                    }
                    Fix(cur);
                    yield return cur;

                    cur = next;
                }

                if (!cur.TriggeredAt.HasValue)
                {
                    yield break;
                }
                Fix(cur);
                yield return cur;
            }
        }

        private static void Fix(IDEEvent cur)
        {
            Asserts.That(cur.TriggeredAt.HasValue);
            if (!cur.TerminatedAt.HasValue || cur is EditEvent)
            {
                cur.TerminatedAt = cur.TriggeredAt;
            }

            var tre = cur as TestRunEvent;
            if (tre != null)
            {
                var lastEnd = cur.TriggeredAt.Value;
                foreach (var tr in tre.Tests)
                {
                    if (!tr.StartTime.HasValue)
                    {
                        tr.StartTime = lastEnd;
                        lastEnd += tr.Duration;
                    }
                }
            }
        }

        private bool IsRepeatedEvent(IDEEvent cur, IDEEvent next, out IDEEvent newCur)
        {
            var ce1 = cur as CommandEvent;
            var ce2 = next as CommandEvent;

            var bothAreCmds = ce1 != null && ce2 != null;
            if (!bothAreCmds)
            {
                newCur = cur;
                return false;
            }

            if (!ce1.TriggeredAt.HasValue || !ce2.TriggeredAt.HasValue)
            {
                newCur = cur;
                return false;
            }

            var haveSameTiming = (ce1.TriggeredAt.Value - ce2.TriggeredAt.Value).Duration() <
                                 TimeSpan.FromMilliseconds(100);
            if (!haveSameTiming)
            {
                newCur = cur;
                return false;
            }

            var id1 = GetId(ce1);
            var id2 = GetId(ce2);
            var haveSameIds = id1.Equals(id2);
            if (!haveSameIds)
            {
                newCur = cur;
                return false;
            }

            if (ce1.TerminatedAt.HasValue)
            {
                newCur = ce1;
                return true;
            }

            newCur = ce2;
            return true;
        }

        private string GetId(CommandEvent ce1)
        {
            var cid = ce1.CommandId;
            if (!cid.Contains(":"))
            {
                return cid;
            }
            var cid2 = cid.Substring(cid.LastIndexOf(':') + 1);
            if (cid2.Length >= 3)
            {
                return cid2;
            }
            return cid;
        }
    }
}