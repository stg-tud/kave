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
using KaVE.FeedbackProcessor.Activities;
using KaVE.FeedbackProcessor.Activities.Model;
using NUnit.Framework;

namespace KaVE.FeedbackProcessor.Tests.Activities
{
    internal class DocumentEventMapperTest : BaseToActivityMapperTest
    {
        public override BaseToActivityMapper Sut
        {
            get { return new DocumentEventToActivityMapper(); }
        }

        [Test]
        public void ShouldMapOpenToNavigation()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeProductionDocumentName,
                Action = DocumentEvent.DocumentAction.Opened
            };

            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapOpenTestToNavigationAndTesting()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeTestDocumentName,
                Action = DocumentEvent.DocumentAction.Opened
            };

            AssertMapsToActivities(@event, Activity.Navigation, Activity.Testing);
        }

        [Test]
        public void ShouldMapSaveToEdit()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeProductionDocumentName,
                Action = DocumentEvent.DocumentAction.Saved
            };

            AssertMapsToActivity(@event, Activity.Editing);
        }

        [Test]
        public void ShouldMapSaveOfTestToEditAndSave()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeTestDocumentName,
                Action = DocumentEvent.DocumentAction.Saved
            };

            AssertMapsToActivities(@event, Activity.Editing, Activity.Testing);
        }

        [Test]
        public void ShouldMapCloseToNavigation()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeProductionDocumentName,
                Action = DocumentEvent.DocumentAction.Closing
            };

            AssertMapsToActivity(@event, Activity.Navigation);
        }

        [Test]
        public void ShouldMapCloseTestToNavigationAndTesting()
        {
            var @event = new DocumentEvent
            {
                Document = TestFixtures.SomeTestDocumentName,
                Action = DocumentEvent.DocumentAction.Closing
            };

            AssertMapsToActivities(@event, Activity.Navigation, Activity.Testing);
        }
    }
}