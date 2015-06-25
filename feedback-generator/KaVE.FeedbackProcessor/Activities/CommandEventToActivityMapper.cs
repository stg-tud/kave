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
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Csv;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Properties;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class CommandEventToActivityMapper : BaseToActivityMapper
    {
        private static readonly ISet<string> WaitingCommands = new HashSet<string>
        {
            "{BC6F0E30-528C-4EEA-BC2E-C6B35E589068}:265:FortifyToolMenu.ExportAuditProject",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:331:File.SaveSelectedItems",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:882:Build.BuildSolution",
            "{5BF14E63-E267-4787-B20B-B814FD043B38}:21014:File.TfsCheckIn",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:6200:TeamFoundationContextMenus.SourceControlExplorer.TfsContextExplorerCloak",
            "{5BF14E63-E267-4787-B20B-B814FD043B38}:21009:ClassViewContextMenus.ClassViewProject.SourceControl.TfsContextUndoCheckout",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:6356:TeamFoundationContextMenus.PendingChangesPageChangestoInclude.TfsContextPendingChangesPageUndo",
            "{5BF14E63-E267-4787-B20B-B814FD043B38}:21008:File.TfsUndoCheckout",
            "{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}:4653:File.TfsFindChangesets",
            "{5BF14E63-E267-4787-B20B-B814FD043B38}:21010:File.TfsGetLatestVersion",
            "{15061D55-E726-4E3C-97D3-1B871D9B5AE9}:20483:Team.GotoWorkItem",
            "{BC6F0E30-528C-4EEA-BC2E-C6B35E589068}:260:FortifyToolMenu.GenerateReport",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:295:Debug.Start",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:249:Debug.StepOver",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:213:Debug.AttachtoProcess",
            "{3A680C5B-F815-414B-AA4A-0BE57DADB1AF}:512:Debug.ReAttach",
            "{5EFC7975-14BC-11CF-9B2B-00AA00573819}:179:Debug.StopDebugging",
            "CleanupCode",
            "Generate",
            "AnalyzeReferences",
            "FindUsages",
            "Template1",
            "SilentCleanupCode",
            "RefactorThis",
            "GotoDeclarationShort",
        };

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
                if (cmd.CommandId.Contains("tfs", CompareOptions.IgnoreCase))
                {
                    activity = Activity.ProjectManagement;
                }
                else if (WaitingCommands.Contains(cmd.CommandId))
                {
                    activity = Activity.Waiting;
                }
                else
                {
                    activity = Activity.Other;
                }
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