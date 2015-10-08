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
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;

namespace KaVE.VS.Statistics.Filters
{
    /// <summary>
    ///     Filter, merge and replacement logic for CompletionEvents
    /// </summary>
    public class CompletionPreprocessor : IEventPreprocessor
    {
        private TimeSpan? _aggregatedTimeSpan = TimeSpan.Zero;

        public IDEEvent Preprocess(IDEEvent @event)
        {
            var completionEvent = @event as CompletionEvent;
            if (completionEvent == null)
            {
                return null;
            }

            _aggregatedTimeSpan += completionEvent.Duration;

            if (IsFilteredCompletionEvent(completionEvent))
            {
                return null;
            }

            completionEvent.Duration = _aggregatedTimeSpan;
            _aggregatedTimeSpan = TimeSpan.Zero;
            return completionEvent;
        }

        /// <summary>
        ///     <para>Returns true iff <paramref name="completionEvent" /> was terminated as filtered</para>
        /// </summary>
        private static bool IsFilteredCompletionEvent(ICompletionEvent completionEvent)
        {
            return completionEvent.TerminatedState == TerminationState.Filtered;
        }
    }
}