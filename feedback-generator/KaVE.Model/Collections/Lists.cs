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
 *    - Sebastian Proksch
 */

using System.Collections.Generic;
using System.Linq;
using KaVE.Utils;

namespace KaVE.Model.Collections
{
    public class Lists
    {
        public static IList<T> NewList<T>()
        {
            return new KaVEList<T>();
        }

        public static IList<T> NewList<T>(params T[] ts)
        {
            var s = new KaVEList<T>();
            s.AddRange(ts);
            return s;
        }

        public static IList<T> NewListFrom<T>(IEnumerable<T> ts)
        {
            var s = new KaVEList<T>();
            s.AddRange(ts);
            return s;
        }
    }

    public class KaVEList<T> : List<T>
    {
        private const int Seed = 1371;

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(IList<T> other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            var init = typeof (T).GetHashCode();
            return Seed*this.Aggregate(init, (current, e) => Seed*current + e.GetHashCode());
        }
    }
}