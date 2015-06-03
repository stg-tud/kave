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
 *    - Uli Fahrer
 */

using System.Collections;
using System.Collections.Generic;
using EnvDTE;
using JetBrains.DataFlow;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.Generators;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    internal class IDEStateEventGeneratorTest : EventGeneratorTestBase
    {
        /// <summary>
        ///     Maybe both visible (open) and invisible (closed).
        /// </summary>
        private readonly ICollection<Window> _instantiatedWindows = new List<Window>();

        private IList<Document> _openDocuments;

        [SetUp]
        public void SetUpIDEWindows()
        {
            _instantiatedWindows.Clear();
            var mockWindows = new Mock<Windows>().As<IEnumerable>();
            mockWindows.Setup(w => w.GetEnumerator()).Returns(() => _instantiatedWindows.GetEnumerator());
            TestIDESession.MockDTE.Setup(dte => dte.Windows).Returns((Windows) mockWindows.Object);
        }

        [SetUp]
        public void SetUpDocuments()
        {
            var document = new Mock<Document>();
            document.Setup(d => d.FullName).Returns("TestDocument");
            document.Setup(d => d.DTE).Returns(TestIDESession.DTE);
            _openDocuments = new[] {document.Object};

            var mockDocuments = new Mock<Documents>().As<IEnumerable>();
            mockDocuments.Setup(d => d.GetEnumerator()).Returns(_openDocuments.GetEnumerator);
            TestIDESession.MockDTE.Setup(dte => dte.Documents).Returns((Documents) mockDocuments.Object);
        }

        private static Mock<Window> CreateWindowMock(string caption, bool visible)
        {
            var window = new Mock<Window>();
            window.Setup(w => w.Visible).Returns(visible);
            window.Setup(w => w.Caption).Returns(caption);
            window.Setup(w => w.Type).Returns(vsWindowType.vsWindowTypeWatch);
            return window;
        }

        [Test]
        public void ShouldCaptureOnlyVisibleWindows()
        {
            var mockVisibleWindow = CreateWindowMock("VisibleWindow", true);
            var mockInvisibleWindow = CreateWindowMock("InvisibleWindow", false);
            _instantiatedWindows.Add(mockVisibleWindow.Object);
            _instantiatedWindows.Add(mockInvisibleWindow.Object);
            var visibleWindowName = mockVisibleWindow.Object.GetName();

            // ReSharper disable once ObjectCreationAsStatement
            new IDEStateEventGenerator(TestRSEnv, TestMessageBus, EternalLifetime.Instance, TestDateUtils, null);

            var actuals = GetSinglePublished<IDEStateEvent>().OpenWindows;
            var expecteds = new[] {visibleWindowName};
            CollectionAssert.AreEqual(expecteds, actuals);
        }

        [Test]
        public void ShouldSetIDESessionUUIDToShutdownEvent()
        {
            IDEStateEvent shutdownEvent = null;
            var mockLogger = new Mock<IEventLogger>();
            mockLogger.Setup(logger => logger.Shutdown(It.IsAny<IDEStateEvent>()))
                      .Callback<IDEStateEvent>(ideEvent => shutdownEvent = ideEvent);

            Lifetimes.Using(
                lt =>
                    new IDEStateEventGenerator(TestRSEnv, TestMessageBus, lt, TestDateUtils, mockLogger.Object));

            Assert.AreEqual(TestIDESession.UUID, shutdownEvent.IDESessionUUID);
            Assert.AreEqual(IDEStateEvent.LifecyclePhase.Shutdown, shutdownEvent.IDELifecyclePhase);
            Assert.AreEqual(TestDateUtils.Now, shutdownEvent.TriggeredAt);
        }

        // TODO test other functionality of the generator
        // use Lifetimes.Using(lt => ...) to test behaviour on Lifetime's end
    }
}