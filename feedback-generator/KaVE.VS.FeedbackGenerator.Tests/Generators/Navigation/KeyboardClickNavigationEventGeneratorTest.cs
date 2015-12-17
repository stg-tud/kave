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
using System.Windows.Input;
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
    internal class KeyboardClickNavigationEventGeneratorTest : EventGeneratorTestBase
    {
        private KeyboardClickNavigationEventGenerator _uut;

        private ISignal<EventArgs<ITextControl>> _keyboardSignal;
        private ISignal<TextControlMouseEventArgs> _mouseSignal;
        private INavigationUtils _navigationUtils;
        private ITextControl _textControl;

        private bool _navigationKeyPressed;
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

            // window
            var window = Mock.Of<ITextControlWindow>();
            Mock.Get(window).Setup(w => w.Keyboard).Returns(_keyboardSignal);
            Mock.Get(window).Setup(w => w.MouseDown).Returns(_mouseSignal);

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
            Mock.Get(_navigationUtils).Setup(navigationUtils => navigationUtils.GetTarget(It.IsAny<ITextControl>()))
                .Returns(Name.UnknownName);
            SetLocation(_method1);

            var keyUtil = Mock.Of<KeyboardClickNavigationEventGenerator.IKeyUtil>();
            Mock.Get(keyUtil).Setup(util => util.IsPressed(It.IsAny<Key>())).Returns(() => _navigationKeyPressed);

            _navigationKeyPressed = true;

            _uut = new KeyboardClickNavigationEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManager,
                _navigationUtils,
                TestLifetime,
                keyUtil);
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
        public void ShouldNotFireIfNoNavigationKeyIsPressed()
        {
            _navigationKeyPressed = false;

            PressKey();
            SetLocation(_method2);
            PressKey();

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnFirstLocation_Keyboard()
        {
            PressKey();
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnFirstLocation_Mouse()
        {
            Click();
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireIfNoChange_Keyboard()
        {
            PressKey();
            SetLocation(_method1);
            PressKey();
            SetLocation(_method1);
            PressKey();
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireIfNoChange_Mouse()
        {
            Click();
            SetLocation(_method1);
            Click();
            SetLocation(_method1);
            Click();
            AssertNoEvent();
        }

        [Test]
        public void ShouldFireEventOnLocationChange_Keyboard()
        {
            PressKey();
            SetLocation(_method2);
            PressKey();

            var actual = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(_method2, actual.Location);
            Assert.AreEqual(Name.UnknownName, actual.Target);
            Assert.AreEqual(IDEEvent.Trigger.Typing, actual.TriggeredBy);
            Assert.AreEqual(NavigationType.Keyboard, actual.TypeOfNavigation);
        }

        [Test]
        public void ShouldFireEventOnLocationChange_Mouse()
        {
            Click();
            SetLocation(_method2);
            Click();

            var actual = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(_method2, actual.Location);
            Assert.AreEqual(Name.UnknownName, actual.Target);
            Assert.AreEqual(IDEEvent.Trigger.Click, actual.TriggeredBy);
            Assert.AreEqual(NavigationType.Click, actual.TypeOfNavigation);
        }

        #region helpers

        private void Click()
        {
            _uut.OnClick(new TextControlMouseEventArgs(_textControl, KeyStateMasks.MK_LBUTTON, Point.Empty));
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

        #endregion
    }
}