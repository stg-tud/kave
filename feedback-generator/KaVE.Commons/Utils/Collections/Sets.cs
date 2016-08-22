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
using System.Collections.Generic;
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Collections
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

        public static IKaVESet<T> NewSortedSet<T>(Func<T, T, ComparisonResult> comparer)
        {
            return new KaVESortedSet<T>(comparer);
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

        public void AddAll(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
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
            var init = typeof(T).GetHashCode();
            return Seed*this.Aggregate(init, (current, e) => current + e.GetHashCode());
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }

    public class KaVESortedSet<T> : SortedSet<T>, IKaVESet<T>
    {
        private const int Seed = 1367;

        public KaVESortedSet(Func<T, T, ComparisonResult> comparer) : base(new SetComparer<T>(comparer)) {}

        public new bool Add(T item)
        {
            Asserts.NotNull(item);
            return base.Add(item);
        }

        public void AddAll(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(KaVESortedSet<T> other)
        {
            return GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            var init = typeof(T).GetHashCode();
            return Seed*this.Aggregate(init, (current, e) => current + e.GetHashCode());
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }

        private class SetComparer<T2> : IComparer<T2>
        {
            private readonly Func<T2, T2, ComparisonResult> _comparer;

            public SetComparer(Func<T2, T2, ComparisonResult> comparer)
            {
                _comparer = comparer;
            }

            public int Compare(T2 x, T2 y)
            {
                switch (_comparer(x, y))
                {
                    case ComparisonResult.Greater:
                        return 1;
                    case ComparisonResult.Equal:
                        return 0;
                    case ComparisonResult.Smaller:
                        return -1;
                }
                throw new AssertException("impossible path");
            }
        }
    }

    // ReSharper disable once InconsistentNaming
    public interface IKaVESet<T> : ISet<T>
    {
        new bool Add([NotNull] T item);
        void AddAll(IEnumerable<T> items);
    }
}