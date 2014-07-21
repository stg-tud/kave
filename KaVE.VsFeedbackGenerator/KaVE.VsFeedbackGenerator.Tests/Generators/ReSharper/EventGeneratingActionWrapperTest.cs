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
using KaVE.Model.Events;
using KaVE.TestUtils;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.ReSharper
{
    [TestFixture]
    internal class EventGeneratingActionWrapperTest : EventGeneratorTestBase
    {
        private Action _testAction;
        private EventGeneratingActionWrapper _uut;
        private bool _testActionInvoked;

        [SetUp]
        public void SetUp()
        {
            _testActionInvoked = false;
            _testAction = () => _testActionInvoked = true;
            _uut = new EventGeneratingActionWrapper(_testAction, TestIDESession, TestMessageBus, TestDateUtils);
        }

        [Test]
        public void ShouldFireEventOnInvocation()
        {
            _uut.Execute();

            var expected = new CommandEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                CommandId = "KaVE.VsFeedbackGenerator.Tests.Generators.ReSharper.EventGeneratingActionWrapperTest",
                TriggeredAt = TestDateUtils.Now,
                Duration = TimeSpan.FromSeconds(0)
            };
            var actual = GetSinglePublished<CommandEvent>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldExecuteAction()
        {
            _uut.Execute();

            Assert.IsTrue(_testActionInvoked);
        }

        [Test]
        public void ShouldExecuteActionIfEventGenerationFails()
        {
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(new Exception("TestException"));

            _uut.Execute();

            Assert.IsTrue(_testActionInvoked);
        }

        [Test]
        public void ShouldLogErrorIfEventGenerationFails()
        {
            var cause = new Exception("TestException");
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(cause);

            _uut.Execute();

            MockLogger.Verify(logger => logger.Error(ItIsException.With("generating command event failed", cause)));
        }
    }
}