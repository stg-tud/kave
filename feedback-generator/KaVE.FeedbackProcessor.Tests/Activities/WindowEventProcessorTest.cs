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

using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Names.VisualStudio;
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using KaVE.FeedbackProcessor.Cleanup;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class WindowEventProcessorTest : BaseEventProcessorTest
    {
        public override IIDEEventProcessor Sut
        {
            get { return new WindowEventActivityProcessor(); }
        }

        [Test]
        public void ShouldMapCreateToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Create
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapMoveToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Move
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapCloseToConfiguration()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Close
            };
            AssertMapsToActivity(@event, Activity.LocalConfiguration);
        }

        [Test]
        public void ShouldMapActivateToXYZ()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Activate
            };
            // TODO window events could be assigned more precisely by taking the window name into account
            AssertDrop(@event);
        }

        [Test]
        public void ShouldDropWindowDeactivations()
        {
            var @event = new WindowEvent
            {
                Window = WindowName.Get(""),
                Action = WindowEvent.WindowAction.Deactivate
            };
            AssertDrop(@event);
        }
    }
}