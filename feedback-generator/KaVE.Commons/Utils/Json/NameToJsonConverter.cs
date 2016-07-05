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
using KaVE.Commons.Model.Naming;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.Commons.Utils.Json
{
    internal class NameToJsonConverter : JsonConverter
    {
        private const string TypePropertyName = "type";
        private const string IdentifierPropertyName = "id";
        private const string OldIdentifierPropertyName = "identifier";
        private const char PropertySeparator = ':';

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //writer.WriteValue(_converter.ConvertToString(value));
            writer.WriteValue(Names.ToJson((IName) value));
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            var serialization = ReadSerializationFrom(reader);
            return Names.ParseJson(serialization);
            //return _converter.ConvertFromString(serialization);
        }

        private static string ReadSerializationFrom(JsonReader reader)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return (string) reader.Value;
            }
            return ReadLegacyFormatFrom(reader);
        }

        private static string ReadLegacyFormatFrom(JsonReader reader)
        {
            var jObject = JObject.Load(reader);
            var type = GetType(jObject);
            var identifier = GetIdentifier(jObject);
            return type + PropertySeparator + identifier;
        }

        private static string GetType(JObject jObject)
        {
            return jObject.GetValue(TypePropertyName).ToString();
        }

        private static string GetIdentifier(JObject jName)
        {
            var idToken = jName.GetValue(IdentifierPropertyName) ?? jName.GetValue(OldIdentifierPropertyName);
            return idToken.ToString();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IName).IsAssignableFrom(objectType);
        }
    }
}