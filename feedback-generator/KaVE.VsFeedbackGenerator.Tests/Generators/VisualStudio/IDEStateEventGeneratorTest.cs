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
using EnvDTE;
using JetBrains.DataFlow;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Generators.VisualStudio;
using KaVE.VsFeedbackGenerator.Utils.Names;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Generators.VisualStudio
{
    [TestFixture]
    internal class IDEStateEventGeneratorTest : EventGeneratorTestBase
    {
        private WindowName _visibleWindowName;

        [SetUp]
        public void SetUp()
        {
            var mockVisibleWindow = CreateWindowMock("VisibleWindow", true);
            var mockInvisibleWindow = CreateWindowMock("InvisibleWindow", false);
            IEnumerable windowPieces = new[] {mockVisibleWindow.Object, mockInvisibleWindow.Object};

            _visibleWindowName = mockVisibleWindow.Object.GetName();

            var mockWindows = new Mock<Windows>().As<IEnumerable>();
            mockWindows.Setup(w => w.GetEnumerator()).Returns(windowPieces.GetEnumerator);

            TestIDESession.MockDTE.Setup(dte => dte.Windows).Returns((Windows) mockWindows.Object);

            var document = new Mock<Document>();
            document.Setup(d => d.FullName).Returns("TestDocument");
            document.Setup(d => d.DTE).Returns(TestIDESession.DTE);
            IEnumerable documentPieces = new[] {document.Object};

            var mockDocuments = new Mock<Documents>().As<IEnumerable>();
            mockDocuments.Setup(d => d.GetEnumerator()).Returns(documentPieces.GetEnumerator);

            TestIDESession.MockDTE.Setup(dte => dte.Documents).Returns((Documents) mockDocuments.Object);

            // ReSharper disable once ObjectCreationAsStatement
            new IDEStateEventGenerator(TestRSEnv, TestMessageBus, EternalLifetime.Instance, TestDateUtils, null);
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
            var actual = GetSinglePublished<IDEStateEvent>();

            var expected = new[] {_visibleWindowName};
            CollectionAssert.AreEqual(expected, actual.OpenWindows);
        }

        // TODO test other functionality of the generator
        // use Lifetimes.Using(lt => ...) to test behaviour on Lifetime's end
    }
}