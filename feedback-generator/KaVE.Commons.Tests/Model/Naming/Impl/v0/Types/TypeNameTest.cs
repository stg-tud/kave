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

using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Types;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
    internal class TypeNameTest
    {
        private const string TestAssemblyIdentifier = "a, 1.0.0.0";

        [Test]
        public void ShouldImplementIsUnknown()
        {
            Assert.That(new TypeName().IsUnknown);
        }

        [Test]
        public void ShouldGetNamespaceFromTypeInGlobalNamespace()
        {
            var uut = TypeUtils.CreateTypeName("MyType, Assembly, 1.2.3.4");

            Assert.AreEqual(new NamespaceName(""), uut.Namespace);
        }

        [Test]
        public void ShouldBeVoidType()
        {
            var uut = TypeUtils.CreateTypeName("System.Void, mscorlib, 4.0.0.0");

            Assert.IsTrue(uut.IsVoidType);
        }

        [TestCase("System.Boolean, mscorlib, 4.0.0.0"),
         TestCase("T -> System.Int32, mscorlib, 4.0.0.0")]
        public void ShouldNotBeVoidType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsVoidType);
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
         TestCase("T -> System.Int32, mscorlib, 4.0.0.0")]
        public void ShouldBeSimpleType(string identifer)
        {
            var uut = TypeUtils.CreateTypeName(identifer);

            Assert.IsTrue(uut.IsSimpleType);
        }

        [TestCase("System.Void, mscorlib, 4.0.0.0"),
         TestCase("My.Custom.Type, A, 1.2.3.4"),
         TestCase("?")]
        public void ShouldNotBeSimpleType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsSimpleType);
        }

        [TestCase("System.Nullable`1[[T -> System.UInt64, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0"),
         TestCase("T -> System.Nullable`1[[T -> System.UInt64, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0")]
        public void ShouldBeNullableType(string identifier)
        {
            var nullableTypeName = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(nullableTypeName.IsNullableType);
        }

        [TestCase("System.UInt64, mscorlib, 4.0.0.0"),
         TestCase("A.Type, B, 5.6.7.8"),
         TestCase("?")]
        public void ShouldNotBeNullableType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsNullableType);
        }


        [TestCase("System.Void, mscorlib, 4.0.0.0"),
         TestCase("System.Int32, mscorlib, 4.0.0.0"),
         TestCase("System.Nullable`1[[T -> System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0"),
         TestCase("s:My.Struct, A, 1.0.0.0")]
        public void ShouldBeStructType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsStructType);
        }

        [TestCase("My.Type, C, 9.0.1.2"),
         TestCase("?")]
        public void ShouldNotBeStructType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsStructType);
        }

        [Test]
        public void ShouldBeEnumType()
        {
            var uut = TypeUtils.CreateTypeName("e:My.EnumType, E, 3.9.5.6");

            Assert.IsTrue(uut.IsEnumType);
        }

        [Test]
        public void ShouldNotBeEnumType()
        {
            var uut = TypeUtils.CreateTypeName("Non.Enum.Type, E, 7.9.3.5");

            Assert.IsFalse(uut.IsEnumType);
        }

        [TestCase("System.Int32, mscorlib, 4.0.0.0"),
         TestCase("s:My.Struct, A, 1.0.0.0"),
         TestCase("System.Void, mscorlib, 4.0.0.0"),
         TestCase("e:My.Enumtype, A, 3.4.5.6"),
         TestCase("T -> System.Boolean, mscorlib, 4.0.0.0")]
        public void ShouldBeValueType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsValueType);
        }

        [TestCase("A.ReferenceType, G, 7.8.9.0"),
         TestCase("?")]
        public void ShouldNotBeValueType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsValueType);
        }

        [TestCase("i:Some.Interface, I, 6.5.4.3"),
         TestCase("TI -> i:MyInterface, Is, 3.8.67.0")]
        public void ShouldBeInterfaceType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsInterfaceType);
        }

        [Test]
        public void ShouldNotBeInterfaceType()
        {
            var uut = TypeUtils.CreateTypeName("Some.Class, C, 3.2.1.0");

            Assert.IsFalse(uut.IsInterfaceType);
        }

        [TestCase(""),
         TestCase("?")]
        public void ShouldBeUnknownType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsUnknown);
        }

        [Test]
        public void ShouldNotBeUnknownType()
        {
            var uut = TypeUtils.CreateTypeName("Some.Known.Type, A, 1.2.3.4");

            Assert.IsFalse(uut.IsUnknown);
        }

        [TestCase("System.Object, mscorlib, 4.0.0.0"),
         TestCase("Some.Class, K, 0.9.8.7"),
         TestCase("T -> Another.Class, F, 4.7.55.6")]
        public void ShouldBeClassType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsClassType);
        }

        [TestCase("System.Boolean, mscorlib, 4.0.0.0"),
         TestCase("i:My.TerfaceType, Is, 2.4.6.3"),
         TestCase("Foo[], A, 5.3.6.7"),
         TestCase("?")]
        public void ShouldNotBeClassType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsClassType);
        }

        [TestCase("My.Namespace.TypeName, A, 3.5.7.9"),
         TestCase("i:My.Nterface, I, 5.3.7.1"),
         TestCase("Vt[], A, 5.2.7.8")]
        public void ShouldBeReferenceType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsReferenceType);
        }

        [TestCase("System.Int64, mscorlib, 4.0.0.0"),
         TestCase("?")]
        public void ShouldNotBeReferenceType(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(uut.IsReferenceType);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "System.UInt16"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Full.Enum.Type"),
         TestCase("d:Full.Delegate.Type, E, 1.2.3.4", "Full.Delegate.Type"),
         TestCase("i:Full.Interface.Type, E, 1.2.3.4", "Full.Interface.Type"),
         TestCase("s:Full.Struct.Type, E, 1.2.3.4", "Full.Struct.Type"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0",
             "System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]]"),
         TestCase("T -> Some.Arbitrary.Type, Assembly, 5.6.4.7", "Some.Arbitrary.Type"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "Outer.Type+InnerType"),
         TestCase("?", "?"),
         TestCase("Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0",
             "Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]]")]
        public void ShouldDetermineFullName(string identifier, string expectedFullName)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.AreEqual(expectedFullName, uut.FullName);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "UInt16"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Type"),
         TestCase("d:Full.Delegate.Type, E, 1.2.3.4", "Type"),
         TestCase("i:Full.Interface.Type, E, 1.2.3.4", "Type"),
         TestCase("s:Full.Struct.Type, E, 1.2.3.4", "Type"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "Nullable"),
         TestCase("T -> Some.Arbitrary.Type, Assembly, 5.6.4.7", "Type"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "InnerType"),
         TestCase("?", "?"),
         TestCase("Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "Task")]
        public void ShouldDetermineName(string identifier, string expectedName)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.AreEqual(expectedName, uut.Name);
        }

        [TestCase("System.UInt16, mscorlib, 4.0.0.0", "System"),
         TestCase("e:Full.Enum.Type, E, 1.2.3.4", "Full.Enum"),
         TestCase("System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0", "System"),
         TestCase("T -> Some.Arbitrary.Type, Assembly, 5.6.4.7", "Some.Arbitrary"),
         TestCase("Outer.Type+InnerType, As, 1.2.3.4", "Outer"),
         TestCase("GlobalType, A, 5.6.7.4", "")]
        public void ShouldDetermineNamespace(string identifier, string expectedNamespace)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.AreEqual(new NamespaceName(expectedNamespace), uut.Namespace);
        }

        [Test]
        public void UnknownTypeShouldHaveUnknownNamespace()
        {
            Assert.AreEqual(new NamespaceName(), new TypeName().Namespace);
        }

        [TestCase("System.Object, mscorlib, 4.0.0.0", "mscorlib, 4.0.0.0"),
         TestCase("i:Some.Interface, I, 1.2.3.4", "I, 1.2.3.4"),
         TestCase("T -> Type.Parameter, A, 1.2.3.4", "A, 1.2.3.4")]
        public void ShouldDetermineAssembly(string identifier, string expectedAssemblyIdentifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.AreEqual(expectedAssemblyIdentifier, uut.Assembly.Identifier);
        }

        [Test]
        public void UnknownTypeShouldHaveUnknownAssembly()
        {
            Assert.AreEqual(new AssemblyName(), new TypeName().Assembly);
        }

        [Test]
        public void ShouldHaveTypeParameters()
        {
            const string stringIdentifier = "S -> System.String, mscore, 4.0.0.0";
            const string intIdentifier = "I -> System.Int32, mscore, 4.0.0.0";

            var parameterizedTypeName =
                TypeUtils.CreateTypeName(
                    "pack.age.MyType`2[[" + stringIdentifier + "],[" + intIdentifier + "]], " + TestAssemblyIdentifier);

            Assert.AreEqual(TestAssemblyIdentifier, parameterizedTypeName.Assembly.Identifier);
            Assert.AreEqual("MyType", parameterizedTypeName.Name);
            Assert.IsTrue(parameterizedTypeName.IsGenericEntity);
            Assert.IsTrue(parameterizedTypeName.HasTypeParameters);
            Assert.IsFalse(parameterizedTypeName.IsArrayType);
            Assert.AreEqual(2, parameterizedTypeName.TypeParameters.Count);
            Assert.AreEqual(stringIdentifier, parameterizedTypeName.TypeParameters[0].Identifier);
            Assert.AreEqual(intIdentifier, parameterizedTypeName.TypeParameters[1].Identifier);
        }

        [Test]
        public void ShouldHaveUninstantiatedTypeParameters()
        {
            var typeName = TypeUtils.CreateTypeName("OuterType`1+InnerType, Assembly, 1.2.3.4");

            Assert.IsTrue(typeName.IsGenericEntity);
            Assert.IsFalse(typeName.HasTypeParameters);
            Assert.AreEqual("OuterType`1+InnerType", typeName.FullName);
            Assert.AreEqual("OuterType`1", typeName.DeclaringType.FullName);
        }

        [Test]
        public void ShouldBeTopLevelType()
        {
            var typeName = TypeUtils.CreateTypeName("this.is.a.top.level.ValueType, " + TestAssemblyIdentifier);

            Assert.IsFalse(typeName.IsNestedType);
            Assert.IsNull(typeName.DeclaringType);
        }

        [TestCase("a.p.T+N", "a.p.T"),
         TestCase("N.O+M+I", "N.O+M"),
         TestCase("n.T+A`1[[T1 -> e:n.T+B, P]]", "n.T"),
         TestCase("n.T`1+U`1[[T2 -> T2]]", "n.T`1[[T2 -> T2]]")]
        // obviously an error in the analysis, second typeparam missing
        public void ShouldBeNestedType(string nestedTypeFullName, string expectedDeclaringTypeFullName)
        {
            var expected = expectedDeclaringTypeFullName.Equals("?")
                ? new TypeName()
                : TypeUtils.CreateTypeName(expectedDeclaringTypeFullName + ", " + TestAssemblyIdentifier);

            var nestedTypeName = TypeUtils.CreateTypeName(nestedTypeFullName + ", " + TestAssemblyIdentifier);
            var actual = nestedTypeName.DeclaringType;

            Assert.IsTrue(nestedTypeName.IsNestedType);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ShouldBeNestedTypeInParameterizedType()
        {
            const string paramIdentifier = "T -> p.OP, A, 1.0.0.0";

            var nestedTypeName =
                TypeUtils.CreateTypeName("p.O`1+I[[" + paramIdentifier + "]], " + TestAssemblyIdentifier);

            Assert.IsTrue(nestedTypeName.IsNestedType);
            Assert.IsTrue(nestedTypeName.HasTypeParameters);
            Assert.AreEqual(1, nestedTypeName.TypeParameters.Count);
            Assert.AreEqual(paramIdentifier, nestedTypeName.TypeParameters[0].Identifier);

            var declaringType = nestedTypeName.DeclaringType;
            Assert.IsFalse(declaringType.IsNestedType);
            Assert.IsTrue(declaringType.HasTypeParameters);
            Assert.AreEqual(1, declaringType.TypeParameters.Count);
            Assert.AreEqual(paramIdentifier, declaringType.TypeParameters[0].Identifier);
        }

        [Test]
        public void ShouldBeNestedParameterizedTypeInParameterizedType()
        {
            const string p1Identifier = "A -> OP, Z, 1.0.0.0";
            const string p2Identifier = "B -> IP1, A, 1.0.0.0";
            const string p3Identifier = "C -> IP2, B, 5.1.0.9";

            var nestedTypeName =
                TypeUtils.CreateTypeName(
                    "p.O`1+I`2[[" + p1Identifier + "],[" + p2Identifier + "],[" + p3Identifier + "]], " +
                    TestAssemblyIdentifier);

            Assert.IsTrue(nestedTypeName.IsNestedType);
            Assert.IsTrue(nestedTypeName.HasTypeParameters);
            Assert.AreEqual(3, nestedTypeName.TypeParameters.Count);
            Assert.AreEqual(p1Identifier, nestedTypeName.TypeParameters[0].Identifier);
            Assert.AreEqual(p2Identifier, nestedTypeName.TypeParameters[1].Identifier);
            Assert.AreEqual(p3Identifier, nestedTypeName.TypeParameters[2].Identifier);

            var declaringType = nestedTypeName.DeclaringType;
            Assert.IsTrue(declaringType.HasTypeParameters);
            Assert.AreEqual(1, declaringType.TypeParameters.Count);
            Assert.AreEqual(p1Identifier, declaringType.TypeParameters[0].Identifier);
        }

        [Test]
        public void ShouldBeParameterizedTypeWithParameterizedTypeParameter()
        {
            const string paramParamIdentifier = "T -> x.A, P1";
            const string paramIdentifier = "y.B`1[[" + paramParamIdentifier + "]], P2";

            var typeName =
                TypeUtils.CreateTypeName("z.C`1[[" + paramIdentifier + "]], P3");

            Assert.IsTrue(typeName.HasTypeParameters);
            Assert.AreEqual(1, typeName.TypeParameters.Count);
            var typeParameter = typeName.TypeParameters[0];

            Assert.AreEqual(new TypeParameterName(paramIdentifier), typeParameter);
        }

        [Test]
        public void ShouldBeDeeplyNestedTypeWithLotsOfTypeParameters()
        {
            var typeName =
                TypeUtils.CreateTypeName(
                    "p.O`1+M`1+I`1[[T -> p.P1, A, 1.0.0.0],[U -> p.P2, A, 1.0.0.0],[V -> p.P3, A, 1.0.0.0]], " +
                    TestAssemblyIdentifier);

            Assert.AreEqual(
                "p.O`1+M`1[[T -> p.P1, A, 1.0.0.0],[U -> p.P2, A, 1.0.0.0]], " + TestAssemblyIdentifier,
                typeName.DeclaringType.Identifier);
        }

        [TestCase("TR -> System.Int32, mscorelib, 4.0.0.0"),
         TestCase("R -> ?"),
         TestCase("TParam")]
        public void ShouldBeTypeParameter(string identifier)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.IsTrue(uut.IsTypeParameter);
        }

        [TestCase("TR -> System.Int32, mscorelib, 4.0.0.0", "TR"),
         TestCase("R -> ?", "R"),
         TestCase("TParam", "TParam")]
        public void ShouldExtractTypeParameterShortName(string identifier, string expectedShortName)
        {
            var uut = TypeUtils.CreateTypeName(identifier);

            Assert.AreEqual(expectedShortName, ((ITypeParameterName) uut).TypeParameterShortName);
        }

        [Test]
        public void TypeParameterTypeShouldBeConcreteType()
        {
            var uut = TypeUtils.CreateTypeName("T -> System.String, mscorlib, 4.0.0.0");

            Assert.AreEqual(false, ((ITypeParameterName) uut).TypeParameterType.IsTypeParameter);
        }

        [Test]
        public void TypeParameterTypeShouldBeShortName()
        {
            var uut = TypeUtils.CreateTypeName("T -> T");

            Assert.AreEqual(true, ((ITypeParameterName) uut).TypeParameterType.IsTypeParameter);
        }

        [Test]
        public void RegularTypeIsNoTypeParameter()
        {
            var uut = TypeUtils.CreateTypeName("Non.Parameter.Type, As, 1.2.3.4");

            Assert.False(uut.IsTypeParameter);
        }

        [TestCase("SomeType`1[[T -> Foo, Bar, 1.2.3.4]], A, 1.2.3.4"),
         TestCase("System.Object, mscorlib, 4.0.0.0")]
        public void ShouldBeNotTypeParameter(string identifier)
        {
            var genericType = TypeUtils.CreateTypeName(identifier);

            Assert.IsFalse(genericType.IsTypeParameter);
        }

        [Test]
        public void TypeParameterParsingIsCached()
        {
            var sut = new TypeName("T,P");
            var a = sut.TypeParameters;
            var b = sut.TypeParameters;
            Assert.AreSame(a, b);
        }
    }
}