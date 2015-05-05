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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Util;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
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
            Registry.RegisterComponent(_ioUtilMock.Object);

            Registry.RegisterComponent<ILogger>(new ConsoleLogger());

            _uut = new LogFileManager(_baseDirectory);
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        [Test]
        public void ShouldFindNoLogFilesInEmptyDirectory()
        {
            GivenLogsExist( /* none */);

            var actuals = _uut.Logs;

            Assert.IsTrue(actuals.IsEmpty());
        }

        [Test]
        public void ShouldFindSingleLog()
        {
            GivenLogsExist(1);

            var actuals = _uut.Logs;

            Assert.AreEqual(1, actuals.Count());
        }

        [Test]
        public void ShouldFindMultipleLogs()
        {
            GivenLogsExist(3);

            var actuals = _uut.Logs;

            Assert.AreEqual(3, actuals.Count());
        }

        [Test]
        public void ShouldFindLogForRightDate()
        {
            var expected = new DateTime(2014, 8, 28);
            GivenLogsExist(expected);

            var logs = _uut.Logs;

            Assert.AreEqual(expected, logs.First().Date);
        }

        [Test]
        public void ShouldReturnSameLogInstance()
        {
            GivenLogsExist(1);

            var log1 = _uut.Logs.First();
            var log2 = _uut.Logs.First();

            Assert.AreSame(log1, log2);
        }

        [Test]
        public void ShouldDetectNewLog()
        {
            Assert.IsEmpty(_uut.Logs);

            GivenLogsExist(1);

            Assert.IsNotEmpty(_uut.Logs);
        }

        [Test]
        public void ShouldDropDeletedLog()
        {
            GivenLogsExist(1);

            Assert.IsNotEmpty(_uut.Logs);

            GivenLogsExist( /* none */);

            Assert.IsEmpty(_uut.Logs);
        }

        [Test]
        public void ShouldReturnTodaysLog()
        {
            var expected = GetLogPath(DateTime.Today);

            var todaysLog = (LogFile) _uut.CurrentLog;

            Assert.AreEqual(expected, todaysLog.Path);
        }

        [Test]
        public void ShouldCreateCurrentLogIfItDoesntExist()
        {
            var currentLogPath = GetLogPath(DateTime.Today);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;

            _ioUtilMock.Verify(iou => iou.CreateFile(currentLogPath));
        }

        [Test(Description = "Manager needs to be thread safe.")]
        public void ShouldNotFailIteratingLogsIfCurrentLogIsCreatedConcurrently()
        {
            GivenLogsExist(1);

            // ReSharper disable UnusedVariable
            foreach (var log in _uut.Logs)
            {
                var currentLog = _uut.CurrentLog;
            }
            // ReSharper restore UnusedVariable
        }

        [Test]
        public void ShouldNotCreateCurrentLogIfItExists()
        {
            GivenLogsExist(DateTime.Today);
            var currentLogPath = GetLogPath(DateTime.Today);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;

            _ioUtilMock.Verify(iou => iou.CreateFile(currentLogPath), Times.Never);
        }

        [Test]
        public void ShouldRaiseLogCreatedEventWhenCreatingLog()
        {
            ILog newLog = null;
            _uut.LogCreated += log => newLog = log;

            var currentLog = _uut.CurrentLog;

            Assert.AreSame(currentLog, newLog);
        }

        [Test]
        public void ShouldNotRaiseLogCreatedWhenLogExists()
        {
            GivenLogsExist(DateTime.Today);
            _uut.LogCreated += log => Assert.Fail("created " + log);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;
        }

        [Test]
        public void ShouldDeleteOldLogs()
        {
            var d1 = new DateTime(2014, 03, 21);
            var d2 = new DateTime(2014, 02, 19);
            GivenLogsExist(d1, d2);

            _uut.DeleteLogsOlderThan(new DateTime(2014, 03, 30, 21, 23, 42));

            _ioUtilMock.Verify(io => io.DeleteFile(GetLogPath(d1)));
            _ioUtilMock.Verify(io => io.DeleteFile(GetLogPath(d2)));
        }

        [Test]
        public void ShouldNotDeleteNewLogs()
        {
            GivenLogsExist(new DateTime(2014, 03, 21), new DateTime(2014, 03, 29));

            _uut.DeleteLogsOlderThan(new DateTime(2014, 03, 20, 09, 00, 00));

            _ioUtilMock.Verify(iou => iou.DeleteFile(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShouldRemoveOlderEntriesFromLogOnSameDate()
        {
            var date = new DateTime(2014, 06, 26);
            GivenLogsExist(date);

            _uut.DeleteLogsOlderThan(new DateTime(2014, 06, 26, 14, 33, 12));

            AssertThatRemoveEntriesOlderThanWasInvokedOnLog(date);
        }

        private void AssertThatRemoveEntriesOlderThanWasInvokedOnLog(DateTime logDate)
        {
            // Since the LogFile is created by the manager, we have no way of mocking it. Therefore,
            // we check the method's invocation by checking its behavior.
            var logFilePath = GetLogPath(logDate);
            _ioUtilMock.Verify(iou => iou.OpenFile(logFilePath, FileMode.OpenOrCreate, FileAccess.Read));
            _ioUtilMock.Verify(iou => iou.DeleteFile(logFilePath));
            _ioUtilMock.Verify(iou => iou.MoveFile(It.IsAny<string>(), logFilePath));
        }

        [Test]
        public void ShouldGetLogsSizeOfNoFile()
        {
            Assert.AreEqual(0, _uut.LogsSize);
        }

        [Test]
        public void ShouldGetLogsSizeOfSingleFile()
        {
            GivenLogsExistWithSizeInBytes(1000);
            Assert.AreEqual(1000, _uut.LogsSize);
        }

        [Test]
        public void ShouldGetLogsSizeOfMultipleFiles()
        {
            GivenLogsExistWithSizeInBytes(2480, 524288, 1);
            Assert.AreEqual(526769, _uut.LogsSize);
        }

        [Test]
        public void ShouldDeleteLogFileDirectoryOnDeleteAllLogs()
        {
            _uut.DeleteAllLogs();

            _ioUtilMock.Verify(io => io.DeleteDirectoryWithContent(_uut.BaseLocation));
        }

        [Test]
        public void ShouldRaiseDeletedOnAllLogWhenDeletingAllLogs()
        {
            GivenLogsExist(3);

            var actuals = new List<ILog>();
            var expecteds = _uut.Logs.ToList();
            expecteds[0].Deleted += actuals.Add;
            expecteds[1].Deleted += actuals.Add;
            expecteds[2].Deleted += actuals.Add;

            _uut.DeleteAllLogs();

            CollectionAssert.AreEqual(expecteds, actuals);
        }

        private void GivenLogsExistWithSizeInBytes(params int[] sizesInBytes)
        {
            var dates = new DateTime[sizesInBytes.Length];
            for (var i = 0; i < sizesInBytes.Length; i++)
            {
                dates[i] = DateTime.Today.AddDays(-i);
                var currentPath = GetLogPath(dates[i]);
                _ioUtilMock.Setup(u => u.GetFileSize(currentPath)).Returns(sizesInBytes[i]);
            }

            GivenLogsExist(dates);
        }

        private void GivenLogsExist(int numberOfLogs)
        {
            var dates = Enumerable.Range(1, numberOfLogs).Select(i => DateTime.Today.AddDays(-i)).ToArray();
            GivenLogsExist(dates);
        }

        private void GivenLogsExist(params DateTime[] dates)
        {
            var logPaths = ToLogPaths(dates);
            _ioUtilMock.Setup(io => io.GetFiles(_baseDirectory, "Log_*")).Returns(logPaths);
            _ioUtilMock.Setup(io => io.FileExists(It.IsAny<string>())).Returns<string>(logPaths.Contains);
        }

        private string[] ToLogPaths(params DateTime[] dates)
        {
            return dates.Select(GetLogPath).ToArray();
        }

        private string GetLogPath(DateTime date)
        {
            var filename = "Log_" + date.ToString("yyyy-MM-dd");
            return Path.Combine(_baseDirectory, filename);
        }
    }
}