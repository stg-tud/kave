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
 *    - Sven Amann
 */

using System;
using System.Collections;
using EnvDTE;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using Microsoft.VisualStudio.CommandBars;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio
{
    class CommandBarEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<WindowEvents> _mockWindowEvents;

        [SetUp]
        public void SetUp()
        {
            TestDateUtils.Now = DateTime.Now;
        }

        private void GivenEventGeneratorIsInitialized()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new CommandBarEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils);
        }

        protected override void MockEvents(Mock<Events> mockEvents)
        {
            _mockWindowEvents = new Mock<WindowEvents>();
            // ReSharper disable once UseIndexedProperty
            mockEvents.Setup(events => events.get_WindowEvents(It.IsAny<Window>())).Returns(_mockWindowEvents.Object);
        }

        private static Mock<CommandBarButton> MockButton(string commandId)
        {
            var mockButton = new Mock<CommandBarButton>();
            mockButton.Setup(b => b.Caption).Returns(commandId);
            mockButton.As<CommandBarControl>().Setup(b => b.Caption).Returns(commandId);
            return mockButton;
        }

        private static Mock<CommandBarComboBox> MockComboBox(string commandId)
        {
            var mockComboBox = new Mock<CommandBarComboBox>();
            mockComboBox.Setup(cb => cb.Caption).Returns(commandId);
            mockComboBox.As<CommandBarControl>().Setup(cb => cb.Caption).Returns(commandId);
            return mockComboBox;
        }

        private void GivenDTECommandBarsAre(params CommandBar[] mockCommandBar)
        {
            var mockCommandBars = MockCommandBars(mockCommandBar);
            TestIDESession.MockDTE.Setup(dte => dte.CommandBars).Returns(mockCommandBars);
        }

        private static Mock<CommandBarPopup> MockCommandBarPopup(string popupCaption, params CommandBarControl[] controls)
        {
            var mockCommandBarPopup = new Mock<CommandBarPopup>();
            mockCommandBarPopup.Setup(popup => popup.Caption).Returns(popupCaption);
            var subControls = MockCommandBarControls(controls);
            mockCommandBarPopup.Setup(popup => popup.Controls).Returns(subControls);
            return mockCommandBarPopup;
        }

        private static CommandBarControls MockCommandBarControls(params CommandBarControl[] controls)
        {
            var mockControls = new Mock<CommandBarControls>();
            mockControls.Setup(c => c.GetEnumerator()).Returns(controls.GetEnumerator);
            return mockControls.Object;
        }

        private static CommandBar MockCommandBar<TC>(Mock<TC> button) where TC : class, CommandBarControl
        {
            var controls = MockCommandBarControls(button.Object);
            var mockCommandBar = new Mock<CommandBar>();
            mockCommandBar.Setup(bar => bar.Controls).Returns(controls);
            return mockCommandBar.Object;
        }

        private static IEnumerable MockCommandBars(params CommandBar[] bars)
        {
            var mockCommandBars = new Mock<CommandBars>().As<_CommandBars>().As<IEnumerable>();
            mockCommandBars.Setup(cb => cb.GetEnumerator()).Returns(bars.GetEnumerator);
            return mockCommandBars.Object;
        }

        [Test]
        public void ShouldFireEventIfMainMenuBarButtonIsClicked()
        {
            var button = MockButton("testButton");
            var commandBar = MockCommandBar(button);
            GivenDTECommandBarsAre(commandBar);
            GivenEventGeneratorIsInitialized();

            WhenCommandBarButtonIsClicked(button);

            var actual = GetSinglePublished<CommandEvent>();
            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                KaVEVersion = TestRSEnv.DefaultVersion.ToString(),
                TriggeredAt = TestDateUtils.Now,
                TriggeredBy = IDEEvent.Trigger.Click,
                CommandId = "testButton"
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldFireEventIfMainMenuBarDropdownSelectionIsChanged()
        {
            var comboBox = MockComboBox("testComboBox");
            var commandBar = MockCommandBar(comboBox);
            GivenDTECommandBarsAre(commandBar);
            GivenEventGeneratorIsInitialized();

            WhenCommandBarComboBoxIsChanged(comboBox);

            var actual = GetSinglePublished<CommandEvent>();
            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                KaVEVersion = TestRSEnv.DefaultVersion.ToString(),
                TriggeredAt = TestDateUtils.Now,
                TriggeredBy = IDEEvent.Trigger.Click,
                CommandId = "testComboBox"
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldFireEventIfMenuButtonIsClicked()
        {
            var button = MockButton("menuButton");
            var popup = MockCommandBarPopup("menu", button.Object);
            var commandBar = MockCommandBar(popup);
            GivenDTECommandBarsAre(commandBar);
            GivenEventGeneratorIsInitialized();

            WhenCommandBarButtonIsClicked(button);

            var actual = GetSinglePublished<CommandEvent>();
            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                KaVEVersion = TestRSEnv.DefaultVersion.ToString(),
                TriggeredAt = TestDateUtils.Now,
                TriggeredBy = IDEEvent.Trigger.Click,
                CommandId = "menuButton"
            };
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldFireEventIfNestedMenuButtonIsClicked()
        {
            var button = MockButton("nestedMenuButton");
            var innerPopup = MockCommandBarPopup("nestedMenu", button.Object);
            var outerPopup = MockCommandBarPopup("menu", innerPopup.Object);
            var commandBar = MockCommandBar(outerPopup);
            GivenDTECommandBarsAre(commandBar);
            GivenEventGeneratorIsInitialized();

            WhenCommandBarButtonIsClicked(button);

            var actual = GetSinglePublished<CommandEvent>();
            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                KaVEVersion = TestRSEnv.DefaultVersion.ToString(),
                TriggeredAt = TestDateUtils.Now,
                TriggeredBy = IDEEvent.Trigger.Click,
                CommandId = "nestedMenuButton"
            };
            Assert.AreEqual(expected, actual);
        }

        [Test, Ignore]
        public void ShouldFireEventIfToolWindowButtonIsClicked()
        {
            // cannot mock ToolWindow as we need an instance of EnvDTE.WindowBase, which is non-public and we cannot mock the reflective access
        }


        private static void WhenCommandBarComboBoxIsChanged(Mock<CommandBarComboBox> comboBox)
        {
            comboBox.Raise(c => c.Change += null, comboBox.Object);
        }

        private static void WhenCommandBarButtonIsClicked(Mock<CommandBarButton> button)
        {
            button.Raise(b => b.Click += null, button.Object, false);
        }
    }
}
