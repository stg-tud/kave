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

using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json
{
    internal static class JsonAssert
    {
        private delegate void Assertion(object expected, object actual);

        public static void SerializationPreservesReferenceIdentity<T>(T obj) where T : class
        {
            // TODO NameUpdate: change back to Assert.AreSame
            SerializationPreserves(obj, Assert.AreEqual);
        }

        public static void SerializationPreservesData<T>(T obj) where T : class
        {
            SerializationPreserves(obj, Assert.AreEqual);
        }

        private static void SerializationPreserves<T>(T original, Assertion assertion) where T : class
        {
            var clone = SerializeAndDeserialize(original);
            assertion.Invoke(original, clone);
        }

        private static T SerializeAndDeserialize<T>(T model) where T : class
        {
            var json = model.ToCompactJson();
            var modelCopy = json.ParseJsonTo<T>();
            return modelCopy;
        }

        public static void SerializesTo<T>(T instance, string expected)
        {
            var actual = instance.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        public static void DeserializesTo<T>(string json, T expected)
        {
            // there are two ways to deserialize json:

            // 1: for statically defined types
            var actual = json.ParseJsonTo<T>();
            Assert.AreEqual(expected, actual);

            // 2: for types that are provided at runtime
            var actual1 = expected == null ? json.ParseJsonTo(typeof(object)) : json.ParseJsonTo(expected.GetType());
            Assert.AreEqual(expected, actual1);

            // both are tested here
        }
    }
}