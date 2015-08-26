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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Util;
using KaVE.Commons.Utils.Exceptions;
using KaVE.Commons.Utils.IO;
using KaVE.RS.Commons.Utils;
using KaVE.VS.FeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;
using ILogger = KaVE.Commons.Utils.Exceptions.ILogger;

namespace KaVE.VS.FeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileManagerTest : LogManagerContractTest
    {
        private string _baseDirectory;
        private LogFileManager _uut;
        private Mock<IIoUtils> _ioUtilMock;
        private ICollection<string> _existingLogsPaths;

        protected override ILogManager CreateLogManager()
        {
            Registry.RegisterComponent<ILogger>(new ConsoleLogger());

            _ioUtilMock = new Mock<IIoUtils>();
            _ioUtilMock.Setup(io => io.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>()))
                       .Returns(() => new MemoryStream());
            _ioUtilMock.Setup(io => io.DeleteFile(It.IsAny<string>()))
                       .Callback<string>(fileName => _existingLogsPaths.Remove(fileName));
            _ioUtilMock.Setup(io => io.MoveFile(It.IsAny<string>(), It.IsAny<string>()))
                       .Callback<string, string>(
                           (oldName, newName) =>
                           {
                               _existingLogsPaths.Remove(oldName);
                               _existingLogsPaths.Add(newName);
                           });
            Registry.RegisterComponent(_ioUtilMock.Object);

            _baseDirectory = IoTestHelper.GetTempDirectoryName();
            _uut = new LogFileManager(_baseDirectory);
            return _uut;
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        protected override void GivenNoLogsExist()
        {
            GivenLogsExist( /* none */);
        }

        [Test]
        public void CreatesTodaysLogWithCorrectPath()
        {
            var expected = GetLogPath(Today);

            var todaysLog = (LogFile) Uut.CurrentLog;

            Assert.AreEqual(expected, todaysLog.Path);
        }

        [Test]
        public void CreatesCurrentLogFileIfNecessary()
        {
            var currentLogPath = GetLogPath(Today);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;

            _ioUtilMock.Verify(iou => iou.CreateFile(currentLogPath));
        }

        [Test]
        public void DoesNotCreateCurrentLogFileIfUnnecessary()
        {
            GivenLogsExist(Today);
            var currentLogPath = GetLogPath(Today);

            // ReSharper disable once UnusedVariable
            var currentLog = _uut.CurrentLog;

            _ioUtilMock.Verify(iou => iou.CreateFile(currentLogPath), Times.Never);
        }

        [Test]
        public void DeletesOldLogFiles()
        {
            GivenLogsExist(Yesterday, TwoDaysAgo);

            _uut.DeleteLogsOlderThan(Today);

            _ioUtilMock.Verify(io => io.DeleteFile(GetLogPath(Yesterday)));
            _ioUtilMock.Verify(io => io.DeleteFile(GetLogPath(TwoDaysAgo)));
        }

        [Test]
        public void KeepsNewLogFiles()
        {
            GivenLogsExist(Today, Yesterday);

            _uut.DeleteLogsOlderThan(TwoDaysAgo);

            _ioUtilMock.Verify(iou => iou.DeleteFile(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void RemovesOldEntriesFromLogOnSameDate()
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
            Assert.AreEqual(0, _uut.LogsSizeInBytes);
        }

        [Test]
        public void ShouldGetLogsSizeOfSingleFile()
        {
            GivenLogsExistWithSizeInBytes(1000);
            Assert.AreEqual(1000, _uut.LogsSizeInBytes);
        }

        [Test]
        public void ShouldGetLogsSizeOfMultipleFiles()
        {
            GivenLogsExistWithSizeInBytes(2480, 524288, 1);
            Assert.AreEqual(526769, _uut.LogsSizeInBytes);
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

        protected override void GivenLogsExist(params DateTime[] dates)
        {
            _existingLogsPaths = ToLogPaths(dates);
            _ioUtilMock.Setup(io => io.GetFiles(_baseDirectory, "Log_*")).Returns(() => _existingLogsPaths.ToArray());
            _ioUtilMock.Setup(io => io.FileExists(It.IsAny<string>())).Returns<string>(_existingLogsPaths.Contains);
        }

        private ICollection<string> ToLogPaths(params DateTime[] dates)
        {
            return dates.Select(GetLogPath).ToList();
        }

        private string GetLogPath(DateTime date)
        {
            var filename = "Log_" + date.ToString("yyyy-MM-dd");
            return Path.Combine(_baseDirectory, filename);
        }
    }
}