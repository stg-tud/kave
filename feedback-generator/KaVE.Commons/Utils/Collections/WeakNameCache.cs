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
using KaVE.Commons.Model.Names;
using KaVE.Commons.Utils.Reflection;

namespace KaVE.Commons.Utils.Collections
{
    internal class WeakNameCache<TName> where TName : class, IName
    {
        public static WeakNameCache<TName> Get(NameFactory factory)
        {
            return new WeakNameCache<TName>(factory);
        }

        private readonly ConditionalWeakTable<string, TName> _cache =
            new ConditionalWeakTable<string, TName>();

        private readonly NameFactory _factory;

        public delegate TName NameFactory(string identifier);

        private WeakNameCache(NameFactory factory)
        {
            _factory = factory;
        }

        public TName GetOrCreate(string identifier)
        {
            // ConditionalWeakTable performs lookup with ReferenceEquals, hence, we first find the right key instance
            // by value equality and then do the lookup. This is might turn out problematic in terms of performance,
            // once many lookups are performed and many names are cached. In that case, we might want to have a close
            // look at https://github.com/nesterovsky-bros/WeakTable/.
            identifier = _cache.InvokeNonPublic<string>("FindEquivalentKeyUnsafe", identifier, null) ?? identifier;
            return _cache.GetValue(identifier, key => _factory(key));
        }
    }
}