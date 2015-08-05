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
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.UI.ActionsRevised;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils;
using KaVE.VS.FeedbackGenerator.Generators.ReSharper;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.ReSharper
{
    // TODO RS9
    internal class EventGeneratingActionHandlerTest : EventGeneratorTestBase
    {
        private const string TestActionId = "TestActionId";
        private Mock<IExecutableAction> _mockTestAction;
        private EventGeneratingActionHandler _uut;
        private IDataContext _testDataContext;

        [SetUp]
        public void SetUp()
        {
            _mockTestAction = new Mock<IExecutableAction>();
            _uut = new EventGeneratingActionHandler(TestActionId, TestRSEnv, TestMessageBus, TestDateUtils);
            _testDataContext = new Mock<IDataContext>().Object;
        }

        [Test]
        public void ShouldForceUpdate()
        {
            var invoked = false;
            var value = _uut.Update(
                _testDataContext,
                new ActionPresentation(),
                () => invoked = true);

            Assert.IsFalse(invoked);
            Assert.IsTrue(value);
        }

        [Test]
        public void ShouldFireEventOnInvocation()
        {
            _uut.Execute(_testDataContext, () => { });

            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                KaVEVersion = TestRSEnv.DefaultVersion,
                CommandId = TestActionId,
                TriggeredAt = TestDateUtils.Now,
                Duration = TimeSpan.FromSeconds(0)
            };
            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldExecuteAction()
        {
            var invoked = false;
            _uut.Execute(_testDataContext, () => invoked = true);

            Assert.IsTrue(invoked);
        }

        [Test]
        public void ShouldExecuteActionIfEventGenerationFails()
        {
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(new Exception("TestException"));

            var invoked = false;
            _uut.Execute(_testDataContext, () => invoked = true);

            Assert.IsTrue(invoked);
        }

        [Test]
        public void ShouldLogErrorIfEventGenerationFails()
        {
            var exception = new Exception("TestException");
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(exception);

            _uut.Execute(_testDataContext, () => { });

            MockLogger.Verify(l => l.Error(ItIsException.With("generating command event failed", exception)));
        }
    }
}