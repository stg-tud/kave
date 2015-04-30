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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities.Model;

namespace KaVE.FeedbackProcessor.Activities
{
    internal class WindowEventActivityProcessor : BaseActivityProcessor
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

        public WindowEventActivityProcessor(ILogger logger)
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
                        if (window.Caption.ContainsAny(WorkItemIndicators))
                        {
                            InsertActivity(@event, Activity.ProjektManagement);
                        }
                        InsertActivity(@event, Activity.Navigation);
                        break;
                    default:
                        _logger.Error("unknown window type '{0}'", window.Type);
                        break;
                }
            }
            DropCurrentEvent();
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
    }
}