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
using KaVE.Commons.Utils;
using KaVE.FeedbackProcessor.Model;

namespace KaVE.FeedbackProcessor.Activities.Model
{
    public class DeveloperDay
    {
        public Developer Developer { get; private set; }
        public DateTime Day { get; private set; }

        public DeveloperDay(Developer developer, DateTime day)
        {
            Developer = developer;
            Day = day;
        }

        public string Id
        {
            get { return string.Format("{0}_{1}", Developer.Id, Day.ToShortDateString()); }
        }

        protected bool Equals(DeveloperDay other)
        {
            return Equals(Developer, other.Developer) && Day.Equals(other.Day);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Developer != null ? Developer.GetHashCode() : 0)*397) ^ Day.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("({0} on {1})", Developer.Id, Day.ToShortDateString());
        }
    }
}
