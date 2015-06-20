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
using Moq;
using NUnit.Framework;

namespace KaVE.ReSharper.Commons.Tests_Unit.Utils.Logging
{
    [TestFixture]
    internal class LogFileTest
    {
        private Mock<IIoUtils> _mockIoUtils;
        private const string TestTempFilePath = @"C:\Test\Tmp\File";
        private const string TestLogFilePath = @"C:\My\Log\Dir\Log_2014-03-21";
        private static readonly DateTime TestLogFileDate = new DateTime(2014, 3, 21);

        private const string TestConcurrentAccessExceptionMessage =
            "The process cannot access the file '" + TestLogFilePath +
            "' because it is being used by another process.";

        private LogFile _uut;
        private MemoryFile _memoryFile;
        
        [SetUp]
        public void SetUp()
        {
            _memoryFile = new MemoryFile();

            _mockIoUtils = new Mock<IIoUtils>();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Returns(_memoryFile.OpenRead);
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(_memoryFile.OpenAppend);
            Registry.RegisterComponent(_mockIoUtils.Object);

            Registry.RegisterComponent<ILogger>(new ConsoleLogger());

            _uut = new LogFile(TestLogFilePath);
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
        public void ShouldReadLogContent()
        {
            var expected = new[] {new TestIDEEvent {TestProperty = "1"}, new TestIDEEvent {TestProperty = "2"}};
            GivenTestLogContains(expected);

            var actual = _uut.ReadAll();

            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldInditicateIfEmpty()
        {
            GivenTestLogContains( /* empty */);

            Assert.IsTrue(_uut.IsEmpty());
        }

        [Test]
        public void ShouldIndicateIfHasContent()
        {
            GivenTestLogContains(new TestIDEEvent {TestProperty = "1"});

            Assert.IsFalse(_uut.IsEmpty());
        }

        [Test]
        public void ShouldCreateLogDirectoryIfNotExists()
        {
            GivenTestLogContains( /* empty */);

            _uut.Append(new TestIDEEvent());

            _mockIoUtils.Verify(iou => iou.CreateDirectory(@"C:\My\Log\Dir"));
        }

        [Test]
        public void ShouldWriteEntriesOnAppend()
        {
            var evt1 = new TestIDEEvent {TestProperty = "23"};
            var evt2 = new TestIDEEvent {TestProperty = "42"};

            _uut.Append(evt1);
            _uut.Append(evt2);

            var expected = CreateLogString(evt1, evt2);
            var actual = _memoryFile.Content;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRaiseEntryAddedOnAppend()
        {
            IDEEvent actual = null;
            _uut.EntryAppended += entry => actual = entry;

            var expected = new TestIDEEvent {TestProperty = "0xDEADBEEF"};
            _uut.Append(expected);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeleteLogFile()
        {
            _uut.Delete();

            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
        }

        [Test]
        public void ShouldRaiseDeletedEvent()
        {
            ILog actual = null;
            _uut.Deleted += log => actual = log;

            _uut.Delete();

            Assert.AreEqual(_uut, actual);
        }

        [Test]
        public void ShouldRemoveEntries()
        {
            var tempFile = GivenWritableTempFile();

            var evt1 = new TestIDEEvent {TestProperty = "1"};
            var evt2 = new TestIDEEvent {TestProperty = "2"};
            var evt3 = new TestIDEEvent {TestProperty = "3"};
            GivenTestLogContains(evt1, evt2, evt3);

            _uut.RemoveRange(new[] {evt1, evt3});

            AssertLogFileContent(tempFile, evt2);
            AssertThatLogWasReplacedWithTempFile();
        }

        private MemoryFile GivenWritableTempFile()
        {
            _mockIoUtils.Setup(iou => iou.GetTempFileName()).Returns(TestTempFilePath);
            var newFile = new MemoryFile();
            _mockIoUtils.Setup(iou => iou.OpenFile(TestTempFilePath, It.IsAny<FileMode>(), FileAccess.Write))
                        .Returns(newFile.OpenAppend);
            return newFile;
        }

        [Test]
        public void ShouldRaiseEntriesRemovedForRemovedEntries()
        {
            GivenWritableTempFile();
            var target = new TestIDEEvent {TestProperty = "B"};
            GivenTestLogContains(new TestIDEEvent {TestProperty = "A"}, target);

            IEnumerable<IDEEvent> actuals = null;
            _uut.EntriesRemoved += entries => actuals = entries;

            _uut.RemoveRange(new[] {target});

            CollectionAssert.AreEqual(new[] {target}, actuals);
        }

        [Test]
        public void ShouldRaiseEntriesRemovedEvenIfNothingIsRemoved()
        {
            GivenWritableTempFile();
            GivenTestLogContains(new TestIDEEvent {TestProperty = "SPD"});

            IEnumerable<IDEEvent> actuals = null;
            _uut.EntriesRemoved += entries => actuals = entries;

            _uut.RemoveRange(new[] {new TestIDEEvent {TestProperty = "CDU"}});

            CollectionAssert.IsEmpty(actuals);
        }

        [Test]
        public void ShouldRemoveOldEntries()
        {
            var tempFile = GivenWritableTempFile();
            var evt1 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 14, 0, 0)};
            var evt2 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 16, 0, 0)};
            var evt3 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 13, 0, 0)};
            GivenTestLogContains(evt1, evt2, evt3);

