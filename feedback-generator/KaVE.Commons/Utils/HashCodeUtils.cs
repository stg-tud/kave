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

namespace KaVE.Commons.Utils
{
    public static class HashCodeUtils
    {
        public static int For<T1, T2>(int seed, IDictionary<T1, IEnumerable<T2>> d)
        {
            return d.Aggregate(seed, (current, e) => seed*current + e.Key.GetHashCode() + For(seed, e.Value));
        }

        public static int For<T1, T2>(int seed, IDictionary<T1, IList<T2>> d)
        {
            return d.Aggregate(seed, (current, e) => seed*current + e.Key.GetHashCode() + For(seed, e.Value));
        }

        public static int For<T1, T2>(int seed, IDictionary<T1, ISet<T2>> d)
        {
            return d.Aggregate(seed, (current, e) => seed*current + e.Key.GetHashCode() + For(seed, e.Value));
        }

        public static int For<T>(int seed, IEnumerable<T> vs)
        {
            return vs.Aggregate(seed, (cur, v) => seed*cur + v.GetHashCode());
        }

        public static int For<T1, T2>(int seed, IDictionary<T1, T2> d)
        {
            return d.Aggregate(seed, (current, e) => seed*current + e.Key.GetHashCode() + e.Value.GetHashCode());
        }
    }
}