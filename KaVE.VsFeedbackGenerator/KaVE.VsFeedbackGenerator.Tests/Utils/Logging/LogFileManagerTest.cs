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
 *    - Sven Amann
 *    - Uli Fahrer
 */

using System;
using System.IO;
using System.Linq;
using KaVE.TestUtils.Model.Events;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileManagerTest
    {
        private string _baseDirectory;
        private LogFileManager<TestIDEEvent> _uut;
        private Mock<IIoUtils> _ioUtilMock;

        [SetUp]
        public void SetUp()
        {
            _baseDirectory = IoTestHelper.GetTempDirectoryName();
            GivenLogsExist();
            WhenLogFileManagerIsInitialized();
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            GivenLogsExist();

            WhenLogFileManagerIsInitialized();

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogsExist(DateTime.Today);

            WhenLogFileManagerIsInitialized();

            Assert.AreEqual(1, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogsExist(
                DateTime.Today,
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(-2));

            WhenLogFileManagerIsInitialized();

            Assert.AreEqual(3, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogDirectory()
        {
            //TODO
            Directory.CreateDirectory(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldIgnoreNonLogFile()
        {
            //TODO
            File.Create(Path.Combine(_uut.BaseLocation, "NonLog"));

            Assert.AreEqual(0, _uut.GetLogs().Count());
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var todaysLog = (LogFile<TestIDEEvent>) _uut.CurrentLog;

            Assert.AreEqual(DateTime.Today, todaysLog.Date);
            Assert.AreEqual(_uut.BaseLocation, Path.GetDirectoryName(todaysLog.Path));
        }

        //TODO: Test with today for .date case

        [Test, Ignore("TODO Mock JSONLogWriter")]
        public void ShouldDeleteOldLogEntries()
        {
            GivenLogsExist(new DateTime(2014, 03, 21), new DateTime(2014, 03, 29));
            WhenLogFileManagerIsInitialized();

            _uut.DeleteLogsOlderThan(new DateTime(2014, 03, 20));

            _ioUtilMock.Verify(io => io.GetTempFileName(), Times.Exactly(2));
        }

        [Test]
        public void ShouldDeleteOldLogs()
        {
            var d1 = new DateTime(2014, 03, 21);
            var d2 = new DateTime(2014, 02, 19);

            GivenLogsExist(d1, d2);
            WhenLogFileManagerIsInitialized();

            _uut.DeleteLogsOlderThan(new DateTime(2014, 03, 30));

            _ioUtilMock.Verify(io => io.DeleteFile(GetValidLogPath(_baseDirectory, d1)));
            _ioUtilMock.Verify(io => io.DeleteFile(GetValidLogPath(_baseDirectory, d2)));
        }

        [Test]
        public void ShouldDeleteLogFileDirectory()
        {
            GivenLogsExist(DateTime.Today);
            WhenLogFileManagerIsInitialized();

            _uut.DeleteLogFileDirectory();

            _ioUtilMock.Verify(io => io.DeleteDirectoryWithContent(_uut.BaseLocation));
        }

        private void WhenLogFileManagerIsInitialized()
        {
            _uut = new LogFileManager<TestIDEEvent>(_baseDirectory);
        }

        private void GivenLogsExist(params DateTime[] dates)
        {
            _ioUtilMock = new Mock<IIoUtils>();
            _ioUtilMock.Setup(io => io.GetFiles(_baseDirectory, It.IsAny<string>()))
                       .Returns(
                           dates.Select(
                               dateTime =>
                                   GetValidLogPath(_baseDirectory, dateTime)).ToArray());

            Registry.Clear();
            Registry.RegisterComponent(_ioUtilMock.Object);
        }

        private static string GetValidLogPath(string path, DateTime date)
        {
            return GetLogPath(path, date, "Log_");
        }

        /*private string GetInvalidLogPath(string path, DateTime date)
        {
            return GetLogPath(path, date, "NonLog_");
        }*/

        private static string GetLogPath(string path, DateTime date, string logPrefix)
        {
            var filename = logPrefix + date.ToString("yyyy-MM-dd");
            return Path.Combine(path, filename);
        }
    }
}