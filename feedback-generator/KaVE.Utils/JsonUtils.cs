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
using System.Collections.Generic;
using System.Linq;
using KaVE.JetBrains.Annotations;
using KaVE.Utils.Assertion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.Utils
{
    public static class JsonUtils
    {
        /// <summary>
        ///     Parses either a Dictionary, a KeyValuePair, a List or a simple value from a Json string.
        ///     The values of a Dictionary or a KeyValuePair as well as the elements of a List are themselves of one of these
        ///     types.
        ///     The keys of a Dictionary or a KeyValuePair are strings.
        /// </summary>
        public static object ParseJson([NotNull] this string json)
        {
            return ParseJson(JsonConvert.DeserializeObject<JToken>(json));
        }

        private static object ParseJson(JToken token)
        {
            if (token == null)
            {
                return null;
            }
            switch (token.Type)
            {
                case JTokenType.Array:
                    var jArray = token as JArray;
                    Asserts.NotNull(jArray);
                    return jArray.Select(ParseJson).ToList();
                case JTokenType.Object:
                    var jObject = token as JObject;
                    Asserts.NotNull(jObject);
                    return jObject.ToDictionary<KeyValuePair<string, JToken>, string, object>(
                        pair => pair.Key,
                        pair => ParseJson(pair.Value));
                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;
                case JTokenType.Raw:
                case JTokenType.Comment:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Date:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    var jValue = token as JValue;
                    Asserts.NotNull(jValue);
                    return jValue.Value;
                default:
                    Asserts.Fail();
                    break;
            }
            throw new AssertException("");
        }

        /// <summary>
        /// Compares two Json-strings element-wise based on the result of <see cref="ParseJson"/>.
        /// </summary>
        public static bool DescribesEquivalentObject([NotNull] this string json1, [NotNull] string json2)
        {
            var obj1 = json1.ParseJson();
            var obj2 = json2.ParseJson();
            return obj1.IsEquivalentObject(obj2);
        }

        private static bool IsEquivalentObject(this object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }
            if (obj1 == null || obj2 == null)
            {
                return false;
            }
            if (obj1 is IDictionary<string, object> && obj2 is IDictionary<string, object>)
            {
                var dict1 = obj1 as IDictionary<string, object>;
                var dict2 = obj2 as IDictionary<string, object>;
                return !dict1.Keys.Except(dict2.Keys).Any() && !dict2.Keys.Except(dict1.Keys).Any() &&
                       dict1.Keys.All(key => dict1[key].IsEquivalentObject(dict2[key]));
            }
            if (obj1 is IDictionary<string, object> || obj2 is IDictionary<string, object>)
            {
                return false;
            }
            if (obj1 is IList<object> && obj2 is IList<object>)
            {
                var list1 = (obj1 as IList<object>);
                var list2 = (obj2 as IList<object>);
                return list1.Count == list2.Count && !list1.Where((t, i) => !t.IsEquivalentObject(list2[i])).Any();
            }
            if (obj1 is IList<object> || obj2 is IList<object>)
            {
                return false;
            }
            return obj1.Equals(obj2);
        }
    }
}
