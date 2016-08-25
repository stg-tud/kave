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
using KaVE.Commons.Model.Naming;

namespace KaVE.Commons.Utils.Collections
{
    internal class WeakNameCache<TName> where TName : class, IName
    {
        private const int NumberOfRequestsBeforeMaintenance = 1000;

        public static WeakNameCache<TName> Get(NameFactory factory)
        {
            return new WeakNameCache<TName>(factory);
        }

        private readonly Dictionary<string, WeakReference<TName>> _cache =
            new Dictionary<string, WeakReference<TName>>();

        private readonly object _lock = new object();
        private readonly NameFactory _factory;
        private int _requestCounter;

        public delegate TName NameFactory(string identifier);

        private WeakNameCache(NameFactory factory)
        {
            _factory = factory;
        }

        public TName GetOrCreate(string identifier)
        {
            lock (_lock)
            {
                RunOccasionalMaintenance();

                TName name = null;
                if (_cache.ContainsKey(identifier))
                {
                    var weakRef = _cache[identifier];
                    weakRef.TryGetTarget(out name);
                }

                if (name == null)
                {
                    name = _factory(identifier);
                    _cache[identifier] = new WeakReference<TName>(name);
                }

                return name;
            }
        }

        private void RunOccasionalMaintenance()
        {
            _requestCounter++;
            if (_requestCounter > NumberOfRequestsBeforeMaintenance)
            {
                _requestCounter = 0;
                RemoveKeysOfDeadValues();
            }
        }

        private void RemoveKeysOfDeadValues()
        {
            // .ToList is necessary to prevent concurrent modification
            foreach (var k in _cache.Keys.ToList())
            {
                TName name;
                if (!_cache[k].TryGetTarget(out name))
                {
                    _cache.Remove(k);
                }
            }
        }
    }
}