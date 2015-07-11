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

using EnvDTE;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio
{
    internal class SolutionEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<SolutionEvents> _mockSolutionEvents;
        private Mock<ProjectItemsEvents> _mockSolutionItemEvents;
        private Mock<ProjectItemsEvents> _mockMiscFilesEvents;
        private Mock<SelectionEvents> _mockSelectionEvents;

        protected override void MockEvents(Mock<Events> mockEvents)
        {
            _mockSolutionEvents = new Mock<SolutionEvents>();
            mockEvents.Setup(evts => evts.SolutionEvents).Returns(_mockSolutionEvents.Object);
            _mockSolutionItemEvents = new Mock<ProjectItemsEvents>();
            mockEvents.Setup(evts => evts.SolutionItemsEvents).Returns(_mockSolutionItemEvents.Object);
            _mockMiscFilesEvents = new Mock<ProjectItemsEvents>();
            mockEvents.Setup(evts => evts.MiscFilesEvents).Returns(_mockMiscFilesEvents.Object);
            _mockSelectionEvents = new Mock<SelectionEvents>();
            mockEvents.Setup(evts => evts.SelectionEvents).Returns(_mockSelectionEvents.Object);
        }

        [SetUp]
        public void SetUp()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new SolutionEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        [TestCase("{test}", "NuGet.Config"),
         TestCase("{test}", "NuGet.exe"),
         TestCase("{test}", "NuGet.targets"),
         TestCase("{test}", "packages.config")]
        public void ShouldNotFireArtificialProjectItemEvents(string itemKind, string itemName)
        {
            var mockProjectItem = new Mock<ProjectItem>();
            mockProjectItem.Setup(pi => pi.Kind).Returns(itemKind);
            mockProjectItem.Setup(pi => pi.Name).Returns(itemName);

            _mockSolutionItemEvents.Raise(e => e.ItemAdded += null, mockProjectItem.Object);

            AssertNoEvent();
        }

        [TestCase("{test}", "<MiscFiles>")]
        public void ShoudNotFireArtificialProjectEvents(string projectKind, string projectUniqueName)
        {
            var mockProject = new Mock<Project>();
            mockProject.Setup(p => p.Kind).Returns(projectKind);
            mockProject.Setup(p => p.UniqueName).Returns(projectUniqueName);

            _mockSolutionEvents.Raise(se => se.ProjectAdded += null, mockProject.Object);
            _mockSolutionEvents.Raise(se => se.ProjectRemoved += null, mockProject.Object);

            AssertNoEvent();
        }
    }
}