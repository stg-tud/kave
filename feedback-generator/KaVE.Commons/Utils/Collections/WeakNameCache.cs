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

using System.Runtime.CompilerServices;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.Collections
{
    internal class WeakNameCache<TName> where TName : class
    {
        private static WeakNameCache<TName> _cacheRegistry;

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object StaticLock = new object();

        private readonly object _lock = new object();

        public static WeakNameCache<TName> Get(NameFactory factory)
        {
            lock (StaticLock)
            {
                return _cacheRegistry ?? (_cacheRegistry = new WeakNameCache<TName>(factory));
            }
        }

        private readonly ConditionalWeakTable<string, Data<TName>> _nameCache2 =
            new ConditionalWeakTable<string, Data<TName>>();

        [UsedImplicitly]
        private class Data<T>
        {
            public T Value;
        }

        private readonly NameFactory _factory;

        public delegate TName NameFactory(string identifier);

        private WeakNameCache(NameFactory factory)
        {
            _factory = factory;
        }

        public TName GetOrCreate(string identifier)
        {
            lock (_lock)
            {
                var data = _nameCache2.GetOrCreateValue(identifier);
                return data.Value ?? (data.Value = _factory(identifier));
            }
        }
    }
}