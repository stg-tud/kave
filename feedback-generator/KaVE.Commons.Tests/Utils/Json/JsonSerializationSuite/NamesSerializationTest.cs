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

using KaVE.Commons.Model.Naming;
using KaVE.Commons.Utils.Json;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite
{
    public class NamesSerializationTest
    {
        [Test]
        public void ShouldSerializeNullNames()
        {
            JsonAssert.SerializesTo((IName) null, "null");
        }

        [Test]
        public void ShouldDeserializeNullNames()
        {
            JsonAssert.DeserializesTo("null", (IName) null);
        }

        [Test]
        public void ShouldSerializeName()
        {
            var name = Names.General("Foobar! That's my Name.");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyName()
        {
            var name = Names.Assembly("AssemblyName, 0.8.15.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyVersion()
        {
            var name = Names.AssemblyVersion("1.3.3.7");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeFieldName()
        {
            var name = Names.Field("static [Declarator, B, 9.2.3.8] [Val, G, 5.4.6.3].Field");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeMethodName()
        {
            var name =
                Names.Method(
                    "[Declarator, B, 9.2.3.8] [Val, G, 5.4.6.3].Method(out [Param, P, 8.1.7.2])");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeNamespaceName()
        {
            var name = Names.Namespace("This.Is.My.Namespace");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeTypeName()
        {
            var name = Names.Type("Foo.Bar, foo, 1.0.0.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeTypeParameterName()
        {
            var name = Names.Type("T -> Foo.Bar, foo, 1.0.0.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeInterfaceTypeName()
        {
            var name = Names.Type("i:IMyInterface, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeEnumTypeName()
        {
            var name = Names.Type("e:MyEnum, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeStructTypeName()
        {
            var name = Names.Type("s:MyStruct, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeDelegateTypeName()
        {
            var name = Names.Type("d:[R,P] [MyDelegate, Assembly, 1.2.3.4].()");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeUnknownTypeName()
        {
            var name = Names.UnknownType;
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeArrayTypeName()
        {
            var name = Names.Type("ValueType[], Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeNameToStringContainingTypeAndIdentifier()
        {
            var name = Names.Type("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "\"0T:My.Custom.Type, AnAssembly, 1.5.2.4\"";
            JsonAssert.SerializesTo(name, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat1()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"identifier\":\"MyAssembly, 1.2.3.4\"}";
            var expected = Names.Assembly("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat2()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"id\":\"MyAssembly, 1.2.3.4\"}";
            var expected = Names.Assembly("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat3()
        {
            const string json = "\"CSharp.AssemblyName:MyAssembly, 1.2.3.4\"";
            var expected = Names.Assembly("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldSerializeNameToStringContainingTypeAndIdentifierWhenFormatting()
        {
            var name = Names.Type("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "\"0T:My.Custom.Type, AnAssembly, 1.5.2.4\"";
            var actual = name.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }
    }
}