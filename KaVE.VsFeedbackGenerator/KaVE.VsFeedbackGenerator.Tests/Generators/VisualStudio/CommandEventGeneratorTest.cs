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
using KaVE.Model.Events;
using KaVE.Model.Names.VisualStudio;
using KaVE.Utils.Assertion;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using Microsoft.VisualStudio.CommandBars;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    internal class CommandEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<CommandEvents> _mockCommandEvents;
        private Mock<Commands> _mockCommands;
        private Mock<CommandBarButton> _mockCommandBarButton;
        private Mock<CommandBarComboBox> _mockCommandBarComboBox;

        protected override void MockEvents(Mock<Events> mockEvents)
        {
            _mockCommandEvents = new Mock<CommandEvents>();
            // ReSharper disable once UseIndexedProperty
            mockEvents.Setup(events => events.get_CommandEvents("{00000000-0000-0000-0000-000000000000}", 0))
                      .Returns(_mockCommandEvents.Object);
        }

        [SetUp]
        public void SetUp()
        {
            SetUpCommandBars();
            SetUpCommands();
            // ReSharper disable once ObjectCreationAsStatement
            new CommandEventGenerator(TestIDESession, TestMessageBus, TestDateUtils);
        }

        private void SetUpCommandBars()
        {
            _mockCommandBarButton = new Mock<CommandBarButton>();
            _mockCommandBarComboBox = new Mock<CommandBarComboBox>();
            var mockCommandBarPopup = new Mock<CommandBarPopup>();
            var subControls = MockControls(_mockCommandBarComboBox.Object);
            mockCommandBarPopup.Setup(popup => popup.Controls).Returns(subControls);

            var mockCommandBar = new Mock<CommandBar>();
            var mainControls = MockControls(_mockCommandBarButton.Object, mockCommandBarPopup.Object);
            mockCommandBar.Setup(bar => bar.Controls).Returns(mainControls);
            IEnumerable bars = new[] {mockCommandBar.Object};

            var mockCommandBars = new Mock<CommandBars>().As<_CommandBars>().As<IEnumerable>();
            mockCommandBars.Setup(cb => cb.GetEnumerator()).Returns(bars.GetEnumerator);
            TestIDESession.MockDTE.Setup(dte => dte.CommandBars).Returns(mockCommandBars.Object);
        }

        private static CommandBarControls MockControls(params CommandBarControl[] controls)
        {
            var mockControls = new Mock<CommandBarControls>();
            mockControls.Setup(c => c.GetEnumerator()).Returns(controls.GetEnumerator);
            return mockControls.Object;
        }

        private void SetUpCommands()
        {
            _mockCommands = new Mock<Commands>();
            TestIDESession.MockDTE.Setup(dte => dte.Commands).Returns(_mockCommands.Object);
        }

        [Test]
        public void ShouldNotFireEventAfterCommandStarts()
        {
            var command = GetCommand("{some-guid}", 42, "some_command");
            GivenCommandIsDefined(command);

            WhenCommandStarts(command);

            AssertNoEvent();
        }

        [Test]
        public void ShouldFireEventIfCommandExecutes()
        {
            var command = GetCommand("{guid}", 23, "arbitrary-command-name");
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            Assert.NotNull(GetSinglePublished<CommandEvent>());
        }

        [Test]
        public void ShouldFireEventForCommandIfCommandExecutes()
        {
            var command = GetCommand("{guid}", 23, "arbitrary-command-name");
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(command.Identifier, actual.CommandId);
        }

        [Test]
        public void ShouldTrackStartTimeOfCommandExecution()
        {
            var command = GetCommand("{g-u-i-d}", 5, "command");
            GivenCommandIsDefined(command);

            var triggeredAt = GivenNowIs(new DateTime(2014, 5, 27, 11, 48, 31));
            WhenCommandExecutes(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(triggeredAt, actual.TriggeredAt);
        }

        [Test]
        public void ShouldTrackDurationOfCommandExecution()
        {
            var command = GetCommand("{slow-command}", 69, "SlowCommand");
            GivenCommandIsDefined(command);

            var triggeredAt = GivenNowIs(new DateTime(2014, 5, 27, 11, 48, 31));
            WhenCommandStarts(command);
            GivenNowIs(triggeredAt.AddSeconds(12));
            WhenCommandEnds(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(TimeSpan.FromSeconds(12), actual.Duration);
        }

        [Test]
        public void ShouldTrackTriggerAsUnknownByDefault()
        {
            var command = GetCommand("{g-u-i-d}", 5, "command");
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Unknown, actual.TriggeredBy);
        }

        [TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 1096, "View.ObjectBrowsingScope"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 1627, null),
         TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 337, "Edit.GoToFindCombo"),
         TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 684, "Build.SolutionConfigurations"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 1990, "Build.SolutionPlatforms"),
         TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 1657, null),
         TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 1717, null),
         TestCase("{CB26E292-901A-419C-B79D-49BD45C43929}", 120, null),
         TestCase("{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}", 4820, null),
         TestCase("{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}", 6155, null),
         TestCase("{FFE1131C-8EA1-4D05-9728-34AD4611BDA9}", 4800, null)]
        public void ShouldNotFireEventsForCommandsThatAreAutomaticallyTriggeredByVs(string commandGuid,
            int commandId,
            string commandName)
        {
            var command = GetCommand(commandGuid, commandId, commandName);
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireEventsForCommandsThatCorrespondToReSharperActions()
        {
            var command = GetCommand("{some-guid}", 123, "Prefix.ReSharper_ActionId");
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            AssertNoEvent();
        }

        [TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 02, "Edit.DeleteBackwards"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 03, "Edit.BreakLine"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 04, "Edit.InsertTab"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 07, "Edit.CharLeft"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 08, "Edit.CharLeftExtend"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 09, "Edit.CharRight"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 10, "Edit.CharRightExtend"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 11, "Edit.LineUp"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 12, "Edit.LineUpExtend"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 13, "Edit.LineDown"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 14, "Edit.LineDownExtend"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 27, "Edit.PageUp"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 29, "Edit.PageDown"),
         TestCase("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 107, "Edit.CompleteWord"),
         TestCase("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 627, "Window.CloseAllDocuments")]
        public void ShouldNotFireEventForCommandDuplicatedByAReSharperAction(string guid, int id, string name)
        {
            var command = GetCommand(guid, id, name);
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);

            AssertNoEvent();
        }

        [Test,
         ExpectedException(typeof (AssertException),
             ExpectedMessage = "command finished that didn't start: {some-guid}:456:command-name")]
        public void ShouldFailIfCommandEndsWithoutHavingStarted()
        {
            var command = GetCommand("{some-guid}", 456, "command-name");
            GivenCommandIsDefined(command);

            WhenCommandEnds(command);
        }

        [Test]
        public void ShouldFireEventsForCodeCompletionCommandDespiteThatItEndsWithoutHavingStartet()
        {
            var completionCommand = GetCommand("{1496A755-94DE-11D0-8C3F-00C04FC2AAE2}", 107, "BasicCompletion");
            GivenCommandIsDefined(completionCommand);

            WhenCommandEnds(completionCommand);

            Assert.IsNotNull(GetSinglePublished<CommandEvent>());
        }

        [Test]
        public void ShouldNotFailIfReSharperCorrespondentCommandEndsWithoutStarting()
        {
            var command = GetCommand("{E272D1BE-8216-4919-AFA3-EEB57FAB3537}", 42, "ReSharper_KaVE_VsFeedbackGenerator_SessionManager");
            GivenCommandIsDefined(command);

            WhenCommandEnds(command);
        }

        [Test]
        public void ShouldSetTriggerToClickedIfCommandBarButtonIsClickedBeforeCommandExecutes()
        {
            var command = GetCommand("{test}", 1, "test");
            GivenCommandIsDefined(command);

            WhenCommandBarButtonIsClicked();
            WhenCommandExecutes(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Click, actual.TriggeredBy);
        }

        [Test]
        public void ShouldSetTriggerToClickedIfCommandBarComboBoxIsChangedBeforeCommandExecutes()
        {
            var command = GetCommand("{test}", 42, "foo");
            GivenCommandIsDefined(command);

            WhenCommandBarComboBoxIsChanged();
            WhenCommandExecutes(command);

            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Click, actual.TriggeredBy);
        }

        private void WhenCommandBarComboBoxIsChanged()
        {
            _mockCommandBarComboBox.Raise(comboBox => comboBox.Change += null, _mockCommandBarComboBox.Object);
        }

        private void WhenCommandBarButtonIsClicked()
        {
            _mockCommandBarButton.Raise(button => button.Click += null, _mockCommandBarButton.Object, false);
        }

        private DateTime GivenNowIs(DateTime time)
        {
            var triggeredAt = time;
            TestDateUtils.Now = triggeredAt;
            return triggeredAt;
        }

        private static CommandName GetCommand(string commandGuid, int commandId, string commandName)
        {
            return CommandName.Get(commandGuid + ":" + commandId + ":" + commandName);
        }

        private void GivenCommandIsDefined(CommandName command)
        {
            var mockCommand = new Mock<Command>();
            mockCommand.Setup(cmd => cmd.Guid).Returns(command.Guid);
            mockCommand.Setup(cmd => cmd.ID).Returns(command.Id);
            mockCommand.Setup(cmd => cmd.Name).Returns(command.Name);
            mockCommand.Setup(cmd => cmd.Bindings).Returns(new string[0]);
            _mockCommands.Setup(cmds => cmds.Item(command.Guid, command.Id)).Returns(mockCommand.Object);
        }

        private void WhenCommandExecutes(CommandName command)
        {
            WhenCommandStarts(command);
            WhenCommandEnds(command);
        }

        private void WhenCommandStarts(CommandName command)
        {
            _mockCommandEvents.Raise(ce => ce.BeforeExecute += null, command.Guid, command.Id, null, null, true);
        }

        private void WhenCommandEnds(CommandName command)
        {
            _mockCommandEvents.Raise(ce => ce.AfterExecute += null, command.Guid, command.Id, null, null);
        }
    }
}