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
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Intentions.Extensibility;
using JetBrains.TextControl;
using KaVE.Model.Events;
using KaVE.Model.Events.ReSharper;
using KaVE.TestUtils;
using KaVE.VsFeedbackGenerator.Generators.ReSharper;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.ReSharper
{
    [TestFixture]
    internal class EventGeneratingBulbActionProxyTest : EventGeneratorTestBase
    {
        private const string TestActionText = "TestActionText";
        private Mock<IBulbAction> _mockTestAction;
        private EventGeneratingBulbActionProxy _uut;
        private ISolution _testSolution;
        private ITextControl _testTextControl;

        [SetUp]
        public void SetUp()
        {
            _mockTestAction = new Mock<IBulbAction>();
            _mockTestAction.Setup(a => a.Text).Returns(TestActionText);
            _uut = new EventGeneratingBulbActionProxy(_mockTestAction.Object, TestIDESession, TestMessageBus);
            _testSolution = new Mock<ISolution>().Object;
            _testTextControl = new Mock<ITextControl>().Object;
        }

        [Test]
        public void ShouldFireEventOnInvocation()
        {
            _uut.Execute(_testSolution, _testTextControl);

            var expected = new BulbActionEvent
            {
                IDESessionUUID = TestIDESession.UUID,
                ActionId = "Castle.Proxies.IBulbActionProxy",
                ActionText = TestActionText,
                TriggeredAt = TestDateUtils.Now,
                Duration = TimeSpan.FromSeconds(0)
            };
            var actual = GetSinglePublished<BulbActionEvent>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldExecuteAction()
        {
            _uut.Execute(_testSolution, _testTextControl);

            _mockTestAction.Verify(a => a.Execute(_testSolution, _testTextControl));
        }

        [Test]
        public void ShouldExecuteActionIfEventGenerationFails()
        {
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(new Exception("TestException"));

            _uut.Execute(_testSolution, _testTextControl);

            _mockTestAction.Verify(a => a.Execute(_testSolution, _testTextControl));
        }

        [Test]
        public void ShouldLogErrorIfEventGenerationFails()
        {
            var cause = new Exception("TestException");
            MockTestMessageBus.Setup(mb => mb.Publish(It.IsAny<IDEEvent>())).Throws(cause);

            _uut.Execute(_testSolution, _testTextControl);

            MockLogger.Verify(logger => logger.Error(ItIsException.With("generating bulb-action event failed", cause)));
        }
    }
}