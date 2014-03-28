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
        private DTE _dte;
        private Globals _globals;
        private static readonly string TodayDateString = DateTime.Today.ToString(CultureInfo.InvariantCulture);

        [SetUp]
        public void MockEnvironment()
        {
            var mockGlobals = new Mock<Globals>();
            SetupGlobal(mockGlobals, "KAVE_EventGenerator_SessionUUID_CreatedAt");
            SetupGlobal(mockGlobals, "KAVE_EventGenerator_SessionUUID");
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
            _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"] = null;

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual(TodayDateString, _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"]);
            var storedSessionUUID = _globals["KAVE_EventGenerator_SessionUUID"];
            Assert.NotNull(storedSessionUUID);
            Assert.AreEqual(storedSessionUUID, actualSessionId);
        }

        [Test]
        public void ShouldCreateNewSessionUUIDAndStoreItForTodayIfStoredOneWasGeneratedInThePast()
        {
            var yesterdayDateString = DateTime.Today.AddDays(-1).ToString(CultureInfo.InvariantCulture);
            _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"] = yesterdayDateString;
            _globals["KAVE_EventGenerator_SessionUUID"] = "OutdatedUUID";

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual(TodayDateString, _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"]);
            Assert.NotNull(actualSessionId);
            Assert.AreNotEqual("OutdatedUUID", actualSessionId);
        }

        [Test]
        public void ShouldMakeUUIDAndCreationDatePropertyPersistent()
        {
            _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"] = null;

            var ideSession = new IDESession(_dte);
            // ReSharper disable once UnusedVariable
            var actualSessionId = ideSession.UUID;

            Assert.IsTrue(_globals.get_VariablePersists("KAVE_EventGenerator_SessionUUID_CreatedAt"));
            Assert.IsTrue(_globals.get_VariablePersists("KAVE_EventGenerator_SessionUUID"));
        }

        [Test]
        public void ShouldReturnExistingSessionUUIDIfItWasGeneratedToday()
        {
            _globals["KAVE_EventGenerator_SessionUUID_CreatedAt"] = TodayDateString;
            _globals["KAVE_EventGenerator_SessionUUID"] = "MyTestUUID";

            var ideSession = new IDESession(_dte);
            var actualSessionId = ideSession.UUID;

            Assert.AreEqual("MyTestUUID", actualSessionId);
        }
    }
}

// ReSharper restore UseIndexedProperty