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
using System.Linq;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KaVE.Commons.Utils
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
        ///     Compares two Json-strings element-wise based on the result of <see cref="ParseJson" />.
        /// </summary>
        public static IList<CompareResult> CompareSerializedObjects([NotNull] this string expected,
            [NotNull] string actual,
            bool actualMayContainAdditionalFields)
        {
            var expectedObj = expected.ParseJson();
            var actualObj = actual.ParseJson();
            return expectedObj.IsEquivalentObject(actualObj, !actualMayContainAdditionalFields);
        }

        private static IList<CompareResult> IsEquivalentObject(this object expected, object actual, bool exact)
        {
            if (expected == null && actual == null)
            {
                return null;
            }
            if (expected == null || actual == null)
            {
                return new List<CompareResult> {new CompareResult(expected, actual)};
            }
            if (expected is IDictionary<string, object> && actual is IDictionary<string, object>)
            {
                var expectedDict = expected as IDictionary<string, object>;
                var actualDict = actual as IDictionary<string, object>;
                if (expectedDict.Keys.Except(actualDict.Keys).Any() ||
                    (exact && actualDict.Keys.Except(expectedDict.Keys).Any()))
                {
                    return new List<CompareResult> {new CompareResult(expected, actual)};
                }
                return
                    expectedDict.Keys.Select(key => expectedDict[key].IsEquivalentObject(actualDict[key], exact))
                                .Where(r => r != null)
                                .SelectMany(i => i)
                                .ToList();
            }
            if (expected is IDictionary<string, object> || actual is IDictionary<string, object>)
            {
                return new List<CompareResult> {new CompareResult(expected, actual)};
            }
            if (expected is IList<object> && actual is IList<object>)
            {
                var expectedList = (expected as IList<object>);
                var actualList = (actual as IList<object>);
                if (expectedList.Count != actualList.Count)
                {
                    return new List<CompareResult> {new CompareResult(expected, actual)};
                }
                return
                    expectedList.Select((t, i) => t.IsEquivalentObject(actualList[i], exact))
                                .Where(r => r != null)
                                .SelectMany(i => i)
                                .ToList();
            }
            if (expected is IList<object> || actual is IList<object>)
            {
                return new List<CompareResult> {new CompareResult(expected, actual)};
            }
            if (expected.Equals(actual))
            {
                return null;
            }
            return new List<CompareResult> {new CompareResult(expected, actual)};
        }
    }

    public class CompareResult
    {
        public object Expected { get; private set; }
        public object Actual { get; private set; }

        public CompareResult(object expected, object actual)
        {
            Expected = expected;
            Actual = actual;
        }

        public override string ToString()
        {
            return string.Format(
                @"expected: ""{0}"" but was: ""{1}""",
                (Expected == null) ? null : Expected.ToString(),
                (Actual == null) ? null : Actual.ToString());
        }
    }
}
