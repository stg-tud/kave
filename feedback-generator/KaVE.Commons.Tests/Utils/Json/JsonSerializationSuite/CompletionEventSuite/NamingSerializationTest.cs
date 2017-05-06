/*
 * Copyright 2017 Sebastian Proksch
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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Impl.References;
using KaVE.Commons.Model.SSTs.References;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite
{
    // this is a regression test for the serialization of KaVE classes in names
    // please keep in sync with the equivalent test that exists in Java 
    internal class NamingSerializationTest
    {
        [Test]
        public void MethodsCanBeSafelyDeSerialized_Regular()
        {
            var id = "[p:void] [C,P]..ctor()";
            AssertMethod(id, id);
        }

        private static void AssertMethod(string id, string serId)
        {
            var obj = Names.Method(id);
            var json = obj.ToCompactJson();
            var expected = "\"0M:{0}\"".FormatEx(serId);
            Assert.AreEqual(expected, json);
            var obj2 = json.ParseJsonTo<IMethodName>();
            Assert.AreEqual(obj, obj2);
        }

        private static void AssertType(string id, string serId)
        {
            var obj = Names.Type(id);
            var json = obj.ToCompactJson();
            var expected = "\"0T:{0}\"".FormatEx(serId);
            Assert.AreEqual(expected, json);
            var obj2 = json.ParseJsonTo<ITypeName>();
            Assert.AreEqual(obj, obj2);
        }

        [Test]
        public void MethodsCanBeSafelyDeSerialized_KaVEType()
        {
            var id = "[p:void] [KaVE.Commons.Model.SSTs.Impl.References.MethodReference, KaVE.Commons].M()";
            var serId = "[p:void] [[SST:References.MethodReference]].M()";
            AssertMethod(id, serId);
        }

        [Test]
        public void TypesCanBeSafelyDeSerialized_Regular()
        {
            var id = "C,P";
            AssertType(id, id);
        }

        [Test]
        public void TypesCanBeSafelyDeSerialized_KaVEType()
        {
            var id = "KaVE.Commons.Model.SSTs.Impl.References.MethodReference, KaVE.Commons";
            var serId = "[SST:References.MethodReference]";
            AssertType(id, serId);
        }

        [Test]
        public void RealTypeAnnotationsShouldBeSimplified()
        {
            var obj = new MethodReference
            {
                MethodName = Names.Method(
                    "[KaVE.Commons.Model.SSTs.Impl.Foo.Bar, KaVE.Commons] [KaVE.Commons.Model.SSTs.Impl.Bla.Blubb, KaVE.Commons].M()")
            };
            var json = obj.ToCompactJson();

            var expectedJson =
                "{\"$type\":\"[SST:References.MethodReference]\"," +
                "\"Reference\":{\"$type\":\"[SST:References.VariableReference]\",\"Identifier\":\"\"}," +
                "\"MethodName\":\"0M:[[SST:Foo.Bar]] [[SST:Bla.Blubb]].M()\"}";

            Assert.AreEqual(expectedJson, json);

            var obj2 = json.ParseJsonTo<IMethodReference>();
            Assert.AreEqual(obj, obj2);
        }
    }
}