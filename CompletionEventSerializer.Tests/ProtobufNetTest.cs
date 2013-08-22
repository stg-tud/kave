using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using ProtoBuf;
using System.Xml;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    class ProtobufNetTest
    {
        class Person 
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }



        private const string Data = "Foobar";
        private const PrefixStyle Style = PrefixStyle.Fixed32;

        [Test]
        public void ShouldSerializeMultipleObjectsIntoOneFile()
        {
            string readData;
            using (var stream = new MemoryStream())
            {
                Serializer.SerializeWithLengthPrefix(stream, Data, Style);
                Serializer.SerializeWithLengthPrefix(stream, Data, Style);

                stream.Position = 0;

                readData = Serializer.DeserializeWithLengthPrefix<String>(stream, Style);
            }
            Assert.AreEqual(Data, readData);
        }

        [Test]
        public void ShouldSerializeToXML()
        {
            using (var stream = new MemoryStream())
            {
                using (XmlWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
                {
                    Serializer.Serialize(writer, new Person { Name = "Bla", Age = 10});
                }    
            }

            
        }
    }
}
