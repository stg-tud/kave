using System;
using System.Runtime.CompilerServices;
using KaVE.Utils.Collections;

namespace KaVE.Model.Utils
{
    internal class WeakNameCache<TName> where TName : class
    {
        private static readonly WeakReferenceDictionary<Type, WeakNameCache<TName>> CacheRegistry = new WeakReferenceDictionary<Type, WeakNameCache<TName>>();

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static WeakNameCache<TName> Get(NameFactory factory)
        {
            WeakNameCache<TName> cache;
            var cacheType = typeof (TName);
            if (!CacheRegistry.TryGetValue(cacheType, out cache))
            {
                cache = new WeakNameCache<TName>(factory);
                CacheRegistry.Add(cacheType, cache);
            }
            return cache;
        }

        private readonly WeakReferenceDictionary<string, TName> _nameRegistry = new WeakReferenceDictionary<string, TName>();
        private readonly NameFactory _factory;

        public delegate TName NameFactory(string identifier);

        private WeakNameCache(NameFactory factory)
        {
            _factory = factory;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TName GetOrCreate(string identifier)
        {
            TName name;
            if (!_nameRegistry.TryGetValue(identifier, out name))
            {
                name = _factory.Invoke(identifier);
                _nameRegistry.Add(identifier, name);
            }
            return name;
        }
    }
}
