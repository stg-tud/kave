using System;
using System.Collections.Generic;
using System.IO;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;
using AssemblyName = KaVE.Model.Names.CSharp.AssemblyName;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
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
                
                Prefix = "Foo",
                Context = new Context
                {
                    EnclosingMethod = MethodName.Get("[Enclosing, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()"),
                    EnclosingMethodFirst = MethodName.Get("[IFace1, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()")
                },
                ProposalCollection = new ProposalCollection(new List<Proposal> {proposal1, proposal2}),
                TerminatedAt = new DateTime(2012, 2, 23, 18, 54, 59, 549),
                TerminatedBy = IDEEvent.Trigger.Typing,
                TerminatedAs = CompletionEvent.TerminationState.Applied
            };
            completionEvent.AddSelection(proposal1);
            completionEvent.AddSelection(proposal2);
            completionEvent.AddSelection(proposal1);

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
        public void ShouldDeserializeCompletionEvent()
        {
            const string jsonEvent = "{\"$type\":\"KaVE.Model.Events.CompletionEvent.CompletionEvent, KaVE.Model\"," +
                "\"ProposalCollection\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalCollection, KaVE.Model\"," +
                    "\"Proposals\":{\"$type\":\"System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model]], mscorlib\"," +
                        "\"$values\":[" +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.MethodName\",\"identifier\":\"[System.IAsyncResult, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].BeginRead([System.Byte[], mscorlib, Version=4.0.0.0] buffer, [System.Int32, mscorlib, Version=4.0.0.0] offset, [System.Int32, mscorlib, Version=4.0.0.0] count, [System.AsyncCallback, mscorlib, Version=4.0.0.0] callback, [System.Object, mscorlib, Version=4.0.0.0] state)\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.MethodName\",\"identifier\":\"[System.IAsyncResult, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].BeginWrite([System.Byte[], mscorlib, Version=4.0.0.0] buffer, [System.Int32, mscorlib, Version=4.0.0.0] offset, [System.Int32, mscorlib, Version=4.0.0.0] count, [System.AsyncCallback, mscorlib, Version=4.0.0.0] callback, [System.Object, mscorlib, Version=4.0.0.0] state)\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].CanRead()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].CanSeek()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].CanTimeout()\"}}," +
                            "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].CanWrite()\"}}," +
                            /* other proposals removed to reduce size of this string */
                        "]}}," +
                "\"Selections\":{\"$type\":\"System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model]], mscorlib\"," +
                    "\"$values\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"SelectedAt\":\"2013-12-06T11:34:24.8101178+01:00\"," +
                        "\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.MethodName\",\"identifier\":\"[System.IAsyncResult, mscorlib, Version=4.0.0.0] [System.IO.Stream, mscorlib, Version=4.0.0.0].BeginRead([System.Byte[], mscorlib, Version=4.0.0.0] buffer, [System.Int32, mscorlib, Version=4.0.0.0] offset, [System.Int32, mscorlib, Version=4.0.0.0] count, [System.AsyncCallback, mscorlib, Version=4.0.0.0] callback, [System.Object, mscorlib, Version=4.0.0.0] state)\"}}}" +
                    "]}," +
                "\"Prefix\":\"\"," +
                "\"TerminatedBy\":1," +
                "\"TerminatedAs\":1," +
                "\"IDESessionUUID\":\"57d18e20-952f-4583-88b3-3aadc1db48b1\"," +
                "\"TriggeredAt\":\"2013-12-06T11:34:24.6697176+01:00\"," +
                "\"TriggeredBy\":0,\"TerminatedAt\":\"2013-12-06T11:34:26.2765204+01:00\"," +
                "\"ActiveWindow\":{\"type\":\"VisualStudio.WindowName\",\"identifier\":\"vsWindowTypeDocument Initializer.cs\"}," +
                "\"ActiveDocument\":{\"type\":\"VisualStudio.DocumentName\",\"identifier\":\"{8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A} CSharp C:\\\\Users\\\\Sven\\\\Documents\\\\KAVE\\\\CodeCompletion.FeedbackGenerator\\\\KaVE.VsFeedbackGenerator\\\\KaVE.VsFeedbackGenerator\\\\Initializer.cs\"}}";

            var @event = Deserialize<IDEEvent>(jsonEvent);

            var completionEvent = @event as CompletionEvent;
            Assert.NotNull(completionEvent);
            Assert.AreEqual("57d18e20-952f-4583-88b3-3aadc1db48b1", completionEvent.IDESessionUUID);
            Assert.AreEqual("vsWindowTypeDocument Initializer.cs", completionEvent.ActiveWindow.Identifier);
            Assert.AreEqual("{8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A} CSharp C:\\Users\\Sven\\Documents\\KAVE\\CodeCompletion.FeedbackGenerator\\KaVE.VsFeedbackGenerator\\KaVE.VsFeedbackGenerator\\Initializer.cs", completionEvent.ActiveDocument.Identifier);
            Assert.AreEqual(new DateTime(635219264646697176, DateTimeKind.Local), completionEvent.TriggeredAt);
            Assert.AreEqual(IDEEvent.Trigger.Unknown, completionEvent.TriggeredBy);
            Assert.AreEqual(String.Empty, completionEvent.Prefix);
            Assert.AreEqual(6, completionEvent.ProposalCollection.Proposals.Count);
            Assert.AreEqual(1, completionEvent.Selections.Count);
            Assert.AreEqual(IDEEvent.Trigger.Click, completionEvent.TerminatedBy);
            Assert.AreEqual(CompletionEvent.TerminationState.Cancelled, completionEvent.TerminatedAs);
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
