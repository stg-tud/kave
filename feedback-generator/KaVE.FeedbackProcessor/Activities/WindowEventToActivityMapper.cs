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
            {".NET Reflector .+", new[] {Activity.Understanding}},
            {"Disassembly", new[] {Activity.Understanding}},
            {"Diagram", new[] {Activity.Understanding}},
            {"UML Model Explorer", new[] {Activity.Understanding}},
            {"XML Schema Explorer", new[] {Activity.Understanding}},
            // analysis and inspection
            {"Code Analysis", new[] {Activity.Understanding}},
            {"Analysis .+", new[] {Activity.Understanding}},
            {"Analyze .+", new[] {Activity.Understanding}},
            {"Call Hierarchy", new[] {Activity.Understanding, Activity.Navigation}},
            {"Hierarchy", new[] {Activity.Understanding, Activity.Navigation}},
            {"Inspection Results", new[] {Activity.Understanding, Activity.Navigation}},
            // testing
            {"Code Coverage Results", new[] {Activity.Testing, Activity.Understanding}},
            {"Tests Explorer", new[] {Activity.Testing, Activity.Navigation}},
            {"Test Results", new[] {Activity.Testing, Activity.Understanding, Activity.Navigation}},
            {"Test Runs", new[] {Activity.Testing, Activity.Understanding}},
            {"Unit Test Explorer", new[] {Activity.Testing, Activity.Navigation}},
            {"Unit Test Sessions .+", new[] {Activity.Testing, Activity.Navigation, Activity.Understanding}},
            {"NCrunch Metrics", new[] {Activity.Testing}},
            {"NCrunch Tests", new[] {Activity.Testing}},
            // build
            {"Build Explorer", new Activity[] {}},
            {"Builds", new Activity[] {}},
            {"BuildVision", new Activity[] {}},
            // version control
            {"Folder Difference", new Activity[] {}},
            {"History", new Activity[] {}},
            {"Pending Changes.+", new Activity[] {}},
            {"Resolve Conflicts", new Activity[] {}},
            {"Source Control Explorer", new Activity[] {}},
            {"Team Explorer (more fine grained?)", new Activity[] {}},
            {"Tracking Changeset.+", new Activity[] {}},
            // debugging
            {"Breakpoints", new[] {Activity.Debugging}},
            {"Call Stack", new[] {Activity.Debugging}},
            {"Locals", new[] {Activity.Debugging}},
            {"Watch.+", new[] {Activity.Debugging}},
            {"Parallel Watch.+", new[] {Activity.Debugging}},
            {"Threads", new[] {Activity.Debugging}},
            {"IntelliTrace", new[] {Activity.Debugging}},
            {"Modules", new[] {Activity.Debugging}},
            {"Immediate Window", new[] {Activity.Debugging, Activity.Testing /*?*/}},
            {"Python .+ Interactive", new[] {Activity.Debugging}},
            {"Registers", new[] {Activity.Debugging}},
            // error tracking
            {"Error List", new[] {Activity.Debugging, Activity.Navigation}},
            {"Errors in Solution", new[] {Activity.Debugging, Activity.Navigation}},
            // find/replace
            {"Find and Replace", new[] {Activity.Navigation, Activity.Editing}},
            {"Find in Source Control", new[] {Activity.Navigation}},
            {"Find Results", new[] {Activity.Navigation}},
            {"Find Symbol Results", new[] {Activity.Navigation}},
            {"Class View", new[] {Activity.Navigation}},
            {"Resource View.+", new[] {Activity.Navigation}},
            // ???
            {"Command Window", new Activity[] {}},
            {"Feedback Manager", new Activity[] {}},
            {"Notifications", new Activity[] {}},
            {"Recommendations", new Activity[] {}},
            // IDE configuration
            {"Data Sources", new[] {Activity.LocalConfiguration}},
            {"Server Explorer", new[] {Activity.LocalConfiguration}},
            {"NCrunch Configuration", new[] {Activity.LocalConfiguration}},
            {"Python Environments", new[] {Activity.LocalConfiguration}},
            {"Templates Explorer", new[] {Activity.LocalConfiguration}},
            // time tracking
            {"Zeiterfassung", new[] {Activity.ProjektManagement}},
            // ???
            {"Source Not Available", new Activity[] {}},
            {"Source Not Found", new Activity[] {}},
            // editing
            {"Properties", new[] {Activity.Editing}},
            {"Refactoring.+", new[] {Activity.Editing}},
            {"Regex Tester", new[] {Activity.Editing}}
        };

        public WindowEventToActivityMapper(ILogger logger)
        {
            _logger = logger;
            RegisterFor<WindowEvent>(ProcessWindowEvent);
        }

        private void ProcessWindowEvent(WindowEvent @event)
        {
            if (IsOpen(@event) || IsMove(@event) || IsClose(@event))
            {
                InsertActivity(@event, Activity.LocalConfiguration);
            }
            else if (IsActivate(@event))
            {
                var window = @event.Window;
                switch (window.Type)
                {
                    case "vsWindowTypeMainWindow":
                        // ignore, since handled by InIDEActivityDetector
                        break;
                    case "vsWindowTypeBrowser":
                    case "vsWindowTypeDocumentOutline":
                        InsertActivities(@event, Activity.Understanding, Activity.Navigation);
                        break;
                    case "vsWindowTypeOutput":
                        InsertActivities(@event, Activity.Understanding, Activity.Debugging);
                        break;
                    case "vsWindowTypeToolbox":
                    case "vsWindowTypeProperties":
                        InsertActivity(@event, Activity.Editing);
                        break;
                    case "vsWindowTypeSolutionExplorer":
                        InsertActivity(@event, Activity.Navigation);
                        break;
                    case "vsWindowTypeTaskList":
                        InsertActivities(@event, Activity.ProjektManagement, Activity.Navigation);
                        break;
                    case "vsWindowTypeDocument":
                        if (IsProjectManagementWindow(window))
                        {
                            InsertActivity(@event, Activity.ProjektManagement);
                        }
                        else
                        {
                            _logger.Info("document window '{0}' treated with default case", window.Caption);
                        }
                        InsertActivity(@event, Activity.Navigation);
                        break;
                    case "vsWindowTypeToolWindow":
                        var activities = GetActivitiesForToolWindow(window.Caption);
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

        private static bool IsProjectManagementWindow(WindowName window)
        {
            return window.Caption.ContainsAny(WorkItemIndicators);
        }

        private bool IsActivate(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Activate;
        }

        private bool IsOpen(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Create;
        }

        private bool IsMove(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Move;
        }

        private bool IsClose(WindowEvent @event)
        {
            return @event.Action == WindowEvent.WindowAction.Close;
        }

        private static Activity[] GetActivitiesForToolWindow(string toolWindowCaption)
        {
            return
                ToolWindowMapping.Where(mapping => Regex.IsMatch(toolWindowCaption, mapping.Key))
                                 .Select(mapping => mapping.Value)
                                 .FirstOrDefault();
        }
    }
}