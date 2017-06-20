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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class CommandFollowupProcessor : BaseEventMapper
    {
        private CommandEvent _commandEvent;

        public CommandFollowupProcessor()
        {
            RegisterFor<IDEEvent>(GenerateConcurrentEvent);
        }

        public override void OnStreamStarts(Developer value)
        {
            _commandEvent = null;
        }

        public void GenerateConcurrentEvent(IDEEvent @event)
        {
            if (@event is CommandEvent)
            {
                _commandEvent = @event as CommandEvent;
            }
            else if (_commandEvent != null)
            {
                var resultEvent = new ConcurrentEvent
                {
                    ActiveDocument = @event.ActiveDocument,
                    ActiveWindow = @event.ActiveWindow,
                    Duration = @event.TerminatedAt - _commandEvent.TriggeredAt,
                    ConcurrentEventList = new KaVEList<IDEEvent> {_commandEvent, @event},
                    IDESessionUUID = @event.IDESessionUUID,
                    KaVEVersion = @event.KaVEVersion,
                    TerminatedAt = @event.TerminatedAt,
                    TriggeredAt = _commandEvent.TriggeredAt,
                    TriggeredBy = _commandEvent.TriggeredBy
                };

                _commandEvent = null;

                ReplaceCurrentEventWith(resultEvent);
            }

            DropCurrentEvent();
        }
    }
}