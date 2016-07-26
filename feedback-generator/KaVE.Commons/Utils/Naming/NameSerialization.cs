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

namespace KaVE.Commons.Utils.Naming
{
    public static class NameSerialization
    {
        private static readonly IEnumerable<INameSerializer> Serializers;

        static NameSerialization()
        {
            Serializers = new List<INameSerializer>
            {
                new NameSerializerV0()
                //new NameSerializerV1()
            };
        }

        public static T Deserialize<T>(this string input) where T : IName
        {
            var delimIdx = input.IndexOf(':');
            var prefix = input.Substring(0, delimIdx);
            delimIdx++; // skip delim
            var id = input.Substring(delimIdx, input.Length - delimIdx);

            foreach (var s in Serializers)
            {
                if (s.CanDeserialize(prefix))
                {
                    return (T) s.Deserialize(prefix, id);
                }
            }
            throw new ArgumentException(
                "no matching deserializer found for prefix '{0}' (id: '{1}')".FormatEx(prefix, id));
        }

        public static string Serialize(this IName n)
        {
            foreach (var s in Serializers)
            {
                if (s.CanSerialize(n))
                {
                    return s.Serialize(n);
                }
            }
            throw new ArgumentException("no matching serializer found for type '{0}'".FormatEx(n.GetType().FullName));
        }
    }

    internal interface INameSerializer
    {
        bool CanDeserialize(string prefix);
        IName Deserialize(string prefix, string id);
        bool CanSerialize(IName name);
        string Serialize(IName name);
    }
}