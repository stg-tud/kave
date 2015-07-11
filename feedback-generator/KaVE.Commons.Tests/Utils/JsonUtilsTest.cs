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
using KaVE.Commons.Utils;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils
{
    [TestFixture]
    internal class JsonUtilsTest
    {
        [TestCase("true", true), TestCase("false", false), TestCase("null", null),
         TestCase("0", 0), TestCase("4.2", 4.2), TestCase("\"text\"", "text")]
        public void ShouldParseSimpleValue(string json, object expected)
        {
            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseEmptyList()
        {
            const string json = "[]";

            var actual = json.ParseJson();

            Assert.AreEqual(new List<object>(), actual);
        }

        [Test]
        public void ShouldParseFlatList()
        {
            const string json = "[0, null, true, false, 4.2, \"text\"]";

            var expected = new List<object>
            {
                0,
                null,
                true,
                false,
                4.2,
                "text"
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseMatrix()
        {
            const string json = "[[1, 0, 0], [0, 1, 0], [0, 0, 1]]";

            var expected = new List<List<object>>
            {
                new List<object> {1, 0, 0},
                new List<object> {0, 1, 0},
                new List<object> {0, 0, 1}
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        public void ShouldParseEmptyObject()
        {
            const string json = "{}";

            var expected = new Dictionary<string, object>();

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseFlatObject()
        {
            const string json = "{\"null-field\":null, \"bool-field\":true, \"num-field\":4.2, \"text-field\":\"text\"}";

            var expected = new Dictionary<string, object>
            {
                {"null-field", null},
                {"bool-field", true},
                {"num-field", 4.2},
                {"text-field", "text"}
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseNestedObject()
        {
            const string json = "{\"subject\":{\"name\":\"Sebastian\"}, \"object\":{\"name\":\"Dennis\"}}";

            var expected = new Dictionary<string, Dictionary<string, object>>
            {
                {"subject", new Dictionary<string, object> {{"name", "Sebastian"}}},
                {"object", new Dictionary<string, object> {{"name", "Dennis"}}}
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseObjectOfLists()
        {
            const string json = "{\"input\":[8, 5, 13, 3, 2], \"sorted\":[2, 3, 5, 8, 13]}";

            var expected = new Dictionary<string, List<object>>
            {
                {"input", new List<object> {8, 5, 13, 3, 2}},
                {"sorted", new List<object> {2, 3, 5, 8, 13}}
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldParseListOfObjects()
        {
            const string json =
                "[{\"name\":\"Dennis\"}, {\"name\":\"Sebastian\"}, {\"name\":\"Sven\"}, {\"name\":\"Uli\"}]";

            var expected = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object> {{"name", "Dennis"}},
                new Dictionary<string, object> {{"name", "Sebastian"}},
                new Dictionary<string, object> {{"name", "Sven"}},
                new Dictionary<string, object> {{"name", "Uli"}}
            };

            var actual = json.ParseJson();

            Assert.AreEqual(expected, actual);
        }

        private static readonly string[] JsonStrings =
        {
            "true",
            "false",
            "null",
            "0",
            "4.2",
            "\"text\"",
            "[]",
            "[0, null, true, false, 4.2, \"text\"]",
            "[[1, 0, 0], [0, 1, 0], [0, 0, 1]]",
            "{}",
            "{\"null-field\":null, \"bool-field\":true, \"num-field\":4.2, \"text-field\":\"text\"}",
            "{\"subject\":{\"name\":\"Sebastian\"}, \"object\":{\"name\":\"Dennis\"}}",
            "{\"input\":[8, 5, 13, 3, 2], \"sorted\":[2, 3, 5, 8, 13]}",
            "[{\"name\":\"Dennis\"}, {\"name\":\"Sebastian\"}, {\"name\":\"Sven\"}, {\"name\":\"Uli\"}]"
        };

        private static IEnumerable<TestCaseData> TestCases()
        {
            for (var i = 0; i < JsonStrings.Length; i++)
            {
                for (var j = 0; j <= i; j++)
                {
                    yield return new TestCaseData(JsonStrings[i], JsonStrings[j], i == j);
                }
            }
        }

        [TestCaseSource("TestCases")]
        public void ShouldRecognizeEquivalentObjects(string json1, string json2, bool expected)
        {
            var compareResult = json1.CompareSerializedObjects(json2, false);
            var actualEquals = compareResult == null || !compareResult.Any();

            Assert.AreEqual(expected, actualEquals);
        }

        [Test]
        public void ShouldRecognizeDifferentlySortedObjects()
        {
            const string json1 = "{\"a\":1, \"b\":2}";
            const string json2 = "{\"b\":2, \"a\":1}";

            CollectionAssert.IsEmpty(json1.CompareSerializedObjects(json2, false));
        }

        [Test]
        public void ShouldRecognizeLargerActualObjectsIfFlagIsSet()
        {
            const string json1 = "{\"a\":1, \"b\":2}";
            const string json2 = "{\"a\":1, \"b\":2, \"c\":3}";

            CollectionAssert.IsEmpty(json1.CompareSerializedObjects(json2, true));
        }
    }
}