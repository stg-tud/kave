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
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class MapEquivalentCommandsProcessor : BaseEventMapper
    {
        public static readonly TimeSpan EventTimeDifference = TimeSpan.FromMilliseconds(10);

        private readonly List<SortedCommandPair> _mappings;

        public MapEquivalentCommandsProcessor(IResourceProvider resourceProvider)
        {
            _mappings = resourceProvider.GetCommandMappings();

            RegisterFor<CommandEvent>(MapCommandEvent);
        }

        private void MapCommandEvent(CommandEvent commandEvent)
        {
            /*
             This approach only works under the following assumptions:
             * - Exactly one of two corresponding CommandEvents (left side or right side) must have a not-unknown trigger type
             *      (Neither none, nor both!)
             * - None of the right-side events may occur without its left side and with an unknown trigger
             */

            if (IsOnAnySide(commandEvent) && commandEvent.TriggeredBy == EventTrigger.Unknown)
            {
                DropCurrentEvent();
            }
            else if (IsOnLeftSide(commandEvent))
            {
                ReplaceCurrentEventWith(
                    ChangeCommandId(commandEvent, MapCommand(commandEvent.CommandId)));
            }
        }

        private bool IsOnAnySide(CommandEvent commandEvent)
        {
            return (IsOnLeftSide(commandEvent) || IsOnRightSide(commandEvent));
        }

        private bool IsOnLeftSide(CommandEvent commandEvent)
        {
            return _mappings.Any(pair => (pair.Item1.Equals(commandEvent.CommandId)));
        }

        private bool IsOnRightSide(CommandEvent commandEvent)
        {
            return _mappings.Any(pair => (pair.Item2.Equals(commandEvent.CommandId)));
        }

        private string MapCommand(string commandId)
        {
            return _mappings.Find(pair => (pair.Item1.Equals(commandId))).Item2;
        }

        private static CommandEvent ChangeCommandId(IDEEvent commandEvent, string newId)
        {
            return new CommandEvent
            {
                CommandId = newId,
                ActiveDocument = commandEvent.ActiveDocument,
                ActiveWindow = commandEvent.ActiveWindow,
                IDESessionUUID = commandEvent.IDESessionUUID,
                KaVEVersion = commandEvent.KaVEVersion,
                TriggeredAt = commandEvent.TriggeredAt,
                TerminatedAt = commandEvent.TerminatedAt,
                TriggeredBy = commandEvent.TriggeredBy
            };
        }
    }
}