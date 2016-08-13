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
using KaVE.Commons.Utils.Assertion;

namespace KaVE.FeedbackProcessor.Model
{
    internal static class EventUtils
    {
        /// <summary>
        ///     Assumes that TriggeredAt is set.
        /// </summary>
        public static DateTime GetTriggeredAt(this IDEEvent ideEvent)
        {
            var triggeredAt = ideEvent.TriggeredAt;
            Asserts.That(triggeredAt.HasValue, "all events should have a trigger date");
            return triggeredAt.Value;
        }


        /// <summary>
        ///     Assumes that TriggeredAt is set.
        /// </summary>
        public static DateTime GetTriggerDate(this IDEEvent ideEvent)
        {
            return ideEvent.GetTriggeredAt().Date;
        }
    }
}