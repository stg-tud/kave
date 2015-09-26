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
using KaVE.Commons.Utils.Assertion;

namespace KaVE.VS.Achievements.Utils
{
    internal class FixedSizePriorityQueue<T>
    {
        private readonly int _limit;

        public IEnumerable<T> Items
        {
            get { return _queue.AsEnumerable(); }
        }

        private readonly JetPriorityQueue<T> _queue;

        public FixedSizePriorityQueue(int limit, IComparer<T> comparer)
        {
            Asserts.Not(limit < 0);
            _limit = limit;
            _queue = new JetPriorityQueue<T>(limit, comparer);
        }

        public void Enqueue(T obj)
        {
            _queue.Add(obj);

            if (_queue.Count > _limit)
            {
                Dequeue();
            }
        }

        public T Dequeue()
        {
            return _queue.ExtractSafe();
        }

        public void Clear()
        {
            _queue.Clear();
        }
    }
}