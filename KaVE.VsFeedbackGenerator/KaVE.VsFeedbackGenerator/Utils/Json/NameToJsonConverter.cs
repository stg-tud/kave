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
using System.Linq;
using System.Reflection;
using KaVE.Model.Names;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal class NameToJsonConverter : JsonConverter
    {
        private const string TypePropertyName = "type";
        private const string IdentifierPropertyName = "id";
        private const string OldIdentifierPropertyName = "identifier";
        private const string NameQualifierPrefix = "KaVE.Model.Names.";
        private const char PropertySeparator = ':';

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typeAlias = AliasFor(value.GetType());
            var identifier = ((IName) value).Identifier;
            writer.WriteValue(typeAlias + PropertySeparator + identifier);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var jName = GetJObject(reader);
            var factoryMethod = GetFactoryMethod(jName);
            var identifier = GetIdentifier(jName);
            return factoryMethod.Invoke(null, new object[] {identifier});
        }

        private static JObject GetJObject(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var data = ((string) reader.Value).Split(PropertySeparator);
                return new JObject {{TypePropertyName, data[0]}, {IdentifierPropertyName, data[1]}};
            }
            return JObject.Load(reader);
        }

        private static string GetIdentifier(JObject jName)
        {
            var idToken = jName.GetValue(IdentifierPropertyName) ?? jName.GetValue(OldIdentifierPropertyName);
            return idToken.ToString();
        }

        private static MethodInfo GetFactoryMethod(JObject jName)
        {
            var typeAlias = jName.GetValue(TypePropertyName).ToString();
            var type = TypeFrom(typeAlias);
            var factoryMethod = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(
                m =>
                {
                    var parameterInfos = m.GetParameters();
                    return parameterInfos.Count() == 1 && parameterInfos[0].ParameterType == typeof (string);
                }).First();
            return factoryMethod;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IName).IsAssignableFrom(objectType);
        }

        private static string AliasFor(Type nameType)
        {
            return nameType.FullName.Substring(NameQualifierPrefix.Length);
        }

        private static Type TypeFrom(string alias)
        {
            var assemblyName = typeof (IName).Assembly.FullName;
            var assemblyQualifiedTypeName = NameQualifierPrefix + alias + ", " + assemblyName;
            var type = Type.GetType(assemblyQualifiedTypeName);
            Asserts.NotNull(type, "Could not load required type " + assemblyQualifiedTypeName);
            return type;
        }
    }
}