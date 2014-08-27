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
using KaVE.Model.Events;
using KaVE.TestUtils.Model.Events;
using KaVE.VsFeedbackGenerator.Tests.Utils.Json;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileTest
    {
        private Mock<IIoUtils> _mockIoUtils;
        private const string TestLogFilePath = @"C:\My\Log\Dir\Log_2014-03-21";
        private static readonly DateTime TestLogFileDate = new DateTime(2014, 3, 21);
        private const string TestConcurrentAccessExceptionMessage =
            "The process cannot access the file '" + TestLogFilePath +
            "' because it is being used by another process.";

        private MemoryStream _inputStream;
        private MemoryStream _outputStream;

        private LogFile _uut;

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(() => _inputStream);
            
            _outputStream = new MemoryStream();
            
            _mockIoUtils.Setup(iou => iou.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(_outputStream);

            Registry.RegisterComponent(_mockIoUtils.Object);

            _uut = new LogFile(TestLogFilePath);
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        private void SetInputStream(string stream)
        {
            _inputStream = stream.AsStream();
        }

        private MemoryStream GetOutputStream()
        {
            return _outputStream;
        }

        [Test]
        public void ShouldRememberLogPath()
        {
            Assert.AreEqual(TestLogFilePath, _uut.Path);
        }

        [Test]
        public void ShouldIdentifyLogDateFromPath()
        {
            Assert.AreEqual(TestLogFileDate, _uut.Date);
        }

        [Test]
        public void ShouldCreateLogDirectoryIfNotExists()
        {
            SetInputStream("");

            _uut.NewLogWriter();

            _mockIoUtils.Verify(iou => iou.CreateDirectory(@"C:\My\Log\Dir"));
        }

        [Test]
        public void ShouldDeleteLogFile()
        {
            _uut.Delete();

            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
        }

        [Test]
        public void ShouldRemoveEntries()
        {
            SetInputStream("{\"TestProperty\":\"1\"}\r\n{\"TestProperty\":\"2\"}\r\n{\"TestProperty\":\"3\"}\r\n");

            _uut.RemoveRange(new [] { new TestIDEEvent{TestProperty = "1"}, new TestIDEEvent{TestProperty = "3"}});

            AssertContainsOnlyGivenEvent("\"TestProperty\":\"2\"", GetOutputStream());
            AssertThatOldLogWasReplacedWithUpdateLog();
        }

        [Test]
        public void ShouldRemoveOldEntries()
        {
            SetInputStream("{\"TriggeredAt\":\"2014-03-10T14:00:00\"}\r\n{\"TriggeredAt\":\"2014-03-10T16:00:00\"}\r\n{\"TriggeredAt\":\"2014-03-10T13:00:00\"}\r\n");

            _uut.RemoveEntriesOlderThan(new DateTime(2014, 3, 10, 15, 0, 0));

            AssertContainsOnlyGivenEvent("\"TriggeredAt\":\"2014-03-10T16:00:00\"", GetOutputStream());
            AssertThatOldLogWasReplacedWithUpdateLog();
        }

        [Test]
        public void ShouldGetCorrectSizeInBytes()
        {
            const long expected = 2097152;
            _mockIoUtils.Setup(iou => iou.GetFileSize(It.IsAny<String>())).Returns(expected);

            var actual = _uut.SizeInBytes;
            
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetryCreatingReaderOnConcurrentAccessException()
        {
            var numberOfCalls = 0;
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read)).Callback<string, FileMode, FileAccess>(
                (path, fileMode, fileAccess) =>
                {
                    numberOfCalls++;
                    if (numberOfCalls < 3)
                    {
                        throw new IOException(TestConcurrentAccessExceptionMessage);
                    }
                }).Returns(new MemoryStream());

            _uut.NewLogReader();

            Assert.AreEqual(3, numberOfCalls);
        }

        [Test, Timeout(2000), ExpectedException(typeof(IOException), ExpectedMessage = TestConcurrentAccessExceptionMessage)]
        public void ShouldFailOnConcurrentAccessExceptionEventually()
        {
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read)).Throws(new IOException(TestConcurrentAccessExceptionMessage));

            _uut.NewLogReader();
        }

        private static void AssertContainsOnlyGivenEvent(string line, MemoryStream outputStream)
        {
            // new log contains exactly one line and that line corresponds to the 2nd line in the original log
            var newLogContent = outputStream.AsString();
            var newLogLines = newLogContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(1, newLogLines.Length);
            StringAssert.Contains(line, newLogLines[0]);
        }

        private void AssertThatOldLogWasReplacedWithUpdateLog()
        {
            // the old log file was deleted
            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
            // the new log file was moved to the path of the old one
            _mockIoUtils.Verify(iou => iou.MoveFile(It.IsAny<string>(), TestLogFilePath));
        }

        [Test]
        public void ShouldCreateCompatibleReaderAndWriter()
        {
            _inputStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(_inputStream);
            
            var expected = new List<TestIDEEvent>
            {
                new TestIDEEvent{TestProperty = "A 1st log message."},
                new TestIDEEvent{TestProperty = "A 2nd log message..."},
                new TestIDEEvent{TestProperty = "And yet another log message!"}
            };

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[0]);
                writer.Write(expected[1]);
                writer.Write(expected[2]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(_inputStream.ToArray()));

            IEnumerable<IDEEvent> actual;
            using (var reader = _uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReadLogWrittenByMultipleWriters()
        {
            _inputStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(_inputStream);

            var expected = new[] { new TestIDEEvent{TestProperty = "message1"}, new TestIDEEvent{TestProperty = "message2"} };

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[0]);

                var buffer = _inputStream.ToArray();
                _inputStream = new MemoryStream();
                _inputStream.Write(buffer, 0, buffer.Length);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(_inputStream);

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[1]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(_inputStream.ToArray()));

            IEnumerable<IDEEvent> actual;
            using (var reader = _uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}