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
 * 
 * Contributors:
 *    - Sven Amann
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class IDECloseEventFixingProcessor : BaseProcessor
    {
        private bool _ideIsRunning;
        private DateTime _lastEventTriggerTime;

        public IDECloseEventFixingProcessor()
        {
            RegisterFor<IDEEvent>(ProcessEvent);
            //RegisterFor<IDEStateEvent>(ProcessIDEStateEvent);
        }

        private void ProcessEvent(IDEEvent currentevent)
        {
            var ideStateEvent = currentevent as IDEStateEvent;
            if (ideStateEvent != null)
            {
                ProcessIDEStateEvent(ideStateEvent);
            }
            else
            {
                Asserts.That(currentevent.TriggeredAt.HasValue, "event without trigger time");
                _lastEventTriggerTime = currentevent.TriggeredAt.Value;
            }
        }

        private void ProcessIDEStateEvent(IDEStateEvent @event)
        {
            switch (@event.IDELifecyclePhase)
            {
                case IDEStateEvent.LifecyclePhase.Startup:
                    if (_ideIsRunning)
                    {
                        Insert(CreateMissingShutdownEvent());
                    }
                    _ideIsRunning = true;
                    break;
                case IDEStateEvent.LifecyclePhase.Shutdown:
                    _ideIsRunning = false;
                    break;
            }
        }

        private IDEStateEvent CreateMissingShutdownEvent()
        {
            return new IDEStateEvent
            {
                IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown,
                TriggeredAt = _lastEventTriggerTime.AddSeconds(1)
            };
        }
    }
}