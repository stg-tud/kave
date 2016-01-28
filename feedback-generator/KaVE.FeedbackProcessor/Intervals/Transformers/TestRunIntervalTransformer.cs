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
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Intervals.Transformers
{
    internal class TestRunIntervalTransformer : IEventToIntervalTransformer<TestRunInterval>
    {
        private readonly TransformerContext _context;
        private readonly IList<TestRunInterval> _currentIntervals;

        public TestRunIntervalTransformer(TransformerContext context)
        {
            _context = context;
            _currentIntervals = new List<TestRunInterval>();
        }

        public void ProcessEvent(IDEEvent @event)
        {
            var commandEvent = @event as CommandEvent;
            if (commandEvent != null && commandEvent.CommandId == "UnitTest_RunContext")
            {
                var interval = _context.CreateIntervalFromEvent<TestRunInterval>(@event);
                interval.Duration = TimeSpan.FromSeconds(1);
                _currentIntervals.Add(interval);
            }
        }

        public IEnumerable<TestRunInterval> SignalEndOfEventStream()
        {
            return _currentIntervals;
        }
    }
}