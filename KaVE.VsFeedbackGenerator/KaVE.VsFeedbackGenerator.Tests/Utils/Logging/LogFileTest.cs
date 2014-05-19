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
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private LogFile<TestIDEEvent> _uut;

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();
            Registry.RegisterComponent(_mockIoUtils.Object);

            _uut = new LogFile<TestIDEEvent>(TestLogFilePath);
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
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
            var inputStream = "".AsStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(inputStream);

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
            var inputStream = "{\"TestProperty\":\"1\"}\r\n{\"TestProperty\":\"2\"}\r\n{\"TestProperty\":\"3\"}\r\n".AsStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(inputStream);
            var outputStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(outputStream);

            _uut.RemoveRange(new [] { new TestIDEEvent{TestProperty = "1"}, new TestIDEEvent{TestProperty = "3"}});

            // new log contains exactly one line and that line corresponds to the 2nd line in the original log
            var newLogContent = outputStream.AsString();
            var newLogLines = newLogContent.Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(1, newLogLines.Length);
            StringAssert.Contains("\"TestProperty\":\"2\"", newLogLines[0]);
            // the old log file was deleted
            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
            // the new log file was moved to the path of the old one
            _mockIoUtils.Verify(iou => iou.MoveFile(It.IsAny<string>(), TestLogFilePath));
        }

        [Test, Ignore("Introduce a LogEntryWrapper with a timestamp to have the notion of time available")]
        public void ShouldRemoveOldEntries()
        {
            // _uut.RemoveOldThan(TestLogFileDate.AddMinutes(-10));
        }

        [Test]
        public void ShouldCreateCompatibleReaderAndWriter()
        {
            var targetStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(targetStream);

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
                        .Returns(new MemoryStream(targetStream.ToArray()));

            IEnumerable<TestIDEEvent> actual;
            using (var reader = _uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldReadLogWrittenByMultipleWriters()
        {
            var targetStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(targetStream);

            var expected = new[] { new TestIDEEvent{TestProperty = "message1"}, new TestIDEEvent{TestProperty = "message2"} };

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[0]);

                var buffer = targetStream.ToArray();
                targetStream = new MemoryStream();
                targetStream.Write(buffer, 0, buffer.Length);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(targetStream);

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[1]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(targetStream.ToArray()));

            IEnumerable<TestIDEEvent> actual;
            using (var reader = _uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }

    }
}