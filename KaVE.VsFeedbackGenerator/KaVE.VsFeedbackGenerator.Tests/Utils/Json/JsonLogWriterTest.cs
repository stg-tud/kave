using System.IO;
using KaVE.VsFeedbackGenerator.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    public class JsonLogWriterTest
    {
        private ILogWriter<object> _writer;
        private MemoryStream _logStream;

        [SetUp]
        public void SetUpWriter()
        {
            _logStream = new MemoryStream();
            _writer = new JsonLogWriter<object>(_logStream);
        }

        [TearDown]
        public void CleanUp()
        {
            _writer.Dispose();
        }

        [Test]
        public void ShouldSerializeInstance()
        {
            var instance = new SerializationTestTarget { Id = "lalalaloooo" };

            _writer.Write(instance);
            
            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"lalalaloooo\"}\r\n", serialization);
        }

        [Test]
        public void ShouldSerializeMultipleInstances()
        {
            var instance1 = new SerializationTestTarget { Id = "foo" };
            var instance2 = new SerializationTestTarget { Id = "bar" };

            _writer.Write(instance1);
            _writer.Write(instance2);

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"foo\"}\r\n{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"bar\"}\r\n", serialization);
        }

        [Test]
        public void ShouldSerializeMultipleInstancesIfProvidedAtOnce()
        {
            var instance1 = new SerializationTestTarget { Id = "foo" };
            var instance2 = new SerializationTestTarget { Id = "bar" };

            _writer.WriteAll(new[] { instance1, instance2 });

            var serialization = _logStream.AsString();
            Assert.AreEqual("{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"foo\"}\r\n{\"$type\":\"KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests\",\"Id\":\"bar\"}\r\n", serialization);
        }
    }
}
