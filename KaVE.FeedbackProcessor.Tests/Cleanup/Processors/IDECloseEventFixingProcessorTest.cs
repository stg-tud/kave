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

using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Cleanup.Processors;
using KaVE.FeedbackProcessor.Model;
using KaVE.FeedbackProcessor.Tests.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Cleanup.Processors
{
    internal class IDECloseEventFixingProcessorTest
    {
        private IDECloseEventFixingProcessor _uut;

        [SetUp]
        public void CreateProcessor()
        {
            _uut = new IDECloseEventFixingProcessor();
        }

        [Test]
        public void InsertsNothingBeforeFirstStartupEvent()
        {
            var startupEvent = CreateStartupEvent();

            AssertUnmodified(Stream(startupEvent));
        }

        [Test]
        public void InsertsNothingIfStartupIsPreceededByShutdown()
        {
            var startupEvent = CreateStartupEvent();
            var shutdownEvent = CreateShutdownEventAfter(startupEvent);

            AssertUnmodified(Stream(startupEvent, shutdownEvent));
        }

        [Test]
        public void InsertsStartupIfFirstEventIsSomethingElse()
        {
            var someEvent = TestEventFactory.SomeEvent();
            someEvent.TriggeredAt = new DateTime(2015, 1, 23, 14, 7, 12);

            var missingStartupEvent = CreateStartupEvent();
            missingStartupEvent.TriggeredAt = someEvent.GetTriggeredAt().AddMilliseconds(-1);

            AssertTransformation(Stream(someEvent), Stream(missingStartupEvent, someEvent));
        }

        [Test]
        public void InsertsShutdownBeforeStartupPreceededByEventOtherThanShutdown()
        {
            var startupEvent = CreateStartupEvent();
            var intermediateEvent = CreateSomeEventAfter(startupEvent);
            var secondStartupEvent = CreateStartupEventAfter(intermediateEvent);

            var missingShutdownEvent = CreateShutdownEventAfter(intermediateEvent, 1 /* ms */);

            AssertTransformation(
                Stream(startupEvent, intermediateEvent, secondStartupEvent),
                Stream(startupEvent, intermediateEvent, missingShutdownEvent, secondStartupEvent));
        }

        [Test]
        public void ResetsForNewDeveloper()
        {
            var startupEvent = CreateStartupEvent();
            var intermediateEvent = CreateSomeEventAfter(startupEvent);

            AssertUnmodified(Stream(startupEvent, intermediateEvent));
            AssertUnmodified(Stream(startupEvent, intermediateEvent));
        }

        private static TestIDEEvent CreateSomeEventAfter(IDEEvent startupEvent)
        {
            var subsequentEvent = TestEventFactory.SomeEvent();
            SetTriggeredAtToAfter(subsequentEvent, startupEvent);
            return subsequentEvent;
        }

        private static void SetTriggeredAtToAfter(IDEEvent subsequentEvent,
            IDEEvent previousEvent,
            int milliseconds = 60000)
        {
            subsequentEvent.TriggeredAt = previousEvent.GetTriggeredAt().AddMilliseconds(milliseconds);
        }

        private static IDEStateEvent CreateStartupEventAfter(IDEEvent previousEvent)
        {
            var startupEvent = CreateStartupEvent();
            SetTriggeredAtToAfter(startupEvent, previousEvent);
            return startupEvent;
        }

        private static IDEStateEvent CreateStartupEvent()
        {
            return new IDEStateEvent
            {
                IDELifecyclePhase = IDELifecyclePhase.Startup,
                TriggeredAt = new DateTime(2015, 4, 7, 13, 20, 51)
            };
        }

        private static IDEStateEvent CreateShutdownEventAfter(IDEEvent previousEvent, int milliseconds = 60000)
        {
            var shutdownEvent = new IDEStateEvent
            {
                IDELifecyclePhase = IDELifecyclePhase.Shutdown
            };
            SetTriggeredAtToAfter(shutdownEvent, previousEvent, milliseconds);
            return shutdownEvent;
        }

        private static IEnumerable<IDEEvent> Stream(params IDEEvent[] events)
        {
            return events;
        }

        private void AssertUnmodified(IEnumerable<IDEEvent> original)
        {
            var events = original.ToList();
            AssertTransformation(events, events);
        }

        private void AssertTransformation(IEnumerable<IDEEvent> original, IEnumerable<IDEEvent> expected)
        {
            _uut.OnStreamStarts(TestFactory.SomeDeveloper());
            var actual = original.SelectMany(_uut.Map).OrderBy(evt => evt.TriggeredAt);
            _uut.OnStreamEnds();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}