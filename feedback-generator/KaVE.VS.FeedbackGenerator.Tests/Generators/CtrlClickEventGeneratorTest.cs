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
using JetBrains.ReSharper.Psi.Cpp.Tree;
using JetBrains.TextControl;
using KaVE.Commons.Model.Events;
using KaVE.VS.FeedbackGenerator.Generators;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators
{
    internal class CtrlClickEventGeneratorTest : EventGeneratorTestBase
    {
        private CtrlClickEventGenerator _uut;

        private Mock<ISignal<TextControlMouseEventArgs>> _mouseUpSignalMock;
        private Mock<ITextControl> _textControlMock;

        private Lifetime _testLifetime
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
                                          EternalLifetime.Instance,
                                          "this can't be empty")
                                      {
                                          _textControlMock.Object
                                      });

            var windowMock = new Mock<ITextControlWindow>();
            _mouseUpSignalMock = new Mock<ISignal<TextControlMouseEventArgs>>();
            windowMock.Setup(window => window.MouseUp).Returns(_mouseUpSignalMock.Object);
            _textControlMock.Setup(tc => tc.Window).Returns(windowMock.Object);

            var treeNodeProviderMock = new Mock<ITreeNodeProvider>();
            treeNodeProviderMock.Setup(treeNodeProvider => treeNodeProvider.GetTreeNode(It.IsAny<ITextControl>()))
                                .Returns(new EmptyStatement());

            _uut = new CtrlClickEventGenerator(
                TestRSEnv,
                TestMessageBus,
                TestDateUtils,
                textControlManagerMock.Object,
                treeNodeProviderMock.Object,
                _testLifetime);
        }

        [Test]
        public void ShouldAdviceOnClick()
        {
            _mouseUpSignalMock.Verify(signal => signal.Advise(_testLifetime, _uut.OnClick));
        }

        [Test]
        public void ShouldFireOnLeftClickIfCtrlIsPressed()
        {
            TriggerLeftClick(true);
            GetSinglePublished<InfoEvent>();
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