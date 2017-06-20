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
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    internal class VisualStudioOpenedTransformer : IEventToIntervalTransformer<VisualStudioOpenedInterval>
    {
        private readonly IDictionary<string, VisualStudioOpenedInterval> _intervals;
        private readonly TransformerContext _context;

        public VisualStudioOpenedTransformer(TransformerContext context)
        {
            _intervals = new Dictionary<string, VisualStudioOpenedInterval>();
            _context = context;
        }

        public void ProcessEvent(IDEEvent e)
        {
            if (_intervals.ContainsKey(e.IDESessionUUID))
            {
                _context.AdaptIntervalTimeData(_intervals[e.IDESessionUUID], e);
            }
            else
            {
                _intervals.Add(
                    e.IDESessionUUID,
                    _context.CreateIntervalFromEvent<VisualStudioOpenedInterval>(e));
            }
        }

        public IEnumerable<VisualStudioOpenedInterval> SignalEndOfEventStream()
        {
            return _intervals.Values;
        }
    }
}