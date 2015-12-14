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
using System.Linq;
using System.Text.RegularExpressions;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class WindowEventToActivityMapper : BaseToActivityMapper
    {
        private readonly ILogger _logger;

        private static readonly string[] WorkItemIndicators =
        {
            "Backlog Item ",
            "Bug ",
            "Initiative ",
            "Query ",
            "Requirement ",
            "Requirements ",
            "Task ",
            "User Story ",
            ".wiq", // 'work item query' file extension
            "[Editor]", // marker seen only in captions like "my open tasks [Editor]"
            "[Results]" // marker seen only in captions like "my open tasks [Results]"
        };

        private static readonly IDictionary<string, Activity[]> ToolWindowMapping = new Dictionary<string, Activity[]>
        {
            // exploration
            {".NET Reflector .+", new[] {Activity.Development}},
            {"Disassembly", new[] {Activity.Development}},
            {"Diagram", new[] {Activity.Development}},
            {"UML Model Explorer", new[] {Activity.Development}},
            {"XML Schema Explorer", new[] {Activity.Development}},
            // analysis and inspection
            {"Code Analysis", new[] {Activity.Development}},
            {"Analysis .+", new[] {Activity.Development}},
            {"Analyze .+", new[] {Activity.Development}},
            {"Call Hierarchy", new[] {Activity.Navigation}},
            {"Hierarchy", new[] {Activity.Navigation}},
            {"Inspection Results", new[] {Activity.Development}},
            // testing
            {"Code Coverage Results", new[] {Activity.Testing}},
            {"Tests Explorer", new[] {Activity.Testing}},
            {"Test Results", new[] {Activity.Testing}},
            {"Test Runs", new[] {Activity.Testing}},
            {"Unit Test Explorer", new[] {Activity.Testing}},
            {"Unit Test Sessions .+", new[] {Activity.Testing}},
            {"NCrunch Metrics", new[] {Activity.Testing}},
            {"NCrunch Tests", new[] {Activity.Testing}},
            // build
            {"Build Explorer", new[] {Activity.Development}},
            {"Builds", new[] {Activity.Development}},
            {"BuildVision", new[] {Activity.Development}},
            // version control
            {"Folder Difference", new[] {Activity.VersionControl}},
            {"History", new[] {Activity.VersionControl}},
            {"Pending Changes.+", new[] {Activity.VersionControl}},
            {"Resolve Conflicts", new[] {Activity.VersionControl}},
            {"Source Control Explorer", new[] {Activity.VersionControl}},
            {"Team Explorer.+", new[] {Activity.VersionControl}},
            {"Tracking Changeset.+", new[] {Activity.VersionControl}},
            // debugging
            {"Breakpoints", new[] {Activity.Debugging}},
            {"Call Stack", new[] {Activity.Debugging}},
            {"Locals", new[] {Activity.Debugging}},
            {"Watch.+", new[] {Activity.Debugging}},
            {"Parallel Watch.+", new[] {Activity.Debugging}},
            {"Threads", new[] {Activity.Debugging}},
            {"IntelliTrace", new[] {Activity.Debugging}},
            {"Modules", new[] {Activity.Debugging}},
            {"Immediate Window", new[] {Activity.Debugging}},
            {"Python .+ Interactive", new[] {Activity.Debugging}},
            {"Registers", new[] {Activity.Debugging}},
            // error tracking
            {"Error List", new[] {Activity.Development}},
            {"Errors in Solution", new[] {Activity.Development}},
            // find/replace
            {"Find and Replace", new[] {Activity.Development}},
            {"Find in Source Control", new[] {Activity.Navigation}},
            {"Find Results", new[] {Activity.Navigation}},
            {"Find Symbol Results", new[] {Activity.Navigation}},
            {"Class View", new[] {Activity.Navigation}},
            {"Resource View.+", new[] {Activity.Navigation}},
            // ???
            {"Command Window", new[] {Activity.Other}},
            {"Feedback Manager", new[] {Activity.Other}},
            {"Notifications", new[] {Activity.Other}},
            {"Recommendations", new[] {Activity.Other}},
            // IDE configuration
            {"Data Sources", new[] {Activity.LocalConfiguration}},
            {"Server Explorer", new[] {Activity.LocalConfiguration}},
            {"NCrunch Configuration", new[] {Activity.LocalConfiguration}},
            {"Python Environments", new[] {Activity.LocalConfiguration}},
            {"Templates Explorer", new[] {Activity.LocalConfiguration}},
            // time tracking
            {"Zeiterfassung", new[] {Activity.ProjectManagement}},
            // editing
            {"Properties", new[] {Activity.Development}},
            {"Refactoring.+", new[] {Activity.Development}},
            {"Regex Tester", new[] {Activity.Development}}
        };

        public WindowEventToActivityMapper(ILogger logger)
        {
            _logger = logger;
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        private void ProcessWindowEvent(WindowEvent @event)
        {
            if (IsMainWindowEvent(@event))
            {
                var activity = GetMainWindowActivity(@event.Action);
                InsertActivity(@event, activity);
            }
            else if (IsOpen(@event) || IsClose(@event))
            {
                InsertActivity(@event, Activity.LocalConfiguration);
            }
            else if (IsActivate(@event))
            {
                var window = @event.Window;
                switch (window.Type)
                {
                    case "vsWindowTypeBrowser":
                    case "vsWindowTypeDocumentOutline":
                        InsertActivities(@event, Activity.Navigation);
                        break;
                    case "vsWindowTypeOutput":
                        InsertActivities(@event, Activity.Debugging);
                        break;
                    case "vsWindowTypeToolbox":
                    case "vsWindowTypeProperties":
                        InsertActivity(@event, Activity.Development);
                        break;
                    case "vsWindowTypeSolutionExplorer":
                        InsertActivity(@event, Activity.Navigation);
                        break;
                    case "vsWindowTypeTaskList":
                        InsertActivities(@event, Activity.ProjectManagement);
                        break;
                    case "vsWindowTypeDocument":
                        if (IsProjectManagementWindow(window))
                        {
                            InsertActivity(@event, Activity.ProjectManagement);
                        }
                        else
                        {
                            InsertActivity(@event, Activity.Navigation);
                            _logger.Info("document window '{0}' treated with default case", window.Caption);
                        }
                        break;
                    case "vsWindowTypeToolWindow":
                        var activities = GetToolWindowActivities(window.Caption);
                        if (activities != null)
                        {
                            InsertActivities(@event, activities);
                        }
                        else
                        {
                            _logger.Info("tool window '{0}' treated with default case", window.Caption);
                        }
                        break;
                    default:
                        _logger.Error("unknown window type '{0}'", window.Type);
                        break;
                }
            }
            DropCurrentEvent();
        }

        private static bool IsMainWindowEvent(WindowEvent @event)
        {
            return "vsWindowTypeMainWindow".Equals(@event.Window.Type);
        }

        private static bool IsProjectManagementWindow(WindowName window)
        {
            return window.Caption.ContainsAny(WorkItemIndicators);
        }

        private static bool IsOpen(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Create;
        }

        private static bool IsActivate(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Activate;
        }

        private static bool IsClose(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Close;
        }

        private static Activity GetMainWindowActivity(WindowEvent.WindowAction action)
        {
            return action == WindowEvent.WindowAction.Activate ? Activity.EnterIDE : Activity.LeaveIDE;
        }

        private static Activity[] GetToolWindowActivities(string toolWindowCaption)
        {
            return
                ToolWindowMapping.Where(mapping => Regex.IsMatch(toolWindowCaption, mapping.Key))
                                 .Select(mapping => mapping.Value)
                                 .FirstOrDefault();
        }
    }
}