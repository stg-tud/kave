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
 *    - 
 */

using System;
using KaVE.Model.Events;
using KaVE.TestUtils.Model.Names;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.SessionManager.Presentation;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    internal abstract class IDEEventAnonymizerTestBase<TEvent> where TEvent : IDEEvent
    {
        private DataExportAnonymizer _uut;
        protected ExportSettings ExportSettings { get; private set; }
        protected TEvent Target { get; private set; }

        [SetUp]
        public void SetUp()
        {
            ExportSettings = new ExportSettings();

            var mockSettingsStore = new Mock<ISettingsStore>();
            mockSettingsStore.Setup(store => store.GetSettings<ExportSettings>()).Returns(ExportSettings);

            _uut = new DataExportAnonymizer(mockSettingsStore.Object);

            Target = CreateEventWithAllAnonymizablePropertiesSet();
            Target.IDESessionUUID = "0xDEADBEEF";
            Target.TriggeredAt = DateTime.Now;
            Target.Duration = TimeSpan.FromSeconds(23);
            Target.ActiveDocument = VsComponentNameTestFactory.CreateAnonymousDocumentName();
            Target.ActiveWindow = VsComponentNameTestFactory.CreateAnonymousWindowName();
            Target.TriggeredBy = IDEEvent.Trigger.Click;
        }

        /// <summary>
        ///     Creates an instance of the event-type-to-anonymize with all custom properties set to non-default values.
        /// </summary>
        protected abstract TEvent CreateEventWithAllAnonymizablePropertiesSet();

        [Test]
        public void ShouldNotTouchEventIfNotOptionsAreSet()
        {
            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(Target, actual);
        }

        [Test]
        public void ShouldCloneTargetForAnonymization()
        {
            var actual = WhenEventIsAnonymized();

            Assert.AreNotSame(Target, actual);
        }

        protected TEvent WhenEventIsAnonymized()
        {
            return _uut.Anonymize(Target);
        }

        [Test]
        public void ShouldRemoveStartTimeWhenRespectiveOptionIsSet()
        {
            ExportSettings.RemoveStartTimes = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.TriggeredAt);
        }

        [Test]
        public void ShouldRemoveDurationWhenRespectiveOptionIsSet()
        {
            ExportSettings.RemoveDurations = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.Duration);
        }

        [Test]
        public void ShouldRemoveSessionUUIDWhenRespectiveOptionIsSet()
        {
            ExportSettings.RemoveSessionIDs = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.IDESessionUUID);
        }

        [Test]
        public void ShouldNotTouchFieldsThatDontNeedToBeAnonymized()
        {
            ExportSettings.RemoveStartTimes = true;
            ExportSettings.RemoveSessionIDs = true;
            ExportSettings.RemoveDurations = true;
            ExportSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(Target.ActiveDocument, actual.ActiveDocument);
            Assert.AreEqual(Target.ActiveWindow, actual.ActiveWindow);
            Assert.AreEqual(Target.TriggeredBy, actual.TriggeredBy);
            AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(Target, actual);
        }

        protected abstract void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(TEvent target,
            TEvent actual);
    }
}