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
using KaVE.Model.Names;
using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json
{
    [TestFixture]
    internal class JsonSerializationTest
    {
        private const string TestTargetTypeName =
            "KaVE.VsFeedbackGenerator.Tests.Utils.Json.SerializationTestTarget, KaVE.VsFeedbackGenerator.Tests";

        [Test]
        public void ShouldSerializeNull()
        {
            object o = null;
            const string expected = "null";
            // ReSharper disable once ExpressionIsAlwaysNull
            var actual = o.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeObjectWithTypeName()
        {
            var target = new SerializationTestTarget {Id = "42"};
            const string expected = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"42\"}";
            var actual = target.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeserializeObject()
        {
            const string json = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"23\"}";
            var expected = new SerializationTestTarget {Id = "23"};
            var actual = json.ParseJsonTo<SerializationTestTarget>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeserializeObjectWithoutTypeInfo()
        {
            const string json = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"HalliGalli\"}";
            var expected = new SerializationTestTarget {Id = "HalliGalli"};
            var actual = json.ParseJsonTo<object>();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotSerializeNullProperty()
        {
            var target = new SerializationTestTarget {Id = null};
            const string expected = "{\"$type\":\"" + TestTargetTypeName + "\"}";
            var actual = target.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeNameToTypeAndIdentifier()
        {
            var name = TypeName.Get("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "{\"type\":\"CSharp.TypeName\",\"id\":\"My.Custom.Type, AnAssembly, 1.5.2.4\"}";
            var actual = name.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldDeserializeNameFromNewFormat()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"id\":\"MyAssembly, 1.2.3.4\"}";
            var expected = AssemblyName.Get("MyAssembly, 1.2.3.4");
            var actual = json.ParseJsonTo<IName>();
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ShouldDeserializeNameFromOldFormat()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"identifier\":\"MyAssembly, 1.2.3.4\"}";
            var expected = AssemblyName.Get("MyAssembly, 1.2.3.4");
            var actual = json.ParseJsonTo<IName>();
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ShouldSerializeEnumByPrimitiveValue()
        {
            const Formatting target = Formatting.None;
            const string expected = "0";
            var actual = target.ToCompactJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeFormatted()
        {
            var target = new SerializationTestTarget {Id = "666"};
            const string expected = "{\r\n" +
                                    "    \"$type\": \"" + TestTargetTypeName + "\",\r\n" +
                                    "    \"Id\": \"666\"\r\n" +
                                    "}";
            var actual = target.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldNotSerializeNullPropertyWhenFormatting()
        {
            var target = new SerializationTestTarget {Id = null};
            const string expected = "{\r\n" +
                                    "    \"$type\": \"" + TestTargetTypeName + "\"\r\n" +
                                    "}";
            var actual = target.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeNameToTypeAndIdentifierWhenFormatting()
        {
            var name = TypeName.Get("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "{\r\n" +
                                    "    \"type\": \"CSharp.TypeName\",\r\n" +
                                    "    \"id\": \"My.Custom.Type, AnAssembly, 1.5.2.4\"\r\n" +
                                    "}";
            var actual = name.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldSerializeEnumByPrimitiveValueWhenFormatting()
        {
            const Formatting target = Formatting.None;
            const string expected = "0";
            var actual = target.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldPrettyPrintObject()
        {
            var target = new SerializationTestTarget {Id = "Foo"};
            const string expected = "{\r\n" +
                                    "    \"Id\": \"Foo\"\r\n" +
                                    "}";
            var actual = target.ToPrettyPrintJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldPrettyPrintName()
        {
            var target = TypeName.Get("Raba, Rababa, 9.5.7.4");
            const string expected = "\"Raba, Rababa, 9.5.7.4\"";
            var actual = target.ToPrettyPrintJson();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldPrettyPrintEnum()
        {
            const Formatting target = Formatting.Indented;
            const string expected = "\"Indented\"";
            var actual = target.ToPrettyPrintJson();
            Assert.AreEqual(expected, actual);
        }
    }
}