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
using JetBrains.Util.src.dataStructures;

namespace KaVE.VS.Achievements.Util
{
    internal class FixedSizePriorityQueue<T>
    {
        public int Limit
        {
            get { return _limit; }
            set
            {
                _limit = value;
                _queue = new PriorityQueue<T>(_limit, _comparer);
                while (_queue.Count > _limit)
                {
                    Dequeue();
                }
            }
        }

        private int _limit;

        public IEnumerable<T> Items
        {
            get { return _queue.AsEnumerable(); }
        }

        private readonly IComparer<T> _comparer;

        private PriorityQueue<T> _queue;

        public FixedSizePriorityQueue(int limit, IComparer<T> comparer)
        {
            _comparer = comparer;
            Limit = limit;
        }

        public void Enqueue(T obj)
        {
            _queue.Add(obj);

            if (_queue.Count > Limit)
            {
                Dequeue();
            }
        }

        public T Dequeue()
        {
            var currentItem = _queue.GetMin();
            _queue.Remove(currentItem);
            return currentItem;
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}