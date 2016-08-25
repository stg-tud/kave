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
using KaVE.FeedbackProcessor.WatchdogExports.Model;
using KaVE.FeedbackProcessor.WatchdogExports.Transformers;

namespace KaVE.FeedbackProcessor.Tests.WatchdogExports.Transformers
{
    internal class TransformerTestBase<TIntervalType>
        where TIntervalType : Interval, new()
    {
        private DateTime _referenceTime = DateTime.Now.Date;
        protected TransformerContext _context = new TransformerContext();

        protected virtual DateTime TestTime(int timeOffsetInMinutes)
        {
            return _referenceTime.AddMinutes(timeOffsetInMinutes);
        }

        protected virtual TestIDEEvent TestIDEEvent(int startOffsetInMinutes,
            int endOffsetInMinutes,
            string sessionId = null)
        {
            return new TestIDEEvent
            {
                IDESessionUUID = sessionId,
                TriggeredAt = _referenceTime.AddMinutes(startOffsetInMinutes),
                TerminatedAt = _referenceTime.AddMinutes(endOffsetInMinutes)
            };
        }

        protected virtual TIntervalType ExpectedInterval(int startOffsetInMinutes,
            int endOffsetInMinutes,
            string sessionId = null)
        {
            return new TIntervalType
            {
                StartTime = _referenceTime.AddMinutes(startOffsetInMinutes),
                Duration = TimeSpan.FromMinutes(endOffsetInMinutes - startOffsetInMinutes),
                IDESessionId = sessionId,
                Project = string.Empty
            };
        }
    }
}