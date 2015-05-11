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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names.VisualStudio;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal class MergeCommandHeuristic
    {
        public static CommandEvent MergeCommandEvents(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return new CommandEvent
            {
                ActiveDocument = GetMergedActiveDocument(commandEvent1, commandEvent2),
                ActiveWindow = GetMergedActiveWindow(commandEvent1, commandEvent2),
                CommandId = GetMergedCommandId(commandEvent1, commandEvent2),
                Duration = GetMergedDuration(commandEvent1, commandEvent2),
                Id = GetMergedId(commandEvent1, commandEvent2),
                IDESessionUUID = commandEvent1.IDESessionUUID,
                KaVEVersion = commandEvent1.KaVEVersion,
                TriggeredAt = GetMergedTriggeredAt(commandEvent1, commandEvent2),
                TerminatedAt = GetMergedTerminatedAt(commandEvent1, commandEvent2),
                TriggeredBy = GetMergedTriggeredBy(commandEvent1, commandEvent2)
            };
        }

        private static DocumentName GetMergedActiveDocument(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return IsVisualStudioCommandEvent(commandEvent1)
                ? commandEvent1.ActiveDocument
                : commandEvent2.ActiveDocument;
        }

        private static WindowName GetMergedActiveWindow(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return IsVisualStudioCommandEvent(commandEvent1) ? commandEvent1.ActiveWindow : commandEvent2.ActiveWindow;
        }

        private static string GetMergedCommandId(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return IsVisualStudioCommandEvent(commandEvent1) ? commandEvent1.CommandId : commandEvent2.CommandId;
        }

        private static TimeSpan? GetMergedDuration(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            var terminatedAt = GetMergedTerminatedAt(commandEvent1, commandEvent2);
            var triggeredAt = GetMergedTriggeredAt(commandEvent1, commandEvent2);
            return terminatedAt - triggeredAt;
        }

        private static string GetMergedId(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return IsVisualStudioCommandEvent(commandEvent1) ? commandEvent1.Id : commandEvent2.Id;
        }

        private static DateTime? GetMergedTerminatedAt(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TerminatedAt > commandEvent2.TerminatedAt
                ? commandEvent1.TerminatedAt
                : commandEvent2.TerminatedAt;
        }

        private static DateTime? GetMergedTriggeredAt(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TriggeredAt < commandEvent2.TriggeredAt
                ? commandEvent1.TriggeredAt
                : commandEvent2.TriggeredAt;
        }

        private static IDEEvent.Trigger GetMergedTriggeredBy(CommandEvent commandEvent1, CommandEvent commandEvent2)
        {
            return commandEvent1.TriggeredBy != IDEEvent.Trigger.Unknown
                ? commandEvent1.TriggeredBy
                : commandEvent2.TriggeredBy;
        }

        private static bool IsVisualStudioCommandEvent(CommandEvent commandEvent)
        {
            return commandEvent.CommandId != null && IsVisualStudioCommandId(commandEvent.CommandId);
        }

        public static bool IsVisualStudioCommandId(string commandId)
        {
            return new Regex(@"^\{.*\}:.*:").IsMatch(commandId);
        }
    }
}