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

using System.Text.RegularExpressions;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal class CommandCompareHeuristic
    {
        public static int CompareCommands(string command1, string command2)
        {
            if (IsVisualStudioCommand(command1) && !IsVisualStudioCommand(command2))
            {
                return -1;
            }
            if (!IsVisualStudioCommand(command1) && IsVisualStudioCommand(command2))
            {
                return 1;
            }
            if (IsReSharperCommand(command1) && !IsReSharperCommand(command2))
            {
                return -1;
            }
            if (!IsReSharperCommand(command1) && IsReSharperCommand(command2))
            {
                return 1;
            }
            return 0;
        }

        public static bool IsVisualStudioCommand(string commandId)
        {
            return new Regex(@"^\{.*\}:.*:").IsMatch(commandId);
        }

        public static bool IsReSharperCommand(string commandId)
        {
            return
                !IsVisualStudioCommand(commandId) &&
                (commandId.Contains(".") || commandId.Contains("_")) &&
                !commandId.Contains(" ");
        }

        public static bool IsOtherCommand(string commandId)
        {
            return !IsVisualStudioCommand(commandId) && !IsReSharperCommand(commandId);
        }
    }
}