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

        private CommandEvent _unmappedTripleMappingEvent;

        private CommandEvent _unmappedPairMappingEvent;

        private Tuple<string, string, string> _unmappedTripleMapping;

        private Tuple<string, string> _unmappedPairMapping;

        // Key - First Event in Mapping; Value - Tuple of (Second Event in Mapping, Third Event in Mapping, Merged Command ID)
        protected readonly Dictionary<string, Tuple<string, string, string>> SpecialTripleMappings = new Dictionary
            <string, Tuple<string, string, string>>
        {
            {
                "Copy",
                Tuple.Create(
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:15:Edit.Copy",
                    "TextControl.Copy",
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:15:Edit.Copy")
            },
            {
                "Cut",
                Tuple.Create(
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:16:Edit.Cut",
                    "TextControl.Cut",
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:16:Edit.Cut")
            },
            {
                "Paste",
                Tuple.Create(
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste",
                    "TextControl.Paste",
                    "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:26:Edit.Paste")
            },
        };

        // Key - First Event in Mapping; Value - Tuple of (Second Event in Mapping, Merged Command ID)
        protected readonly Dictionary<string, Tuple<string, string>> SpecialPairMappings = new Dictionary
            <string, Tuple<string, string>>
        {
            {"Start Debugging", Tuple.Create("{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start", "Debug.Start")},
            {"Add", Tuple.Create("{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Add")},
            {"Exclude", Tuple.Create("{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Exclude")},
            {"Include", Tuple.Create("{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove", "Git.Include")}
        };

        public MapEquivalentCommandsProcessor(IResourceProvider resourceProvider)
        {
            _mappings = resourceProvider.GetCommandMappings();

            RegisterFor<CommandEvent>(MapCommandEvent);
        }

        public override void OnStreamStarts(Developer value)
        {
            _unmappedLeftSideEvent = null;
            _unmappedRightSideEvent = null;
            _unmappedTripleMappingEvent = null;
            _unmappedPairMappingEvent = null;
            _unmappedDebugEvent = null;
        }

        public override void OnStreamEnds()
        {
            // TODO: implement a way to insert the cached event on stream end.
        }

        private void MapCommandEvent(CommandEvent commandEvent)
        {
            if (IsDebugClickEvent(commandEvent) || _unmappedDebugEvent != null)
            {
                HandleDebugCommands(commandEvent);
            }
            else if (SpecialTripleMappings.ContainsKey(commandEvent.CommandId) || _unmappedTripleMappingEvent != null)
            {
                HandleSpecialTripleMappings(commandEvent);
            }
            else if (SpecialPairMappings.ContainsKey(commandEvent.CommandId) || _unmappedPairMappingEvent != null)
            {
                HandleSpecialPairMappings(commandEvent);
            }
            else
            {
                StandardMappingProcedure(commandEvent);
            }
        }

        private void StandardMappingProcedure(CommandEvent commandEvent)
        {
            if (_unmappedRightSideEvent != null && IsLate(commandEvent))
            {
                Insert(_unmappedRightSideEvent);
                _unmappedRightSideEvent = null;
            }

            if (FindMappingFromLeftSideFor(commandEvent) != null)
            {
                if (commandEvent.TriggeredBy != IDEEvent.Trigger.Unknown)
                {
                    ReplaceCurrentEventWith(
                        ChangeCommandId(commandEvent, FindMappingFromLeftSideFor(commandEvent).Item2));
                }
                else
                {
                    _unmappedLeftSideEvent = commandEvent;
                    DropCurrentEvent();
                    if (_unmappedRightSideEvent != null)
                    {
                        Insert(_unmappedRightSideEvent);
                    }
                }

                _unmappedRightSideEvent = null;
            }

            if (FindMappingFromRightSideFor(commandEvent) != null)
            {
                if (_unmappedLeftSideEvent == null)
                {
                    _unmappedRightSideEvent = commandEvent;
                    DropCurrentEvent();
                }

                _unmappedLeftSideEvent = null;
            }
        }

        private void HandleDebugCommands(CommandEvent commandEvent)
        {
            if (_unmappedDebugEvent != null)
            {
                if (IsFollowupDebugCommand(commandEvent))
                {
                    DropCurrentEvent();
                    return;
                }
                _unmappedDebugEvent = null;
                MapCommandEvent(commandEvent);
                return;
            }
            _unmappedDebugEvent = commandEvent;
            ReplaceCurrentEventWith(CreateMergedCommand(commandEvent));
        }

        private void HandleSpecialTripleMappings(CommandEvent commandEvent)
        {
            if (_unmappedTripleMappingEvent != null)
            {
                if (_unmappedTripleMapping.Item1.Equals(commandEvent.CommandId))
                {
                    DropCurrentEvent();
                    return;
                }
                if (_unmappedTripleMapping.Item2.Equals(commandEvent.CommandId))
                {
                    DropCurrentEvent(); 
                    _unmappedTripleMappingEvent = null;
                    return;
                }
                _unmappedTripleMappingEvent = null;
                MapCommandEvent(commandEvent);
                return;
            }
            _unmappedTripleMapping = SpecialTripleMappings[commandEvent.CommandId];
            _unmappedTripleMappingEvent = commandEvent;
            ReplaceCurrentEventWith(ChangeCommandId(_unmappedTripleMappingEvent, _unmappedTripleMapping.Item3)); ;
        }

        private void HandleSpecialPairMappings(CommandEvent commandEvent)
        {
            if (_unmappedPairMappingEvent != null)
            {
                if (_unmappedPairMapping.Item1.Equals(commandEvent.CommandId))
                {
                    DropCurrentEvent();
                    _unmappedPairMappingEvent = null;
                    return;
                }
                _unmappedPairMappingEvent = null;
                MapCommandEvent(commandEvent);
                return;
            }
            _unmappedPairMapping = SpecialPairMappings[commandEvent.CommandId];
            _unmappedPairMappingEvent = commandEvent;
            ReplaceCurrentEventWith(ChangeCommandId(_unmappedPairMappingEvent, _unmappedPairMapping.Item2));
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

        private static CommandEvent CreateMergedCommand(CommandEvent commandEvent)
        {
            var newEvent = new CommandEvent
            {
                CommandId = "Debug." + commandEvent.CommandId
            };
            newEvent.CopyIDEEventPropertiesFrom(commandEvent);
            return newEvent;
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