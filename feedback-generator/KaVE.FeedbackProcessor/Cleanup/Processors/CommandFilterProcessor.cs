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
 */

using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Collections;

namespace KaVE.FeedbackProcessor.Cleanup.Processors
{
    internal class CommandFilterProcessor : BaseProcessor
    {
        private CommandEvent _oldCommandEvent;

        public CommandFilterProcessor()
        {
            RegisterFor<CommandEvent>(FilterCommandEvents);
        }

        public IKaVESet<IDEEvent> FilterCommandEvents(CommandEvent commandEvent)
        {
            if (IsDuplicate(commandEvent) || IsNotNamed(commandEvent.CommandId))
            {
                return AnswerDrop();
            }

            _oldCommandEvent = commandEvent;

            return AnswerKeep();
        }

        private bool IsDuplicate(CommandEvent commandEvent)
        {
            return _oldCommandEvent != null && _oldCommandEvent.CommandId == commandEvent.CommandId;
        }

        private static bool IsNotNamed(string commandId)
        {
            // example: "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:331:"
            var unnamedCommandPattern = new Regex(@"^\{.*\}:.*:$");
            return commandId == null || unnamedCommandPattern.IsMatch(commandId);
        }
    }
}