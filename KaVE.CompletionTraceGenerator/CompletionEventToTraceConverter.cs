using System;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils;

namespace KaVE.CompletionTraceGenerator
{
    public class CompletionEventToTraceConverter
    {
        private readonly ILogWriter<CompletionTrace> _writer;
        private CompletionTrace _trace;

        public CompletionEventToTraceConverter(ILogWriter<CompletionTrace> writer)
        {
            _writer = writer;
        }

        public void Process(CompletionEvent completionEvent)
        {
            if (_trace == null)
            {
                _trace = new CompletionTrace {DurationInMillis = ComputeDuration(completionEvent)};
            }
            else // event was triggered by a filtering
            {
                _trace.DurationInMillis += ComputeDuration(completionEvent);
                _trace.AppendAction(CompletionAction.NewFilter(completionEvent.Prefix));
            }

            if (completionEvent.Selections.Count > 1)
            {
                var initial = completionEvent.Selections[0];
                var oldPos = completionEvent.ProposalCollection.Proposals.IndexOf(initial.Proposal);

                foreach (var selection in completionEvent.Selections)
                {
                    var newPos = completionEvent.ProposalCollection.Proposals.IndexOf(selection.Proposal);
                    var stepSize = Math.Abs(oldPos - newPos);

                    if (stepSize == 1)
                    {
                        if (newPos > oldPos)
                        {
                            _trace.AppendAction(CompletionAction.NewStep(Direction.Down));
                        }
                        else
                        {
                            _trace.AppendAction(CompletionAction.NewStep(Direction.Up));
                        }
                    }
                    else if (stepSize > 1)
                    {
                        _trace.AppendAction(CompletionAction.NewMouseGoto(newPos));
                    }

                    oldPos = newPos;
                }
            }

            switch (completionEvent.TerminatedAs)
            {
                case CompletionEvent.TerminationState.Applied:
                    _trace.AppendAction(CompletionAction.NewApply());
                    _writer.Write(_trace);
                    break;
                case CompletionEvent.TerminationState.Cancelled:
                    _trace.AppendAction(CompletionAction.NewCancel());
                    _writer.Write(_trace);
                    break;
                case CompletionEvent.TerminationState.Filtered:
                    // filter action is added only when the next event is processed, because only then we know the new prefix
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static int ComputeDuration(CompletionEvent completionEvent)
        {
            return
                (completionEvent.TerminatedAt - completionEvent.TriggeredAt).GetValueOrDefault(TimeSpan.FromSeconds(0))
                    .Milliseconds;
        }
    }
}