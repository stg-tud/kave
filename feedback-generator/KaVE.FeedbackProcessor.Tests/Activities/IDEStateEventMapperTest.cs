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
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class IDEStateEventMapperTest : BaseToActivityMapperTest
    {
        public override BaseToActivityMapper Sut
        {
            get { return new IDEStateEventToActivityMapper(); }
        }

        [Test]
        public void ShouldMapStartupEventsToEnterIDE()
        {
            var @event = CreateArbitraryEvent();
            @event.IDELifecyclePhase = IDELifecyclePhase.Startup;
            AssertMapsToActivity(@event, Activity.EnterIDE);
        }

        [Test]
        public void ShouldDropRuntimeEvents()
        {
            var @event = CreateArbitraryEvent();
            @event.IDELifecyclePhase = IDELifecyclePhase.Runtime;
            AssertDrop(@event);
        }

        [Test]
        public void ShouldMapShutdownEventsToLeaveIDE()
        {
            var @event = CreateArbitraryEvent();
            @event.IDELifecyclePhase = IDELifecyclePhase.Shutdown;
            AssertMapsToActivity(@event, Activity.LeaveIDE);
        }

        private static IDEStateEvent CreateArbitraryEvent()
        {
            var @event = new IDEStateEvent
            {
                OpenDocuments = {Names.Document("some doc")},
                OpenWindows = {Names.Window("some window")}
            };
            return @event;
        }
    }
}