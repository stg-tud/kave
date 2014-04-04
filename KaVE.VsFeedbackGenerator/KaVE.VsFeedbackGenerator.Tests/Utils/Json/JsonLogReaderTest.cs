using System.IO;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    public class JsonLogReaderTest
    {
        [Test]
        public void ShouldDeserializeInstance()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("{\"Id\":\"0xDEADBEEF\"}"))
            {
                instance = reader.ReadNext();
            }

            Assert.AreEqual("0xDEADBEEF", instance.Id);
        }

        [Test]
        public void ShouldDeserializeMultipleInstancesOneAtATime()
        {
            SerializationTestTarget instance1;
            SerializationTestTarget instance2;
            using (var reader = CreateReader("{\"Id\":\"A\"}\r\n{\"Id\":\"B\"}\r\n"))
            {
                instance1 = reader.ReadNext();
                instance2 = reader.ReadNext();
            }

            Assert.AreEqual("A", instance1.Id);
            Assert.AreEqual("B", instance2.Id);
        }

        [Test]
        public void ShouldReadNullFromEmptyStream()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader(""))
            {
                instance = reader.ReadNext();
            }

            Assert.IsNull(instance);
        }

        [Test]
        public void ShouldIgnoreLeadingWhitespaces()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("      {\"Id\":\"Later\"}"))
            {
                instance = reader.ReadNext();
            }

            Assert.AreEqual("Later", instance.Id);
        }

        [Test]
        public void ShouldIgnoreTrailingWhitespaces()
        {
            SerializationTestTarget instance;
            using (var reader = CreateReader("{\"Id\":\"Later\"}   \r\n       "))
            {
                reader.ReadNext();
                instance = reader.ReadNext();
            }

            Assert.IsNull(instance);
        }

        [Test]
        public void ShouldEnumerateLogContent()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\r\n{\"Id\":\"1\"}\r\n{\"Id\":\"2\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual("0", instances[0].Id);
            Assert.AreEqual("1", instances[1].Id);
            Assert.AreEqual("2", instances[2].Id);
        }

        [Test]
        public void ShouldSkipOverBrokeLines()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\r\nI'am a broken line\r\n{\"Id\":\"2\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual("0", instances[0].Id);
            Assert.AreEqual("2", instances[1].Id);
        }

        [Test]
        public void ShouldReadNothingFromBrokenFile()
        {
            using (var reader = CreateReader("Broken Content\r\nMore broken stuff\r\nAll broken"))
            {
                Assert.IsNull(reader.ReadNext());
            }
        }

        [Test]
        public void ShouldAllowUnixLikeLinebreaks()
        {
            SerializationTestTarget[] instances;
            using (var reader = CreateReader("{\"Id\":\"0\"}\n{\"Id\":\"1\"}"))
            {
                instances = reader.ReadAll().ToArray();
            }

            Assert.AreEqual(2, instances.Length);
        }

        [NotNull]
        private static ILogReader<SerializationTestTarget> CreateReader(string input)
        {
            return new JsonLogReader<SerializationTestTarget>(new MemoryStream(input.AsBytes()));
        }
    }
}