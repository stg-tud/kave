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

using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;

namespace KaVE.FeedbackProcessor.Statistics
{
    internal class CompletionInvocationStatistic : BaseEventProcessor
    {
        private bool _lastWasCompletionEvent;
        public int CompletionInvocations { get; private set; }

        public CompletionInvocationStatistic()
        {
            RegisterFor<IDEEvent>(OnAnyEvent);
        }

        private void OnAnyEvent(IDEEvent @event)
        {
            if (@event is CompletionEvent)
            {
                if (!_lastWasCompletionEvent)
                {
                    CompletionInvocations++;
                }
                _lastWasCompletionEvent = true;
            }
            else
            {
                _lastWasCompletionEvent = false;
            }
        }
    }
}