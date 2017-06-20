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
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Utils;

namespace KaVE.FeedbackProcessor.StatisticsUltimate
{
    public interface IInteractionStatistics
    {
        DateTime DayFirst { get; }
        DateTime DayLast { get; }
        int NumDays { get; }
        int NumMonth { get; }
        int NumEvents { get; }
        Educations Education { get; }
        Positions Position { get; }
        int NumCodeCompletion { get; }
        int NumTestRuns { get; }
        TimeSpan ActiveTime { get; }
    }

    public class InteractionStatistics : IInteractionStatistics
    {
        public DateTime DayFirst { get; set; }
        public DateTime DayLast { get; set; }

        public int NumDays { get; set; }
        public int NumMonth { get; set; }
        public int NumEvents { get; set; }

        public Educations Education { get; set; }
        public Positions Position { get; set; }

        public int NumCodeCompletion { get; set; }
        public int NumTestRuns { get; set; }

        public TimeSpan ActiveTime { get; set; }

        public InteractionStatistics()
        {
            ActiveTime = TimeSpan.Zero;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        protected bool Equals(InteractionStatistics other)
        {
            return DayFirst.Equals(other.DayFirst) && DayLast.Equals(other.DayLast) && NumDays == other.NumDays &&
                   NumMonth == other.NumMonth && NumEvents == other.NumEvents && Education == other.Education &&
                   Position == other.Position && NumCodeCompletion == other.NumCodeCompletion &&
                   NumTestRuns == other.NumTestRuns && ActiveTime.Equals(other.ActiveTime);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 2954786;
                hashCode = (hashCode * 397) ^ DayFirst.GetHashCode();
                hashCode = (hashCode * 397) ^ DayLast.GetHashCode();
                hashCode = (hashCode * 397) ^ NumDays;
                hashCode = (hashCode * 397) ^ NumMonth;
                hashCode = (hashCode * 397) ^ NumEvents;
                hashCode = (hashCode * 397) ^ (int) Education;
                hashCode = (hashCode * 397) ^ (int) Position;
                hashCode = (hashCode * 397) ^ NumCodeCompletion;
                hashCode = (hashCode * 397) ^ NumTestRuns;
                hashCode = (hashCode * 397) ^ ActiveTime.GetHashCode();
                return hashCode;
            }
        }
    }
}