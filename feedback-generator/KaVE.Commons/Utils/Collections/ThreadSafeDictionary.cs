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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Collections
{
    public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly object _lock = new object();
        private IDictionary<TKey, TValue> _dictionary;
        private readonly Func<TKey, TValue> _defaultValueCreator;

        public ThreadSafeDictionary([NotNull] Func<TKey, TValue> defaultValueCreator)
        {
            _defaultValueCreator = defaultValueCreator;
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveValue(item.Key);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            AddValue(key, value);
        }

        public bool Remove(TKey key)
        {
            return RemoveValue(key);
        }

        private bool RemoveValue(TKey key)
        {
            lock (_lock)
            {
                var result = false;
                if (_dictionary != null)
                {
                    var newDictionary = new Dictionary<TKey, TValue>(_dictionary);
                    result = newDictionary.Remove(key);
                    Thread.MemoryBarrier();
                    _dictionary = newDictionary;
                }
                return result;
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue obj;
                return _dictionary.TryGetValue(key, out obj) ? obj : AddValue(key, _defaultValueCreator(key));
            }
            set { SetValue(key, value); }
        }

        private TValue AddValue(TKey key, TValue obj)
        {
            lock (_lock)
            {
                Asserts.Not(_dictionary.ContainsKey(key), "adding duplicate");
                SetValueThreadSafe(key, obj);
                return obj;
            }
        }

        private void SetValueThreadSafe(TKey key, TValue value)
        {
            var newDictionary = new Dictionary<TKey, TValue>(_dictionary);
            newDictionary[key] = value;
            Thread.MemoryBarrier();
            _dictionary = newDictionary;
        }

        private void SetValue(TKey key, TValue value)
        {
            lock (_lock)
            {
                SetValueThreadSafe(key, value);
            }
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }
    }
}