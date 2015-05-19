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
 *    - Sebastian Proksch
 *    - Sven Amann
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class CommandEventToActivityMapper : BaseToActivityMapper
    {
        private readonly IDictionary<string, Activity> _commandIdToActivityMapping = new Dictionary<string, Activity>();

        public CommandEventToActivityMapper()
        {
            // TODO @Sven make constructor paramters for mapper possible and move this to outside of here
            var mappingCsv = Resources.CommandIdToActivityMapping;
            var commandIdToActivityMap = CsvTable.Read(mappingCsv);
            foreach (var csvRow in commandIdToActivityMap.Rows)
            {
                _commandIdToActivityMapping[csvRow["Command ID"]] = ToActivity(csvRow["Mapping"]);
            }


            RegisterFor<CommandEvent>(ProcessCommandEvent);
        }

        private static Activity ToActivity(string shorthand)
        {
            switch (shorthand)
            {
                case "D":
                    return Activity.Development;
                case "U":
                    return Activity.Navigation;
                case "PM":
                    return Activity.ProjectManagement;
                case "LC":
                    return Activity.LocalConfiguration;
                case "":
                    return Activity.Other;
                default:
                    throw new Exception("Unknown activity shorthand: " + shorthand);
            }
        }

        private void ProcessCommandEvent(CommandEvent cmd)
        {
            var commandId = cmd.CommandId;
            Activity activity;
            if (IsOpenRecentFileCommand(commandId))
            {
                activity = Activity.Navigation;
            }
            else if (!_commandIdToActivityMapping.TryGetValue(commandId, out activity))
            {
                activity = Activity.Other;
            }
            InsertActivity(cmd, activity);
        }

        private static bool IsOpenRecentFileCommand(string commandId)
        {
            // open recent file commands look like "0 - C:\Users\Sven\Documents\MySolution\MySolution.sln"
            return Regex.IsMatch(commandId, @"^[0-9] (?:[a-zA-Z]\:|\\\\[\w\.]+\\[\w.$]+)\\(?:[\w]+\\)*\w([\w.])+$");
        }
    }
}