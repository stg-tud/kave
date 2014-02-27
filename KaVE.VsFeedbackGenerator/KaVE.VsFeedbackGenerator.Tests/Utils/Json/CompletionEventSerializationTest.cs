using System;
using System.Collections.Generic;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    internal class CompletionEventSerializationTest
    {
        [Test]
        public void ShouldSerializeCompletionEvent()
        {
            var proposal1 = new Proposal
            {
                Name =
                    MethodName.Get("[Declarator, Assmbly, Version=1.2.3.4] [ReturnType, Ass, Version=9.8.7.6].Method()"),
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
                    EnclosingMethod =
                        MethodName.Get(
                            "[Enclosing, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()"),
                    EnclosingMethodFirst =
                        MethodName.Get(
                            "[IFace1, Bssmbly, Version=4.2.3.1] [System.Void, mscore, Version=4.0.0.0].EncMeth()")
                },
                ProposalCollection = new ProposalCollection(new List<Proposal> {proposal1, proposal2}),
                TerminatedAt = new DateTime(2012, 2, 23, 18, 54, 59, 549),
                TerminatedBy = IDEEvent.Trigger.Typing,
                TerminatedAs = CompletionEvent.TerminationState.Applied
            };
            completionEvent.AddSelection(proposal1);
            completionEvent.AddSelection(proposal2);
            completionEvent.AddSelection(proposal1);

            JsonAssert.SerializationPreservesData(completionEvent);
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

            var @event = jsonEvent.Deserialize<CompletionEvent>();

            var completionEvent = @event;
            Assert.NotNull(completionEvent);
            Assert.AreEqual("57d18e20-952f-4583-88b3-3aadc1db48b1", completionEvent.IDESessionUUID);
            Assert.AreEqual("vsWindowTypeDocument Initializer.cs", completionEvent.ActiveWindow.Identifier);
            Assert.AreEqual(
                "{8E7B96A8-E33D-11D0-A6D5-00C04FB67F6A} CSharp C:\\Users\\Sven\\Documents\\KAVE\\CodeCompletion.FeedbackGenerator\\KaVE.VsFeedbackGenerator\\KaVE.VsFeedbackGenerator\\Initializer.cs",
                completionEvent.ActiveDocument.Identifier);
            Assert.AreEqual(new DateTime(635219264646697176, DateTimeKind.Local), completionEvent.TriggeredAt);
            Assert.AreEqual(IDEEvent.Trigger.Unknown, completionEvent.TriggeredBy);
            Assert.AreEqual(String.Empty, completionEvent.Prefix);
            Assert.AreEqual(6, completionEvent.ProposalCollection.Proposals.Count);
            Assert.AreEqual(1, completionEvent.Selections.Count);
            Assert.AreEqual(IDEEvent.Trigger.Click, completionEvent.TerminatedBy);
            Assert.AreEqual(CompletionEvent.TerminationState.Cancelled, completionEvent.TerminatedAs);
        }
    }
}