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

using KaVE.Commons.Model.Names.CSharp;
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Names.CSharp
{
    [TestFixture]
    internal class ArrayTypeNameTest
    {
        [TestCase("T, P", "T[], P"),
         TestCase("A, B, 1.2.3.4", "A[], B, 1.2.3.4"),
         TestCase("GT`1[[T -> PT, A]], A", "GT`1[][[T -> PT, A]], A"),
         TestCase("T", "T[]"),
         TestCase("s:S, P", "s:S[], P"),
         TestCase("d:[RT, A] [DT, A].()", "d:[RT, A] [DT, A].()[]"),
         TestCase("d:[RT[], A] [DT, A].([PT[], A] p)", "d:[RT[], A] [DT, A].([PT[], A] p)[]"),
         TestCase("A[], B", "A[,], B"),
         TestCase("A[,,], B", "A[,,,], B")]
        public void DerivesFrom(string baseTypeIdentifer, string expectedDerivedNameIdentifier)
        {
            var arrayTypeName = ArrayTypeName.From(TypeName.Get(baseTypeIdentifer), 1);

            Assert.AreSame(TypeName.Get(expectedDerivedNameIdentifier), arrayTypeName);
        }

        [Test]
        public void DerivesMultiDimensionalArray()
        {
            var arrayTypeName = ArrayTypeName.From(TypeName.Get("SomeType, Assembly, 1.2.3.4"), 2);

            Assert.AreSame(TypeName.Get("SomeType[,], Assembly, 1.2.3.4"), arrayTypeName);
        }

        [TestCase("ValueType[,,], As, 9.8.7.6"),
         TestCase("ValueType[], As, 5.4.3.2"),
         TestCase("a.Foo`1[][[T -> int, mscore, 1.0.0.0]], Y, 4.3.6.1"),
         TestCase("A[]"),
         TestCase("T -> System.String[], mscorlib, 4.0.0.0"),
         TestCase("System.Int32[], mscorlib, 4.0.0.0"),
         TestCase("d:[RT, A] [DT, A].()[]")]
        public void ShouldBeArrayType(string identifier)
        {
            var arrayTypeName = TypeName.Get(identifier);

            Assert.IsTrue(arrayTypeName.IsArrayType);
        }

        [TestCase("ValueType[,,], As, 9.8.7.6", "ValueType, As, 9.8.7.6"),
         TestCase("ValueType[], As, 5.4.3.2", "ValueType, As, 5.4.3.2"),
         TestCase("a.Foo`1[][[T -> int, mscore, 1.0.0.0]], A, 1.2.3.4",
             "a.Foo`1[[T -> int, mscore, 1.0.0.0]], A, 1.2.3.4"),
         TestCase("A[]", "A"),
         TestCase("T -> System.String[], mscorlib, 4.0.0.0", "System.String, mscorlib, 4.0.0.0"),
         TestCase("System.Int32[], mscorlib, 4.0.0.0", "System.Int32, mscorlib, 4.0.0.0"),
         TestCase("d:[RT, A] [DT, A].()[]", "d:[RT, A] [DT, A].()"),
         TestCase("d:[RT[], A] [DT, A].([PT[], A] p)[]", "d:[RT[], A] [DT, A].([PT[], A] p)")]
        public void ShouldGetArrayBaseType(string identifier, string expected)
        {
            var arrayTypeName = TypeName.Get(identifier);

            Assert.AreEqual(expected, arrayTypeName.ArrayBaseType.Identifier);
        }

        [Test]
        public void ArrayOfNullablesShouldNotBeNullable()
        {
            var actual = TypeName.Get("System.Nullable`1[][[System.Int32, mscorlib, 1.2.3.4]], mscorlib, 1.2.3.4");

            Assert.IsFalse(actual.IsNullableType);
        }

        [Test]
        public void ArrayOfSimpleTypesShouldNotBeSimpleType()
        {
            var actual = TypeName.Get("System.Int64[], mscorlib, 1.2.3.4");

            Assert.IsFalse(actual.IsSimpleType);
        }

        [Test]
        public void ArrayOfCustomStructsShouldNotBeStruct()
        {
            var actual = TypeName.Get("s:My.Custom.Struct[], A, 1.2.3.4");

            Assert.IsFalse(actual.IsStructType);
        }

        [Test]
        public void ShouldHaveArrayBracesInName()
        {
            var uut = TypeName.Get("ValueType[,,], As, 9.8.7.6");

            Assert.AreEqual("ValueType[,,]", uut.Name);
        }

        [Test]
        public void ShouldNotBeArrayType()
        {
            var uut = TypeName.Get("ValueType, As, 2.5.1.6");

            Assert.IsFalse(uut.IsArrayType);
        }

        [Test]
        public void HandlesDelegateBaseType()
        {
            var uut = TypeName.Get("d:[RT, A] [N.O+DT, AA, 1.2.3.4].()[]");

            Assert.AreEqual("N.O+DT[]", uut.FullName);
            Assert.AreEqual("N", uut.Namespace.Identifier);
            Assert.AreEqual("DT[]", uut.Name);
            Assert.AreEqual("AA, 1.2.3.4", uut.Assembly.Identifier);
            Assert.AreEqual("N.O, AA, 1.2.3.4", uut.DeclaringType.Identifier);
        }

        [Test]
        public void HandlesGenericDelegateBaseType()
        {
            var uut = TypeName.Get("d:[T] [DT`1[[T -> String, mscorlib]]].([T] p)[]");

            Assert.IsTrue(uut.HasTypeParameters);
            CollectionAssert.AreEqual(new[] { TypeParameterName.Get("T -> String, mscorlib") }, uut.TypeParameters);
        }
    }
}