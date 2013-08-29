using System;
using System.IO;
using System.Text;
using CodeCompletion.Model.CompletionEvent;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    public class JsonSerializerTest
    {
        private JsonSerializer _jsonSerializer;

        [SetUp]
        public void CreateSerializer()
        {
            _jsonSerializer = new JsonSerializer();
        }

        [Test]
        public void ShouldSerializeInstance()
        {
            var instance = new SerializationTestTarget {Id = "lalalaloooo"};

            using (var stream = new MemoryStream())
            {
                _jsonSerializer.AppendTo(stream, instance);

                var serialization = stream.AsString();
                Assert.AreEqual("{\"Id\":\"lalalaloooo\"}\r\n", serialization);
            }
        }

        [Test]
        public void ShouldDeserializeInstance()
        {
            var input = Encoding.Default.GetBytes("{\"Id\":\"0xDEADBEEF\"}");

            using (var stream = new MemoryStream(input))
            {
                var instance = _jsonSerializer.Read<SerializationTestTarget>(stream);

                Assert.AreEqual("0xDEADBEEF", instance.Id);
            }
        }

        [Test]
        public void ShouldSerializeMultipleInstances()
        {
            var instance1 = new SerializationTestTarget {Id = "foo"};
            var instance2 = new SerializationTestTarget {Id = "bar"};

            using (var stream = new MemoryStream())
            {
                _jsonSerializer.AppendTo(stream, instance1);
                _jsonSerializer.AppendTo(stream, instance2);

                var serialization = stream.AsString();
                Assert.AreEqual("{\"Id\":\"foo\"}\r\n{\"Id\":\"bar\"}\r\n", serialization);
            }
        }

        [Test]
        public void ShouldDeserializeMultipleInstancesOneAtATime()
        {
            var input = Encoding.Default.GetBytes("{\"Id\":\"A\"}\r\n{\"Id\":\"B\"}\r\n");

            using (var stream = new MemoryStream(input))
            {
                var instance1 = _jsonSerializer.Read<SerializationTestTarget>(stream);
                var instance2 = _jsonSerializer.Read<SerializationTestTarget>(stream);

                Assert.AreEqual("A", instance1.Id);
                Assert.AreEqual("B", instance2.Id);
            }
        }

        [Test]
        public void ShouldReadNullFromEmptyStream()
        {
            using (var stream = new MemoryStream())
            {
                var instance = _jsonSerializer.Read<SerializationTestTarget>(stream);

                Assert.IsNull(instance);
            }
        }

        [Test]
        public void ShouldIgnoreLeadingWhitespaces()
        {
            var input = Encoding.Default.GetBytes("      {\"Id\":\"Later\"}");

            using (var stream = new MemoryStream(input))
            {
                var instance = _jsonSerializer.Read<SerializationTestTarget>(stream);

                Assert.AreEqual("Later", instance.Id);
            }
        }

        [Test]
        public void ShouldIgnoreTrailingWhitespaces()
        {
            var input = Encoding.Default.GetBytes("{\"Id\":\"Later\"}   \r\n       ");

            using (var stream = new MemoryStream(input))
            {
                _jsonSerializer.Read<SerializationTestTarget>(stream);
                var instance = _jsonSerializer.Read<SerializationTestTarget>(stream);

                Assert.IsNull(instance);
            }
        }
    }

    static class TestExtensions
    {
        public static string AsString(this MemoryStream stream)
        {
            return Encoding.Default.GetString(stream.ToArray());
        }
    }
}
