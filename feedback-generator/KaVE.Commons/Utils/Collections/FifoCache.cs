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
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.Collections
{
    public class FifoCache<TKey, TValue>
    {
        private readonly int _maxCacheSize;
        private readonly Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();
        private readonly ISet<TKey> _keySet = new HashSet<TKey>();
        private readonly Queue<TKey> _keyQueue = new Queue<TKey>();

        private int _curSize;

        public FifoCache(int maxCacheSize)
        {
            _maxCacheSize = maxCacheSize;
        }

        public int CacheSize
        {
            get { return _maxCacheSize; }
        }

        public IKaVEList<TKey> Keys
        {
            get { return Lists.NewListFrom(_keyQueue); }
        }

        public bool ContainsKey(TKey key)
        {
            return _values.ContainsKey(key);
        }

        public TValue GetValue(TKey hashCode)
        {
            Asserts.That(_values.ContainsKey(hashCode));
            return _values[hashCode];
        }

        public void SetValue(TKey key, TValue context)
        {
            var isKnown = _keySet.Contains(key);
            if (isKnown)
            {
                return;
            }

            _values[key] = context;
            _keyQueue.Enqueue(key);
            _keySet.Add(key);
            _curSize++;

            if (_curSize > _maxCacheSize)
            {
                var oldestKey = _keyQueue.Dequeue();
                _values.Remove(oldestKey);
                _keySet.Remove(oldestKey);
                _curSize--;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(FifoCache<TKey, TValue> other)
        {
            var areValuesEqual = EqualityUtils.Equals(_values, other._values);
            return _maxCacheSize == other._maxCacheSize && areValuesEqual;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var valuesHashCode = HashCodeUtils.For(337, _values);
                return (_maxCacheSize*397) ^ valuesHashCode;
            }
        }

        public override string ToString()
        {
            return this.ToStringReflection();
        }
    }
}