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
using KaVE.JetBrains.Annotations;
using KaVE.Utils;
using KaVE.Utils.Assertion;

namespace KaVE.Model.Collections
{
    public class Sets
    {
        public static IKaVESet<T> NewHashSet<T>()
        {
            return new KaVEHashSet<T>();
        }

        public static IKaVESet<T> NewHashSet<T>(params T[] ts)
        {
            return NewHashSetFrom(ts);
        }

        public static IKaVESet<T> NewHashSetFrom<T>(IEnumerable<T> ts)
        {
            var s = new KaVEHashSet<T>();
            foreach (var t in ts)
            {
                Asserts.NotNull(t);
                s.Add(t);
            }
            return s;
        }
    }

    public class KaVEHashSet<T> : HashSet<T>, IKaVESet<T>
    {
        private const int Seed = 1367;

        public new bool Add(T item)
        {
            Asserts.NotNull(item);
            return base.Add(item);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(KaVEHashSet<T> other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            var init = typeof (T).GetHashCode();
            return Seed*this.Aggregate(init, (current, e) => current + e.GetHashCode());
        }
    }

    // ReSharper disable once InconsistentNaming
    public interface IKaVESet<T> : ISet<T>
    {
        new bool Add([NotNull] T item);
    }
}