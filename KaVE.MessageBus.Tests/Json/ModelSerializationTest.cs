using System.Collections.Generic;
using System.IO;
using KaVE.MessageBus.Json;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.MessageBus.Tests.Json
{
    [TestFixture]
    public class ModelSerializationTest
    {
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
                TriggeredBy = IDEEvent.Trigger.Unknown,
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
                TerminatedBy = CompletionEvent.TerminationAction.Apply
            };
            completionEvent.AddSelection(new ProposalSelection(proposal1, "Foo."));
            completionEvent.AddSelection(new ProposalSelection(proposal2, "Foo."));
            completionEvent.AddSelection(new ProposalSelection(proposal1, "Foo."));

            var eventCopy = SerializeAndDeserialize(completionEvent);
            Assert.AreEqual(completionEvent.IDESessionUUID, eventCopy.IDESessionUUID);
            Assert.AreEqual(completionEvent.TriggeredAt, eventCopy.TriggeredAt);
            Assert.AreEqual(completionEvent.TriggeredBy, eventCopy.TriggeredBy);
            Assert.AreEqual(completionEvent.Context, eventCopy.Context);
            Assert.AreEqual(completionEvent.ProposalCollection, eventCopy.ProposalCollection);
            CollectionAssert.AreEqual(completionEvent.Selections, eventCopy.Selections);
            Assert.AreEqual(completionEvent.TerminatedBy, eventCopy.TerminatedBy);
        }

        [Test]
        public void ShouldSerializeName()
        {
            Serialize(Name.Get("Foobar! That's my Name."), Assert.AreSame);
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
                var writer = new JsonLogWriter(stream);
                writer.Write(model);
                return stream.AsString();
            }
        }

        private TModel Deserialize<TModel>(string serialization)
        {
            using (var stream = new MemoryStream(serialization.AsBytes()))
            {
                var reader = new JsonLogReader(stream);
                return reader.Read<TModel>();
            }
        }
    }
}
