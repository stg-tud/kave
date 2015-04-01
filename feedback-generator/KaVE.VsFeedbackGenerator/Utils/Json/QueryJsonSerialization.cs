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

using KaVE.JetBrains.Annotations;
using KaVE.Commons.Model.ObjectUsage;
using KaVE.Commons.Utils.Assertion;
using Newtonsoft.Json;

namespace KaVE.VsFeedbackGenerator.Utils.Json
{
    internal static class QueryJsonSerialization
    {
        private static readonly JsonSerializerSettings QuerySerializationSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new CoReNameToStringConverter(),
                new EnumToStringConverter()
            },
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.None
        };

        /// <summary>
        ///     Parses an instance of Query from a Json string.
        /// </summary>
        /// <remarks>
        ///     Uses the same serialization settings as <see cref="ToJson" />.
        /// </remarks>
        internal static Query ParseJsonToQuery([NotNull] this string json)
        {
            var query = JsonConvert.DeserializeObject<Query>(json, QuerySerializationSettings);
            Asserts.NotNull(query);
            return query;
        }

        /// <summary>
        ///     Converts an object to a Json string without type-information and any unnecessary whitespaces or newlines.
        /// </summary>
        /// <remarks>
        ///     typeless serialization cannot generally be deserialized, because information is lost during serialization.
        /// </remarks>
        [NotNull]
        internal static string ToJson([CanBeNull] this Query instance)
        {
            return JsonConvert.SerializeObject(instance, QuerySerializationSettings);
        }
    }
}