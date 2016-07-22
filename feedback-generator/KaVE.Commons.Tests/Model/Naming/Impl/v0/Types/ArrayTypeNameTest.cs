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
using NUnit.Framework;

namespace KaVE.Commons.Tests.Model.Naming.Impl.v0.Types
{
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
         TestCase("A[,,], B", "A[,,,], B"), // seems strange, but is because of [,,][] becomes [,,,]
         TestCase("T`1[[T -> d:[TR] [T2, P2].([T] arg)]], P", "T`1[][[T -> d:[TR] [T2, P2].([T] arg)]], P")]
        public void DerivesFrom(string baseTypeIdentifer, string expectedDerivedNameIdentifier)
        {
            var arrayTypeName = ArrayTypeName.From(TypeUtils.CreateTypeName(baseTypeIdentifer), 1);

            Assert.AreEqual(new ArrayTypeName(expectedDerivedNameIdentifier), arrayTypeName);
        }

        [Test]
        public void DerivesMultiDimensionalArray()
        {
            var arrayTypeName = ArrayTypeName.From(new TypeName("SomeType, Assembly, 1.2.3.4"), 2);

            Assert.AreEqual(new ArrayTypeName("SomeType[,], Assembly, 1.2.3.4"), arrayTypeName);
        }

        [TestCase("ValueType[,,], As, 9.8.7.6"),
         TestCase("ValueType[], As, 5.4.3.2"),
         TestCase("a.Foo`1[][[T -> int, mscore, 1.0.0.0]], Y, 4.3.6.1"),
         TestCase("A[]"),
         TestCase("T -> System.String[], mscorlib, 4.0.0.0"),
         TestCase("System.Int32[], mscorlib, 4.0.0.0"),
         TestCase("d:[RT, A] [DT, A].()[]"),
         TestCase("T`1[][[T -> d:[TR] [T2, P2].([T] arg)]], P")]
        public void ShouldBeArrayType(string identifier)
        {
            Assert.IsTrue(TypeUtils.IsArrayTypeIdentifier(identifier));
            Assert.IsTrue(new ArrayTypeName(identifier).IsArrayType);
        }

        [TestCase("ValueType[,,], As, 9.8.7.6", "ValueType, As, 9.8.7.6"),
         TestCase("ValueType[], As, 5.4.3.2", "ValueType, As, 5.4.3.2"),
         TestCase("a.Foo`1[][[T -> int, mscore, 1.0.0.0]], A, 1.2.3.4",
             "a.Foo`1[[T -> int, mscore, 1.0.0.0]], A, 1.2.3.4"),
         TestCase("A[]", "A"),
         TestCase("System.Int32[], mscorlib, 4.0.0.0", "System.Int32, mscorlib, 4.0.0.0"),
         TestCase("d:[RT, A] [DT, A].()[]", "d:[RT, A] [DT, A].()"),
         TestCase("d:[RT[], A] [DT, A].([PT[], A] p)[]", "d:[RT[], A] [DT, A].([PT[], A] p)"),
         TestCase("T`1[][[T -> d:[TR] [T2, P2].([T] arg)]], P", "T`1[[T -> d:[TR] [T2, P2].([T] arg)]], P")]
        public void ShouldGetArrayBaseType(string identifier, string expected)
        {
            var sut = TypeUtils.CreateTypeName(identifier).AsArrayTypeName;
            Assert.AreEqual(expected, sut.ArrayBaseType.Identifier);
        }

        [TestCase("ValueType[,,], As, 9.8.7.6", 3),
         TestCase("T`1[][[T -> d:[TR] [T2, P2].([T] arg)]], P", 1)]
        public void ShouldIdentifyRank(string id, int expectedRank)
        {
            var actualRank = ArrayTypeName.GetArrayRank(new ArrayTypeName(id));
            Assert.AreEqual(expectedRank, actualRank);
        }

        [Test]
        public void ArrayOfNullablesShouldNotBeNullable()
        {
            const string id = "System.Nullable`1[][[System.Int32, mscorlib, 1.2.3.4]], mscorlib, 1.2.3.4";
            Assert.IsFalse(new ArrayTypeName(id).IsNullableType);
        }

        [Test]
        public void ArrayOfSimpleTypesShouldNotBeSimpleType()
        {
            var actual = new ArrayTypeName("System.Int64[], mscorlib, 1.2.3.4");

            Assert.IsFalse(actual.IsSimpleType);
        }

        [Test]
        public void ArrayOfCustomStructsShouldNotBeStruct()
        {
            var actual = new ArrayTypeName("s:My.Custom.Struct[], A, 1.2.3.4");

            Assert.IsFalse(actual.IsStructType);
        }

        [Test]
        public void ShouldHaveArrayBracesInName()
        {
            var uut = new ArrayTypeName("ValueType[,,], As, 9.8.7.6");

            Assert.AreEqual("ValueType[,,]", uut.Name);
        }

        [Test]
        public void HandlesDelegateBaseType()
        {
            var uut = new ArrayTypeName("d:[RT, A] [N.O+DT, AA, 1.2.3.4].()[]");

            Assert.AreEqual("N.O+DT[]", uut.FullName);
            Assert.AreEqual("N", uut.Namespace.Identifier);
            Assert.AreEqual("DT[]", uut.Name);
            Assert.AreEqual("AA, 1.2.3.4", uut.Assembly.Identifier);
        }

        [Test]
        public void TypeDetection()
        {
            var id = "d:[RT, A] [N.O+DT, AA, 1.2.3.4].()[]";
            Assert.IsTrue(TypeUtils.IsArrayTypeIdentifier(id));
            Assert.IsFalse(TypeUtils.IsDelegateTypeIdentifier(id));
        }
    }
}