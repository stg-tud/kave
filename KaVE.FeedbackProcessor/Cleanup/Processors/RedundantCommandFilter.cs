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

using System.Collections.Generic;
using KaVE.Commons.Model.Events;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class RedundantCommandFilter : BaseEventMapper
    {
        public static readonly List<string> RedundantCommands = new List<string>
        {
            "TextControl.Copy",
            "TextControl.Cut",
            "TextControl.Paste",
            "TextControl.Delete",
            "{6E87CFAD-6C05-4ADF-9CD7-3B7943875B7C}:257:Debug.StartDebugTarget",
            "{57735D06-C920-4415-A2E0-7D6E6FBDFA99}:4100:Team.Git.Remove"
        };

        public RedundantCommandFilter()
        {
            RegisterFor<CommandEvent>(FilterRedundantCommands);
        }

        private void FilterRedundantCommands(CommandEvent commandEvent)
        {
            if (IsRedundantCommand(commandEvent.CommandId))
            {
                DropCurrentEvent();
            }
        }

        public static bool IsRedundantCommand(string commandId)
        {
            return RedundantCommands.Contains(commandId);
        }
    }
}