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
using KaVE.Commons.Model.Naming.IDEComponents;
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
            {"Code Coverage Results", new[] {Activity.Development}},
            {"Tests Explorer", new[] {Activity.Development}},
            {"Test Results", new[] {Activity.Development}},
            {"Test Runs", new[] {Activity.Development}},
            {"Unit Test Explorer", new[] {Activity.Development}},
            {"Unit Test Sessions .+", new[] {Activity.Development}},
            {"NCrunch Metrics", new[] {Activity.Development}},
            {"NCrunch Tests", new[] {Activity.Development}},
            // build
            {"Build Explorer", new[] {Activity.Development}},
            {"Builds", new[] {Activity.Development}},
            {"BuildVision", new[] {Activity.Development}},
            // version control
            {"Folder Difference", new[] {Activity.ProjectManagement}},
            {"History", new[] {Activity.ProjectManagement}},
            {"Pending Changes.+", new[] {Activity.ProjectManagement}},
            {"Resolve Conflicts", new[] {Activity.ProjectManagement}},
            {"Source Control Explorer", new[] {Activity.ProjectManagement}},
            {"Team Explorer.+", new[] {Activity.ProjectManagement}},
            {"Tracking Changeset.+", new[] {Activity.ProjectManagement}},
            // debugging
            {"Breakpoints", new[] {Activity.Development}},
            {"Call Stack", new[] {Activity.Development}},
            {"Locals", new[] {Activity.Development}},
            {"Watch.+", new[] {Activity.Development}},
            {"Parallel Watch.+", new[] {Activity.Development}},
            {"Threads", new[] {Activity.Development}},
            {"IntelliTrace", new[] {Activity.Development}},
            {"Modules", new[] {Activity.Development}},
            {"Immediate Window", new[] {Activity.Development}},
            {"Python .+ Interactive", new[] {Activity.Development}},
            {"Registers", new[] {Activity.Development}},
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
            // ???
            {"Source Not Available", new[] {Activity.Other}},
            {"Source Not Found", new[] {Activity.Other}},
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
            else if (IsOpen(@event) || IsMove(@event) || IsClose(@event))
            {
                InsertActivity(@event, Activity.LocalConfiguration);
            }
            else
            {
                var window = @event.Window;
                switch (window.Type)
                {
                    case "vsWindowTypeBrowser":
                    case "vsWindowTypeDocumentOutline":
                        InsertActivities(@event, Activity.Navigation);
                        break;
                    case "vsWindowTypeOutput":
                        InsertActivities(@event, Activity.Development);
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

        private static bool IsProjectManagementWindow(IWindowName window)
        {
            return window.Caption.ContainsAny(WorkItemIndicators);
        }

        private static bool IsOpen(WindowEvent @event)
        {
            return @event.Action == WindowAction.Create;
        }

        private static bool IsMove(WindowEvent @event)
        {
            return @event.Action == WindowAction.Move;
        }

        private static bool IsClose(WindowEvent @event)
        {
            return @event.Action == WindowAction.Close;
        }

        private static Activity GetMainWindowActivity(WindowAction action)
        {
            return action == WindowAction.Activate ? Activity.EnterIDE : Activity.LeaveIDE;
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