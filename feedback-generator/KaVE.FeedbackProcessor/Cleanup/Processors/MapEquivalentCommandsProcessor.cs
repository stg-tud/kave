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
 *    - Mattis Manfred Kämmerer
 *    - Markus Zimmermann
 */

using System;
using System.Collections.Generic;
using KaVE.Commons.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Heuristics;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Utils;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class MapEquivalentCommandsProcessor : BaseEventMapper
    {
        public static readonly TimeSpan EventTimeDifference = TimeSpan.FromMilliseconds(10);

        private readonly List<SortedCommandPair> _mappings;

        private CommandEvent _unmappedLeftSideEvent;

        private CommandEvent _unmappedRightSideEvent;

        private CommandEvent _unmappedDebugEvent;

        private CommandEvent _unmappedTextControlEvent;


        public MapEquivalentCommandsProcessor(IResourceProvider resourceProvider)
        {
            _mappings = resourceProvider.GetCommandMappings();

            RegisterFor<CommandEvent>(MapCommandEvent);
        }

        public override void OnStreamStarts(Developer value)
        {
            _unmappedRightSideEvent = null;
            _unmappedDebugEvent = null;
            _unmappedTextControlEvent = null;
        }

        public override void OnStreamEnds()
        {
            // TODO: implement a way to insert the cached event on stream end.
        }

        private void MapCommandEvent(CommandEvent commandEvent)
        {
            // returns true if StandardMappingProcedure should be called
            if (!HandleDebugCommands(commandEvent))
            {
                return;
            }
            if (!HandleTextControlCommands(commandEvent))
            {
                return;
            }

            StandardMappingProcedure(commandEvent);
        }

        private void StandardMappingProcedure(CommandEvent commandEvent)
        {
            var mapping = FindMappingFromLeftSideFor(commandEvent);
            var isOnLeftSide = mapping != null;
            if (isOnLeftSide)
            {
                if (commandEvent.TriggeredBy == IDEEvent.Trigger.Unknown)
                {
                    _unmappedLeftSideEvent = commandEvent;
                    DropCurrentEvent();
                    if (_unmappedRightSideEvent != null) Insert(_unmappedRightSideEvent);
                }
                else 
                {
                    if (_unmappedRightSideEvent != null && _unmappedRightSideEvent.CommandId.Equals(mapping.Item2))
                    {
                        _unmappedRightSideEvent = null;
                    }

                    ReplaceCurrentEventWith(ChangeCommandId(commandEvent, mapping.Item2));
                }
            }

            if (_unmappedRightSideEvent != null && IsLate(commandEvent))
            {
                Insert(_unmappedRightSideEvent);
                _unmappedRightSideEvent = null;
            }

            mapping = FindMappingFromRightSideFor(commandEvent);
            var isOnRightSide = mapping != null;
            if (isOnRightSide)
            {
                if (_unmappedLeftSideEvent == null)
                {
                    _unmappedRightSideEvent = commandEvent;
                    DropCurrentEvent();
                }
                else
                {
                    _unmappedLeftSideEvent = null;
                }
            }
        }

        private bool HandleTextControlCommands(CommandEvent commandEvent)
        {
            if (_unmappedTextControlEvent != null)
            {
                if (ConcurrentEventHeuristic.AreConcurrent(_unmappedTextControlEvent, commandEvent))
                {
                    DropCurrentEvent();
                    _unmappedTextControlEvent = commandEvent;
                    return false;
                }
                _unmappedTextControlEvent = null;
            }

            if (IsTextControlClickEvent(commandEvent))
            {
                _unmappedTextControlEvent = commandEvent;
            }
            return true;
        }

        private bool HandleDebugCommands(CommandEvent commandEvent)
        {
            if (_unmappedDebugEvent != null)
            {
                if (IsFollowupDebugCommand(commandEvent))
                {
                    DropCurrentEvent();
                    return false;
                }
                InsertMergedDebugCommand();
                _unmappedDebugEvent = null;
            }

            if (IsDebugClickEvent(commandEvent))
            {
                _unmappedDebugEvent = commandEvent;
                DropCurrentEvent();
                return false;
            }
            return true;
        }

        private static bool IsDebugClickEvent(CommandEvent commandEvent)
        {
            return commandEvent.CommandId.Equals("Continue") || commandEvent.CommandId.Equals("Start");
        }

        private static bool IsFollowupDebugCommand(CommandEvent commandEvent)
        {
            return (commandEvent.CommandId.Equals("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start") ||
                    commandEvent.CommandId.Equals("{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget"));
        }

        private static bool IsTextControlClickEvent(CommandEvent commandEvent)
        {
            return commandEvent.CommandId.Equals("Copy") || commandEvent.CommandId.Equals("Cut") ||
                   commandEvent.CommandId.Equals("Paste");
        }

        private bool IsLate(IDEEvent commandEvent)
        {
            return !ConcurrentEventHeuristic.AreConcurrent(commandEvent, _unmappedRightSideEvent);
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

        private void InsertMergedDebugCommand()
        {
            var newEvent = new CommandEvent
            {
                CommandId = "Debug." + _unmappedDebugEvent.CommandId
            };
            newEvent.CopyIDEEventPropertiesFrom(_unmappedDebugEvent);
            Insert(newEvent);
        }

        private SortedCommandPair FindMappingFromLeftSideFor(CommandEvent commandEvent)
        {
            return _mappings.Find(pair => (pair.Item1.Equals(commandEvent.CommandId)));
        }

        private SortedCommandPair FindMappingFromRightSideFor(CommandEvent commandEvent)
        {
            return _mappings.Find(pair => (pair.Item2.Equals(commandEvent.CommandId)));
        }
    }
}