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
 * 
 * Contributors:
 *    - Dennis Albrecht
 */

using System;
using KaVE.Model.ObjectUsage;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal class CoReNameToStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((CoReName) value).Name);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            Asserts.That(reader.TokenType == JsonToken.String);
            var name = (string)reader.Value;
            if (objectType == typeof(CoReTypeName))
                return new CoReTypeName(name);
            if (objectType == typeof(CoReMethodName))
                return new CoReMethodName(name);
            if (objectType == typeof(CoReFieldName))
                return new CoReFieldName(name);
            throw new ArgumentException("this converter may only convert to CoReNames");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (CoReName).IsAssignableFrom(objectType);
        }
    }
}