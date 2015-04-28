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
 *    - Sebastian Proksch
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Cleanup;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal abstract class BaseEventProcessorTest
    {
        public abstract IIDEEventProcessor Sut { get; }

        protected void AssertDrop(IDEEvent @event)
        {
            FillWithBasicInformation(@event);

            var events = Sut.Process(@event);

            CollectionAssert.AreEquivalent(new ActivityEvent[] {}, events);
        }

        protected void AssertMapsToActivity(IDEEvent @event, Activity activity, ActivityPhase phase)
        {
            FillWithBasicInformation(@event);

            var events = Sut.Process(@event);

            var expectedEvent = new ActivityEvent
            {
                Activity = activity,
                Phase = phase,
                IDESessionUUID = @event.IDESessionUUID,
                KaVEVersion = @event.KaVEVersion,
                TriggeredAt = @event.TriggeredAt,
                TriggeredBy = @event.TriggeredBy,
                Duration = @event.Duration,
                ActiveWindow = @event.ActiveWindow
            };
            CollectionAssert.AreEquivalent(new[] {expectedEvent}, events);
        }

        private static void FillWithBasicInformation(IDEEvent @event)
        {
            @event.IDESessionUUID = "sessionId";
            @event.KaVEVersion = "1.0";
            @event.TriggeredAt = new DateTime(2015, 04, 24, 15, 28, 30);
            @event.TriggeredBy = IDEEvent.Trigger.Unknown;
            @event.Duration = TimeSpan.FromMilliseconds(10);
            @event.ActiveWindow = WindowName.Get("vsWindowTypeToolWindow Startseite");
        }
    }
}