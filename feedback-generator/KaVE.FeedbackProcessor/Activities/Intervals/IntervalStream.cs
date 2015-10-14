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

using System.Collections;
using System.Collections.Generic;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal class IntervalStream<T> : IEnumerable<Interval<T>>
    {
        public int NumberOfAnyActivities;
        public int TotalNumberOfActivities;

        private readonly IList<Interval<T>> _intervals = new List<Interval<T>>();

        public void Append(Interval<T> interval)
        {
            _intervals.Add(interval);
        }

        public int Length { get { return _intervals.Count; } }

        public Interval<T> this[int index] { get { return _intervals[index]; } }

        public void RemoveAt(int index) { _intervals.RemoveAt(index);}

        public void Insert(int index, Interval<T> interval) { _intervals.Insert(index, interval);}

        public IEnumerator<Interval<T>> GetEnumerator()
        {
            return _intervals.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IntervalStream<T>> SplitByDay()
        {
            var clone = new IntervalStream<T>();
            foreach (var interval in _intervals)
            {
                clone._intervals.Add(interval);
            }

            yield return clone;
        }
    }
}