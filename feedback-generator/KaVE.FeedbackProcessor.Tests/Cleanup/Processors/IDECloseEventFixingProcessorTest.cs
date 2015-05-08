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
 *    - Sven Amann
 */

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    [TestFixture]
    internal class IDECloseEventFixingProcessorTest
    {
        private static readonly IDEStateEvent StartupEvent = new IDEStateEvent
        {
            IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Startup
        };

        private static readonly IDEStateEvent ShutdownEvent = new IDEStateEvent
        {
            IDELifecyclePhase = IDEStateEvent.LifecyclePhase.Shutdown
        };

        private IDECloseEventFixingProcessor _uut;

        [SetUp]
        public void CreateProcessor()
        {
            _uut = new IDECloseEventFixingProcessor();
        }

        [Test]
        public void InsertsNothingBeforeFirstStartupEvent()
        {
            var actuals = _uut.Process(StartupEvent);

            CollectionAssert.AreEquivalent(new[] {StartupEvent}, actuals);
        }

        [Test]
        public void InsertsNothingIfStartupIsPreceededByShutdown()
        {
            _uut.Process(StartupEvent);
            _uut.Process(ShutdownEvent);
            var actuals = _uut.Process(StartupEvent);

            CollectionAssert.AreEquivalent(new[] {StartupEvent}, actuals);
        }

        [Test]
        public void InsertsShutdownBeforeStartupPreceededByEventOtherThanShutdown()
        {
            _uut.Process(StartupEvent);
            _uut.Process(IDEEventTestFactory.SomeEvent());
            var actuals = _uut.Process(StartupEvent);

            CollectionAssert.AreEquivalent(new [] {StartupEvent, ShutdownEvent}, actuals);
        }
    }
}