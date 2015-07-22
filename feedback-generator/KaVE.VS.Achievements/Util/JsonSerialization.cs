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
using Newtonsoft.Json;

namespace KaVE.VS.Achievements.Util
{
    public class JsonSerialization
    {
        public static string JsonSerializeObject(object content)
        {
            var json = JsonConvert.SerializeObject(
                content,
                Formatting.None,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = new List<JsonConverter> {new TimeSpanConverter()}
                });
            return json;
        }

        public static T JsonDeserializeObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(
                jsonString,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });
        }
    }

    public class TimeSpanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (TimeSpan);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("$type");
            writer.WriteValue(value.GetType().FullName);
            writer.WritePropertyName("$value");
            writer.WriteValue(value);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}