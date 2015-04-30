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
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.Commons.TestUtils.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Cleanup;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class WindowEventProcessorTest : BaseEventProcessorTest
    {
        private readonly TestLogger _logger = new TestLogger(true);

        public override IIDEEventProcessor Sut
        {
            get { return new WindowEventActivityProcessor(_logger); }
        }

        [Test]
        public void ShouldMapCreateToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Create
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapMoveToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Move
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapCloseToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Close
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapActivateOutlineToUnderstandingAndNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeDocumentOutline Document Outline"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.Understanding, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateBrowserToUnderStandingNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeBrowser Object Browser"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.Understanding, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateDocumentWindowToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeDocument MyClass.cs"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Navigation);
        }

        private readonly string[] _workItemTypes =
        {
            "Backlog Item",
            "Bug",
            "Business Requirement",
            "Business Requirements",
            "Initiative",
            "Query",
            "Requirement",
            "Task",
            "User Story"
        };

        [TestCaseSource("_workItemTypes")]
        public void ShouldMapActivateWorkItemWindowToProjectManagementAndNavigation(string workItemType)
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(string.Format("vsWindowTypeDocument {0} 90210", workItemType)),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjektManagement, Activity.Navigation);
        }

        [TestCaseSource("_workItemTypes")]
        public void ShouldMapActivateNewWorkItemWindowToProjectManagementAndNavigation(string workItemType)
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(string.Format("vsWindowTypeDocument New {0} An Item", workItemType)),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjektManagement, Activity.Navigation);
        }

        [TestCase("Editor"),
         TestCase("Results")]
        public void ShouldMapActivateMarkedDocumentWindowToProjectManagementAndNavigation(string marker)
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(string.Format("vsWindowTypeDocument blabla [{0}]", marker)),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjektManagement, Activity.Navigation);
        }

        public void ShouldMapActivateWiqDocumentWindowToProjectManagementAndNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeDocument something.wiq"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjektManagement, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivatePropertiesToEditing()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeProperties Properties"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Editing);
        }

        [Test(Description = "Handled by InIDEActivityDetector.")]
        public void ShouldDropActivateMainWindow()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeMainWindow Microsoft Visual Studio"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertDrop(@event);
        }

        [Test]
        public void ShouldMapActivateOutputWindowToUnderstandingAndDebugging()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeOutput Output"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.Understanding, Activity.Debugging);
        }

        [Test]
        public void ShouldMapActivateSolutionExplorerToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeSolutionExplorer Solution Explorer"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateTaskListToProjectManagementAndNavigation()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeTaskList Task List"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjektManagement, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateToolboxToEditing()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeToolbox Toolbox"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Editing);
        }

        [Test]
        public void ShouldMapActivateToolWindowToXYZ()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeToolWindow (Test,"),
                Action = WindowEvent.WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test(Description = "Only main window gets deactivated. Handled by InIDEActivityDetector.")]
        public void ShouldDropWindowDeactivations()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Deactivate
            };
            AssertDrop(@event);
        }

        [Test, ExpectedException(ExpectedMessage = "unknown window type 'unknownWindowType'")]
        public void ShouldLogUnhandledWindowType()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("unknownWindowType Foo"),
                Action = WindowEvent.WindowAction.Activate
            };
            Sut.Process(@event);
        }
    }
}