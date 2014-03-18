using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Model.Events.VisualStudio;
using KaVE.Utils.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class LogFileManagerTest
    {
        [NotNull] private IEnumerable<ILogFileManager<Message>> _manager;

        [SetUp]
        public void SetUp()
        {
            _manager = new List<ILogFileManager<Message>>
            {
                new LogFileManager<Message>(
                    IoTestHelper.GetTempDirectoryName(),
                    JsonLogIoProvider.JsonFormatWriter<Message>()),
                new LogFileManager<Message>(
                    IoTestHelper.GetTempDirectoryName(),
                    JsonLogIoProvider.CompressedJsonFormatWriter<Message>())
            };
        }

        [Test]
        public void DirectoryIsEmptyInitially()
        {
            foreach (var fileManager in _manager)
            {
                Assert.AreEqual(0, fileManager.GetLogFileNames().Count());
            }
        }

        [Test]
        public void DirectoryContainsFilesAfterWriting()
        {
            foreach (var fileManager in _manager)
            {
                for (var i = 1; i < 12; i ++)
                {
                    using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("File" + i)))
                    {
                        writer.Write(LogFileManagerFixtures.RandomMessage(5));
                    }

                    Assert.AreEqual(i, fileManager.GetLogFileNames().Count());
                }
            }
        }

        [Test, Ignore]
        public void DefaultWriterWritesCompressed()
        {
            var manager = new JsonLogFileManager();
            var file = Path.GetTempFileName();
            IDEEvent message = new SolutionEvent
            {
                Action = SolutionEvent.SolutionAction.OpenSolution,
                ActiveDocument = null,
                ActiveWindow = null,
                IDESessionUUID = "Test_Session",
                Target = null,
                TerminatedAt = DateTime.Now.AddMilliseconds(200),
                TriggeredAt = DateTime.Now.AddMilliseconds(-200),
                TriggeredBy = IDEEvent.Trigger.Automatic
            };
            using (var writer = manager.Writer.First().NewWriter(file))
            {
                writer.Write(message);
            }
            IDEEvent actual;
            using (var reader = JsonLogIoProvider.CompressedJsonFormatWriter<IDEEvent>().NewReader(file))
            {
                actual = reader.ReadNext();
            }
            Assert.AreEqual(message, actual);
        }

        [Test]
        public void WriteReadSingle()
        {
            foreach (var fileManager in _manager)
            {
                var message = LogFileManagerFixtures.RandomMessage(5);
                using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("Test1")))
                {
                    writer.Write(message);
                }
                Message actual;
                using (var reader = fileManager.NewLogReader(fileManager.GetLogFileName("Test1")))
                {
                    actual = reader.ReadNext();
                }

                Assert.AreEqual(message, actual);
            }
        }

        [Test]
        public void WriteReadMulti()
        {
            foreach (var fileManager in _manager)
            {
                var messages = new List<Message>();
                for (var i = 0; i < 25; i++)
                {
                    messages.Add(LogFileManagerFixtures.RandomMessage(5));
                }
                using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("Test1")))
                {
                    writer.WriteRange(messages);
                }
                List<Message> actual;
                using (var reader = fileManager.NewLogReader(fileManager.GetLogFileName("Test1")))
                {
                    actual = new List<Message>(reader.ReadAll());
                }

                Assert.AreEqual(messages, actual);
            }
        }

        [Test]
        public void WriteReadMultiIndividually()
        {
            foreach (var fileManager in _manager)
            {
                var messages = new List<Message>();
                for (var i = 0; i < 25; i++)
                {
                    messages.Add(LogFileManagerFixtures.RandomMessage(5));
                }
                using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("Test1")))
                {
                    foreach (var message in messages)
                    {
                        writer.Write(message);
                    }
                }
                var actual = new List<Message>();
                using (var reader = fileManager.NewLogReader(fileManager.GetLogFileName("Test1")))
                {
                    Message read;
                    while ((read = reader.ReadNext()) != null)
                    {
                        actual.Add(read);
                    }
                }

                Assert.AreEqual(messages, actual);
            }
        }

        [TestCase(0, 0), TestCase(5, 0), TestCase(0, 5), TestCase(5, 5)]
        public void DeleteWorksProperly(int delete, int preserve)
        {
            var message = LogFileManagerFixtures.RandomMessage(7);
            foreach (var fileManager in _manager)
            {
                for (var i = 0; i < delete; i ++)
                {
                    using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("DeleteMe" + i)))
                    {
                        writer.Write(message);
                    }
                }
                Thread.Sleep(10); //sleep because otherwise the timestamp of the last written file is to similar to the pivot
                var deletionPivot = DateTime.Now;
                Thread.Sleep(10); //sleep to prevent problems similar to the problem noted above
                for (var i = 0; i < preserve; i++)
                {
                    using (var writer = fileManager.NewLogWriter(fileManager.GetLogFileName("PreserveMe" + i)))
                    {
                        writer.Write(message);
                    }
                }
                fileManager.DeleteLogsOlderThan(deletionPivot);
                Assert.AreEqual(preserve, fileManager.GetLogFileNames().Count(l => l.Contains("PreserveMe")));
                Assert.AreEqual(0, fileManager.GetLogFileNames().Count(l => l.Contains("DeleteMe")));
            }
        }
    }
}