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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.VS.FeedbackGenerator.Generators.Navigation;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Navigation
{
    internal class NavigationEventGeneratorTest : EventGeneratorTestBase
    {
        private NavigationEventGenerator _uut;

        private ISignal<EventArgs<ITextControl>> _keyboardSignal;
        private ISignal<TextControlMouseEventArgs> _mouseSignal;
        private INavigationUtils _navigationUtils;
        private ITextControl _textControl;

        private IName _testTarget;

        private readonly IMethodName _method1 = MethodName.Get("[TR,P] [TD,P].M()");
        private readonly IMethodName _method2 = MethodName.Get("[TR,P] [TD,P].M2()");

        private static Lifetime TestLifetime
        {
            get { return EternalLifetime.Instance; }
        }

        [SetUp]
        public void Setup()
        {
            // mouse + keyboard
            _keyboardSignal = Mock.Of<ISignal<EventArgs<ITextControl>>>();
            _mouseSignal = Mock.Of<ISignal<TextControlMouseEventArgs>>();

            // ctrl click
            _testTarget = TypeName.Get("System.Int32, mscore, 4.0.0.0");

            // window
            var window = Mock.Of<ITextControlWindow>();
            Mock.Get(window).Setup(w => w.Keyboard).Returns(_keyboardSignal);
            Mock.Get(window).Setup(w => w.MouseUp).Returns(_mouseSignal);

            // textcontrol
            _textControl = Mock.Of<ITextControl>();
            var textControlManager = Mock.Of<ITextControlManager>();
            Mock.Get(textControlManager).Setup(tcManager => tcManager.TextControls)
                .Returns(
                    new CollectionEvents<ITextControl>(
                        TestLifetime,
                        "this can't be empty")
                    {
                        _textControl
                    });
            Mock.Get(_textControl).Setup(tc => tc.Window).Returns(window);

            // navigation utils
            _navigationUtils = Mock.Of<INavigationUtils>();
            SetTarget(_testTarget);
            SetLocation(_method1);

            _uut = new NavigationEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManager,
                _navigationUtils,
                TestLifetime);
        }

        [Test]
        public void ShouldAdviceOnKeyPress()
        {
            Mock.Get(_keyboardSignal).Verify(keyboard => keyboard.Advise(TestLifetime, _uut.OnKeyPress));
        }

        [Test]
        public void ShouldAdviceOnClick()
        {
            Mock.Get(_mouseSignal).Verify(signal => signal.Advise(TestLifetime, _uut.OnClick));
        }

        [Test]
        public void ShouldFireOnNewLocation_Keyboard()
        {
            PressKey();

            var actual = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(Name.UnknownName, actual.Target);
            Assert.AreEqual(_method1, actual.Location);
            Assert.AreEqual(IDEEvent.Trigger.Typing, actual.TriggeredBy);
            Assert.AreEqual(NavigationType.Keyboard, actual.TypeOfNavigation);
        }

        [Test]
        public void ShouldFireOnNewLocation_Mouse()
        {
            Click();

            var actual = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(Name.UnknownName, actual.Target);
            Assert.AreEqual(_method1, actual.Location);
            Assert.AreEqual(IDEEvent.Trigger.Click, actual.TriggeredBy);
            Assert.AreEqual(NavigationType.Click, actual.TypeOfNavigation);
        }

        [Test]
        public void ShouldNotFireIfNoChange_Keyboard()
        {
            PressKey();
            DropAllEvents();

            SetLocation(_method1);
            PressKey();

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireIfNoChange_Mouse()
        {
            Click();
            DropAllEvents();

            SetLocation(_method1);
            Click();

            AssertNoEvent();
        }

        [Test]
        public void ShouldFireEventOnCtrlClick()
        {
            CtrlClick();

            var actualEvent = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(NavigationType.CtrlClick, actualEvent.TypeOfNavigation);
            Assert.AreEqual(IDEEvent.Trigger.Click, actualEvent.TriggeredBy);
            Assert.AreEqual(_testTarget, actualEvent.Target);
            Assert.AreEqual(_method1, actualEvent.Location);
        }

        [Test]
        public void ShouldNotFireKeyboardEventAfterCtrlClick()
        {
            SetTarget(_method2);
            CtrlClick();
            DropAllEvents();

            SetLocation(_method2);
            PressKey();

            AssertNoEvent();
        }

        #region helpers

        private void Click()
        {
            _uut.OnClick(new TextControlMouseEventArgs(_textControl, KeyStateMasks.MK_LBUTTON, Point.Empty));
        }

        private void CtrlClick()
        {
            _uut.OnClick(new TextControlMouseEventArgs(_textControl, KeyStateMasks.MK_CONTROL, Point.Empty));
        }

        private void PressKey()
        {
            _uut.OnKeyPress(new EventArgs<ITextControl>(_textControl));
        }

        private void SetLocation(IName value)
        {
            Mock.Get(_navigationUtils)
                .Setup(navigationUtils => navigationUtils.GetLocation(_textControl))
                .Returns(value);
        }

        private void SetTarget(IName value)
        {
            Mock.Get(_navigationUtils)
                .Setup(navigationUtils => navigationUtils.GetTarget(_textControl))
                .Returns(value);
        }

        #endregion
    }
}