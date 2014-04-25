using System;
using System.IO;
using System.Linq;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileManagerTest
    {
        private LogFileManager<string> _uut;

        [SetUp]
        public void SetUp()
        {
            Registry.RegisterComponent<IIoUtils>(new IoUtils());
            _uut = new LogFileManager<string>(IoTestHelper.GetTempDirectoryName());
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogExists(DateTime.Today);

            Assert.AreEqual(1, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            Assert.AreEqual(3, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogDirectory()
        {
            Directory.CreateDirectory(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogFile()
        {
            File.Create(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var todaysLog = (LogFile<string>) _uut.CurrentLog;

            Assert.AreEqual(DateTime.Today, todaysLog.Date);
            Assert.AreEqual(_uut.BaseLocation, Path.GetDirectoryName(todaysLog.Path));
        }

        [Test]
        public void ShouldDeleteOldLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            _uut.DeleteLogsOlderThan(DateTime.Now.AddDays(-1));

            var expected = new[] {_uut.CurrentLog};
            var actual = _uut.GetLogs();
            CollectionAssert.AreEqual(expected, actual);
        }

        private void GivenLogExists(DateTime logDate)
        {
            var filename = "Log_" + logDate.ToString("yyyy-MM-dd");
            var logPath = Path.Combine(_uut.BaseLocation, filename);
            File.Create(logPath).Close();
        }
    }
}