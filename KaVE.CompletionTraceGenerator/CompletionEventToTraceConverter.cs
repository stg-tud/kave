/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using KaVE.CompletionTraceGenerator.Model;
using KaVE.JetBrains.Annotations;
using KaVE.Model.Events;
using KaVE.Model.Events.CompletionEvent;
using KaVE.VsFeedbackGenerator.Utils.Logging;

namespace KaVE.CompletionTraceGenerator
{
    public class CompletionEventToTraceConverter : IDisposable
    {
        private readonly ILogWriter<CompletionTrace> _writer;
        private CompletionTrace _trace;

        public CompletionEventToTraceConverter(ILogWriter<CompletionTrace> writer)
        {
            _writer = writer;
        }

        public void Process([NotNull] CompletionEvent completionEvent)
        {
            var trace = GetNewTraceOrFilterContinuation(completionEvent);
            trace.DurationInMillis += completionEvent.ComputeDuration();
            trace.AppendSelectionChangeActions(completionEvent);

            switch (completionEvent.TerminatedAs)
            {
                case CompletionEvent.TerminationState.Applied:
                    trace.AppendAction(CompletionAction.NewApply());
                    _writer.Write(trace);
                    break;
                case CompletionEvent.TerminationState.Cancelled:
                    trace.AppendAction(CompletionAction.NewCancel());
                    _writer.Write(trace);
                    break;
                case CompletionEvent.TerminationState.Filtered:
                    // Filtering is not a termination of code completion. Moreover, we know the changed prefix
                    // only from the subsequent completion event. Therefore, no action is appended here.
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private CompletionTrace GetNewTraceOrFilterContinuation(CompletionEvent completionEvent)
        {
            if (_trace != null && completionEvent.IsContinuationAfterFiltering())
            {
                _trace.AppendAction(CompletionAction.NewFilter(completionEvent.Prefix));
            }
            else
            {
                _trace = new CompletionTrace {DurationInMillis = 0};
            }
            return _trace;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }

    internal static class CompletionEventToTraceConverterUtils
    {
        internal static bool IsContinuationAfterFiltering(this CompletionEvent completionEvent)
        {
            return completionEvent.TriggeredBy == IDEEvent.Trigger.Automatic;
        }

        internal static long ComputeDuration(this CompletionEvent completionEvent)
        {
            var duration = completionEvent.Duration.GetValueOrDefault(TimeSpan.FromSeconds(0));
            return (long) Math.Ceiling(duration.TotalMilliseconds);
        }

        internal static void AppendSelectionChangeActions([NotNull] this CompletionTrace trace,
            CompletionEvent completionEvent)
        {
            if (!completionEvent.HasSelections())
            {
                return;
            }

            var initialSelection = completionEvent.Selections[0];
            var oldPos = completionEvent.ProposalCollection.GetPosition(initialSelection.Proposal);

            foreach (var newSelection in completionEvent.Selections)
            {
                var newPos = completionEvent.ProposalCollection.GetPosition(newSelection.Proposal);
                var stepSize = Math.Abs(oldPos - newPos);

                if (stepSize == 1)
                {
                    var direction = (newPos > oldPos) ? Direction.Down : Direction.Up;
                    trace.AppendAction(CompletionAction.NewStep(direction));
                }
                else if (stepSize > 1)
                {
                    trace.AppendAction(CompletionAction.NewMouseGoto(newPos));
                }

                oldPos = newPos;
            }
        }

        private static bool HasSelections(this CompletionEvent completionEvent)
        {
            return completionEvent.Selections.Count > 1;
        }
    }
}