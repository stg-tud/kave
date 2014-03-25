using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal abstract class LogFileManagerTest
    {
        protected ILogFileManager<string> Uut { get; private set; }

        [SetUp]
        public void SetUp()
        {
            Uut = CreateManager(IoTestHelper.GetTempDirectoryName());
        }

        protected abstract ILogFileManager<string> CreateManager(string baseLocation);

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            Assert.AreEqual(0, Uut.GetLogFileNames().Count());
        }

        [Test]
        public void ShouldFindSingleLogFile()
        {
            GivenFileInManagersBaseLocation("File1" + Uut.DefaultExtension);

            Assert.AreEqual(1, Uut.GetLogFileNames().Count());
        }

        [Test]
        public void ShouldFindMultipleLogFiles()
        {
            GivenFileInManagersBaseLocation("File1" + Uut.DefaultExtension);
            GivenFileInManagersBaseLocation("File2" + Uut.DefaultExtension);
            GivenFileInManagersBaseLocation("File3" + Uut.DefaultExtension);

            Assert.AreEqual(3, Uut.GetLogFileNames().Count());
        }

        [Test]
        public void ShouldNotFindUnsupportedFile()
        {
            GivenFileInManagersBaseLocation("File1.unsupported");

            Assert.AreEqual(0, Uut.GetLogFileNames().Count());
        }

        private void GivenFileInManagersBaseLocation(string filename)
        {
            File.Create(Path.Combine(Uut.BaseLocation, filename)).Close();
        }

        [Test]
        public void ShouldCreateCompatibleReaderAndWriter()
        {
            var expected = new List<string>
            {
                LogFileManagerFixtures.RandomMessage(5).ToString(),
                LogFileManagerFixtures.RandomMessage(5).ToString(),
                LogFileManagerFixtures.RandomMessage(5).ToString()
            };
            var logFileName = Uut.GetLogFileName("Test1");
            using (var writer = Uut.NewLogWriter(logFileName))
            {
                writer.Write(expected[0]);
                writer.Write(expected[1]);
                writer.Write(expected[2]);
            }
            IEnumerable<string> actual;
            using (var reader = Uut.NewLogReader(logFileName))
            {
                actual = reader.ReadAll().ToList();
            }

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DeleteWorksProperly()
        {
            GivenFileInManagersBaseLocation("DeleteMe1" + Uut.DefaultExtension);
            GivenFileInManagersBaseLocation("DeleteMe2" + Uut.DefaultExtension);
            GivenFileInManagersBaseLocation("DeleteMe3" + Uut.DefaultExtension);
            //sleep because otherwise the timestamp of the last written file is to similar to the pivot
            Thread.Sleep(1);
            var deletionPivot = DateTime.Now;
            Thread.Sleep(1);
            var preservedFile1 = "PreserveMe1" + Uut.DefaultExtension;
            var preservedFile2 = "PreserveMe2" + Uut.DefaultExtension;
            GivenFileInManagersBaseLocation(preservedFile1);
            GivenFileInManagersBaseLocation(preservedFile2);
            Uut.DeleteLogsOlderThan(deletionPivot);

            var actual = Directory.EnumerateFiles(Uut.BaseLocation).Select(Path.GetFileName);
            var expected = new List<string>
            {
                preservedFile1,
                preservedFile2
            };

            Assert.AreEqual(expected, actual);
        }
    }
}