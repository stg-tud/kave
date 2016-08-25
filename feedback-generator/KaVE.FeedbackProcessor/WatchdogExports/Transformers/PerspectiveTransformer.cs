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
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.WatchdogExports.Model;

namespace KaVE.FeedbackProcessor.WatchdogExports.Transformers
{
    internal class PerspectiveTransformer : IEventToIntervalTransformer<PerspectiveInterval>
    {
        private readonly IList<PerspectiveInterval> _intervals;
        private PerspectiveInterval _currentInterval;
        private DateTime _referenceTime;
        private readonly TransformerContext _context;

        public PerspectiveTransformer(TransformerContext context)
        {
            _intervals = new List<PerspectiveInterval>();
            _context = context;
            _referenceTime = DateTime.MinValue;
        }

        public void ProcessEvent(IDEEvent @event)
        {
            if (_currentInterval != null &&
                _currentInterval.Perspective == PerspectiveType.Production &&
                MarksStartOfDebugSession(@event))
            {
                _currentInterval = null;
            }

            if (_currentInterval == null)
            {
                _currentInterval = _context.CreateIntervalFromEvent<PerspectiveInterval>(@event);

                if (_currentInterval.StartTime < _referenceTime)
                {
                    _currentInterval.Duration -= _referenceTime - _currentInterval.StartTime;
                    _currentInterval.StartTime = _referenceTime;
                }

                _intervals.Add(_currentInterval);

                _currentInterval.Perspective = MarksStartOfDebugSession(@event)
                    ? PerspectiveType.Debug
                    : PerspectiveType.Production;
            }
            else
            {
                _context.AdaptIntervalTimeData(_currentInterval, @event);
                _referenceTime = @event.TerminatedAt.GetValueOrDefault();

                if (MarksEndOfDebugSession(@event))
                {
                    _currentInterval = null;
                }
            }
        }

        private static bool MarksStartOfDebugSession(IDEEvent @event)
        {
            var debuggerEvent = @event as DebuggerEvent;
            return debuggerEvent != null &&
                   debuggerEvent.Mode != DebuggerMode.Design;
        }

        private static bool MarksEndOfDebugSession(IDEEvent @event)
        {
            var debuggerEvent = @event as DebuggerEvent;
            return debuggerEvent != null &&
                   debuggerEvent.Mode == DebuggerMode.Design;
        }

        public IEnumerable<PerspectiveInterval> SignalEndOfEventStream()
        {
            return _intervals;
        }
    }
}