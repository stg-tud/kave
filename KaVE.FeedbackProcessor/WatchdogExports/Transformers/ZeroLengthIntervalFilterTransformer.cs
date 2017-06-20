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
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    public class ZeroLengthIntervalFilterTransformer : IEventToIntervalTransformer<Interval>
    {
        private readonly IEventToIntervalTransformer<Interval> _subTransformer;

        public ZeroLengthIntervalFilterTransformer(IEventToIntervalTransformer<Interval> subTransformer)
        {
            _subTransformer = subTransformer;
        }

        public void ProcessEvent(IDEEvent @event)
        {
            _subTransformer.ProcessEvent(@event);
        }

        public IEnumerable<Interval> SignalEndOfEventStream()
        {
            return _subTransformer.SignalEndOfEventStream().Where(i => i.Duration > TimeSpan.Zero);
        }
    }
}