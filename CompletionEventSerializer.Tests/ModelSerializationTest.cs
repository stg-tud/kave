using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CodeCompletion.Model.CompletionEvent;
using CodeCompletion.Model.Context;
using CodeCompletion.Model.Names;
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
            var proposal1 = new Proposal
            {
                Name = MethodName.Get("[Declarator, Assmbly, Version=1.2.3.4] [ReturnType, Ass, Version=9.8.7.6].Method()"),
                Relevance = 42
            };
            var proposal2 = new Proposal
            {
                Name = NamespaceName.Get("Foo.Bar"),
                Relevance = -23
            };

            var completionEvent = new CompletionEvent
            {
                IDESessionUUID = "0xDEADBEEF",
                CompletionTimeStamp = new DateTime(),
                Context = new Context
                {
                    EnclosingMethod = MethodName.Get("[Enclosing, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()"),
                    EnclosingMethodFirst = new HashSet<IMethodName>
                    {
                        MethodName.Get("[IFace1, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()"),
                        MethodName.Get("[IFace2, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()")
                    }
                },
                ProposalCollection = new ProposalCollection(new List<Proposal> {proposal1, proposal2}),
                Actions = new Dictionary<DateTime, ProposalAction>
                {
                    { new DateTime(2012, 4, 12, 12, 23, 42), new ProposalAction(proposal1, ProposalAction.SubActionKind.Select)},
                    { new DateTime(2012, 4, 12, 12, 23, 43), new ProposalAction(proposal2, ProposalAction.SubActionKind.Select)},
                    { new DateTime(2012, 4, 12, 12, 23, 45), new ProposalAction(proposal1, ProposalAction.SubActionKind.Apply)},
                }
            };

            var eventCopy = SerializeAndDeserialize(completionEvent);
            Assert.AreEqual(completionEvent.IDESessionUUID, eventCopy.IDESessionUUID);
            Assert.AreEqual(completionEvent.CompletionTimeStamp, eventCopy.CompletionTimeStamp);
            Assert.AreEqual(completionEvent.Context, eventCopy.Context);
            Assert.AreEqual(completionEvent.ProposalCollection, eventCopy.ProposalCollection);
            Assert.AreEqual(completionEvent.Actions, eventCopy.Actions);
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
            Serialize(FieldName.Get("static [Declarator, B, Version=9.2.3.8] [Val, G, Version=5.4.6.3].Field"), Assert.AreSame);
        }

        [Test]
        public void ShouldSerializeMethodName()
        {
            Serialize(MethodName.Get("[Declarator, B, Version=9.2.3.8] [Val, G, Version=5.4.6.3].Method(out [Param, P, Version=8.1.7.2])"), Assert.AreSame);
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
            var modelCopy = SerializeAndDeserialize(model);
            assert.Invoke(model, modelCopy);
        }

        private TModel SerializeAndDeserialize<TModel>(TModel model)
        {
            var serialization = Serialize(model);
            var modelCopy = Deserialize<TModel>(serialization);
            return modelCopy;
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
