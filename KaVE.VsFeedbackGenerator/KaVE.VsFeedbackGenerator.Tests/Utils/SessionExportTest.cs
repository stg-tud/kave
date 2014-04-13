using System;
using System.Collections.Generic;
using System.Globalization;
using KaVE.VsFeedbackGenerator.Utils;
using Moq;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils
{
    [TestFixture]
    internal class SessionExportTest
    {
        [Test]
        public void ShouldInvokePublisherWithTheCorrectFile()
        {
            var mock = new Mock<ISessionPublisher>();
            string expected = null;
            string actual = null;
            mock.Setup(m => m.Publish(It.IsAny<string>()))
                .Callback<string>(loc => { actual = loc; });

            var uut = new SessionExport(mock.Object);
            uut.Export(
                RandomlyFilledList(5),
                loc =>
                {
                    expected = loc;
                    return MockWriter(new List<string>());
                });

            Assert.IsNotNull(expected);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldInvokeWriterWithTheGivenList()
        {
            var uut = new SessionExport(new Mock<ISessionPublisher>().Object);
            var expected = RandomlyFilledList(5);

            var actual = new List<string>();
            var writer = MockWriter(actual);

            uut.ExportToTemporaryFile(expected, location => writer);

            Assert.AreEqual(expected, actual);
        }

        private static ILogWriter<string> MockWriter(List<string> actual)
        {
            var writer = new Mock<ILogWriter<string>>();
            writer.Setup(w => w.WriteAll(It.IsAny<IEnumerable<string>>()))
                  .Callback<IEnumerable<string>>(actual.AddRange);
            return writer.Object;
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