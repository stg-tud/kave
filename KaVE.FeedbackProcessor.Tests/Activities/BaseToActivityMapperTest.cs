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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Collections;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;
using ActivityEvent = KaVE.FeedbackProcessor.Activities.Model.ActivityEvent;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal abstract class BaseToActivityMapperTest
    {
        public abstract BaseToActivityMapper Sut { get; }

        protected void AssertDrop(IDEEvent @event)
        {
            FillWithBasicInformation(@event);

            var events = Sut.Map(@event);

            CollectionAssert.AreEquivalent(new ActivityEvent[] {}, events);
        }

        protected void AssertMapsToActivity(IDEEvent @event, Activity activity)
        {
            FillWithBasicInformation(@event);

            var events = Sut.Map(@event);

            var expectedEvent = DeriveActivityDocument(@event, activity);
            CollectionAssert.AreEquivalent(new[] {expectedEvent}, events);
        }

        protected void AssertMapsToActivities(IDEEvent @event, params Activity[] activities)
        {
            FillWithBasicInformation(@event);

            var actuals = Sut.Map(@event);

            var expecteds = Lists.NewList<ActivityEvent>();
            foreach (var activity in activities)
            {
                expecteds.Add(DeriveActivityDocument(@event, activity));
            }
            CollectionAssert.AreEquivalent(expecteds, actuals);
        }

        private static ActivityEvent DeriveActivityDocument(IDEEvent @event, Activity activity)
        {
            return new ActivityEvent
            {
                Activity = activity,
                IDESessionUUID = @event.IDESessionUUID,
                KaVEVersion = @event.KaVEVersion,
                TriggeredAt = @event.TriggeredAt,
                TriggeredBy = @event.TriggeredBy,
                Duration = @event.Duration,
                ActiveWindow = @event.ActiveWindow,
                ActiveDocument = @event.ActiveDocument
            };
        }

        private static void FillWithBasicInformation(IDEEvent @event)
        {
            @event.IDESessionUUID = "sessionId";
            @event.KaVEVersion = "1.0";
            @event.TriggeredAt = new DateTime(2015, 04, 24, 15, 28, 30);
            @event.TriggeredBy = EventTrigger.Unknown;
            @event.Duration = TimeSpan.FromMilliseconds(10);
            @event.ActiveWindow = Names.Window("vsWindowTypeToolWindow Startseite");
        }
    }
}