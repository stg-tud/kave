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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.VS.FeedbackGenerator.Generators.VisualStudio;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.Generators.VisualStudio
{
    internal class VsFocusEventGeneratorTest : EventGeneratorTestBase
    {
        private VsFocusEventGenerator _sut;
        private bool _isActive;

        [SetUp]
        public void SetUp()
        {
            _isActive = true;
            var focusHelper = Mock.Of<IFocusHelper>();
            Mock.Get(focusHelper).Setup(fh => fh.IsCurrentApplicationActive()).Returns(() => _isActive);

            _sut = new VsFocusEventGenerator(TestRSEnv, TestMessageBus, TestDateUtils, focusHelper, TestThreading);
        }

        [Test]
        public void TriggersOncePerSecond()
        {
            Assert.AreEqual(1000, VsFocusEventGenerator.TimerIntervalSize);
        }

        [Test]
        public void Startup()
        {
            WaitForTimer();
            var publishedEvent = GetSinglePublished<WindowEvent>();
            Assert.AreEqual(WindowEvent.WindowAction.Activate, publishedEvent.Action);
            Assert.AreEqual(Names.Window(VsFocusEventGenerator.MainWindowName), publishedEvent.Window);
        }

        [Test]
        public void StartupIsOnlyFiredOnce()
        {
            WaitForTimer();
            WaitForTimer();
            AssertNumEvent(1);
        }

        [Test]
        public void EventIsFiredAgainOnChange()
        {
            WaitForTimer();
            _isActive = false;
            WaitForTimer();
            AssertNumEvent(2);
            var publishedEvent = GetLastPublished<WindowEvent>();
            Assert.AreEqual(WindowEvent.WindowAction.Deactivate, publishedEvent.Action);
            Assert.AreEqual(Names.Window(VsFocusEventGenerator.MainWindowName), publishedEvent.Window);
        }

        private void WaitForTimer()
        {
            _sut.OnTimerElapsed(null, null);
        }
    }
}