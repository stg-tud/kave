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

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KaVE.Commons.Utils.Collections
{
    public interface IMultiset<T> : IEnumerable<T>
    {
        void Add(T element, int occurences = 1);
        int Count();
        int Count(T element);
        bool RemoveAll(T element);

        IKaVESet<T> ElementSet { get; }
        IDictionary<T, int> EntryDictionary { get; }
    }

    public class Multiset<T> : IMultiset<T>
    {
        private readonly IDictionary<T, int> _data = new Dictionary<T, int>();

        public Multiset(IEnumerable<T> elements)
        {
            foreach (var element in elements)
            {
                Add(element);
            }
        }

        public Multiset() {}

        public IKaVESet<T> ElementSet
        {
            get { return Sets.NewHashSetFrom(_data.Keys); }
        }

        public IDictionary<T, int> EntryDictionary
        {
            get { return new Dictionary<T, int>(_data); }
        }

        public void Add(T element, int occurences = 1)
        {
            if (_data.ContainsKey(element))
            {
                _data[element] += occurences;
            }
            else
            {
                _data[element] = occurences;
            }
        }

        public int Count()
        {
            return _data.Values.Sum();
        }

        public int Count(T element)
        {
            return _data.ContainsKey(element) ? _data[element] : 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var kvp in _data)
            {
                for (var i = 0; i < kvp.Value; i++)
                {
                    yield return kvp.Key;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool RemoveAll(T element)
        {
            return _data.Remove(element);
        }
    }
}