using System.IO;
using KAVE.KAVE_MessageBus.Json;
using NUnit.Framework;

namespace KAVE.MessageBus.Tests.Json
{
    [TestFixture]
    public class JsonLogWriterTest
    {
        private JsonLogWriter _writer;
        private MemoryStream _logStream;

        [SetUp]
        public void SetUpWriter()
        {
            _logStream = new MemoryStream();
            _writer = new JsonLogWriter(_logStream);
        }

        [Test]
        public void ShouldSerializeInstance()
        {
            var instance = new SerializationTestTarget { Id = "lalalaloooo" };

            _writer.Write(instance);
            
            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"Id\":\"lalalaloooo\"}\r\n", serialization);
        }

        [Test]
        public void ShouldSerializeMultipleInstances()
        {
            var instance1 = new SerializationTestTarget { Id = "foo" };
            var instance2 = new SerializationTestTarget { Id = "bar" };

            _writer.Write(instance1);
            _writer.Write(instance2);

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"Id\":\"foo\"}\r\n{\"Id\":\"bar\"}\r\n", serialization);
        }
    }
}