            _uut.RemoveEntriesOlderThan(new DateTime(2014, 3, 10, 15, 0, 0));

            AssertLogFileContent(tempFile, evt2);
            AssertThatLogWasReplacedWithTempFile();
        }

        [Test]
        public void ShouldRaiseEntriesRemovedForRemovedOldEntries()
        {
            GivenWritableTempFile();
            var evt1 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 5, 29, 0)};
            var evt2 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 5, 42, 0)};
            var evt3 = new TestIDEEvent {TriggeredAt = new DateTime(2014, 3, 10, 5, 35, 0)};
            GivenTestLogContains(evt1, evt2, evt3);

            IEnumerable<IDEEvent> actuals = null;
            _uut.EntriesRemoved += entries => actuals = entries;

            _uut.RemoveEntriesOlderThan(new DateTime(2014, 3, 10, 5, 36, 0));

            CollectionAssert.AreEqual(new[] {evt1, evt3}, actuals);
        }

        [Test]
        public void ShouldGetCorrectSizeInBytes()
        {
            const long expected = 2097152;
            _mockIoUtils.Setup(iou => iou.GetFileSize(TestLogFilePath)).Returns(expected);

            var actual = _uut.SizeInBytes;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldRetryFileOpenOnConcurrentAccessException()
        {
            var numberOfCalls = 0;
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Callback<string, FileMode, FileAccess>(
                            (path, fileMode, fileAccess) =>
                            {
                                numberOfCalls++;
                                if (numberOfCalls < 3)
                                {
                                    throw new IOException(TestConcurrentAccessExceptionMessage);
                                }
                            }).Returns(new MemoryStream());

            _uut.ReadAll();

            Assert.AreEqual(3, numberOfCalls);
        }

        [Test, Timeout(2000),
         ExpectedException(typeof (IOException), ExpectedMessage = TestConcurrentAccessExceptionMessage)]
        public void ShouldFailOnConcurrentAccessExceptionEventually()
        {
            _mockIoUtils.Setup(iou => iou.OpenFile(TestLogFilePath, It.IsAny<FileMode>(), FileAccess.Read))
                        .Throws(new IOException(TestConcurrentAccessExceptionMessage));

            _uut.ReadAll();
        }

        [Test]
        public void ShouldCreateCompatibleReaderAndWriter()
        {
            var expected = new List<TestIDEEvent>
            {
                new TestIDEEvent {TestProperty = "A 1st log message."},
                new TestIDEEvent {TestProperty = "A 2nd log message..."},
                new TestIDEEvent {TestProperty = "And yet another log message!"}
            };

            _uut.Append(expected[0]);
            _uut.Append(expected[1]);
            _uut.Append(expected[2]);

            var actual = _uut.ReadAll();

            CollectionAssert.AreEqual(expected, actual);
        }

        private void GivenTestLogContains(params TestIDEEvent[] expected)
        {
            _memoryFile.Content = CreateLogString(expected);
        }

        private static string CreateLogString(params TestIDEEvent[] es)
        {
            return string.Join("", es.Select(e => e.ToCompactJson() + "\r\n"));
        }

        private static void AssertLogFileContent(MemoryFile file, params TestIDEEvent[] events)
        {
            var logString = CreateLogString(events);
            Assert.AreEqual(logString, file.Content);
        }

        private void AssertThatLogWasReplacedWithTempFile()
        {
            // the old log file was deleted
            _mockIoUtils.Verify(iou => iou.DeleteFile(TestLogFilePath));
            // the new log file was moved to the path of the old one
            _mockIoUtils.Verify(iou => iou.MoveFile(TestTempFilePath, TestLogFilePath));
        }
    }
}