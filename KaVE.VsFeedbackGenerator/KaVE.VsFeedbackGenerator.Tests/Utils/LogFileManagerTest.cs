using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
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
        public void CompressedTest()
        {
            // to summarize the idea behind this test:
            // the SessionManager does not work properly with the compressed format as default
            // my best guess was, that some writers aren't disposed properly and therefore the stream looses some information (linebreaks)
            // this test was written to emulate this situation but it seems to be impossible to write such a test
            // where a writer is finalized (garbage collected) in such a way that i can reuse the logFile for further writes
            var manager = new LogFileManager<Message>(
                IoTestHelper.GetTempDirectoryName(),
                JsonLogIoProvider.CompressedJsonFormatWriter<Message>());

            var file = manager.GetLogFileName("Compressed");
            for (var i = 0; i < 15; i ++)
            {
                Write(manager, file);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            //Assert.AreEqual(1, manager.NewLogReader(file).ReadAll().Count());
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Assert.Throws<InvalidDataException>(() => manager.NewLogReader(file).ReadAll().Count());
        }

        private void Write(ILogFileManager<Message> manager, string file)
        {
            manager.NewLogWriter(file).Write(LogFileManagerFixtures.RandomMessage(2));
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
            var manager = new JsonLogFileManager<Message>();
            var message = LogFileManagerFixtures.RandomMessage(8);
            string file;
            do
            {
                file = Path.Combine(Path.GetTempFileName(), new Random().Next() + manager.DefaultExtention);
            } while (!File.Exists(file));
            using (var writer = manager.NewLogWriter(file))
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