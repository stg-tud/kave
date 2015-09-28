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

using System.Windows.Forms;
using EnvDTE;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    internal class CtrlClickEventGeneratorTest : EventGeneratorTestBase
    {
        private CtrlClickEventGenerator _uut;
        private CtrlKeyStateProviderDummy _keyState;
        private Mock<Window> _testWindowMock;

        [SetUp]
        public void Setup()
        {
            _testWindowMock = new Mock<Window>();
            _testWindowMock.Setup(window => window.Top).Returns(0);
            _testWindowMock.Setup(window => window.Left).Returns(0);
            _testWindowMock.Setup(window => window.Width).Returns(0);
            _testWindowMock.Setup(window => window.Height).Returns(0);
            TestIDESession.MockDTE.Setup(dte => dte.ActiveWindow).Returns(_testWindowMock.Object);

            _keyState = new CtrlKeyStateProviderDummy();
            _uut = new CtrlClickEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, _keyState);
        }

        [TearDown]
        public void Teardown()
        {
            _uut.Dispose();
        }

        [Test]
        public void ShouldFireOnLeftClickIfCtrlIsPressed()
        {
            _keyState.CtrlIsPressed = true;

            _uut.OnClick(null, GenerateMouseEventArgs(MouseButtons.Left));

            GetSinglePublished<InfoEvent>();
        }

        [Test]
        public void ShouldNotFireOnLeftClickIfCtrlIsNotPressed()
        {
            _keyState.CtrlIsPressed = false;

            _uut.OnClick(null, GenerateMouseEventArgs(MouseButtons.Left));

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnAnyOtherClick()
        {
            _keyState.CtrlIsPressed = true;

            _uut.OnClick(null, GenerateMouseEventArgs(MouseButtons.Right));

            AssertNoEvent();
        }

        private static MouseEventArgs GenerateMouseEventArgs(MouseButtons clickedButton)
        {
            return new MouseEventArgs(clickedButton, 0, 0, 0, 0);
        }

        private class CtrlKeyStateProviderDummy : ICtrlKeyStateProvider
        {
            public bool CtrlIsPressed { get; set; }
        }
    }
}