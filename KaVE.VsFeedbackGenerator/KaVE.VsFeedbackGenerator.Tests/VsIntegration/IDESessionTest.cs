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
using System.Globalization;
using EnvDTE;
using KaVE.VsFeedbackGenerator.VsIntegration;
using Moq;
using NUnit.Framework;

// ReSharper disable UseIndexedProperty

namespace KaVE.VsFeedbackGenerator.Tests.VsIntegration
{
    [TestFixture]
    public class IDESessionTest
    {
        private const string SessionUuidCreatedAt = "KAVE_EventGenerator_SessionUUID_CreatedAt";
        private const string SessionUuid = "KAVE_EventGenerator_SessionUUID";
        private DTE _dte;
        private Globals _globals;
        private static readonly string TodayDateString = DateTime.Today.ToString(CultureInfo.InvariantCulture);

        [SetUp]
        public void MockEnvironment()
        {
            var mockGlobals = new Mock<Globals>();
            SetupGlobal(mockGlobals, SessionUuidCreatedAt);
            SetupGlobal(mockGlobals, SessionUuid);
            _globals = mockGlobals.Object;

            var mockDTE = new Mock<DTE>();
            mockDTE.Setup(dte => dte.Globals).Returns(_globals);
            _dte = mockDTE.Object;
        }

        private static void SetupGlobal(Mock<Globals> mockGlobals, string index)
        {
            string value = null;
            mockGlobals.Setup(g => g.get_VariableExists(index)).Returns(() => value != null);
            mockGlobals.SetupSet(g => g[index] = It.IsAny<string>())
                .Callback<string, object>((idx, val) => value = (string) val);
            mockGlobals.SetupGet(g => g[index]).Returns(() => value);
            var persistent = false;
            mockGlobals.Setup(g => g.set_VariablePersists(index, It.IsAny<bool>()))
                .Callback<string, bool>((idx, persists) => persistent = persists);
            mockGlobals.Setup(g => g.get_VariablePersists(index)).Returns(() => persistent);
        }


        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfNoneExisted()
        {
            _globals[SessionUuidCreatedAt] = null;

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual(TodayDateString, _globals[SessionUuidCreatedAt]);
            var storedSessionUUID = _globals[SessionUuid];
            Assert.NotNull(storedSessionUUID);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfStoredOneWasGeneratedInThePast()
        {
            var yesterdayDateString = DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture);
            _globals[SessionUuidCreatedAt] = yesterdayDateString;
            _globals[SessionUuid] = "OutdatedUUID";

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual(TodayDateString, _globals[SessionUuidCreatedAt]);
            var storedSessionUUID = _globals[SessionUuid];
            Assert.NotNull(actualSessionId);
            Assert.AreNotEqual("OutdatedUUID", actualSessionId);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfStoredOneWasGeneratedOnFutureDate()
        {
            var tomorrowDateString = DateTime.Today.AddDays(1).ToString(CultureInfo.InvariantCulture);
            _globals[SessionUuidCreatedAt] = tomorrowDateString;
            _globals[SessionUuid] = "TemporaryUUID";

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual(TodayDateString, _globals[SessionUuidCreatedAt]);
            var storedSessionUUID = _globals[SessionUuid];
            Assert.NotNull(actualSessionId);
            Assert.AreNotEqual("TemporaryUUID", actualSessionId);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldMakeUUIDAndCreationDatePropertyPersistent()
        {
            _globals[SessionUuidCreatedAt] = null;

            var ideSession = new IDESession(_dte);
            // ReSharper disable once UnusedVariable
            var actualSessionId = ideSession.UUID;

            Assert.IsTrue(_globals.get_VariablePersists(SessionUuidCreatedAt));
            Assert.IsTrue(_globals.get_VariablePersists(SessionUuid));
        }

        [Test]
        public void ShouldReturnExistingSessionUUIDIfItWasGeneratedToday()
        {
            _globals[SessionUuidCreatedAt] = TodayDateString;
            _globals[SessionUuid] = "MyTestUUID";

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual("MyTestUUID", actualSessionId);
        }
    }
}

// ReSharper restore UseIndexedProperty