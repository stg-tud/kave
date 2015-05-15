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
 * 
 * Contributors:
 *    - Markus Zimmermann
 *    - Sven Amann
 */

using System;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils.DateTime;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Cleanup.Heuristics
{
    internal class ConcurrentEventHeuristic
    {
        public static readonly TimeSpan EventTimeDifference = TimeSpan.FromMilliseconds(10);

        public static bool AreConcurrent(IDEEvent evt1, IDEEvent evt2)
        {
            return AreSimilar(evt1.GetTriggeredAt(), evt2.GetTriggeredAt());
        }

        public static bool AreSimilar(DateTime dateTime1, DateTime dateTime2)
        {
            return new SimilarDateTimeComparer(EventTimeDifference.Milliseconds).Equal(dateTime1, dateTime2);
        }
    }
}