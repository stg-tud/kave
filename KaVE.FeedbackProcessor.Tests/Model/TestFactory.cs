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
using System.Globalization;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.TestUtils.Model.Events;
using KaVE.FeedbackProcessor.Model;
using MongoDB.Bson;

namespace KaVE.FeedbackProcessor.Tests.Model
{
    static class TestFactory
    {
        private static int _developerId = 1;

        public static Developer SomeDeveloper()
        {
            var id = string.Format("{0:D24}", _developerId);
            var someDeveloper = new Developer
            {
                Id = new ObjectId(id),
                SessionIds = { _developerId.ToString(CultureInfo.InvariantCulture) }
            };
            _developerId++;
            return someDeveloper;
        }

        private static int _eventValue = 1;

        public static IDEEvent SomeEventFor(Developer developer, DateTime? triggeredAt = null)
        {
            if (triggeredAt == null)
            {
                triggeredAt = DateTime.Now;
            }

            var ideEvent = new TestIDEEvent
            {
                IDESessionUUID = developer.SessionIds.First(),
                TriggeredAt = triggeredAt,
                TestProperty = _eventValue.ToString(CultureInfo.InvariantCulture)
            };
            _eventValue++;
            return ideEvent;
        }
    }
}
