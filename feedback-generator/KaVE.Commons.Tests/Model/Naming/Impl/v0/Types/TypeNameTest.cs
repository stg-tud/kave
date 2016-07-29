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
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Collections;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class TypeNameTest
    {
        private const string TestAssemblyIdentifier = "a, 1.0.0.0";

        private static ITypeName T(string id = null)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            return TypeUtils.CreateTypeName(id);
        }

        [Test]
        public void DefaultValues()
        {
            var sut = new TypeName();

            Assert.AreEqual("?", sut.FullName);
            Assert.AreEqual("?", sut.Name);
            Assert.AreEqual(new NamespaceName(), sut.Namespace);
            Assert.AreEqual(new AssemblyName(), sut.Assembly);

            Assert.IsTrue(sut.IsUnknown);
            Assert.IsFalse(sut.IsHashed);

            Assert.IsFalse(sut.IsArray);
            Assert.IsFalse(sut.IsClassType);
            Assert.IsFalse(sut.IsDelegateType);
            Assert.IsFalse(sut.IsEnumType);
            Assert.IsFalse(sut.IsInterfaceType);
            Assert.IsFalse(sut.IsNestedType);
            Assert.IsFalse(sut.IsNullableType);
            Assert.IsFalse(sut.IsReferenceType);
            Assert.IsFalse(sut.IsSimpleType);
            Assert.IsFalse(sut.IsStructType);
            Assert.IsFalse(sut.IsTypeParameter);
            Assert.IsFalse(sut.IsValueType);
            Assert.IsFalse(sut.IsVoidType);

            Assert.Null(sut.DeclaringType);
            Assert.IsFalse(sut.HasTypeParameters);
            Assert.AreEqual(Lists.NewList<ITypeParameterName>(), sut.TypeParameters);
        }


        public IEnumerable<string[]> ValidTypes()
        {
            // use only ',P' Assemblies
            var ids = Sets.NewHashSet<string[]>();
            ids.Add(new[] {"T,P", "P", "", "T", "T"});
            ids.Add(new[] {"n.T,P", "P", "n", "n.T", "T"});
            ids.Add(new[] {"s:T,P", "P", "", "T", "T"});
            ids.Add(new[] {"s:n.T,P", "P", "n", "n.T", "T"});
            ids.Add(new[] {"e:T,P", "P", "", "T", "T"});
            ids.Add(new[] {"e:n.T,P", "P", "n", "n.T", "T"});
            ids.Add(new[] {"i:T,P", "P", "", "T", "T"});
            ids.Add(new[] {"i:n.T,P", "P", "n", "n.T", "T"});
            ids.Add(new[] {"n.T1`1[[T2]], P", "P", "n", "n.T1`1[[T2]]", "T1"});
            ids.Add(new[] {"n.T1+T2, P", "P", "n", "n.T1+T2", "T2"});
            ids.Add(new[] {"n.T1`1[[T2]]+T3`1[[T4]], P", "P", "n", "n.T1`1[[T2]]+T3`1[[T4]]", "T3"});
            return ids;
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldRecognizeRegularTypes(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            Assert.IsFalse(TypeUtils.IsUnknownTypeIdentifier(typeId));
            Assert.IsFalse(ArrayTypeName.IsArrayTypeNameIdentifier(typeId));
            Assert.IsFalse(DelegateTypeName.IsDelegateTypeNameIdentifier(typeId));
            Assert.IsFalse(TypeParameterName.IsTypeParameterNameIdentifier(typeId));
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldParseAssembly(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            var onlyTypeId = typeId.Substring(0, typeId.LastIndexOf(','));
            foreach (var asmId in new[] {"P", "A, 1.2.3.4"})
            {
                typeId = "{0}, {1}".FormatEx(onlyTypeId, asmId);
                Assert.AreEqual(new AssemblyName(asmId), T(typeId).Assembly);
            }
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldParseNamespace(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            Assert.AreEqual(new NamespaceName(namespaceId), T(typeId).Namespace);
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldParseFullName(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            Assert.AreEqual(fullName, T(typeId).FullName);
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldParseName(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            Assert.AreEqual(name, T(typeId).Name);
        }

        [TestCaseSource("ValidTypes")]
        public void ShouldParseAllChecker(string typeId,
            string assemblyId,
            string namespaceId,
            string fullName,
            string name)
        {
            var isClassType = !(typeId.StartsWith("e:") || typeId.StartsWith("i:") || typeId.StartsWith("s:"));
            var isReferenceType = !(typeId.StartsWith("e:") || typeId.StartsWith("s:"));
            var isValueType = typeId.StartsWith("e:") || typeId.StartsWith("s:");

            var sut = T(typeId);

            Assert.IsFalse(sut.IsUnknown);
            Assert.IsFalse(sut.IsHashed);

            Assert.IsFalse(sut.IsArray);

            Assert.AreEqual(isClassType, sut.IsClassType);
            Assert.IsFalse(sut.IsDelegateType);
            Assert.AreEqual(typeId.StartsWith("e:"), sut.IsEnumType);
            Assert.AreEqual(typeId.StartsWith("i:"), sut.IsInterfaceType);
            Assert.AreEqual(typeId.Contains("+"), sut.IsNestedType);
            Assert.IsFalse(sut.IsNullableType);
            Assert.AreEqual(isReferenceType, sut.IsReferenceType);
            Assert.IsFalse(sut.IsSimpleType);
            Assert.AreEqual(typeId.StartsWith("s:"), sut.IsStructType);
            Assert.IsFalse(sut.IsTypeParameter);
            Assert.AreEqual(isValueType, sut.IsValueType);
            Assert.IsFalse(sut.IsVoidType);

            if (typeId.Contains("+"))
            {
                Assert.NotNull(sut.DeclaringType);
            }
            else
            {
                Assert.Null(sut.DeclaringType);
            }
            Assert.AreEqual(typeId.Contains("`"), sut.HasTypeParameters);
        }

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(T().IsUnknown);
            Assert.That(T("?").IsUnknown);
            Assert.That(TypeUtils.IsUnknownTypeIdentifier(T().Identifier));
        }

        [Test]
        public void ShouldGetNamespaceFromTypeInGlobalNamespace()
        {
            var uut = T("MyType, Assembly, 1.2.3.4");

            Assert.AreEqual(new NamespaceName(""), uut.Namespace);
        }

        [TestCase("System.Boolean, mscorlib, 4.0.0.0"),
         TestCase("System.Decimal, mscorlib, 4.0.0.0"),
         TestCase("System.SByte, mscorlib, 4.0.0.0"),
         TestCase("System.Byte, mscorlib, 4.0.0.0"),
         TestCase("System.Int16, mscorlib, 4.0.0.0"),
         TestCase("System.UInt16, mscorlib, 4.0.0.0"),
         TestCase("System.Int32, mscorlib, 4.0.0.0"),
         TestCase("System.UInt32, mscorlib, 4.0.0.0"),
         TestCase("System.Int64, mscorlib, 4.0.0.0"),
         TestCase("System.UInt64, mscorlib, 4.0.0.0"),
         TestCase("System.Char, mscorlib, 4.0.0.0"),
         TestCase("System.Single, mscorlib, 4.0.0.0"),
         TestCase("System.Double, mscorlib, 4.0.0.0"),
         TestCase("System.Void, mscorlib, 4.0.0.0")]
        public void ShouldRecognizeBuiltInDataStructures(string identifer)
        {
            var sut = T(identifer);

            Assert.IsFalse(sut.IsUnknown);
            Assert.IsFalse(sut.IsHashed);

            Assert.IsFalse(sut.IsArray);
            Assert.IsFalse(sut.IsClassType);
            Assert.IsFalse(sut.IsDelegateType);
            Assert.IsFalse(sut.IsEnumType);
            Assert.IsFalse(sut.IsInterfaceType);
            Assert.IsFalse(sut.IsNestedType);
            Assert.IsFalse(sut.IsNullableType);
            Assert.IsFalse(sut.IsReferenceType);
            Assert.AreEqual(!identifer.Contains("Void"), sut.IsSimpleType);
            Assert.IsTrue(sut.IsStructType);
            Assert.IsFalse(sut.IsTypeParameter);
            Assert.IsTrue(sut.IsValueType);
            Assert.AreEqual(identifer.Contains("Void"), sut.IsVoidType);

            Assert.IsFalse(sut.HasTypeParameters);
        }

        [TestCase("System.Nullable`1[[T]], mscorlib, 4.0.0.0"),
         TestCase("System.Nullable`1[[T -> System.UInt64, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0")]
        public void ShouldBeNullableType(string identifier)
        {
            Assert.IsTrue(T(identifier).IsNullableType);
        }


        [TestCase("System.Void, mscorlib, 4.0.0.0"),
         TestCase("System.Int32, mscorlib, 4.0.0.0"),
         TestCase("System.Nullable`1[[T -> System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0"),
         TestCase("s:My.Struct, A, 1.0.0.0")]
        public void ShouldRecognizeStructTypes(string identifier)
        {
            Assert.IsTrue(T(identifier).IsStructType);
        }

        [TestCase("System.Int32, mscorlib, 4.0.0.0"),
         TestCase("s:My.Struct, A, 1.0.0.0"),
         TestCase("System.Void, mscorlib, 4.0.0.0"),
         TestCase("e:My.Enumtype, A, 3.4.5.6")]
        public void ShouldBeValueType(string identifier)
        {
            var uut = T(identifier);

            Assert.IsTrue(uut.IsValueType);
        }

        [TestCase("i:Some.Interface, I, 6.5.4.3")]
        public void ShouldBeInterfaceType(string identifier)
        {
            var uut = T(identifier);

            Assert.IsTrue(uut.IsInterfaceType);
        }

        [Test]
        public void ShouldNotBeInterfaceType()
        {
            var uut = T("Some.Class, C, 3.2.1.0");

            Assert.IsFalse(uut.IsInterfaceType);
        }

        [TestCase(""),
         TestCase("?")]
        public void ShouldBeUnknownType(string identifier)
        {
            var uut = T(identifier);

            Assert.IsTrue(uut.IsUnknown);
        }

        [Test]
        public void ShouldNotBeUnknownType()
        {
            var uut = T("Some.Known.Type, A, 1.2.3.4");

            Assert.IsFalse(uut.IsUnknown);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "System.UInt16"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Full.Enum.Type"),
         TestCase("i:Full.Interface.Type, E, 1.2.3.4", "Full.Interface.Type"),
         TestCase("s:Full.Struct.Type, E, 1.2.3.4", "Full.Struct.Type"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0",
             "System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]]"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "Outer.Type+InnerType"),
         TestCase("?", "?"),
         TestCase("Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0",
             "Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]]")]
        public void ShouldDetermineFullName(string identifier, string expectedFullName)
        {
            var uut = T(identifier);

            Assert.AreEqual(expectedFullName, uut.FullName);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "UInt16"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Type"),
         TestCase("i:Full.Interface.Type, E, 1.2.3.4", "Type"),
         TestCase("s:Full.Struct.Type, E, 1.2.3.4", "Type"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "Nullable"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "InnerType"),
         TestCase("?", "?"),
         TestCase("Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "Task")]
        public void ShouldDetermineName(string identifier, string expectedName)
        {
            var uut = T(identifier);

            Assert.AreEqual(expectedName, uut.Name);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "System"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Full.Enum"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "System"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "Outer"),
         TestCase("GlobalType, A, 5.6.7.4", "")]
        public void ShouldDetermineNamespace(string identifier, string expectedNamespace)
        {
            var uut = T(identifier);

            Assert.AreEqual(new NamespaceName(expectedNamespace), uut.Namespace);
        }

        [TestCase("System.Object, mscorlib, 4.0.0.0", "mscorlib, 4.0.0.0"),
         TestCase("i:Some.Interface, I, 1.2.3.4", "I, 1.2.3.4")]
        public void ShouldDetermineAssembly(string identifier, string expectedAssemblyIdentifier)
        {
            var uut = T(identifier);

            Assert.AreEqual(expectedAssemblyIdentifier, uut.Assembly.Identifier);
        }

        [Test]
        public void ShouldHaveTypeParameters()
        {
            const string stringIdentifier = "S -> System.String, mscore, 4.0.0.0";
            const string intIdentifier = "I -> System.Int32, mscore, 4.0.0.0";

            var parameterizedTypeName =
                T(
                    "pack.age.MyType`2[[" + stringIdentifier + "],[" + intIdentifier + "]], " + TestAssemblyIdentifier);

            Assert.AreEqual(TestAssemblyIdentifier, parameterizedTypeName.Assembly.Identifier);
            Assert.AreEqual("MyType", parameterizedTypeName.Name);
            Assert.IsTrue(parameterizedTypeName.HasTypeParameters);
            Assert.IsFalse(parameterizedTypeName.IsArray);
            Assert.AreEqual(2, parameterizedTypeName.TypeParameters.Count);
            Assert.AreEqual(stringIdentifier, parameterizedTypeName.TypeParameters[0].Identifier);
            Assert.AreEqual(intIdentifier, parameterizedTypeName.TypeParameters[1].Identifier);
        }

        [Test]
        public void ShouldBeTopLevelType()
        {
            var typeName = T("this.is.a.top.level.ValueType, " + TestAssemblyIdentifier);

            Assert.IsFalse(typeName.IsNestedType);
            Assert.IsNull(typeName.DeclaringType);
        }

        [Test]
        public void ShouldBeParameterizedTypeWithParameterizedTypeParameter()
        {
            const string paramParamIdentifier = "T -> x.A, P1";
            const string paramIdentifier = "y.B`1[[" + paramParamIdentifier + "]], P2";

            var typeName =
                T("z.C`1[[" + paramIdentifier + "]], P3");

            Assert.IsTrue(typeName.HasTypeParameters);
            Assert.AreEqual(1, typeName.TypeParameters.Count);
            var typeParameter = typeName.TypeParameters[0];

            Assert.AreEqual(new TypeParameterName(paramIdentifier), typeParameter);
        }

        [TestCase("TR -> System.Int32, mscorelib, 4.0.0.0"),
         TestCase("R -> ?"),
         TestCase("TParam")]
        public void ShouldBeTypeParameter(string identifier)
        {
            var uut = T(identifier);

            Assert.IsTrue(uut.IsTypeParameter);
        }

        [TestCase("TR -> System.Int32, mscorelib, 4.0.0.0", "TR"),
         TestCase("R -> ?", "R"),
         TestCase("TParam", "TParam")]
        public void ShouldExtractTypeParameterShortName(string identifier, string expectedShortName)
        {
            var uut = T(identifier);

            Assert.AreEqual(expectedShortName, ((ITypeParameterName) uut).TypeParameterShortName);
        }

        [Test]
        public void TypeParameterTypeShouldBeConcreteType()
        {
            var uut = T("T -> System.String, mscorlib, 4.0.0.0");

            Assert.AreEqual(false, ((ITypeParameterName) uut).TypeParameterType.IsTypeParameter);
        }

        [Test]
        public void TypeParameterTypeShouldBeShortName()
        {
            var uut = T("T -> T");

            Assert.AreEqual(true, ((ITypeParameterName) uut).TypeParameterType.IsTypeParameter);
        }

        [Test]
        public void RegularTypeIsNoTypeParameter()
        {
            var uut = T("Non.Parameter.Type, As, 1.2.3.4");

            Assert.False(uut.IsTypeParameter);
        }

        [TestCase("SomeType`1[[T -> Foo, Bar, 1.2.3.4]], A, 1.2.3.4"),
         TestCase("System.Object, mscorlib, 4.0.0.0")]
        public void ShouldBeNotTypeParameter(string identifier)
        {
            var genericType = T(identifier);

            Assert.IsFalse(genericType.IsTypeParameter);
        }

        [TestCase("a.b.TC`1[[T1]]+NC`1[[T2]], P", "a.b.TC`1[[T1]]+NC`1[[T2]]", "NC"),
         TestCase("e:n.E,P", "n.E", "E"),
         TestCase("n.T,P", "n.T", "T"),
         TestCase("n.T[],P", "n.T[]", "T[]"),
         TestCase("d:[?] [T+D,P].()", "T+D", "D"),
         TestCase("n.T`1[T2],P", "n.T`1[T2]", "T")
        ]
        public void ShouldParseFullAndSimpleNames(string typeId, string fullName, string simpleName)
        {
            var sut = T(typeId);
            Assert.AreEqual(fullName, sut.FullName);
            Assert.AreEqual(simpleName, sut.Name);
        }

        [Test]
        public void TypeParameterParsingIsCached()
        {
            var sut = T("T,P");
            var a = sut.TypeParameters;
            var b = sut.TypeParameters;
            Assert.AreSame(a, b);
        }

        [TestCaseSource("NestedTypes")]
        public void ShouldParseDeclaringTypes(string typeId, string declTypeId)
        {
            var sut = T(typeId);
            Assert.IsTrue(sut.IsNestedType);
            Assert.AreEqual(T(declTypeId), sut.DeclaringType);
        }

        public string[][] NestedTypes()
        {
            return new[]
            {
                new[] {"n.T1+T2, P", "n.T1, P"},
                new[] {"d:[?] [n.T1+T2, P].()", "n.T1, P"},
                new[] {"e:n.T1+E, P", "n.T1, P"},
                new[] {"i:n.T1+I, P", "n.T1, P"},
                new[] {"s:n.T1+S, P", "n.T1, P"},
                new[] {"n.T1`1[[G1]]+T2`1[[G2]], P", "n.T1`1[[G1]], P"},
                new[] {"a.p.T+N, P", "a.p.T, P"},
                new[] {"N.O+M+I, P", "N.O+M, P"},
                new[] {"n.T+A`1[[T1 -> e:n.T+B, P]], P", "n.T, P"}
            };
        }
    }
}