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
using EnvDTE;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Utils.Assertion;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio
{
    internal class DTECommandEventGeneratorTest : VisualStudioEventGeneratorTestBase
    {
        private Mock<CommandEvents> _mockCommandEvents;
        private Mock<Commands> _mockCommands;

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
            SetUpCommands();
            // ReSharper disable once ObjectCreationAsStatement
            new DTECommandEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, TestThreading);
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

        [Test(
            Description =
                "BUGFIX: Superfluous events weren't removed from queue, which caused 'executing same event twice at a time' exception"
            )]
        public void ShouldNotFailIfSuperfluousEventIsFiredTwice()
        {
            var command = GetCommand("{5EFC7975-14BC-11CF-9B2B-00AA00573819}", 337, "Edit.GoToFindCombo");
            GivenCommandIsDefined(command);

            WhenCommandExecutes(command);
            WhenCommandExecutes(command);
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
         ExpectedException(typeof(AssertException),
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
            var command = GetCommand(
                "{E272D1BE-8216-4919-AFA3-EEB57FAB3537}",
                42,
                "ReSharper_KaVE_VsFeedbackGenerator_SessionManager");
            GivenCommandIsDefined(command);

            WhenCommandEnds(command);
        }

        private DateTime GivenNowIs(DateTime time)
        {
            var triggeredAt = time;
            TestDateUtils.Now = triggeredAt;
            return triggeredAt;
        }

        private static ICommandName GetCommand(string commandGuid, int commandId, string commandName)
        {
            return Names.Command(commandGuid + ":" + commandId + ":" + commandName);
        }

        private void GivenCommandIsDefined(ICommandName command)
        {
            var mockCommand = new Mock<Command>();
            mockCommand.Setup(cmd => cmd.Guid).Returns(command.Guid);
            mockCommand.Setup(cmd => cmd.ID).Returns(command.Id);
            mockCommand.Setup(cmd => cmd.Name).Returns(command.Name);
            mockCommand.Setup(cmd => cmd.Bindings).Returns(new string[0]);
            _mockCommands.Setup(cmds => cmds.Item(command.Guid, command.Id)).Returns(mockCommand.Object);
        }

        private void WhenCommandExecutes(ICommandName command)
        {
            WhenCommandStarts(command);
            WhenCommandEnds(command);
        }

        private void WhenCommandStarts(ICommandName command)
        {
            _mockCommandEvents.Raise(ce => ce.BeforeExecute += null, command.Guid, command.Id, null, null, true);
        }

        private void WhenCommandEnds(ICommandName command)
        {
            _mockCommandEvents.Raise(ce => ce.AfterExecute += null, command.Guid, command.Id, null, null);
        }
    }
}