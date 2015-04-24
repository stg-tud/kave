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

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Cleanup;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    [TestFixture]
    internal class InIDEActivityDetectorTest
    {
        [Test]
        public void MapsMainWindowActivationToInIDEActivityStart()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeMainWindow Startseite - Microsoft Visual Studio"),
                Action = WindowEvent.WindowAction.Activate
            };

            var uut = new InIDEActivityDetector();

            AssertMapsToActivity(uut, @event, Activity.InIDE, ActivityPhase.Start);
        }

        [Test]
        public void MapsMainWindowDeactivationToInIDEActivityEnd()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeMainWindow Aktueller Fenstertitel"),
                Action = WindowEvent.WindowAction.Deactivate
            };

            var uut = new InIDEActivityDetector();

            AssertMapsToActivity(uut, @event, Activity.InIDE, ActivityPhase.End);
        }

        private static void AssertMapsToActivity(IIDEEventProcessor uut, IDEEvent @event, Activity activity, ActivityPhase phase)
        {
            @event.IDESessionUUID = "sessionId";
            @event.KaVEVersion = "1.0";
            @event.TriggeredAt = new DateTime(2015, 04, 24, 15, 28, 30);
            @event.TriggeredBy = IDEEvent.Trigger.Unknown;
            @event.Duration = TimeSpan.FromMilliseconds(10);
            @event.ActiveWindow = WindowName.Get("vsWindowTypeToolWindow Startseite");

            var events = uut.Process(@event);

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
    }
}