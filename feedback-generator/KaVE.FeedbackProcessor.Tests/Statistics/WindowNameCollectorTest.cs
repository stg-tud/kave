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

using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Statistics;
using KaVE.FeedbackProcessor.Tests.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Statistics
{
    class WindowNameCollectorTest
    {
        private WindowNameCollector _uut;

        [SetUp]
        public void CreateCollector()
        {
            _uut = new WindowNameCollector();
        }

        [Test]
        public void CollectsNameFromWindowEvent()
        {
            var windowEvent = new WindowEvent {Window = Names.Window("window event")};

            Process(windowEvent);

            AssertNameCollected(windowEvent.Window);
        }

        [Test]
        public void CollectsActiveWindow()
        {
            var ideEvent = new TestIDEEvent {ActiveWindow = Names.Window("active window name")};

            Process(ideEvent);

            AssertNameCollected(ideEvent.ActiveWindow);
        }

        [Test]
        public void CollectsOpenWindowsFromIDEStateEvent()
        {
            var ideEvent = new IDEStateEvent {OpenWindows = {Names.Window("window 1"), Names.Window("window 2")}};

            Process(ideEvent);

            AssertNameCollected(ideEvent.OpenWindows.ToArray());
        }

        private void Process(IDEEvent windowEvent)
        {
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            _uut.OnEvent(windowEvent);
            _uut.OnStreamEnds();
        }

        private void AssertNameCollected(params IWindowName[] expected)
        {
            var actuals = _uut.AllWindowNames;
            CollectionAssert.IsSubsetOf(expected.Select(wn => wn.Identifier), actuals);
        }
    }
}