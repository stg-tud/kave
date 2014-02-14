using System;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.CompletionTraceGenerator
{
    public class CompletionEventToTraceConverter
    {
        private readonly ILogWriter<CompletionTrace> _writer;

        public CompletionEventToTraceConverter(ILogWriter<CompletionTrace> writer)
        {
            _writer = writer;
        }

        public void Process(CompletionEvent completionEvent)
        {
            var trace = new CompletionTrace {DurationInMillis = ComputeDuration(completionEvent)};

            if (completionEvent.Selections.Count > 1)
            {
                var initial = completionEvent.Selections[0];
                var oldPos = completionEvent.ProposalCollection.Proposals.IndexOf(initial.Proposal);

                foreach (var selection in completionEvent.Selections)
                {
                    var pos = completionEvent.ProposalCollection.Proposals.IndexOf(selection.Proposal);

                    if (pos > oldPos)
                    {
                        trace.AppendAction(CompletionAction.NewStep(Direction.Down));
                    }

                    if (pos < oldPos)
                    {
                        trace.AppendAction(CompletionAction.NewStep(Direction.Up));
                    }

                    oldPos = pos;
                }
            }

            switch (completionEvent.TerminatedAs)
            {
                case CompletionEvent.TerminationState.Applied:
                    trace.AppendAction(CompletionAction.NewApply());
                    break;
                case CompletionEvent.TerminationState.Cancelled:
                    trace.AppendAction(CompletionAction.NewCancel());
                    break;
                default:
                    throw new NotImplementedException();
            }
            _writer.Write(trace);
        }

        private static int ComputeDuration(CompletionEvent completionEvent)
        {
            return (completionEvent.TerminatedAt - completionEvent.TriggeredAt).GetValueOrDefault(TimeSpan.FromSeconds(0))
                .Milliseconds;
        }
    }
}