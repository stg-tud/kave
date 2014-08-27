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
        private LogFileManager _uut;
        private Mock<IIoUtils> _ioUtilMock;

        [SetUp]
        public void SetUp()
        {
            _baseDirectory = IoTestHelper.GetTempDirectoryName();
            _ioUtilMock = new Mock<IIoUtils>();

            _ioUtilMock.Setup(io => io.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                       .Returns(() => new MemoryStream());

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

            Assert.AreEqual(0, _uut.Logs.Count());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogsExist(DateTime.Today);
            WhenLogFileManagerIsInitialized();

            Assert.AreEqual(1, _uut.Logs.Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogsExist(
                DateTime.Today,
                DateTime.Today.AddDays(-1),
                DateTime.Today.AddDays(-2));

            WhenLogFileManagerIsInitialized();

            Assert.AreEqual(3, _uut.Logs.Count());
        }

        [Test]
        public void ShouldCreateLogsFromFilesWhoseNamesFollowTheLogFileNamePattern()
        {
            // ReSharper disable once UnusedVariable
            var logs = _uut.Logs;

            _ioUtilMock.Verify(iou => iou.GetFiles(_baseDirectory, "Log_*"));
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var todaysLog = (LogFile) _uut.CurrentLog;

            Assert.AreEqual(DateTime.Today, todaysLog.Date);
            Assert.AreEqual(_uut.BaseLocation, Path.GetDirectoryName(todaysLog.Path));
        }

        [Test]
        public void ShouldCreateCurrentLogIfItDoesntExist()
        {
            Assert.IsEmpty(_uut.Logs);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;

            Assert.AreEqual(1, _uut.Logs.Count());
        }

        [Test]
        public void ShouldFireLogAddedEventWhenCreatingLog()
        {
            ILog newLog = null;
            _uut.LogAdded += log => newLog = log;

            var currentLog = _uut.CurrentLog;

            Assert.AreSame(currentLog, newLog);
        }

        [Test]
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
        public void ShouldNotDeleteTodayLog()
        {
            var today = new DateTime(2014, 06, 26);
            GivenLogsExist(today);
            WhenLogFileManagerIsInitialized();

            _uut.DeleteLogsOlderThan(today);

            const string logFileName = "Log_2014-06-26";
            var logFilePath = Path.Combine(_baseDirectory, logFileName);
            _ioUtilMock.Verify(iou => iou.OpenFile(logFilePath, FileMode.OpenOrCreate, FileAccess.Read));
            _ioUtilMock.Verify(iou => iou.DeleteFile(logFilePath));
            _ioUtilMock.Verify(iou => iou.MoveFile(It.IsAny<string>(), logFilePath));
            Assert.AreEqual(1, _uut.Logs.Count());
        }

        [Test]
        public void ShouldFireChangedEventOnDeletion()
        {
            var logsChanged = false;
            _uut.LogsChanged += (sender, args) => { logsChanged = true; };

            _uut.DeleteLogsOlderThan(DateTime.Now);

            Assert.IsTrue(logsChanged);
        }

        [Test]
        public void ShouldGetFormatedLogsSize_B()
        {
            GivenLogsExistWithSizeInBytes(1000);
            Assert.AreEqual("1000 B", _uut.FormatedLogsSize);
        }

        [Test]
        public void ShouldGetFormatedLogsSize_KB()
        {
            GivenLogsExistWithSizeInBytes(1024 * 23);
            Assert.AreEqual("23.0 KB", _uut.FormatedLogsSize);
        }

        [Test]
        public void ShouldGetFormatedLogsSize_MB()
        {
            GivenLogsExistWithSizeInBytes(1024 * 1024 * 20);
            Assert.AreEqual("20.0 MB", _uut.FormatedLogsSize);
        }

        [Test]
        public void ShouldGetFormatedLogsSize_MultipleFiles()
        {
            GivenLogsExistWithSizeInBytes(1024 * 1024 * 20, 1024 * 512);
            Assert.AreEqual("20.5 MB", _uut.FormatedLogsSize);
        }

        [Test]
        public void ShouldDeleteLogFileDirectory()
        {
            GivenLogsExist(DateTime.Today);
            WhenLogFileManagerIsInitialized();

            _uut.DeleteAllLogs();

            _ioUtilMock.Verify(io => io.DeleteDirectoryWithContent(_uut.BaseLocation));
        }

        private void WhenLogFileManagerIsInitialized()
        {
            _uut = new LogFileManager(_baseDirectory);
        }

        private void GivenLogsExist(params DateTime[] dates)
        {
            _ioUtilMock.Setup(io => io.GetFiles(_baseDirectory, It.IsAny<string>()))
                       .Returns(
                           dates.Select(
                               dateTime =>
                                   GetValidLogPath(_baseDirectory, dateTime)).ToArray());

            Registry.Clear();
            Registry.RegisterComponent(_ioUtilMock.Object);
        }

        private void GivenLogsExistWithSizeInBytes(params int[] sizesInBytes)
        {
            var pathes = new string[sizesInBytes.Length];

            for (var i = 0; i < sizesInBytes.Length; i++)
            {
                var currentPath = GetValidLogPath(_baseDirectory, DateTime.Today.AddDays(-i));
                _ioUtilMock.Setup(u => u.GetFileSize(currentPath)).Returns(sizesInBytes[i]);
                pathes[i] = currentPath;
            }

            _ioUtilMock.Setup(io => io.GetFiles(_baseDirectory, It.IsAny<string>()))
                       .Returns(pathes);
        }


        private static string GetValidLogPath(string path, DateTime date)
        {
            return GetLogPath(path, date, "Log_");
        }

        private static string GetLogPath(string path, DateTime date, string logPrefix)
        {
            var filename = logPrefix + date.ToString("yyyy-MM-dd");
            return Path.Combine(path, filename);
        }
    }
}