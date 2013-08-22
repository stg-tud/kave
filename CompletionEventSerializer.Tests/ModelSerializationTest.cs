using CodeCompletion.Model.Names.CSharp;
using NUnit.Framework;
using ProtoBuf;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    public class ModelSerializationTest
    {
        [Test]
        public void ShouldSerializeName()
        {
            TestSerialization(new Name("SomeName"));
        }

        private static void TestSerialization(Name name)
        {
            var deepClone = Serializer.DeepClone(name);
            Assert.AreEqual(name, deepClone);
        }
    }
}
