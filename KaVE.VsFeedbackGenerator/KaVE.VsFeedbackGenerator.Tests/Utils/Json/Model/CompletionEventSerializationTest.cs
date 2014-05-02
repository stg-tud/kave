using System;
using System.Collections.Generic;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.Model.Names.CSharp;
using KaVE.Model.Names.VisualStudio;
using KaVE.VsFeedbackGenerator.Tests.TestFactories;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.Model
{
    [TestFixture]
    internal class CompletionEventSerializationTest
    {
        [Test]
        public void ShouldSerializeCompletionEvent()
        {
            var proposal1 = new Proposal
            {
                Name = TestNameFactory.GetAnonymousMethodName(),
                Relevance = 42
            };
            var proposal2 = new Proposal
            {
                Name = TestNameFactory.GetAnonymousNamespace(),
                Relevance = -23
            };

            var completionEvent = new CompletionEvent
            {
                IDESessionUUID = "0xDEADBEEF",
                TriggeredBy = IDEEvent.Trigger.Unknown,
                Prefix = "Foo",
                Context = new Context
                {
                    EnclosingMethod = TestNameFactory.GetAnonymousMethodName()
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
                                     "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanRead()\"}}," +
                                     "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanSeek()\"}}," +
                                     "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanTimeout()\"}}," +
                                     "{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanWrite()\"}}," +
                                     /* other proposals removed to reduce size of this string */
                                     "]}}," +
                                     "\"Selections\":{\"$type\":\"System.Collections.Generic.List`1[[KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model]], mscorlib\"," +
                                     "\"$values\":[{\"$type\":\"KaVE.Model.Events.CompletionEvent.ProposalSelection, KaVE.Model\",\"SelectedAt\":\"2013-12-06T11:34:24\"," +
                                     "\"Proposal\":{\"$type\":\"KaVE.Model.Events.CompletionEvent.Proposal, KaVE.Model\",\"Name\":{\"type\":\"CSharp.PropertyName\",\"identifier\":\"get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanRead()\"}}}" +
                                     "]}," +
                                     "\"Prefix\":\"\"," +
                                     "\"TerminatedBy\":1," +
                                     "\"TerminatedAs\":1," +
                                     "\"IDESessionUUID\":\"57d18e20-952f-4583-88b3-3aadc1db48b1\"," +
                                     "\"TriggeredAt\":\"2013-12-06T11:34:24\"," +
                                     "\"TriggeredBy\":0," +
                                     "\"TerminatedAt\":\"2013-12-06T11:34:26\"," +
                                     "\"ActiveWindow\":{\"type\":\"VisualStudio.WindowName\",\"identifier\":\"vsWindowTypeDocument Initializer.cs\"}," +
                                     "\"ActiveDocument\":{\"type\":\"VisualStudio.DocumentName\",\"identifier\":\"\\\\CodeCompletion.FeedbackGenerator\\\\KaVE.VsFeedbackGenerator\\\\KaVE.VsFeedbackGenerator\\\\Initializer.cs\"}}";

            var actual = jsonEvent.ParseJsonTo<CompletionEvent>();

            var initialySelectedProposal = CreatePropertyProposal(
                "get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanRead()");
            var expected = new CompletionEvent
            {
                ProposalCollection =
                    new ProposalCollection(
                        initialySelectedProposal,
                        CreatePropertyProposal(
                            "get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanSeek()"),
                        CreatePropertyProposal(
                            "get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanTimeout()"),
                        CreatePropertyProposal(
                            "get [System.Boolean, mscorlib, 4.0.0.0] [System.IO.Stream, mscorlib, 4.0.0.0].CanWrite()")),
                Selections =
                    new List<ProposalSelection>
                    {
                        new ProposalSelection(initialySelectedProposal)
                        {
                            SelectedAt = new DateTime(2013, 12, 6, 11, 34, 24)
                        }
                    },
                Prefix = "",
                TerminatedBy = IDEEvent.Trigger.Click,
                TerminatedAs = CompletionEvent.TerminationState.Cancelled,
                IDESessionUUID = "57d18e20-952f-4583-88b3-3aadc1db48b1",
                TriggeredAt = new DateTime(2013, 12, 6, 11, 34, 24),
                TerminatedAt = new DateTime(2013, 12, 6, 11, 34, 26),
                TriggeredBy = IDEEvent.Trigger.Unknown,
                ActiveWindow = WindowName.Get("vsWindowTypeDocument Initializer.cs"),
                ActiveDocument =
                    DocumentName.Get(
                        "\\CodeCompletion.FeedbackGenerator\\KaVE.VsFeedbackGenerator\\KaVE.VsFeedbackGenerator\\Initializer.cs"),
            };

            Assert.AreEqual(expected, actual);
        }

        private Proposal CreatePropertyProposal(string identifier)
        {
            return new Proposal {Name = PropertyName.Get(identifier)};
        }
    }
}