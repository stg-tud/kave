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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.JetBrains.Annotations;

namespace KaVE.Commons.Utils.ObjectUsageExport
{
    public class ScopedNameResolver
    {
        public readonly ScopedNameResolver ParentResolver;

        [NotNull]
        private readonly IDictionary<CoReTypeName, Query> _typeToQuery;

        [NotNull]
        private readonly IDictionary<string, Query> _idToQuery;

        [NotNull]
        public IEnumerable<string> BoundNames
        {
            get
            {
                var boundNames = Sets.NewHashSet<string>();
                if (ParentResolver != null)
                {
                    foreach (var name in ParentResolver.BoundNames)
                    {
                        boundNames.Add(name);
                    }
                }
                foreach (var name in _idToQuery.Keys)
                {
                    boundNames.Add(name);
                }
                return boundNames;
            }
        }

        public ScopedNameResolver(ScopedNameResolver parentResolver = null)
        {
            ParentResolver = parentResolver;

            _typeToQuery = new Dictionary<CoReTypeName, Query>();
            _idToQuery = new Dictionary<string, Query>();
        }

        public bool IsExistingInCurrentScope(string id)
        {
            return _idToQuery.ContainsKey(id);
        }

        public bool IsExisting(string id)
        {
            var exists = IsExistingInCurrentScope(id);
            if (!exists && ParentResolver != null)
            {
                return ParentResolver.IsExisting(id);
            }
            return exists;
        }

        public Query Find(string id)
        {
            var existsInCurrentScope = _idToQuery.ContainsKey(id);
            if (existsInCurrentScope)
            {
                return _idToQuery[id];
            }
            return ParentResolver != null ? ParentResolver.Find(id) : null;
        }

        public bool IsExistingInCurrentScope(CoReTypeName type)
        {
            return _typeToQuery.ContainsKey(type);
        }

        public bool IsExisting(CoReTypeName type)
        {
            var exists = IsExistingInCurrentScope(type);
            if (!exists && ParentResolver != null)
            {
                return ParentResolver.IsExisting(type);
            }
            return exists;
        }

        public Query Find(CoReTypeName type)
        {
            var existsInCurrentScope = _typeToQuery.ContainsKey(type);
            if (existsInCurrentScope)
            {
                return _typeToQuery[type];
            }
            return ParentResolver != null ? ParentResolver.Find(type) : null;
        }

        public void Register(string id, Query q)
        {
            Asserts.Not(_idToQuery.ContainsKey(id), "id '{0}' is already bound in current scope", id);
            _idToQuery.Add(id, q);
        }

        public void Register(CoReTypeName type, Query q)
        {
            Asserts.Not(_typeToQuery.ContainsKey(type), "type '{0}' is already bound in current scope", type);
            _typeToQuery.Add(type, q);
        }

        public CoReTypeName GetStaticType(string id)
        {
            Asserts.That(IsExisting(id));
            var q = Find(id);
            return q.type;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj, Equals);
        }

        private bool Equals(ScopedNameResolver other)
        {
            var isTypeMapEq = EqualityUtils.Equals(_typeToQuery, other._typeToQuery);
            var isIdMapEq = EqualityUtils.Equals(_idToQuery, other._idToQuery);
            return Equals(ParentResolver, other.ParentResolver) && isTypeMapEq && isIdMapEq;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ParentResolver != null ? ParentResolver.GetHashCode() : 2);
                hashCode = (hashCode*397) ^ HashCodeUtils.For(13, _typeToQuery);
                hashCode = (hashCode*397) ^ HashCodeUtils.For(17, _idToQuery);
                return hashCode;
            }
        }
    }
}