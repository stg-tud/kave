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

using KaVE.Model.Names.CSharp;
using KaVE.VsFeedbackGenerator.Utils.Json;
using NUnit.Framework;

namespace KaVE.VsFeedbackGenerator.Tests.Utils.Json.JsonSerializationSuite
{
    [TestFixture]
    public class NamesSerializationTest
    {
        [Test]
        public void ShouldSerializeName()
        {
            var name = Name.Get("Foobar! That's my Name.");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyName()
        {
            var name = AssemblyName.Get("AssemblyName, 0.8.15.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeAssemblyVersion()
        {
            var name = AssemblyVersion.Get("1.3.3.7");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeFieldName()
        {
            var name = FieldName.Get("static [Declarator, B, 9.2.3.8] [Val, G, 5.4.6.3].Field");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeMethodName()
        {
            var name =
                MethodName.Get(
                    "[Declarator, B, 9.2.3.8] [Val, G, 5.4.6.3].Method(out [Param, P, 8.1.7.2])");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeNamespaceName()
        {
            var name = NamespaceName.Get("This.Is.My.Namespace");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeTypeName()
        {
            var name = TypeName.Get("Foo.Bar, foo, 1.0.0.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeTypeParameterName()
        {
            var name = TypeName.Get("T -> Foo.Bar, foo, 1.0.0.0");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeInterfaceTypeName()
        {
            var name = TypeName.Get("i:IMyInterface, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeEnumTypeName()
        {
            var name = TypeName.Get("e:MyEnum, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeStructTypeName()
        {
            var name = TypeName.Get("s:MyStruct, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeDelegateTypeName()
        {
            var name = TypeName.Get("d:MyDelegate, Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeUnknownTypeName()
        {
            var name = UnknownTypeName.Instance;
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeArrayTypeName()
        {
            var name = TypeName.Get("ValueType[], Assembly, 1.2.3.4");
            JsonAssert.SerializationPreservesReferenceIdentity(name);
        }

        [Test]
        public void ShouldSerializeNameToStringContainingTypeAndIdentifier()
        {
            var name = TypeName.Get("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "\"CSharp.TypeName:My.Custom.Type, AnAssembly, 1.5.2.4\"";
            JsonAssert.SerializesTo(name, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat1()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"identifier\":\"MyAssembly, 1.2.3.4\"}";
            var expected = AssemblyName.Get("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat2()
        {
            const string json = "{\"type\":\"CSharp.AssemblyName\",\"id\":\"MyAssembly, 1.2.3.4\"}";
            var expected = AssemblyName.Get("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldDeserializeNameFromFormat3()
        {
            const string json = "\"CSharp.AssemblyName:MyAssembly, 1.2.3.4\"";
            var expected = AssemblyName.Get("MyAssembly, 1.2.3.4");
            JsonAssert.DeserializesTo(json, expected);
        }

        [Test]
        public void ShouldSerializeNameToStringContainingTypeAndIdentifierWhenFormatting()
        {
            var name = TypeName.Get("My.Custom.Type, AnAssembly, 1.5.2.4");
            const string expected = "\"CSharp.TypeName:My.Custom.Type, AnAssembly, 1.5.2.4\"";
            var actual = name.ToFormattedJson();
            Assert.AreEqual(expected, actual);
        }
    }
}