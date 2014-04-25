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
        protected LogFileManager<string> Uut { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Registry.RegisterComponent<IIoUtils>(new IoUtils());
            Uut = new LogFileManager<string>(IoTestHelper.GetTempDirectoryName());
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            Assert.AreEqual(0, Uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogExists(DateTime.Today);

            Assert.AreEqual(1, Uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            Assert.AreEqual(3, Uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogDirectory()
        {
            Directory.CreateDirectory(Path.Combine(Uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, Uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogFile()
        {
            File.Create(Path.Combine(Uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, Uut.GetLogs().Count());
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var todaysLog = (LogFile<string>) Uut.TodaysLog;

            Assert.AreEqual(DateTime.Today, todaysLog.Date);
            Assert.AreEqual(Uut.BaseLocation, Path.GetDirectoryName(todaysLog.Path));
        }

        [Test]
        public void ShouldDeleteOldLogs()
        {
            GivenLogExists(DateTime.Today);
            GivenLogExists(DateTime.Today.AddDays(-1));
            GivenLogExists(DateTime.Today.AddDays(-2));

            Uut.DeleteLogsOlderThan(DateTime.Now.AddDays(-1));

            var expected = new[] {Uut.TodaysLog};
            var actual = Uut.GetLogs();
            CollectionAssert.AreEqual(expected, actual);
        }

        private void GivenLogExists(DateTime logDate)
        {
            var filename = "Log_" + logDate.ToString("yyyy-MM-dd");
            var logPath = Path.Combine(Uut.BaseLocation, filename);
            File.Create(logPath).Close();
        }
    }
}