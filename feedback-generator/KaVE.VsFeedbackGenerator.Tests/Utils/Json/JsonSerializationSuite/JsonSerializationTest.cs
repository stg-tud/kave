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
 *    - Sven Amann
 */

using System.Collections.Generic;
using JetBrains.Util;
using KaVE.Model.Names.CSharp;
using KaVE.Model.ObjectUsage;
using KaVE.Utils;
using KaVE.VsFeedbackGenerator.Utils.Json;
using Newtonsoft.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
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
            JsonAssert.SerializesTo(o, expected);
        }

        [Test]
        public void ShouldSerializeObjectWithTypeName()
        {
            var target = new SerializationTestTarget {Id = "42"};
            const string expected = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"42\"}";
            JsonAssert.SerializesTo(target, expected);
        }

        [Test]
        public void ShouldDeserializeObject()
        {
            const string json = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"23\"}";
            var expected = new SerializationTestTarget {Id = "23"};
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldDeserializeObjectWithoutTypeInfo()
        {
            const string json = "{\"$type\":\"" + TestTargetTypeName + "\",\"Id\":\"HalliGalli\"}";
            var expected = new SerializationTestTarget {Id = "HalliGalli"};
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldNotSerializeNullProperty()
        {
            var target = new SerializationTestTarget {Id = null};
            const string expected = "{\"$type\":\"" + TestTargetTypeName + "\"}";
            JsonAssert.SerializesTo(target, expected);
        }

        [Test]
        public void ShouldSerializeEnumByPrimitiveValue()
        {
            const Formatting target = Formatting.None;
            const string expected = "0";
            JsonAssert.SerializesTo(target, expected);
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

        [Test]
        public void ShouldProduceTypelessSerialization()
        {
            var query = new Query
            {
                type = new CoReTypeName("Lusages/Query"),
                classCtx = new CoReTypeName("LContext"),
                methodCtx =
                    new CoReMethodName(
                        "LReceiver.equals(LArgument;)LResult;"),
                definition =
                    new DefinitionSite
                    {
                        kind = DefinitionKind.THIS,
                        method =
                            new CoReMethodName(
                                "LDefiner.define(LScheme;)LPattern;"),
                        field = new CoReFieldName("LField.field;LType"),
                        argIndex = 42
                    }
            };
            query.sites.AddRange(
                new List<CallSite>
                {
                    new CallSite
                    {
                        kind = CallSiteKind.PARAMETER,
                        method =
                            new CoReMethodName(
                                "LCallSite.param(LParam;)LReturn;"),
                        argIndex = 23
                    },
                    new CallSite
                    {
                        kind = CallSiteKind.RECEIVER,
                        method =
                            new CoReMethodName(
                                "LCallSite.param(LParam;)LReturn;"),
                        argIndex = 0
                    }
                });

            const string compare =
                @"{""type"":""Lusages/Query"",""classCtx"":""LContext"",""methodCtx"":""LReceiver.equals(LArgument;)LResult;"",""definition"":{""kind"":""THIS"",""field"":""LField.field;LType"",""method"":""LDefiner.define(LScheme;)LPattern;"",""argIndex"":42},""sites"":[{""kind"":""PARAMETER"",""method"":""LCallSite.param(LParam;)LReturn;"",""argIndex"":23},{""kind"":""RECEIVER"",""method"":""LCallSite.param(LParam;)LReturn;""}]}";

            var actual = query.ToTypelessJson();

            CollectionAssert.IsEmpty(compare.CompareSerializedObjects(actual, true));
        }
    }
}