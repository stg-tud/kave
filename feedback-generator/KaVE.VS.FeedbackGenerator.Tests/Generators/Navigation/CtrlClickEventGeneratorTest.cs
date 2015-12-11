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

using System.Drawing;
using JetBrains.DataFlow;
using JetBrains.Interop.WinApi;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.VS.FeedbackGenerator.Generators.Navigation;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Navigation
{
    internal class CtrlClickEventGeneratorTest : EventGeneratorTestBase
    {
        private CtrlClickEventGenerator _uut;

        private Mock<ISignal<TextControlMouseEventArgs>> _mouseUpSignalMock;
        private Mock<ITextControl> _textControlMock;

        private IName _testLocation;
        private IName _testTarget;

        private static Lifetime TestLifetime
        {
            get { return EternalLifetime.Instance; }
        }

        [SetUp]
        public void Setup()
        {
            _textControlMock = new Mock<ITextControl>();
            var textControlManagerMock = new Mock<ITextControlManager>();
            textControlManagerMock.Setup(tcManager => tcManager.TextControls)
                                  .Returns(
                                      new CollectionEvents<ITextControl>(
                                          TestLifetime,
                                          "this can't be empty")
                                      {
                                          _textControlMock.Object
                                      });

            var windowMock = new Mock<ITextControlWindow>();
            _mouseUpSignalMock = new Mock<ISignal<TextControlMouseEventArgs>>();
            windowMock.Setup(window => window.MouseUp).Returns(_mouseUpSignalMock.Object);
            _textControlMock.Setup(tc => tc.Window).Returns(windowMock.Object);

            _testTarget = TypeName.Get("System.Int32, mscore, 4.0.0.0");
            _testLocation =
                MethodName.Get("[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()");

            var navigationUtilsMock = new Mock<INavigationUtils>();
            navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetTarget(It.IsAny<ITextControl>()))
                                .Returns(() => _testTarget);
            navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetLocation(It.IsAny<ITextControl>()))
                                .Returns(() => _testLocation);

            _uut = new CtrlClickEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManagerMock.Object,
                navigationUtilsMock.Object,
                TestLifetime);
        }

        [Test]
        public void ShouldAdviceOnClick()
        {
            _mouseUpSignalMock.Verify(signal => signal.Advise(TestLifetime, _uut.OnClick));
        }

        [Test]
        public void ShouldFireOnLeftClickIfCtrlIsPressed()
        {
            TriggerLeftClick(true);
            GetSinglePublished<NavigationEvent>();
        }

        [Test]
        public void ShouldUseClickTrigger()
        {
            TriggerLeftClick(true);
            Assert.AreEqual(IDEEvent.Trigger.Click, GetSinglePublished<NavigationEvent>().TriggeredBy);
        }

        [Test]
        public void ShouldNotFireOnLeftClickIfCtrlIsNotPressed()
        {
            TriggerLeftClick(false);
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnAnyOtherClick()
        {
            TriggerRightClick();
            AssertNoEvent();
        }

        [Test]
        public void ShouldSetTarget()
        {
            TriggerLeftClick(true);
            Assert.AreEqual(_testTarget, GetSinglePublished<NavigationEvent>().Target);
        }

        [Test]
        public void ShouldSetLocation()
        {
            TriggerLeftClick(true);
            Assert.AreEqual(_testLocation, GetSinglePublished<NavigationEvent>().Location);
        }

        private void TriggerLeftClick(bool withCtrl)
        {
            var keyStateMasks = withCtrl ? KeyStateMasks.MK_CONTROL : KeyStateMasks.MK_LBUTTON;
            _uut.OnClick(
                new TextControlMouseEventArgs(
                    _textControlMock.Object,
                    keyStateMasks,
                    new Point()));
        }

        private void TriggerRightClick()
        {
            _uut.OnClick(
                new TextControlMouseEventArgs(
                    _textControlMock.Object,
                    KeyStateMasks.MK_RBUTTON | KeyStateMasks.MK_CONTROL,
                    new Point()));
        }
    }
}