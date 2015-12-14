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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Csv;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class CommandEventToActivityMapper : BaseToActivityMapper
    {
        private readonly ILogger _logger;

        private readonly IDictionary<string, Activity> _commandIdToActivityMapping = new Dictionary<string, Activity>();

        public CommandEventToActivityMapper(ILogger logger)
        {
            _logger = logger;
            // TODO @Sven make constructor paramters for mapper possible and move this to outside of here
            var mappingCsv = Resources.CommandIdToActivityMapping;
            var commandIdToActivityMap = CsvTable.Read(mappingCsv, ';');
            foreach (var csvRow in commandIdToActivityMap.Rows)
            {
                _commandIdToActivityMapping[csvRow["Command Id"]] = ToActivity(csvRow["Activity"]);
            }


            RegisterFor<CommandEvent>(ProcessCommandEvent);
        }

        private static Activity ToActivity(string shorthand)
        {
            switch (shorthand)
            {
                case "E":
                    return Activity.Development;
                case "N":
                    return Activity.Navigation;
                case "D":
                    return Activity.Debugging;
                case "T":
                    return Activity.Testing;
                case "VC":
                    return Activity.VersionControl;
                case "PM":
                    return Activity.ProjectManagement;
                case "LC":
                    return Activity.LocalConfiguration;
                case "O":
                    return Activity.Other;
                default:
                    throw new Exception("Unknown activity shorthand: " + shorthand);
            }
        }

        private void ProcessCommandEvent(CommandEvent cmd)
        {
            var commandId = cmd.CommandId;
            var candidate = _commandIdToActivityMapping.Keys.FirstOrDefault(commandId.StartsWith);
            if (candidate != null)
            {
                var activity = _commandIdToActivityMapping[candidate];
                InsertActivity(cmd, activity);
            }
            else if (IsOpenRecentFileCommand(commandId))
            {
                InsertActivity(cmd, Activity.Navigation);
            }
            else if (IsCompilerMessageTab(commandId))
            {
                InsertActivity(cmd, Activity.Development);
            }
            else
            {
                _logger.Info("unmapped command: " + commandId.Replace("{", "{{").Replace("}", "}}"));
            }
            DropCurrentEvent();
        }

        private static bool IsCompilerMessageTab(string commandId)
        {
            return commandId.EndsWith("Error") || commandId.EndsWith("Errors") || commandId.EndsWith("Warning") || commandId.EndsWith("Warnings") || commandId.EndsWith("Message") || commandId.EndsWith("Messages");
        }

        private static bool IsOpenRecentFileCommand(string commandId)
        {
            // open recent file commands look like "0 - C:\Users\Sven\Documents\MySolution\MySolution.sln"
            return Regex.IsMatch(commandId, @"^[0-9]+ ");
        }
    }
}