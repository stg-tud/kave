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

namespace KaVE.Commons.Utils.Collections
{
    /// <summary>
    /// Compares sets based on the equality of their content.
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class SetEqualityComparer<TElement> : IEqualityComparer<ISet<TElement>>
    {
        /// <summary>
        /// Uses <see cref="ISet{T}.SetEquals"/> to check for equality.
        /// </summary>
        /// <returns><code>true</code>, if the sets have the same size and for
        /// every element in <code>x</code> there is an equal element in
        /// <code>y</code>. <code>false</code> otherwise.</returns>
        public bool Equals(ISet<TElement> x, ISet<TElement> y)
        {
            return x.SetEquals(y);
        }

        /// <summary>
        /// The code is the sum of the hashcodes of the set's elements, i.e.,
        /// it is equal for sets containing equal elements.
        /// </summary>
        public int GetHashCode(ISet<TElement> obj)
        {
            return obj.GetSetHashCode();
        }
    }

    public static class SetEqualityUtils
    {
        /// <summary>
        /// The code is the sum of the hashcodes of the set's elements, i.e.,
        /// it is equal for sets containing equal elements.
        /// </summary>
        public static int GetSetHashCode<TElement>(this ISet<TElement> obj)
        {
            return obj.Aggregate(0, (hc, next) => hc + next.GetHashCode());
        }
    }
}