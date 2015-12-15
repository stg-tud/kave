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
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Intervals.Model
{
    public abstract class Interval
    {
        [NotNull]
        public DateTime StartTime { get; set; }

        [NotNull]
        public TimeSpan Duration { get; set; }

        public DateTime EndTime
        {
            get { return StartTime + Duration; }
        }

        protected Interval()
        {
            StartTime = DateTime.MinValue;
            Duration = TimeSpan.Zero;
        }

        protected bool Equals(Interval other)
        {
            return StartTime.Equals(other.StartTime) && Duration.Equals(other.Duration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Interval) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StartTime.GetHashCode()*397) ^ Duration.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format(
                "{0}: {1} ~ {2} ({3}d {4}h {5}m {6}s)",
                GetType().Name,
                StartTime,
                EndTime,
                Duration.Days,
                Duration.Hours,
                Duration.Minutes,
                Duration.Seconds);
        }
    }
}