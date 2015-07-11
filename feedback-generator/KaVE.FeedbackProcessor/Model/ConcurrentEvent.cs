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

using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Utils;
using KaVE.JetBrains.Annotations;

namespace KaVE.FeedbackProcessor.Model
{
    internal class ConcurrentEvent : IDEEvent
    {
        [NotNull]
        public IList<IDEEvent> ConcurrentEventList { get; set; }

        public ConcurrentEvent()
        {
            ConcurrentEventList = new List<IDEEvent>();
        }

        protected bool Equals(ConcurrentEvent other)
        {
            return base.Equals(other) && ConcurrentEventList.SequenceEqual(other.ConcurrentEventList);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode*397) ^ (HashCodeUtils.For(397, ConcurrentEventList));
                return hashCode;
            }
        }
    }
}