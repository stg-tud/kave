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
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class DuplicateCommandFilterProcessor : BaseEventMapper
    {
        private CommandEvent _oldCommandEvent;

        public DuplicateCommandFilterProcessor()
        {
            RegisterFor<CommandEvent>(FilterCommandEvents);
        }

        public override void OnStreamStarts(Developer value)
        {
            _oldCommandEvent = null;
        }

        public void FilterCommandEvents(CommandEvent commandEvent)
        {
            if (ConcurrentEventHeuristic.IsIgnorableTextControlCommand(commandEvent.CommandId)) return;

            var previousCommandEvent = _oldCommandEvent;
            _oldCommandEvent = commandEvent;
            if (IsDuplicate(commandEvent, previousCommandEvent))
            {
                DropCurrentEvent();
            }
        }

        private static bool IsDuplicate(CommandEvent commandEvent, CommandEvent previousCommandEvent)
        {
            return previousCommandEvent != null && previousCommandEvent.CommandId == commandEvent.CommandId &&
                   ConcurrentEventHeuristic.AreConcurrent(previousCommandEvent, commandEvent);
        }
    }
}