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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class IDECloseEventFixingProcessor : BaseEventMapper
    {
        private bool _firstEvent;
        private bool _ideIsRunning;
        private DateTime _lastEventTriggerTime;

        public IDECloseEventFixingProcessor()
        {
            RegisterFor<IDEStateEvent>(ProcessIDEStateEvent);
            RegisterFor<IDEEvent>(ProcessAnyEvent);
        }

        public override void OnStreamStarts(Developer value)
        {
            _firstEvent = true;
            _ideIsRunning = false;
            _lastEventTriggerTime = default(DateTime);
        }

        private void ProcessIDEStateEvent(IDEStateEvent @event)
        {
            switch (@event.IDELifecyclePhase)
            {
                case IDELifecyclePhase.Startup:
                    if (_ideIsRunning)
                    {
                        Insert(CreateMissingShutdownEvent(_lastEventTriggerTime));
                    }
                    _ideIsRunning = true;
                    break;
                case IDELifecyclePhase.Shutdown:
                    _ideIsRunning = false;
                    break;
            }
        }

        private void ProcessAnyEvent(IDEEvent currentEvent)
        {
            _lastEventTriggerTime = currentEvent.GetTriggeredAt();

            if (!_ideIsRunning && _firstEvent)
            {
                Insert(CreateMissingStartupEvent(_lastEventTriggerTime));
            }

            _firstEvent = false;
        }

        private static IDEStateEvent CreateMissingStartupEvent(DateTime subsequentEventTime)
        {
            return new IDEStateEvent
            {
                IDELifecyclePhase = IDELifecyclePhase.Startup,
                TriggeredAt = subsequentEventTime.AddMilliseconds(-1)
            };
        }

        private static IDEStateEvent CreateMissingShutdownEvent(DateTime previousEventTime)
        {
            return new IDEStateEvent
            {
                IDELifecyclePhase = IDELifecyclePhase.Shutdown,
                TriggeredAt = previousEventTime.AddMilliseconds(1)
            };
        }
    }
}