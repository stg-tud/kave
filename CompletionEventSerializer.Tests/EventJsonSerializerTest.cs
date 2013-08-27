using System;
using System.IO;
using CodeCompletion.Model.CompletionEvent;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    public class EventJsonSerializerTest
    {
        private EventJsonSerializer _eventJsonSerializer;

        [SetUp]
        public void CreateSerializer()
        {
            _eventJsonSerializer = new EventJsonSerializer();
        }

        [Test]
        public void ShouldSerializeToJson()
        {
            var completionEvent = new CompletionEvent
                {
                    IDESessionUUID = "lalalaloooo",
                    CompletionTimeStamp = DateTime.Now
                };

            using (var stream = new MemoryStream())
            {
                _eventJsonSerializer.AppendTo(stream, completionEvent);

                stream.Position = 0;
                var reader = new StreamReader(stream);
                var serialization = reader.ReadToEnd();
                Console.Out.WriteLine(serialization);
            }
        }

        [Test]
        public void ShouldSerializeAndDeserialize()
        {
            var completionEvent = new CompletionEvent
            {
                IDESessionUUID = "lalalaloooo",
                CompletionTimeStamp = DateTime.Now
            };

            using (var stream = new MemoryStream())
            {
                _eventJsonSerializer.AppendTo(stream, completionEvent);
                stream.Position = 0;
                var deserializedCompletionEvent = _eventJsonSerializer.ReadFrom(stream);
                Assert.AreEqual(completionEvent, deserializedCompletionEvent);
            }
        }
    }
}
