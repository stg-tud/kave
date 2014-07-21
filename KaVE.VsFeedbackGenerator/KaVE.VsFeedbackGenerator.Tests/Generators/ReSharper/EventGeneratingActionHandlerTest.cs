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
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.ReSharper
{
    [TestFixture]
    internal class EventGeneratingActionHandlerTest : EventGeneratorTestBase
    {
        private const string TestActionId = "TestActionId";
        private Mock<IUpdatableAction> _mockTestAction;
        private EventGeneratingActionHandler _uut;
        private IDataContext _testDataContext;

        [SetUp]
        public void SetUp()
        {
            _mockTestAction = new Mock<IUpdatableAction>();
            _mockTestAction.Setup(a => a.Id).Returns(TestActionId);
            _uut = new EventGeneratingActionHandler(_mockTestAction.Object, TestIDESession, TestMessageBus, TestDateUtils);
            _testDataContext = new Mock<IDataContext>().Object;
        }

        [Test]
        public void ShouldInvokeUpdate()
        {
            var invoked = false;
            _uut.Update(
                _testDataContext,
                new ActionPresentation(),
                () => invoked = true);

            Assert.IsTrue(invoked);
        }

        [Test]
        public void ShouldFireEventOnInvocation()
        {
            _uut.Execute(_testDataContext, () => { });

            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
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