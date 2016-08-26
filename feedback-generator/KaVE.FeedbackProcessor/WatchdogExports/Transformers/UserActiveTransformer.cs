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
using KaVE.Commons.Utils.Assertion;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    internal class UserActiveTransformer : IEventToIntervalTransformer<UserActiveInterval>
    {
        private readonly IList<UserActiveInterval> _intervals;
        private readonly TimeSpan _maxInactivityTime;
        private readonly TransformerContext _context;

        private UserActiveInterval curInt;

        public UserActiveTransformer(TransformerContext context, TimeSpan? maxInactivityTime = null)
        {
            _intervals = new List<UserActiveInterval>();
            _context = context;
            _maxInactivityTime = maxInactivityTime ?? TimeSpan.FromSeconds(16);
        }

        public void ProcessEvent(IDEEvent e)
        {
            Asserts.That(e.TriggeredAt.HasValue);
            Asserts.That(e.TerminatedAt.HasValue);

            var curEnd = e.TerminatedAt.Value;

            if (IsTimeout(e))
            {
                curInt = null;
            }

            if (curInt == null)
            {
                _intervals.Add(curInt = _context.CreateIntervalFromEvent<UserActiveInterval>(e));
            }

            _context.UpdateDurationForIntervalToMaximum(curInt, curEnd);
        }

        private bool IsTimeout(IIDEEvent e)
        {
            if (curInt == null)
            {
                return false;
            }
            Asserts.That(e.TriggeredAt.HasValue);
            return curInt.EndTime + _maxInactivityTime < e.TriggeredAt.Value;
        }

        public IEnumerable<UserActiveInterval> SignalEndOfEventStream()
        {
            return _intervals;
        }
    }
}