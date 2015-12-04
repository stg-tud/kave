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
    internal class ClickNavigationEventGeneratorTest : EventGeneratorTestBase
    {
        private ClickNavigationEventGenerator _uut;

        private Mock<ISignal<TextControlMouseEventArgs>> _mouseSignalMock;
        private Mock<INavigationUtils> _navigationUtilsMock;
        private Mock<ITextControl> _textControlMock;

        private IName _testLocation;

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
            _mouseSignalMock = new Mock<ISignal<TextControlMouseEventArgs>>();
            windowMock.Setup(window => window.MouseDown).Returns(_mouseSignalMock.Object);
            _textControlMock.Setup(tc => tc.Window).Returns(windowMock.Object);

            _testLocation =
                MethodName.Get("[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()");

            _navigationUtilsMock = new Mock<INavigationUtils>();
            _navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetLocation(It.IsAny<ITextControl>()))
                                .Returns(_testLocation);

            _uut = new ClickNavigationEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManagerMock.Object,
                _navigationUtilsMock.Object,
                TestLifetime);
        }

        [Test]
        public void ShouldAdviceOnClick()
        {
            _mouseSignalMock.Verify(signal => signal.Advise(TestLifetime, _uut.OnClick));
        }

        [Test]
        public void ShouldNotFireOnNonMouseEvents()
        {
            PressKey();
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnFirstNewLocation()
        {
            SetLocation(TypeName.Get("System.Int32, mscore, 4.0.0.0"));
            Click();
            Click();
            AssertNoEvent();
        }

        [Test]
        public void ShouldNotFireOnClickIfNoNewLocation()
        {
            SetLocation(TypeName.Get("System.Int32, mscore, 4.0.0.0"));
            Click();
            Click();
            Click();
            AssertNoEvent();
        }

        [Test]
        public void ShouldFireOnClickIfNewLocation()
        {
            SetLocation(TypeName.Get("System.Int32, mscore, 4.0.0.0"));
            Click();
            SetLocation(TypeName.Get("System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0"));
            Click();

            GetSinglePublished<NavigationEvent>();
        }

        [Test]
        public void ShouldSetLocationToOldLocation()
        {
            var oldLocation = TypeName.Get("System.Int32, mscore, 4.0.0.0");
            var newLocation = TypeName.Get("System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0");

            SetLocation(oldLocation);
            Click();
            SetLocation(newLocation);
            Click();

            Assert.AreEqual(oldLocation, GetSinglePublished<NavigationEvent>().Location);
        }

        [Test]
        public void ShouldSetTargetToNewLocation()
        {
            var oldLocation = TypeName.Get("System.Int32, mscore, 4.0.0.0");
            var newLocation = TypeName.Get("System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0");

            SetLocation(oldLocation);
            Click();
            SetLocation(newLocation);
            Click();

            Assert.AreEqual(newLocation, GetSinglePublished<NavigationEvent>().Target);
        }

        #region helpers

        private void PressKey()
        {
            _uut.OnClick(new TextControlMouseEventArgs(_textControlMock.Object, KeyStateMasks.MK_SHIFT, Point.Empty));
        }

        private void Click()
        {
            _uut.OnClick(new TextControlMouseEventArgs(_textControlMock.Object, KeyStateMasks.MK_LBUTTON, Point.Empty));
        }

        private void SetLocation(IName value)
        {
            _navigationUtilsMock.Setup(navigationUtils => navigationUtils.GetLocation(_textControlMock.Object))
                                .Returns(value);
        }

        #endregion
    }
}