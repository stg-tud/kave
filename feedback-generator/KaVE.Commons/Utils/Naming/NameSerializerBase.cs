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
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Assertion;

namespace KaVE.Commons.Utils.Naming
{
    internal abstract class NameSerializerBase : INameSerializer
    {
        private readonly Dictionary<Type, Type> _sourceToTarget;
        private readonly Dictionary<string, Func<string, IName>> _idToFactory;
        private readonly Dictionary<Type, string> _typeToId;

        protected NameSerializerBase()
        {
            _sourceToTarget = new Dictionary<Type, Type>();
            _idToFactory = new Dictionary<string, Func<string, IName>>();
            _typeToId = new Dictionary<Type, string>();

            // ReSharper disable once VirtualMemberCallInConstructor
            RegisterTypes();
        }

        protected abstract void RegisterTypes();

        protected void RegisterTypeMapping(Type targetType, params Type[] types)
        {
            Asserts.That(types.Length > 0);
            foreach (var type in types)
            {
                _sourceToTarget[type] = targetType;
            }
        }

        // first prefix is primary and will be used for serialization
        protected void Register(Type type, Func<string, IName> cbCreate, params string[] prefixes)
        {
            Asserts.That(prefixes.Length > 0);
            foreach (var prefix in prefixes)
            {
                _idToFactory[prefix] = cbCreate;
            }
            _typeToId[type] = prefixes[0];
        }

        public bool CanDeserialize(string prefix)
        {
            return _idToFactory.ContainsKey(prefix);
        }

        public IName Deserialize(string prefix, string id)
        {
            Asserts.That(_idToFactory.ContainsKey(prefix));
            id = FixLegacyIdentifiers(prefix, id);
            return _idToFactory[prefix](id);
        }

        // this method can be overridden in serializers to fix broken ids
        protected virtual string FixLegacyIdentifiers(string prefix, string id)
        {
            return id;
        }

        public bool CanSerialize(IName name)
        {
            var effectiveType = GetEffectiveType(name);
            return _typeToId.ContainsKey(effectiveType);
        }

        public string Serialize(IName n)
        {
            var effectiveType = GetEffectiveType(n);
            Asserts.That(_typeToId.ContainsKey(effectiveType));
            return "{0}:{1}".FormatEx(_typeToId[effectiveType], n.Identifier);
        }

        private Type GetEffectiveType(IName n)
        {
            var type = n.GetType();
            if (_sourceToTarget.ContainsKey(type))
            {
                type = _sourceToTarget[type];
            }
            return type;
        }
    }
}