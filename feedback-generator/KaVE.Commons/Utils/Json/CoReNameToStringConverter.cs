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
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.Commons.Utils.Json
{
    public class CoReNameToStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var coreName = value as CoReName;
            if (coreName != null)
            {
                Write(writer, coreName);
            }

            var callSite = value as CallSite;
            if (callSite != null)
            {
                Write(writer, callSite);
            }

            var defSite = value as DefinitionSite;
            if (defSite != null)
            {
                Write(writer, defSite);
            }

            if (value is CallSiteKind)
            {
                writer.WriteValue(value.ToString());
            }

            if (value is DefinitionSiteKind)
            {
                writer.WriteValue(value.ToString());
            }
        }

        private void Write(JsonWriter writer, CoReName coreName)
        {
            writer.WriteValue(coreName.Name);
        }

        private void Write(JsonWriter writer, CallSite callSite)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("kind");
            writer.WriteValue(callSite.kind.ToString());
            writer.WritePropertyName("method");
            Write(writer, callSite.method);
            if (callSite.argIndex != 0)
            {
                writer.WritePropertyName("argIndex");
                writer.WriteValue(callSite.argIndex);
            }
            writer.WriteEndObject();
        }

        private void Write(JsonWriter writer, DefinitionSite defSite)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("kind");
            writer.WriteValue(defSite.kind.ToString());
            if (defSite.field != null)
            {
                writer.WritePropertyName("field");
                Write(writer, defSite.field);
            }
            if (defSite.method != null)
            {
                writer.WritePropertyName("method");
                Write(writer, defSite.method);
            }
            if (defSite.argIndex != 0)
            {
                writer.WritePropertyName("argIndex");
                writer.WriteValue(defSite.argIndex);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var name = reader.Value as string;
                if (objectType == typeof (CoReTypeName))
                {
                    return new CoReTypeName(name);
                }
                if (objectType == typeof (CoReMethodName))
                {
                    return new CoReMethodName(name);
                }
                if (objectType == typeof (CoReFieldName))
                {
                    return new CoReFieldName(name);
                }
                if (objectType == typeof (CallSiteKind))
                {
                    CallSiteKind csk;
                    Enum.TryParse(name, out csk);
                    return csk;
                }
                if (objectType == typeof (DefinitionSiteKind))
                {
                    DefinitionSiteKind dsk;
                    Enum.TryParse(name, out dsk);
                    return dsk;
                }
            }

            if (reader.TokenType == JsonToken.StartObject)
            {
                if (objectType == typeof (CallSite))
                {
                    return ReadCallSite(reader);
                }
                if (objectType == typeof (DefinitionSite))
                {
                    return ReadDefSite(reader);
                }
            }

            throw new AssertException("unknown type or incompatible JSON");
        }

        private CallSite ReadCallSite(JsonReader reader)
        {
            var cs = new CallSite();
            var obj = JObject.Load(reader);

            // kind
            var kindToken = obj.GetValue("kind");
            var kindStr = kindToken.Value<string>();
            CallSiteKind csk;
            Enum.TryParse(kindStr, out csk);
            cs.kind = csk;

            // method
            var methodToken = obj.GetValue("method");
            var methodStr = methodToken.Value<string>();
            if (methodStr != null)
            {
                cs.method = new CoReMethodName(methodStr);
            }

            // argIndex
            var argToken = obj.GetValue("argIndex");
            if (argToken != null)
            {
                cs.argIndex = argToken.Value<int>();
            }

            return cs;
        }

        private DefinitionSite ReadDefSite(JsonReader reader)
        {
            var ds = new DefinitionSite();
            var obj = JObject.Load(reader);

            // kind
            var kindToken = obj.GetValue("kind");
            var kindStr = kindToken.Value<string>();
            DefinitionSiteKind dsk;
            Enum.TryParse(kindStr, out dsk);
            ds.kind = dsk;

            // field
            var fieldToken = obj.GetValue("field");
            if (fieldToken != null)
            {
                var fieldStr = fieldToken.Value<string>();
                if (fieldStr != null)
                {
                    ds.field = new CoReFieldName(fieldStr);
                }
            }

            // method
            var methodToken = obj.GetValue("method");
            if (methodToken != null)
            {
                var methodStr = methodToken.Value<string>();
                if (methodStr != null)
                {
                    ds.method = new CoReMethodName(methodStr);
                }
            }

            // argIndex
            var argToken = obj.GetValue("argIndex");
            if (argToken != null)
            {
                ds.argIndex = argToken.Value<int>();
            }

            return ds;
        }

        public override bool CanConvert(Type objectType)
        {
            var isCoreName = typeof (CoReName).IsAssignableFrom(objectType);
            var isCallSite = typeof (CallSite).IsAssignableFrom(objectType);
            var isDefSite = typeof (DefinitionSite).IsAssignableFrom(objectType);
            var isCallSiteKind = typeof (CallSiteKind).IsAssignableFrom(objectType);
            var isDefSiteKind = typeof (DefinitionSiteKind).IsAssignableFrom(objectType);
            return isCoreName || isCallSite || isDefSite || isCallSiteKind || isDefSiteKind;
        }
    }
}