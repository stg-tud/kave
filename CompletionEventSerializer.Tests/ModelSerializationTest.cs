using System.IO;
using System.Text;
using CodeCompletion.Model.Names.CSharp;
using NUnit.Framework;

namespace CompletionEventSerializer.Tests
{
    [TestFixture]
    public class ModelSerializationTest
    {
        private JsonSerializer _serializer;

        [SetUp]
        public void SetUpSerializer()
        {
            _serializer = new JsonSerializer();
        }

        [Test]
        public void ShouldSerializeCompletionEvent()
        {
            Assert.Fail();
        }

        [Test]
        public void ShouldSerializeProposal()
        {
            
        }

        [Test]
        public void ShouldSerializeProposalAction()
        {
            
        }

        [Test]
        public void ShouldSerializeProposalCollection()
        {
            
        }

        [Test]
        public void ShouldSerializeContext()
        {
            
        }

        [Test]
        public void ShouldSerializeTyperHiarchy()
        {
            
        }

        [Test]
        public void ShouldSerializeAssemblyName()
        {
            Serialize(AssemblyName.Get("AssemblyName, Version=0.8.15.0"), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeAssemblyVersion()
        {
            Serialize(AssemblyVersion.Get("1.3.3.7"), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeFieldName()
        {
            Serialize(FieldName.Get(""), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeMethodName()
        {
            Serialize(MethodName.Get(""), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeNamespaceName()
        {
            Serialize(NamespaceName.Get("This.Is.My.Namespace"), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeTypeName()
        {
            Serialize(TypeName.Get("Foo.Bar, foo, Version=1.0.0.0"), Assert.AreSame);
        }

        private delegate void Assertion(object expected, object actual);

        private void Serialize<TModel>(TModel model, Assertion assert)
        {
            var serialization = Serialize(model);
            var modelCopy = Deserialize<TModel>(serialization);
            assert.Invoke(model, modelCopy);
        }

        private string Serialize<TModel>(TModel model)
        {
            using (var stream = new MemoryStream())
            {
                _serializer.AppendTo(stream, model);
                return stream.AsString();
            }
        }

        private TModel Deserialize<TModel>(string serialization)
        {
            var input = Encoding.Default.GetBytes(serialization);
            using (var stream = new MemoryStream(input))
            {
                return _serializer.Read<TModel>(stream);
            }
        }
    }
}
