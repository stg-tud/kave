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
 *    - Sebastian Proksch
 */

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class InIDEActivityDetectorTest : BaseActivityProcessorTest
    {
        public override BaseActivityProcessor Sut
        {
            get { return new InIDEActivityDetector(); }
        }

        [Test]
        public void MapsMainWindowActivationToEnterIDEActivity()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeMainWindow Startseite - Microsoft Visual Studio"),
                Action = WindowEvent.WindowAction.Activate
            };

            AssertMapsToActivity(@event, Activity.EnterIDE);
        }

        [Test]
        public void MapsMainWindowDeactivationToLeaveIDEActivity()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get("vsWindowTypeMainWindow Aktueller Fenstertitel"),
                Action = WindowEvent.WindowAction.Deactivate
            };

            AssertMapsToActivity(@event, Activity.LeaveIDE);
        }
    }
}