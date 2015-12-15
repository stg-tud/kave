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
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Intervals.Model;

namespace KaVE.FeedbackProcessor.Tests.Intervals.Transformers
{
    internal class TransformerTestBase<TIntervalType>
        where TIntervalType : Interval, new()
    {
        protected DateTime _referenceTime = DateTime.Now;

        protected TestIDEEvent TestIDEEvent(string sessionId, int startOffsetInMinutes, int endOffsetInMinutes)
        {
            return new TestIDEEvent
            {
                IDESessionUUID = sessionId,
                TriggeredAt = _referenceTime.AddMinutes(startOffsetInMinutes),
                TerminatedAt = _referenceTime.AddMinutes(endOffsetInMinutes)
            };
        }

        protected TIntervalType ExpectedInterval(int startOffsetInMinutes, int endOffsetInMinutes)
        {
            return new TIntervalType
            {
                StartTime = _referenceTime.AddMinutes(startOffsetInMinutes),
                Duration = TimeSpan.FromMinutes(endOffsetInMinutes - startOffsetInMinutes)
            };
        }
    }
}