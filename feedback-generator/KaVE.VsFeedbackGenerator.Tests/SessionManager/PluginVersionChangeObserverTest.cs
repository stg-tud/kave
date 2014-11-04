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
using JetBrains.Application.Extensions;
using JetBrains.Util;
using KaVE.Model.Events.VisualStudio;
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
            WhenPluginVersionChangeObserverReceives(VersionA, VersionA);

            AssertNoEvent();
        }

        [Test]
        public void ShouldNotUpdatePluginVersionIfVersionsMatch()
        {
            WhenPluginVersionChangeObserverReceives(VersionA, VersionA);

            StoreMock.Verify(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()), Times.Never);
        }

        [Test]
        public void ShouldGenerateInstallEventIfVersionsDoesntMatch()
        {
            WhenPluginVersionChangeObserverReceives(InitialVersion, VersionA);

            var installEvent = GetSinglePublished<InstallEvent>();

            Assert.AreEqual(VersionA, installEvent.PluginVersion);
        }

        [Test]
        public void ShouldGenerateUpdateEventIfVersionsDoesntMatch()
        {
            WhenPluginVersionChangeObserverReceives(VersionA, VersionB);

            var updateEvent = GetSinglePublished<UpdateEvent>();

            Assert.AreEqual(VersionA, updateEvent.OldPluginVersion);
            Assert.AreEqual(VersionB, updateEvent.NewPluginVersion);
        }

        [Test]
        public void ShouldUpdatePluginVersionIfVersionsDoesntMatch()
        {
            WhenPluginVersionChangeObserverReceives(VersionA, VersionB);

            StoreMock.Verify(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()));
            Assert.AreEqual(VersionB, StoreMock.Object.GetSettings<FeedbackSettings>().PluginVersion);
        }

        private void WhenPluginVersionChangeObserverReceives(string storedVersion, string currentVersion)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new PluginVersionChangeObserver(
                SetUpSettingsStore(storedVersion),
                SetUpRSEnv(currentVersion),
                TestIDESession,
                TestMessageBus,
                TestDateUtils);
        }

        private ISettingsStore SetUpSettingsStore(string storedVersion)
        {
            var settings = new FeedbackSettings
            {
                PluginVersion = storedVersion
            };
            StoreMock = new Mock<ISettingsStore>();
            StoreMock.Setup(s => s.GetSettings<FeedbackSettings>()).Returns(settings);
            StoreMock.Setup(s => s.UpdateSettings(It.IsAny<Action<FeedbackSettings>>()))
                     .Callback<Action<FeedbackSettings>>(a => a(settings));
            return StoreMock.Object;
        }

        private IRSEnv SetUpRSEnv(string currentVersion)
        {
            var extensionMock = new Mock<IExtension>();
            extensionMock.Setup(e => e.Version).Returns(SemanticVersion.Parse(currentVersion));
            var environmentMock = new Mock<IRSEnv>();
            environmentMock.Setup(e => e.KaVEExtension).Returns(extensionMock.Object);
            return environmentMock.Object;
        }
    }
}