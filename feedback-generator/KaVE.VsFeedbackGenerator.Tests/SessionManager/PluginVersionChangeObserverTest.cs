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
 *    - Dennis Albrecht
 */

using System;
using JetBrains.Util;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.VsFeedbackGenerator.SessionManager;
using KaVE.VsFeedbackGenerator.Tests.Generators;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.SessionManager
{
    [TestFixture]
    internal class PluginVersionChangeObserverTest : EventGeneratorTestBase
    {
        private const string VersionA = "1.0.0.0";
        private const string VersionB = "1.1.0.0";
        private const string InitialVersion = "";
        private Mock<ISettingsStore> StoreMock { get; set; }

        [TearDown]
        public void ClearMocks()
        {
            StoreMock = null;
        }

        [Test]
        public void ShouldNotGenerateEventsIfVersionsMatch()
        {
            GivenStoredVersion(VersionA);
            GivenCurrentVersion(VersionA);

            WhenPluginVersionChangeObserverStarts();

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotUpdatePluginVersionIfVersionsMatch()
        {
            GivenStoredVersion(VersionA);
            GivenCurrentVersion(VersionA);

            WhenPluginVersionChangeObserverStarts();

            StoreMock.Verify(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()), Times.Never);
        }

        [Test]
        public void ShouldGenerateInstallEventIfVersionsDoesntMatch()
        {
            GivenStoredVersion(InitialVersion);
            GivenCurrentVersion(VersionA);

            WhenPluginVersionChangeObserverStarts();

            var installEvent = GetSinglePublished<InstallEvent>();

            Assert.AreEqual(VersionA, installEvent.PluginVersion);
        }

        [Test]
        public void ShouldGenerateUpdateEventIfVersionsDoesntMatch()
        {
            GivenStoredVersion(VersionA);
            GivenCurrentVersion(VersionB);

            WhenPluginVersionChangeObserverStarts();

            var updateEvent = GetSinglePublished<UpdateEvent>();

            Assert.AreEqual(VersionA, updateEvent.OldPluginVersion);
            Assert.AreEqual(VersionB, updateEvent.NewPluginVersion);
        }

        [Test]
        public void ShouldUpdatePluginVersionIfVersionsDoesntMatch()
        {
            GivenStoredVersion(VersionA);
            GivenCurrentVersion(VersionB);

            WhenPluginVersionChangeObserverStarts();

            StoreMock.Verify(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()));
            Assert.AreEqual(VersionB, StoreMock.Object.GetSettings<FeedbackSettings>().PluginVersion);
        }

        private void GivenStoredVersion(string storedVersion)
        {
            var settings = new FeedbackSettings
            {
                PluginVersion = storedVersion
            };
            StoreMock = new Mock<ISettingsStore>();
            StoreMock.Setup(s => s.GetSettings<FeedbackSettings>()).Returns(settings);
            StoreMock.Setup(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()))
                     .Callback<Action<FeedbackSettings>>(a => a(settings));
        }

        private void GivenCurrentVersion(string currentVersion)
        {
            TestRSEnv.MockExtension.Setup(e => e.Version).Returns(SemanticVersion.Parse(currentVersion));
        }

        private void WhenPluginVersionChangeObserverStarts()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PluginVersionChangeObserver(
                StoreMock.Object,
                TestRSEnv,
                TestMessageBus,
                TestDateUtils);
        }
    }
}