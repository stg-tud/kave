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
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.RS.Commons.Settings;
using KaVE.VS.FeedbackGenerator.SessionManager.Anonymize;
using KaVE.VS.FeedbackGenerator.Settings;
using Moq;
using NUnit.Framework;

namespace KaVE.VS.FeedbackGenerator.Tests.SessionManager.Anonymize
{
    internal abstract class IDEEventAnonymizerTestBase<TEvent> where TEvent : IDEEvent
    {
        private DataExportAnonymizer _uut;
        protected AnonymizationSettings AnonymizationSettings { get; private set; }
        protected TEvent OriginalEvent { get; private set; }

        [SetUp]
        public void SetUp()
        {
            AnonymizationSettings = new AnonymizationSettings();

            var mockSettingsStore = new Mock<ISettingsStore>();
            mockSettingsStore.Setup(store => store.GetSettings<AnonymizationSettings>()).Returns(AnonymizationSettings);

            _uut = new DataExportAnonymizer(mockSettingsStore.Object);

            OriginalEvent = CreateEventWithAllAnonymizablePropertiesSet();
            OriginalEvent.IDESessionUUID = "0xDEADBEEF";
            OriginalEvent.TriggeredAt = DateTime.Now;
            OriginalEvent.Duration = TimeSpan.FromSeconds(23);
            OriginalEvent.ActiveDocument = DocumentName.Get("CSharp \\P1\\Class1.cs");
            OriginalEvent.ActiveWindow = WindowName.Get("vsWindowTypeDocument Class1.cs");
            OriginalEvent.TriggeredBy = IDEEvent.Trigger.Click;
        }

        /// <summary>
        ///     Creates an instance of the event-type-to-anonymize with all custom properties set to non-default values.
        /// </summary>
        protected abstract TEvent CreateEventWithAllAnonymizablePropertiesSet();

        [Test]
        public void ShouldNotTouchEventIfNoOptionsAreSet()
        {
            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(OriginalEvent, actual);
        }

        [Test]
        public void ShouldCloneTargetForAnonymization()
        {
            var actual = WhenEventIsAnonymized();

            Assert.AreNotSame(OriginalEvent, actual);
        }

        [Test]
        public void ShouldRemoveStartTimeWhenRespectiveOptionIsSet()
        {
            AnonymizationSettings.RemoveStartTimes = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.TriggeredAt);
        }

        [Test]
        public void ShouldRemoveDurationWhenRespectiveOptionIsSet()
        {
            AnonymizationSettings.RemoveDurations = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.Duration);
        }

        [Test]
        public void ShouldNotRemoveSessionUUIDWhenRespectiveOptionIsSet()
        {
            // we removed this option in March 2016, but left it in the options to show a warning dialog that also disables the option.
            // this test ensures that the session id is in fact never removed on export, even if a developers hacks the settings file.

            AnonymizationSettings.RemoveSessionIDs = true;

            var actual = WhenEventIsAnonymized();

            Assert.NotNull(actual.IDESessionUUID);
            Assert.AreEqual(OriginalEvent.IDESessionUUID, actual.IDESessionUUID);
        }

        [Test]
        public void ShouldRemovePathFromActiveDocumentNameWhenRemoveNamesIsSet()
        {
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("CSharp V95bjWgLii851OlpGj2Btw==", actual.ActiveDocument.Identifier);
        }

        [Test]
        public void ShouldNotFailWhenActiveDocumentIsNullAndRemoveNamesIsSet()
        {
            OriginalEvent.ActiveDocument = null;
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.ActiveDocument);
        }

        [Test]
        public void ShouldRemovePathFromActiveWindowNameWhenRemoveNamesIsSet()
        {
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("vsWindowTypeDocument srcJ_SmXbxiIkauvY52nqA==", actual.ActiveWindow.Identifier);
        }

        [Test]
        public void ShouldNotChangeActiveWindowNameWhenRemoveNamesIsSetButWindowNameContainsNoPath()
        {
            OriginalEvent.ActiveWindow = WindowName.Get("vsWindowTypeToolWindow Start Page");
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual("vsWindowTypeToolWindow Start Page", actual.ActiveWindow.Identifier);
        }

        [Test]
        public void ShouldNotFailWhenActiveWindowIsNullAndRemoveNamesIsSet()
        {
            OriginalEvent.ActiveWindow = null;
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.IsNull(actual.ActiveWindow);
        }

        [Test]
        public void ShouldNotTouchFieldsThatDontNeedToBeAnonymized()
        {
            AnonymizationSettings.RemoveStartTimes = true;
            AnonymizationSettings.RemoveSessionIDs = true;
            AnonymizationSettings.RemoveDurations = true;
            AnonymizationSettings.RemoveCodeNames = true;

            var actual = WhenEventIsAnonymized();

            Assert.AreEqual(OriginalEvent.TriggeredBy, actual.TriggeredBy);
            AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(OriginalEvent, actual);
        }

        protected abstract void AssertThatPropertiesThatAreNotTouchedByAnonymizationAreUnchanged(TEvent original,
            TEvent anonymized);

        protected TEvent WhenEventIsAnonymized()
        {
            return (TEvent) _uut.Anonymize(OriginalEvent);
        }
    }
}