using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KaVE.VsFeedbackGenerator.Tests.Utils.Json;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Logging;
using Moq;
using NuGet;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Logging
{
    [TestFixture]
    internal class LogFileTest
    {
        private Mock<IIoUtils> _mockIoUtils;
        private const string TestLogFilePath = @"C:\My\Log\Dir\Log_2014-03-21";

        [SetUp]
        public void SetUp()
        {
            _mockIoUtils = new Mock<IIoUtils>();
            Registry.RegisterComponent(_mockIoUtils.Object);

            Uut = new LogFile<string>(TestLogFilePath);
        }

        [TearDown]
        public void CleanUp()
        {
            Registry.Clear();
        }

        protected LogFile<string> Uut { get; private set; }

        [Test]
        public void ShouldRememberLogPath()
        {
            Assert.AreEqual(TestLogFilePath, Uut.Path);
        }

        [Test]
        public void ShouldIdentifyLogDateFromPath()
        {
            var expected = new DateTime(2014, 3, 21);
            var actual = Uut.Date;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeleteLogFile()
        {
            Uut.Delete();

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

            Uut.RemoveRange(new [] { "Line 1", "Line 3"});

            Assert.AreEqual("\"Line 2\"\r\n", outputStream.AsString());
            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
            _mockIoUtils.Verify(iou => iou.MoveFile(It.IsAny<string>(), TestLogFilePath));
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

            using (var writer = Uut.NewLogWriter())
            {
                writer.Write(expected[0]);
                writer.Write(expected[1]);
                writer.Write(expected[2]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(targetStream.ToArray()));

            IEnumerable<string> actual;
            using (var reader = Uut.NewLogReader())
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

            using (var writer = Uut.NewLogWriter())
            {
                writer.Write(expected[0]);

                var buffer = targetStream.ToArray();
                targetStream = new MemoryStream();
                targetStream.Write(buffer, 0, buffer.Length);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(targetStream);

            using (var writer = Uut.NewLogWriter())
            {
                writer.Write(expected[1]);
            }

            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(new MemoryStream(targetStream.ToArray()));

            IEnumerable<string> actual;
            using (var reader = Uut.NewLogReader())
            {
                actual = reader.ReadAll().ToList();
            }

            CollectionAssert.AreEqual(expected, actual);
        }

    }
}