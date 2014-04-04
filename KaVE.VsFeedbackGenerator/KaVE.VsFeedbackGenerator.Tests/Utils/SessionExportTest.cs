using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class SessionExportTest
    {
        private ILogWriter<string> _writer;

        [Test]
        public void TemporaryExportContainsGivenList()
        {
            var uut = new SessionExport(new Mock<ISessionPublisher>().Object);
            var expected = RandomlyFilledList(5);
            var actual = new List<string>();
            _writer = MockWriter(actual);
            uut.ExportToTemporaryFile(expected, location => _writer);

            Assert.AreEqual(expected, actual);
        }

        private static ILogWriter<string> MockWriter(List<string> actual)
        {
            var writer = new Mock<ILogWriter<string>>();
            writer.Setup(w => w.WriteAll(It.IsAny<IEnumerable<string>>())).Callback<IEnumerable<string>>(actual.AddRange);
            return writer.Object;
        }

        [Test]
        public void FileExportContainsGivenList()
        {
            const string content = "some text to test the file-copy";
            var sourceLocation = Path.GetTempFileName();
            using (var writer = new StreamWriter(new FileStream(sourceLocation, FileMode.Create, FileAccess.Write)))
            {
                writer.Write(content);
            }
            var targetLocation = Path.GetTempFileName();
            var uut = new FilePublisher(() => targetLocation);
            uut.Publish(sourceLocation);
            string actual;
            using (var reader = new StreamReader(new FileStream(targetLocation, FileMode.Open)))
            {
                actual = reader.ReadToEnd();
            }

            Assert.AreEqual(content, actual);
        }

        [Test]
        public void FileExportInterruptsWithInvalidFileName()
        {
            var uut = new FilePublisher(() => null);
            var response = uut.Publish(Path.GetTempFileName());
            Assert.AreEqual(State.Fail, response.Status);
        }

        private static IList<string> RandomlyFilledList(int count)
        {
            var list = new List<string>();
            for (var i = 0; i < count; i++)
            {
                list.Add(RandomString());
            }
            return list;
        }

        private static string RandomString()
        {
            return new Random().Next().ToString(CultureInfo.InvariantCulture);
        }
    }
}