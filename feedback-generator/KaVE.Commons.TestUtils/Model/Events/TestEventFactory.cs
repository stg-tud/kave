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
using System.Collections.Generic;
using System.Globalization;
using KaVE.Commons.Model.Events;

namespace KaVE.Commons.TestUtils.Model.Events
{
    public static class TestEventFactory
    {
        private static readonly Random Random = new Random();

        public static List<IDEEvent> SomeEvents(int num)
        {
            var list = new List<IDEEvent>();
            for (var i = 0; i < num; i ++)
            {
                list.Add(SomeEvent());
            }
            return list;
        }

        /// <summary>
        /// Creates a random TestIDEEvent. If no trigger date is passed, DateTime.Now is used.
        /// </summary>
        public static TestIDEEvent SomeEvent(DateTime? triggeredAt = null)
        {
            var testIDEEvent = Some<TestIDEEvent>(triggeredAt);
            testIDEEvent.TestProperty = Random.Next().ToString(CultureInfo.InvariantCulture);
            return testIDEEvent;
        }

        /// <summary>
        /// Creates a fresh event instance. If no trigger date is passed, DateTime.Now is used.
        /// </summary>
        public static TEvent Some<TEvent>(DateTime? triggeredAt = null) where TEvent : IDEEvent, new()
        {
            if (triggeredAt == null)
            {
                triggeredAt = DateTime.Now;
            }
            return new TEvent {TriggeredAt = triggeredAt};
        }
    }
}