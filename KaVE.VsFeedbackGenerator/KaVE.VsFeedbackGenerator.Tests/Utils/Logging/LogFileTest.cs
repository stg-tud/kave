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

        private LogFile<string> _uut;

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();
            Registry.RegisterComponent(_mockIoUtils.Object);

            _uut = new LogFile<string>(TestLogFilePath);
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
        public void ShouldDeleteLogFile()
        {
            _uut.Delete();

            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
        }

        [Test]
        public void ShouldRemoveEntries()
        {
            var inputStream = "\"Line 1\"\r\n\"Line 2\"\r\n\"Line 3\"\r\n".AsStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(inputStream);
            var outputStream = new MemoryStream();
            _mockIoUtils.Setup(iou => iou.OpenFile(It.IsAny<string>(), It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(outputStream);

            _uut.RemoveRange(new [] { "Line 1", "Line 3"});

            Assert.AreEqual("\"Line 2\"\r\n", outputStream.AsString());
            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
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

            var expected = new List<string>
            {
                "A 1st log message.",
                "A 2nd log message...",
                "And yet another log message!"
            };

            using (var writer = _uut.NewLogWriter())
            {
                writer.Write(expected[0]);
                writer.Write(expected[1]);
                writer.Write(expected[2]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(targetStream.ToArray()));

            IEnumerable<string> actual;
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

            var expected = new[] { "message1", "message2" };

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

            IEnumerable<string> actual;
            using (var reader = _uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }

    }
}