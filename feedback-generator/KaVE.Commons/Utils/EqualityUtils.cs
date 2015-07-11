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
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils
{
    public static class EqualityUtils
    {
        public static bool Equals<T>([NotNull] this T self, object other, [NotNull] Predicate<T> equalsIfSameType)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(self, other))
            {
                return true;
            }
            if (other.GetType() != self.GetType())
            {
                return false;
            }
            return equalsIfSameType((T) other);
        }

        public static bool Equals<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> self,
            IDictionary<TKey, TValue> other)
        {
            return self.Count == other.Count && !self.Except(other).Any();
        }

        public static bool DeepEquals<TKey, TDValue>([NotNull] this IDictionary<TKey, ISet<TDValue>> self,
            IDictionary<TKey, ISet<TDValue>> other)
        {
            if (self.Count != other.Count)
            {
                return false;
            }
            foreach (var keyValuePair in self)
            {
                if (!other.ContainsKey(keyValuePair.Key))
                {
                    return false;
                }
                if (!other[keyValuePair.Key].SetEquals(keyValuePair.Value))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool DeepEquals<T>([NotNull] this T[] self, T[] others)
        {
            if (self == others)
            {
                return true;
            }

            if (others == null)
            {
                return false;
            }

            if (self.Length != others.Length)
            {
                return false;
            }

            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < self.Length; i++)
            {
                if (!self[i].Equals(others[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}