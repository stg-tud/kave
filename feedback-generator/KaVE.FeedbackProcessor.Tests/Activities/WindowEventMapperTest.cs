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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.TestUtils.Utils.Exceptions;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class WindowEventMapperTest : BaseToActivityMapperTest
    {
        private readonly TestLogger _logger = new TestLogger(true);

        public override BaseToActivityMapper Sut
        {
            get { return new WindowEventToActivityMapper(_logger); }
        }

        [Test]
        public void ShouldMapCreateToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("Type Caption"),
                Action = WindowAction.Create
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapMoveToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("Type Caption"),
                Action = WindowAction.Move
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapCloseToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("Type Caption"),
                Action = WindowAction.Close
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapActivateOutlineToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeDocumentOutline Document Outline"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateBrowserToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeBrowser Object Browser"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateDocumentWindowToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeDocument MyClass.cs"),
                Action = WindowAction.Activate
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
        public void ShouldMapActivateWorkItemWindowToProjectManagement(string workItemType)
        {
            var @event = new WindowEvent
            {
                Window = Names.Window(string.Format("vsWindowTypeDocument {0} 90210", workItemType)),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.ProjectManagement);
        }

        [TestCaseSource("_workItemTypes")]
        public void ShouldMapActivateNewWorkItemWindowToProjectManagement(string workItemType)
        {
            var @event = new WindowEvent
            {
                Window = Names.Window(string.Format("vsWindowTypeDocument New {0} An Item", workItemType)),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.ProjectManagement);
        }

        [TestCase("Editor"),
         TestCase("Results")]
        public void ShouldMapActivateMarkedDocumentWindowToProjectManagement(string marker)
        {
            var @event = new WindowEvent
            {
                Window = Names.Window(string.Format("vsWindowTypeDocument blabla [{0}]", marker)),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.ProjectManagement);
        }

        [Test]
        public void ShouldMapActivateWiqDocumentWindowToProjectManagement()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeDocument something.wiq"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.ProjectManagement);
        }

        [Test]
        public void ShouldLogNonTreatedDocumentWindowEvents()
        {
            string infoMessage = null;
            _logger.InfoLogged += info => infoMessage = info;

            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeDocument bla"),
                Action = WindowAction.Activate
            };
            Sut.Map(@event);

            Assert.AreEqual("document window 'bla' treated with default case", infoMessage);
        }

        [Test]
        public void ShouldMapActivatePropertiesToDevelopment()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeProperties Properties"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Development);
        }

        [Test]
        public void ShouldMapActivateOutputWindowToDevelopment()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeOutput Output"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.Development);
        }

        [Test]
        public void ShouldMapActivateSolutionExplorerToNavigation()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeSolutionExplorer Solution Explorer"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapActivateTaskListToProjectManagement()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeTaskList Task List"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivities(@event, Activity.ProjectManagement);
        }

        [Test]
        public void ShouldMapActivateToolboxToDevelopment()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeToolbox Toolbox"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Development);
        }

        [Test]
        public void ShouldMapActivateToolWindowToActivities_FullNameMatch()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeToolWindow Unit Test Explorer"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Development);
        }

        [Test]
        public void ShouldMapActivateToolWindowToActivities_PartialNameMatch()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeToolWindow Analyze <Some Target>"),
                Action = WindowAction.Activate
            };
            AssertMapsToActivity(@event, Activity.Development);
        }

        [Test]
        public void ShouldMapActivateToolWindowToActivities_Fallback()
        {
            string infoMessage = null;
            _logger.InfoLogged += info => infoMessage = info;

            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeToolWindow Unknown Window"),
                Action = WindowAction.Activate
            };
            Sut.Map(@event);

            Assert.AreEqual("tool window 'Unknown Window' treated with default case", infoMessage);
        }

        [Test]
        public void MapsMainWindowActivationToEnterIDEActivity()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeMainWindow Startseite - Microsoft Visual Studio"),
                Action = WindowAction.Activate
            };

            AssertMapsToActivity(@event, Activity.EnterIDE);
        }

        [Test]
        public void MapsMainWindowDeactivationToLeaveIDEActivity()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("vsWindowTypeMainWindow Aktueller Fenstertitel"),
                Action = WindowAction.Deactivate
            };

            AssertMapsToActivity(@event, Activity.LeaveIDE);
        }

        [Test, ExpectedException(ExpectedMessage = "unknown window type 'unknownWindowType'")]
        public void ShouldLogUnhandledWindowType()
        {
            var @event = new WindowEvent
            {
                Window = Names.Window("unknownWindowType Foo"),
                Action = WindowAction.Activate
            };
            Sut.Map(@event);
        }
    }
}