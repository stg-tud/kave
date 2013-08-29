using CodeCompletion.Model.Names.CSharp;
using NUnit.Framework;
using Newtonsoft.Json;
using ProtoBuf;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    public class ModelSerializationTest
    {
        [Test]
        public void ShouldSerializeName()
        {
            TestSerialization(TypeName.Get("Foo.Bar", AssemblyName.Get("foo", "1.0.0.0")));
        }

        private static void TestSerialization(Name name)
        {
            var serializeObject = JsonConvert.SerializeObject(name, new TypeNameJsonConverter());
        }
    }
}
