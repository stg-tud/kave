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

using JetBrains.DataFlow;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names;
using KaVE.Commons.Model.Names.CSharp;
using KaVE.VS.FeedbackGenerator.Generators.Navigation;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.Navigation
{
    internal class KeyboardNavigationEventGeneratorTest : EventGeneratorTestBase
    {
        private KeyboardNavigationEventGenerator _uut;

        private Mock<ISignal<EventArgs<ITextControl>>> _keyboardSignalMock;
        private Mock<INavigationUtils> _navigationUtilsMock;
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
            _keyboardSignalMock = new Mock<ISignal<EventArgs<ITextControl>>>();
            windowMock.Setup(window => window.Keyboard).Returns(_keyboardSignalMock.Object);

            _textControlMock.Setup(tc => tc.Window).Returns(windowMock.Object);

            _testTarget = TypeName.Get("System.Int32, mscore, 4.0.0.0");
            _testLocation =
                MethodName.Get("[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()");

            _navigationUtilsMock = new Mock<INavigationUtils>();
            _navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetTarget(It.IsAny<ITextControl>()))
                                .Returns(() => _testTarget);
            _navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetLocation(It.IsAny<ITextControl>()))
                                .Returns(() => _testLocation);

            _uut = new KeyboardNavigationEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManagerMock.Object,
                _navigationUtilsMock.Object,
                TestLifetime);
        }

        [Test]
        public void ShouldAdviceOnKeyPress()
        {
            _keyboardSignalMock.Verify(keyboard => keyboard.Advise(TestLifetime, _uut.OnKeyPress));
        }

        [Test]
        public void ShouldNotFireOnFirstLocation()
        {
            PressKey();
            AssertNoEvent();
        }

        [Test]
        public void ShouldFireEventOnLocationChange()
        {
            PressKey();
            SetLocation(
                MethodName.Get("static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()"));
            PressKey();
            GetSinglePublished<NavigationEvent>();
        }

        [Test]
        public void ShouldSetLocationToOldLocation()
        {
            PressKey();
            SetLocation(
                MethodName.Get("static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()"));
            PressKey();

            var actualEvent = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(_testLocation, actualEvent.Location);
        }

        [Test]
        public void ShouldSetTargetToNewLocation()
        {
            var newLocation =
                MethodName.Get("static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()");
            PressKey();
            SetLocation(newLocation);
            PressKey();

            var actualEvent = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(newLocation, actualEvent.Target);
        }

        [Test]
        public void ShouldSetTriggerAndTypeToKeyboard()
        {
            PressKey();
            SetLocation(
                MethodName.Get("static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()"));
            PressKey();

            var actualEvent = GetSinglePublished<NavigationEvent>();
            Assert.AreEqual(IDEEvent.Trigger.Typing, actualEvent.TriggeredBy);
            Assert.AreEqual(NavigationEvent.NavigationType.Keyboard, actualEvent.TypeOfNavigation);
        }

        #region helpers

        private void PressKey()
        {
            _uut.OnKeyPress(new EventArgs<ITextControl>(_textControlMock.Object));
        }

        private void SetLocation(IName value)
        {
            _navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetLocation(_textControlMock.Object))
                                .Returns(value);
        }

        #endregion
    }
}