using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KaVE.Utils.Collections
{
    /// <summary>
    /// A dictionary that stores the values as <see cref="WeakReference{TRef}"/>. Mappings whose value gets garbage collected are implicitely removed.
    /// </summary>
    public class WeakReferenceDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TValue : class
    {
        private readonly IDictionary<TKey, WeakReference<TValue>> _dictionary;

        public WeakReferenceDictionary()
        {
            _dictionary = new Dictionary<TKey, WeakReference<TValue>>();
        }

        private static KeyValuePair<TKey, WeakReference<TValue>> Pack(KeyValuePair<TKey, TValue> keyValuePair)
        {
            return new KeyValuePair<TKey, WeakReference<TValue>>(keyValuePair.Key, new WeakReference<TValue>(keyValuePair.Value));
        }

        private static KeyValuePair<TKey, TValue> Unpack(KeyValuePair<TKey, WeakReference<TValue>> keyValuePair)
        {
            return new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value.Target);
        }

        /// <summary>
        /// An Enumerable of all key-value pairs whose values are alive, i.e., have not been garbage collected.
        /// </summary>
        private IEnumerable<KeyValuePair<TKey, TValue>> Dictionary
        {
            get
            {
                return (from keyValuePair in _dictionary where keyValuePair.Value.IsAlive() select Unpack(keyValuePair));
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
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

        /// <summary>
        /// Removes all key-value pairs whose value has been garbage collected from the internal dictionary.
        /// </summary>
        private void CleanUp()
        {
            GC.Collect();
            var deadEntries = _dictionary.Where(kvp => !kvp.Value.IsAlive()).ToArray();
            foreach (var entry in deadEntries)
            {
                _dictionary.Remove(entry);
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            CleanUp();
            return _dictionary.Contains(Pack(item));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            CleanUp();
            var innerArray = new KeyValuePair<TKey, WeakReference<TValue>>[array.Length];
            _dictionary.CopyTo(innerArray, arrayIndex);
            innerArray.Select(Unpack).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            CleanUp();
            return _dictionary.Remove(Pack(item));
        }

        public int Count
        {
            get
            {
                CleanUp();
                return _dictionary.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;
            }
        }

        public bool ContainsKey(TKey key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }

        public void Add(TKey key, TValue value)
        {
            WeakReference<TValue> oldReference;
            if (_dictionary.TryGetValue(key, out oldReference) && !oldReference.IsAlive())
            {
                _dictionary.Remove(key);
            }
            _dictionary.Add(key, new WeakReference<TValue>(value));
        }

        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            WeakReference<TValue> innerValue;
            if (_dictionary.TryGetValue(key, out innerValue) && innerValue.IsAlive())
            {
                value = innerValue.Target;
                return true;
            }
            value = null;
            return false;
        }

        public TValue this[TKey key]
        {
            get { return _dictionary[key].Target; }
            set
            {
                var reference = _dictionary[key];
                reference.Target = value;
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                CleanUp();
                return _dictionary.Keys;
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return Dictionary.Select(kvp => kvp.Value).ToList();
            }
        }
    }
}
