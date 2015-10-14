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
using System.Collections;
using System.Collections.Generic;
using KaVE.Commons.Utils;

namespace KaVE.FeedbackProcessor.Activities.Intervals
{
    internal class IntervalStream<T> : IEnumerable<Interval<T>>
    {
        public int NumberOfAnyActivities;
        public int TotalNumberOfActivities;

        private readonly IList<Interval<T>> _intervals = new List<Interval<T>>();

        public IntervalStream() {}

        public IntervalStream(IEnumerable<Interval<T>> intervals)
        {
            foreach (var interval in intervals)
            {
                _intervals.Add(interval);
            }
        }

        public void Append(Interval<T> interval)
        {
            _intervals.Add(interval);
        }

        public int Length
        {
            get { return _intervals.Count; }
        }

        public DateTime Start
        {
            get { return _intervals.Count > 0 ? _intervals[0].Start : default(DateTime); }
        }

        public Interval<T> this[int index]
        {
            get { return _intervals[index]; }
        }

        public void RemoveAt(int index)
        {
            _intervals.RemoveAt(index);
        }

        public void Insert(int index, Interval<T> interval)
        {
            _intervals.Insert(index, interval);
        }

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
            return SplitByDay(TimeSpan.Zero);
        } 

        public IEnumerable<IntervalStream<T>> SplitByDay(TimeSpan offset)
        {
            var lastStreamDate = GetStreamDate(Start, offset);
            var currentStream = new IntervalStream<T>();
            foreach (var interval in _intervals)
            {
                var newStreamDate = GetStreamDate(interval.Start, offset);
                if (newStreamDate != lastStreamDate)
                {
                    yield return currentStream;
                    currentStream = new IntervalStream<T>();
                    lastStreamDate = newStreamDate;
                }
                currentStream.Append(interval);
            }
            yield return currentStream;
        }

        private static DateTime GetStreamDate(DateTime dateTime, TimeSpan offset)
        {
            try
            {
                return (dateTime - offset).Date;
            }
            catch (ArgumentOutOfRangeException)
            {
                return dateTime.Date;
            }
        }

        protected bool Equals(IntervalStream<T> other)
        {
            return Equals(_intervals, other._intervals);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            return (_intervals != null ? _intervals.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format(
                "NumberOfAnyActivities: {0}, TotalNumberOfActivities: {1}, Intervals: {2}",
                NumberOfAnyActivities,
                TotalNumberOfActivities,
                _intervals);
        }
    }
}