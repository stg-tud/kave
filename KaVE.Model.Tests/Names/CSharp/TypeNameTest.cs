using KaVE.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Model.Tests.Names.CSharp
{
    [TestFixture]
    class TypeNameTest
    {
        private const string TestAssemblyIdentifier = "a, 1.0.0.0";

        [Test]
        public void ShouldBeSimpleTypeVoid()
        {
            const string identifier = "System.Void, " + TestAssemblyIdentifier;

            var voidTypeName = TypeName.Get(identifier);

            Assert.AreEqual(identifier, voidTypeName.Identifier);
            Assert.IsTrue(voidTypeName.IsVoidType);
            Assert.IsTrue(voidTypeName.IsValueType);
            Assert.IsFalse(voidTypeName.IsReferenceType);
        }

        [Test]
        public void ShouldBeValueTypeInt()
        {
            const string identifier = "System.Int32, " + TestAssemblyIdentifier;

            var intTypeName = TypeName.Get(identifier);

            Assert.AreEqual(identifier, intTypeName.Identifier);
            Assert.IsTrue(intTypeName.IsValueType);
            Assert.IsTrue(intTypeName.IsStructType);
            Assert.IsFalse(intTypeName.IsNullableType);
            Assert.IsFalse(intTypeName.IsReferenceType);
        }

        [Test]
        public void ShouldBeNullableType()
        {
            const string identifier = "System.Nullable`1[[T -> System.UInt64]], " + TestAssemblyIdentifier;

            var nullableTypeName = TypeName.Get(identifier);

            Assert.AreEqual(identifier, nullableTypeName.Identifier);
            Assert.IsTrue(nullableTypeName.IsNullableType);
            Assert.IsTrue(nullableTypeName.IsStructType);
            Assert.IsTrue(nullableTypeName.IsValueType);
            Assert.IsFalse(nullableTypeName.IsReferenceType);
        }

        [Test]
        public void ObjectShouldBeClassType()
        {
            const string identifier = "System.Object, " + TestAssemblyIdentifier;

            var objectTypeName = TypeName.Get(identifier);

            Assert.AreEqual(identifier, objectTypeName.Identifier);
            Assert.IsTrue(objectTypeName.IsReferenceType);
            Assert.IsTrue(objectTypeName.IsClassType);
        }

        [Test]
        public void ShouldBeAReferenceType()
        {
            var referenceType = TypeName.Get("my.namespace.TypeName, " + TestAssemblyIdentifier);

            Assert.AreEqual("my.namespace", referenceType.Namespace.Identifier);
            Assert.AreEqual("TypeName", referenceType.Name);
            Assert.IsTrue(referenceType.IsReferenceType);
            Assert.IsFalse(referenceType.IsValueType);
            Assert.AreEqual(TestAssemblyIdentifier, referenceType.Assembly.Identifier);
        }

        [Test]
        public void ShouldHaveTypeParameters()
        {
            const string stringIdentifier = "S -> System.String, mscore, 4.0.0.0";
            const string intIdentifier = "I -> System.Int32, mscore, 4.0.0.0";

            var parameterizedTypeName = TypeName.Get("pack.age.MyType`2[[" + stringIdentifier + "],[" + intIdentifier + "]], " + TestAssemblyIdentifier);

            Assert.AreEqual(TestAssemblyIdentifier, parameterizedTypeName.Assembly.Identifier);
            Assert.AreEqual("MyType`2", parameterizedTypeName.Name);
            Assert.IsTrue(parameterizedTypeName.IsGenericType);
            Assert.IsTrue(parameterizedTypeName.HasTypeParameters);
            Assert.IsFalse(parameterizedTypeName.IsArrayType);
            Assert.AreEqual(2, parameterizedTypeName.TypeParameters.Count);
            Assert.AreEqual(stringIdentifier, parameterizedTypeName.TypeParameters[0].Identifier);
            Assert.AreEqual(intIdentifier, parameterizedTypeName.TypeParameters[1].Identifier);
        }

        [Test]
        public void ShouldHaveUninstantiatedTypeParameters()
        {
            var typeName = TypeName.Get("OuterType`1+InnerType, Assembly, 1.2.3.4");

            Assert.IsTrue(typeName.IsGenericType);
            Assert.IsFalse(typeName.HasTypeParameters);
            Assert.AreEqual("OuterType`1+InnerType", typeName.FullName);
            Assert.AreEqual("OuterType`1", typeName.DeclaringType.FullName);
        }

        [Test]
        public void ShouldBeArrayType()
        {
            var arrayTypeName = TypeName.Get("ValueType[,,], " + TestAssemblyIdentifier);

            Assert.IsTrue(arrayTypeName.IsArrayType);
            Assert.IsTrue(arrayTypeName.IsReferenceType);
            Assert.IsFalse(arrayTypeName.IsGenericType);
            Assert.IsFalse(arrayTypeName.HasTypeParameters);
            Assert.AreEqual("ValueType[,,]", arrayTypeName.Name);
        }

        [Test]
        public void ShouldBeMultidimensionalArrayType()
        {
            var arrayTypeName = TypeName.Get("ValueType[], " + TestAssemblyIdentifier);

            Assert.IsTrue(arrayTypeName.IsArrayType);
        }

        [Test]
        public void ShouldBeParameterizedArrayType()
        {
            var typeName = TypeName.Get("a.Foo`1[][[T -> int, mscore, 1.0.0.0]]");

            Assert.IsTrue(typeName.IsArrayType);
            Assert.IsTrue(typeName.IsGenericType);
            Assert.IsTrue(typeName.HasTypeParameters);
        }

        [Test]
        public void ShouldBeTopLevelType()
        {
            var typeName = TypeName.Get("this.is.a.top.level.ValueType, " + TestAssemblyIdentifier);

            Assert.IsFalse(typeName.IsNestedType);
            Assert.IsNull(typeName.DeclaringType);
        }

        [Test]
        public void ShouldBeNestedType()
        {
            var nestedTypeName = TypeName.Get("a.p.T+N, " + TestAssemblyIdentifier);

            Assert.IsTrue(nestedTypeName.IsNestedType);
            Assert.AreEqual("a.p.T, " + TestAssemblyIdentifier, nestedTypeName.DeclaringType.Identifier);
            Assert.AreEqual("a.p.T", nestedTypeName.Namespace.Identifier);
        }

        [Test]
        public void ShouldBeNestedTypeInParameterizedType()
        {
            const string paramIdentifier = "T -> p.OP, A, 1.0.0.0";

            var nestedTypeName = TypeName.Get("p.O`1+I[["+ paramIdentifier + "]], " + TestAssemblyIdentifier);

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

            var nestedTypeName = TypeName.Get("p.O`1+I`2[[" + p1Identifier + "],[" + p2Identifier + "],[" + p3Identifier + "]], " + TestAssemblyIdentifier);

            Assert.IsTrue(nestedTypeName.IsNestedType);
            Assert.IsTrue(nestedTypeName.HasTypeParameters);
            Assert.AreEqual(3, nestedTypeName.TypeParameters.Count);
            Assert.AreEqual(p1Identifier, nestedTypeName.TypeParameters[0].Identifier);
            Assert.AreEqual(p2Identifier, nestedTypeName.TypeParameters[1].Identifier);
            Assert.AreEqual(p3Identifier, nestedTypeName.TypeParameters[2].Identifier);
            Assert.AreEqual("p.O`1", nestedTypeName.Namespace.Identifier);

            var declaringType = nestedTypeName.DeclaringType;
            Assert.IsTrue(declaringType.HasTypeParameters);
            Assert.AreEqual(1, declaringType.TypeParameters.Count);
            Assert.AreEqual(p1Identifier, declaringType.TypeParameters[0].Identifier);
        }

        [Test]
        public void ShouldBeParameterizedTypeWithParameterizedTypeParameter()
        {
            const string paramParamIdentifier = "T -> yan.PTPT, Z, 1.0.0.0";
            const string paramIdentifier = "on.PT`1[[" + paramParamIdentifier + "]], Y, 1.0.0.0";

            var typeName = TypeName.Get("n.OT`1[[" + paramIdentifier + "]], " + TestAssemblyIdentifier);

            Assert.IsTrue(typeName.HasTypeParameters);
            Assert.AreEqual(1, typeName.TypeParameters.Count);

            var typeParameter = typeName.TypeParameters[0];
            Assert.AreEqual(paramIdentifier, typeParameter.Identifier);
            Assert.IsTrue(typeParameter.HasTypeParameters);
            Assert.AreEqual(1, typeParameter.TypeParameters.Count);
            Assert.AreEqual(paramParamIdentifier, typeParameter.TypeParameters[0].Identifier);
        }

        [Test]
        public void ShouldBeDeeplyNestedTypeWithLotsOfTypeParameters()
        {
            var typeName = TypeName.Get("p.O`1+M`1+I`1[[T -> p.P1, A, 1.0.0.0],[U -> p.P2, A, 1.0.0.0],[V -> p.P3, A, 1.0.0.0]], " + TestAssemblyIdentifier);

            Assert.AreEqual("p.O`1+M`1[[T -> p.P1, A, 1.0.0.0],[U -> p.P2, A, 1.0.0.0]], " + TestAssemblyIdentifier, typeName.DeclaringType.Identifier);
        }

        [Test]
        public void ShouldParseFullDescriptorWithAdditionInfo()
        {
            const string assemblyIdentifier = "Assembly, 0.100.90.666666, PublicKeyToken=DEADBEEF";

            var typeName = TypeName.Get("an.other.ValueType, " + assemblyIdentifier);

            Assert.AreEqual(assemblyIdentifier, typeName.Assembly.Identifier);
            Assert.AreEqual("an.other", typeName.Namespace.Identifier);
            Assert.AreEqual("ValueType", typeName.Name);
        }

        [Test]
        public void ShouldParseUnknownType()
        {
            var typeName = TypeName.Get("");

            Assert.AreEqual("?", typeName.Identifier);
            Assert.AreEqual("?", typeName.FullName);
            Assert.IsNull(typeName.Namespace);
            Assert.IsNull(typeName.Assembly);
            Assert.IsTrue(typeName.IsUnknownType);
            Assert.IsFalse(typeName.IsArrayType);
            Assert.IsFalse(typeName.IsClassType);
            Assert.IsFalse(typeName.IsDelegateType);
            Assert.IsFalse(typeName.IsEnumType);
            Assert.IsFalse(typeName.IsGenericType);
            Assert.IsFalse(typeName.IsInterfaceType);
            Assert.IsFalse(typeName.IsNestedType);
            Assert.IsFalse(typeName.IsNullableType);
            Assert.IsFalse(typeName.IsReferenceType);
            Assert.IsFalse(typeName.IsSimpleType);
            Assert.IsFalse(typeName.IsStructType);
            Assert.IsFalse(typeName.IsValueType);
            Assert.IsFalse(typeName.IsVoidType);
        }

        [Test]
        public void ShouldCreateArrayTypeNameFromTypeName()
        {
            var arrayTypeName = TypeName.Get("SomeType, Assembly, 1.2.3.4").DeriveArrayTypeName(2);

            Assert.AreEqual("SomeType[,], Assembly, 1.2.3.4", arrayTypeName.Identifier);
        }

        [Test]
        public void ShouldCreateArrayTypeNameFromGenericTypeName()
        {
            var arrayTypeName = TypeName.Get("SomeGenericType`1[[T -> System.Int32, mscore, 5.6.7.8]], A, 9.10.11.12").DeriveArrayTypeName(1);

            Assert.AreEqual("SomeGenericType`1[][[T -> System.Int32, mscore, 5.6.7.8]], A, 9.10.11.12", arrayTypeName.Identifier);
        }

        [Test]
        public void ShouldBeTypeParameter()
        {
            var typeParameter = TypeName.Get("TR -> System.Int32, mscorelib, .4.0.0.0");

            Assert.IsTrue(typeParameter.IsTypeParameter);
            Assert.AreEqual("TR", typeParameter.TypeParameterShortName);
            Assert.AreEqual("System.Int32", typeParameter.FullName);
        }

        [Test]
        public void ShouldBeTypeParameterWithUnkownTargetType()
        {
            var typeParameter = TypeName.Get("R -> ?");

            Assert.IsTrue(typeParameter.IsTypeParameter);
            Assert.IsTrue(typeParameter.IsUnknownType);
        }

        [Test]
        public void ShouldBeNotTypeParameter()
        {
            var genericType = TypeName.Get("SomeType`1[[T -> Foo, Bar, 1.2.3.4]]");

            Assert.IsFalse(genericType.IsTypeParameter);
            Assert.AreEqual("SomeType`1[[T -> Foo, Bar, 1.2.3.4]]", genericType.FullName);
        }
    }
}
